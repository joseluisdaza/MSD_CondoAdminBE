# AWS Deployment Helper Script
# This script automates the deployment of Condominio API to AWS ECS
# Run this after setting up AWS CLI credentials

param(
    [string]$Region = "us-east-1",
    [string]$AccountId = "",
    [switch]$SkipBuild,
    [switch]$DeployOnly,
    [switch]$Cleanup
)

# Color output functions
function Write-Success { param($msg) Write-Host $msg -ForegroundColor Green }
function Write-Info { param($msg) Write-Host $msg -ForegroundColor Cyan }
function Write-Warning { param($msg) Write-Host $msg -ForegroundColor Yellow }
function Write-Error { param($msg) Write-Host $msg -ForegroundColor Red }

# Get AWS Account ID if not provided
if ([string]::IsNullOrEmpty($AccountId)) {
    Write-Info "Getting AWS Account ID..."
    $AccountId = (aws sts get-caller-identity --query Account --output text)
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to get AWS Account ID. Please run 'aws configure' first."
        exit 1
    }
    Write-Success "AWS Account ID: $AccountId"
}

$ECR_REPO = "${AccountId}.dkr.ecr.${Region}.amazonaws.com/condominio-api"

# Cleanup mode
if ($Cleanup) {
    Write-Warning "This will delete all AWS resources created for Condominio API."
    $confirm = Read-Host "Are you sure? (yes/no)"
    if ($confirm -ne "yes") {
        Write-Info "Cleanup cancelled."
        exit 0
    }

    Write-Info "Deleting ECS service..."
    aws ecs delete-service --cluster condominio-cluster --service condominio-api-service --force --region $Region 2>$null

    Write-Info "Waiting for service deletion..."
    Start-Sleep -Seconds 30

    Write-Info "Deleting ECS cluster..."
    aws ecs delete-cluster --cluster condominio-cluster --region $Region 2>$null

    Write-Info "Deleting ALB and target group..."
    $ALB_ARN = (aws elbv2 describe-load-balancers --names condominio-alb --query 'LoadBalancers[0].LoadBalancerArn' --output text --region $Region 2>$null)
    if ($ALB_ARN -and $ALB_ARN -ne "None") {
        aws elbv2 delete-load-balancer --load-balancer-arn $ALB_ARN --region $Region 2>$null
    }

    $TG_ARN = (aws elbv2 describe-target-groups --names condominio-tg --query 'TargetGroups[0].TargetGroupArn' --output text --region $Region 2>$null)
    if ($TG_ARN -and $TG_ARN -ne "None") {
        Start-Sleep -Seconds 10
        aws elbv2 delete-target-group --target-group-arn $TG_ARN --region $Region 2>$null
    }

    Write-Info "Deleting RDS instance..."
    aws rds delete-db-instance --db-instance-identifier condominio-db --skip-final-snapshot --region $Region 2>$null

    Write-Info "Deleting secrets..."
    aws secretsmanager delete-secret --secret-id condominio/db/credentials --force-delete-without-recovery --region $Region 2>$null
    aws secretsmanager delete-secret --secret-id condominio/jwt/secret --force-delete-without-recovery --region $Region 2>$null

    Write-Info "Deleting ECR repository..."
    aws ecr delete-repository --repository-name condominio-api --force --region $Region 2>$null

    Write-Info "Deleting CloudWatch log group..."
    aws logs delete-log-group --log-group-name /ecs/condominio-api --region $Region 2>$null

    Write-Success "Cleanup completed! Note: Security groups and IAM roles may need manual deletion."
    exit 0
}

# Deploy mode
Write-Info "Starting AWS deployment process..."
Write-Info "Region: $Region"
Write-Info "ECR Repository: $ECR_REPO"

# Step 1: Create ECR repository if doesn't exist
Write-Info "Checking ECR repository..."
$repoExists = aws ecr describe-repositories --repository-names condominio-api --region $Region 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Info "Creating ECR repository..."
    aws ecr create-repository --repository-name condominio-api --region $Region
    Write-Success "ECR repository created!"
} else {
    Write-Success "ECR repository already exists."
}

# Step 2: Build and push Docker image
if (-not $SkipBuild -and -not $DeployOnly) {
    Write-Info "Authenticating Docker to ECR..."
    aws ecr get-login-password --region $Region | docker login --username AWS --password-stdin $ECR_REPO

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Docker authentication failed!"
        exit 1
    }

    Write-Info "Building Docker image..."
    docker build -t condominio-api:latest .

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Docker build failed!"
        exit 1
    }

    Write-Info "Tagging Docker image..."
    $timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
    docker tag condominio-api:latest "${ECR_REPO}:latest"
    docker tag condominio-api:latest "${ECR_REPO}:${timestamp}"

    Write-Info "Pushing to ECR..."
    docker push "${ECR_REPO}:latest"
    docker push "${ECR_REPO}:${timestamp}"

    Write-Success "Docker image pushed successfully!"
    Write-Success "Image tags: latest, ${timestamp}"
}

# Step 3: Check if ECS cluster exists
if (-not $DeployOnly) {
    Write-Info "Checking ECS cluster..."
    $clusterExists = aws ecs describe-clusters --clusters condominio-cluster --region $Region --query 'clusters[0].status' --output text 2>$null
    
    if ($clusterExists -ne "ACTIVE") {
        Write-Info "Creating ECS cluster..."
        aws ecs create-cluster --cluster-name condominio-cluster --region $Region
        Write-Success "ECS cluster created!"
    } else {
        Write-Success "ECS cluster already exists."
    }
}

# Step 4: Update ECS service (force new deployment)
Write-Info "Checking ECS service..."
$serviceExists = aws ecs describe-services --cluster condominio-cluster --services condominio-api-service --region $Region --query 'services[0].status' --output text 2>$null

if ($serviceExists -eq "ACTIVE") {
    Write-Info "Forcing new deployment of ECS service..."
    aws ecs update-service `
        --cluster condominio-cluster `
        --service condominio-api-service `
        --force-new-deployment `
        --region $Region

    Write-Success "Deployment initiated! Waiting for service to stabilize..."
    
    Write-Info "This may take 2-5 minutes..."
    aws ecs wait services-stable `
        --cluster condominio-cluster `
        --services condominio-api-service `
        --region $Region

    Write-Success "Service deployment completed!"
} else {
    Write-Warning "ECS service doesn't exist yet. Please follow the manual steps in Deployment.md to create the initial infrastructure."
    Write-Info "After initial setup, you can use this script for updates with: .\deploy-aws.ps1"
}

# Step 5: Get ALB DNS
Write-Info "Getting Application Load Balancer DNS..."
$ALB_DNS = (aws elbv2 describe-load-balancers --names condominio-alb --query 'LoadBalancers[0].DNSName' --output text --region $Region 2>$null)

if ($ALB_DNS -and $ALB_DNS -ne "None") {
    Write-Success "`n=== Deployment Summary ==="
    Write-Success "API URL: http://${ALB_DNS}"
    Write-Success "Swagger: http://${ALB_DNS}/swagger"
    Write-Success "Health: http://${ALB_DNS}/health"
    
    Write-Info "`nTesting health endpoint..."
    Start-Sleep -Seconds 5
    try {
        $response = Invoke-WebRequest -Uri "http://${ALB_DNS}/health" -Method GET -TimeoutSec 10
        Write-Success "API is healthy! Status: $($response.StatusCode)"
    } catch {
        Write-Warning "Health check failed. The service may still be starting up."
        Write-Info "Check logs with: aws logs tail /ecs/condominio-api --follow --region $Region"
    }
} else {
    Write-Info "Load balancer not found. Initial infrastructure setup may be required."
}

Write-Success "`nDeployment process completed!"
Write-Info "To view logs: aws logs tail /ecs/condominio-api --follow --region $Region"
Write-Info "To check service: aws ecs describe-services --cluster condominio-cluster --services condominio-api-service --region $Region"
