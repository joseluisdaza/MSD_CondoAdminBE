# Condominio API - AWS Deployment Script
# This script automates the deployment of Condominio API to AWS ECS Fargate
# 
# PREREQUISITES:
# 1. AWS CLI configured with credentials (run: aws configure)
# 2. Docker Desktop running
# 3. Update variables below with your values
#
# Usage: .\deploy-to-aws.ps1

# ============================================
# CONFIGURATION - UPDATE THESE VALUES
# ============================================

$AWS_REGION = "us-east-1"
$DB_PASSWORD = "CondominioSecurePass123!"  # CHANGE THIS!
$DB_USERNAME = "admin"
$DB_NAME = "Condominio2"
$JWT_SECRET = "clave_super_secreta_para_jwt_1234567890"  # CHANGE THIS!

# ============================================
# SCRIPT START
# ============================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Condominio API - AWS Deployment" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Get AWS Account ID
Write-Host "Getting AWS Account ID..." -ForegroundColor Yellow
$AWS_ACCOUNT_ID = (aws sts get-caller-identity --query Account --output text)
if (-not $AWS_ACCOUNT_ID) {
    Write-Host "ERROR: AWS CLI not configured or credentials invalid" -ForegroundColor Red
    Write-Host "Run: aws configure" -ForegroundColor Yellow
    exit 1
}
Write-Host "Account ID: $AWS_ACCOUNT_ID" -ForegroundColor Green

# ============================================
# 1. CREATE ECR REPOSITORY
# ============================================

Write-Host "`n[1/12] Creating ECR Repository..." -ForegroundColor Yellow
$ECR_REPO_URI = "$AWS_ACCOUNT_ID.dkr.ecr.$AWS_REGION.amazonaws.com/condominio-api"

try {
    aws ecr create-repository --repository-name condominio-api --region $AWS_REGION --image-scanning-configuration scanOnPush=true --encryption-configuration encryptionType=AES256 2>$null
    Write-Host "ECR Repository created: $ECR_REPO_URI" -ForegroundColor Green
}
catch {
    Write-Host "ECR Repository already exists" -ForegroundColor Green
}

# ============================================
# 2. BUILD AND PUSH DOCKER IMAGE
# ============================================

Write-Host "`n[2/12] Building and Pushing Docker Image..." -ForegroundColor Yellow

Write-Host "Authenticating Docker to ECR..." -ForegroundColor Gray
aws ecr get-login-password --region $AWS_REGION | docker login --username AWS --password-stdin $ECR_REPO_URI

$PROJECT_DIR = Split-Path -Parent $PSScriptRoot
Set-Location "$PROJECT_DIR\CondominioAPI"

Write-Host "Building Docker image (this takes 5-10 minutes)..." -ForegroundColor Gray
docker build --build-arg GIT_BRANCH=main -t condominio-api:latest -f Dockerfile .

docker tag condominio-api:latest ${ECR_REPO_URI}:latest
docker tag condominio-api:latest ${ECR_REPO_URI}:v1.0.0

Write-Host "Pushing to ECR..." -ForegroundColor Gray
docker push ${ECR_REPO_URI}:latest
docker push ${ECR_REPO_URI}:v1.0.0

Write-Host "Docker image pushed to ECR" -ForegroundColor Green

# ============================================
# 3. GET VPC INFORMATION
# ============================================

Write-Host "`n[3/12] Getting VPC Information..." -ForegroundColor Yellow

$VPC_ID = (aws ec2 describe-vpcs --filters "Name=isDefault,Values=true" --query "Vpcs[0].VpcId" --output text --region $AWS_REGION)
$SUBNETS = (aws ec2 describe-subnets --filters "Name=vpc-id,Values=$VPC_ID" --query "Subnets[*].SubnetId" --output text --region $AWS_REGION)
$SUBNET_ARRAY = $SUBNETS -split '\s+'
$SUBNET1 = $SUBNET_ARRAY[0]
$SUBNET2 = $SUBNET_ARRAY[1]

Write-Host "VPC ID: $VPC_ID" -ForegroundColor Green
Write-Host "Subnets: $SUBNET1, $SUBNET2" -ForegroundColor Green

# ============================================
# 4. CREATE RDS SUBNET GROUP
# ============================================

Write-Host "`n[4/12] Creating RDS Subnet Group..." -ForegroundColor Yellow

try {
    aws rds create-db-subnet-group --db-subnet-group-name condominio-db-subnet-group --db-subnet-group-description "Subnet group for Condominio API database" --subnet-ids $SUBNET1 $SUBNET2 --region $AWS_REGION 2>$null
    Write-Host "DB Subnet Group created" -ForegroundColor Green
}
catch {
    Write-Host "DB Subnet Group already exists" -ForegroundColor Green
}

# ============================================
# 5. CREATE SECURITY GROUPS
# ============================================

Write-Host "`n[5/12] Creating Security Groups..." -ForegroundColor Yellow

# DB Security Group
try {
    $DB_SG_ID = (aws ec2 create-security-group --group-name condominio-db-sg --description "Security group for Condominio MySQL database" --vpc-id $VPC_ID --region $AWS_REGION --query 'GroupId' --output text 2>$null)
    Write-Host "DB Security Group created: $DB_SG_ID" -ForegroundColor Green
}
catch {
    $DB_SG_ID = (aws ec2 describe-security-groups --filters "Name=group-name,Values=condominio-db-sg" --query "SecurityGroups[0].GroupId" --output text --region $AWS_REGION)
    Write-Host "DB Security Group already exists: $DB_SG_ID" -ForegroundColor Green
}

# ECS Security Group
try {
    $ECS_SG_ID = (aws ec2 create-security-group --group-name condominio-ecs-sg --description "Security group for Condominio ECS tasks" --vpc-id $VPC_ID --region $AWS_REGION --query 'GroupId' --output text 2>$null)
    Write-Host "ECS Security Group created: $ECS_SG_ID" -ForegroundColor Green
}
catch {
    $ECS_SG_ID = (aws ec2 describe-security-groups --filters "Name=group-name,Values=condominio-ecs-sg" --query "SecurityGroups[0].GroupId" --output text --region $AWS_REGION)
    Write-Host "ECS Security Group already exists: $ECS_SG_ID" -ForegroundColor Green
}

# ALB Security Group
try {
    $ALB_SG_ID = (aws ec2 create-security-group --group-name condominio-alb-sg --description "Security group for Condominio ALB" --vpc-id $VPC_ID --region $AWS_REGION --query 'GroupId' --output text 2>$null)
    aws ec2 authorize-security-group-ingress --group-id $ALB_SG_ID --protocol tcp --port 80 --cidr 0.0.0.0/0 --region $AWS_REGION 2>$null
    aws ec2 authorize-security-group-ingress --group-id $ALB_SG_ID --protocol tcp --port 443 --cidr 0.0.0.0/0 --region $AWS_REGION 2>$null
    Write-Host "ALB Security Group created: $ALB_SG_ID" -ForegroundColor Green
}
catch {
    $ALB_SG_ID = (aws ec2 describe-security-groups --filters "Name=group-name,Values=condominio-alb-sg" --query "SecurityGroups[0].GroupId" --output text --region $AWS_REGION)
    Write-Host "ALB Security Group already exists: $ALB_SG_ID" -ForegroundColor Green
}

# Configure security group rules
aws ec2 authorize-security-group-ingress --group-id $DB_SG_ID --protocol tcp --port 3306 --source-group $ECS_SG_ID --region $AWS_REGION 2>$null
aws ec2 authorize-security-group-ingress --group-id $ECS_SG_ID --protocol tcp --port 8080 --source-group $ALB_SG_ID --region $AWS_REGION 2>$null

# ============================================
# 6. CREATE RDS INSTANCE
# ============================================

Write-Host "`n[6/12] Creating RDS MySQL Instance..." -ForegroundColor Yellow

try {
    aws rds create-db-instance --db-instance-identifier condominio-db --db-instance-class db.t3.micro --engine mysql --engine-version 8.0.45 --master-username $DB_USERNAME --master-user-password $DB_PASSWORD --allocated-storage 20 --storage-type gp3 --db-subnet-group-name condominio-db-subnet-group --vpc-security-group-ids $DB_SG_ID --no-publicly-accessible --region $AWS_REGION 2>$null
    Write-Host "RDS instance creation started (takes 5-10 minutes)" -ForegroundColor Green
    Write-Host "Waiting for database to be available..." -ForegroundColor Gray
    aws rds wait db-instance-available --db-instance-identifier condominio-db --region $AWS_REGION
    Write-Host "RDS instance is available" -ForegroundColor Green
}
catch {
    Write-Host "RDS instance already exists or is being created" -ForegroundColor Green
}

# Get DB endpoint
$DB_ENDPOINT = (aws rds describe-db-instances --db-instance-identifier condominio-db --query 'DBInstances[0].Endpoint.Address' --output text --region $AWS_REGION)
Write-Host "Database Endpoint: $DB_ENDPOINT" -ForegroundColor Green

# ============================================
# 7. CREATE SECRETS MANAGER SECRETS
# ============================================

Write-Host "`n[7/12] Creating Secrets in Secrets Manager..." -ForegroundColor Yellow

# Database credentials secret (use proper JSON formatting)
$dbSecretJson = @{
    username = $DB_USERNAME
    password = $DB_PASSWORD
    host = $DB_ENDPOINT
    port = 3306
    dbname = $DB_NAME
} | ConvertTo-Json -Compress

try {
    aws secretsmanager create-secret --name condominio/db/credentials --description "Database credentials for Condominio API" --secret-string $dbSecretJson --region $AWS_REGION 2>$null | Out-Null
    Write-Host "Database secret created" -ForegroundColor Green
}
catch {
    Write-Host "Database secret already exists, updating..." -ForegroundColor Yellow
    aws secretsmanager update-secret --secret-id condominio/db/credentials --secret-string $dbSecretJson --region $AWS_REGION 2>$null | Out-Null
}

# Always get the actual ARN after creation/update
$DB_SECRET_ARN = (aws secretsmanager describe-secret --secret-id condominio/db/credentials --query 'ARN' --output text --region $AWS_REGION)
Write-Host "DB Secret ARN: $DB_SECRET_ARN" -ForegroundColor Cyan

# JWT secret
try {
    aws secretsmanager create-secret --name condominio/jwt/secret --description "JWT secret key for Condominio API" --secret-string $JWT_SECRET --region $AWS_REGION 2>$null | Out-Null
    Write-Host "JWT secret created" -ForegroundColor Green
}
catch {
    Write-Host "JWT secret already exists, updating..." -ForegroundColor Yellow
    aws secretsmanager update-secret --secret-id condominio/jwt/secret --secret-string $JWT_SECRET --region $AWS_REGION 2>$null | Out-Null
}

# Always get the actual ARN after creation/update
$JWT_SECRET_ARN = (aws secretsmanager describe-secret --secret-id condominio/jwt/secret --query 'ARN' --output text --region $AWS_REGION)
Write-Host "JWT Secret ARN: $JWT_SECRET_ARN" -ForegroundColor Cyan

# ============================================
# 8. CREATE ECS CLUSTER AND IAM ROLE
# ============================================

Write-Host "`n[8/12] Creating ECS Cluster and IAM Role..." -ForegroundColor Yellow

# Create ECS cluster
try {
    aws ecs create-cluster --cluster-name condominio-cluster --region $AWS_REGION 2>$null
    Write-Host "ECS Cluster created" -ForegroundColor Green
}
catch {
    Write-Host "ECS Cluster already exists" -ForegroundColor Green
}

# Create CloudWatch log group
try {
    aws logs create-log-group --log-group-name /ecs/condominio-api --region $AWS_REGION 2>$null
    aws logs put-retention-policy --log-group-name /ecs/condominio-api --retention-in-days 7 --region $AWS_REGION 2>$null
    Write-Host "CloudWatch log group created" -ForegroundColor Green
}
catch {
    Write-Host "CloudWatch log group already exists" -ForegroundColor Green
}

# Create IAM execution role
$TRUST_POLICY = @"
{
  "Version": "2012-10-17",
  "Statement": [{
    "Effect": "Allow",
    "Principal": {"Service": "ecs-tasks.amazonaws.com"},
    "Action": "sts:AssumeRole"
  }]
}
"@

try {
    Set-Content -Path trust-policy.json -Value $TRUST_POLICY
    aws iam create-role --role-name condominioECSExecutionRole --assume-role-policy-document file://trust-policy.json 2>$null
    aws iam attach-role-policy --role-name condominioECSExecutionRole --policy-arn arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy 2>$null
    Remove-Item trust-policy.json
    Write-Host "IAM Execution Role created" -ForegroundColor Green
}
catch {
    Write-Host "IAM Execution Role already exists" -ForegroundColor Green
}

# ALWAYS update the secrets policy with current ARNs (whether role is new or existing)
$SECRETS_POLICY = @"
{
  "Version": "2012-10-17",
  "Statement": [{
    "Effect": "Allow",
    "Action": ["secretsmanager:GetSecretValue"],
    "Resource": ["$DB_SECRET_ARN","$JWT_SECRET_ARN"]
  }]
}
"@

Set-Content -Path secrets-policy.json -Value $SECRETS_POLICY
aws iam put-role-policy --role-name condominioECSExecutionRole --policy-name SecretsManagerAccess --policy-document file://secrets-policy.json 2>$null
Remove-Item secrets-policy.json
Write-Host "IAM Secrets Policy updated with current ARNs" -ForegroundColor Cyan

$EXECUTION_ROLE_ARN = (aws iam get-role --role-name condominioECSExecutionRole --query 'Role.Arn' --output text)

# ============================================
# 9. CREATE APPLICATION LOAD BALANCER
# ============================================

Write-Host "`n[9/12] Creating Application Load Balancer..." -ForegroundColor Yellow

try {
    $ALB_ARN = (aws elbv2 create-load-balancer --name condominio-alb --subnets $SUBNET1 $SUBNET2 --security-groups $ALB_SG_ID --scheme internet-facing --type application --ip-address-type ipv4 --region $AWS_REGION --query 'LoadBalancers[0].LoadBalancerArn' --output text 2>$null)
    Write-Host "ALB created: $ALB_ARN" -ForegroundColor Green
}
catch {
    $ALB_ARN = (aws elbv2 describe-load-balancers --names condominio-alb --query 'LoadBalancers[0].LoadBalancerArn' --output text --region $AWS_REGION)
    Write-Host "ALB already exists: $ALB_ARN" -ForegroundColor Green
}

# Get ALB DNS
$ALB_DNS = (aws elbv2 describe-load-balancers --load-balancer-arns $ALB_ARN --query 'LoadBalancers[0].DNSName' --output text --region $AWS_REGION)

# ============================================
# 10. CREATE TARGET GROUP
# ============================================

Write-Host "`n[10/12] Creating Target Group..." -ForegroundColor Yellow

try {
    $TG_ARN = (aws elbv2 create-target-group --name condominio-tg --protocol HTTP --port 8080 --vpc-id $VPC_ID --target-type ip --health-check-enabled --health-check-protocol HTTP --health-check-path /api/health --health-check-interval-seconds 30 --health-check-timeout-seconds 5 --healthy-threshold-count 2 --unhealthy-threshold-count 3 --region $AWS_REGION --query 'TargetGroups[0].TargetGroupArn' --output text 2>$null)
    Write-Host "Target Group created: $TG_ARN" -ForegroundColor Green
}
catch {
    $TG_ARN = (aws elbv2 describe-target-groups --names condominio-tg --query 'TargetGroups[0].TargetGroupArn' --output text --region $AWS_REGION)
    Write-Host "Target Group already exists: $TG_ARN" -ForegroundColor Green
}

# Create listener
try {
    aws elbv2 create-listener --load-balancer-arn $ALB_ARN --protocol HTTP --port 80 --default-actions Type=forward,TargetGroupArn=$TG_ARN --region $AWS_REGION 2>$null
    Write-Host "Listener created" -ForegroundColor Green
}
catch {
    Write-Host "Listener already exists" -ForegroundColor Green
}

# ============================================
# 11. REGISTER TASK DEFINITION
# ============================================

Write-Host "`n[11/12] Registering ECS Task Definition..." -ForegroundColor Yellow

# Validate all required variables before creating task definition
Write-Host "Validating configuration variables..." -ForegroundColor Gray
if (-not $EXECUTION_ROLE_ARN) { Write-Host "ERROR: EXECUTION_ROLE_ARN is empty" -ForegroundColor Red; exit 1 }
if (-not $ECR_REPO_URI) { Write-Host "ERROR: ECR_REPO_URI is empty" -ForegroundColor Red; exit 1 }
if (-not $DB_ENDPOINT) { Write-Host "ERROR: DB_ENDPOINT is empty" -ForegroundColor Red; exit 1 }
if (-not $DB_SECRET_ARN) { Write-Host "ERROR: DB_SECRET_ARN is empty" -ForegroundColor Red; exit 1 }
if (-not $JWT_SECRET_ARN) { Write-Host "ERROR: JWT_SECRET_ARN is empty" -ForegroundColor Red; exit 1 }
if (-not $ALB_DNS) { Write-Host "ERROR: ALB_DNS is empty" -ForegroundColor Red; exit 1 }
Write-Host "All variables validated" -ForegroundColor Green

$taskDefContent = @"
{
  "family": "condominio-api-task",
  "networkMode": "awsvpc",
  "requiresCompatibilities": ["FARGATE"],
  "cpu": "512",
  "memory": "1024",
  "executionRoleArn": "$EXECUTION_ROLE_ARN",
  "containerDefinitions": [{
    "name": "condominio-api",
    "image": "${ECR_REPO_URI}:latest",
    "essential": true,
    "portMappings": [{"containerPort": 8080, "protocol": "tcp"}],
    "environment": [
      {"name": "DB_SERVER", "value": "$DB_ENDPOINT"},
      {"name": "DB_NAME", "value": "$DB_NAME"},
      {"name": "DB_USER", "value": "$DB_USERNAME"},
      {"name": "CORS_ALLOWED_ORIGIN", "value": "http://$ALB_DNS"},
      {"name": "LOG_PATH", "value": "/app/Logs/log-.txt"},
      {"name": "ASPNETCORE_ENVIRONMENT", "value": "Production"}
    ],
    "secrets": [
      {"name": "DB_PASSWORD", "valueFrom": "${DB_SECRET_ARN}:password::"},
      {"name": "JWT_SECRET_KEY", "valueFrom": "$JWT_SECRET_ARN"}
    ],
    "logConfiguration": {
      "logDriver": "awslogs",
      "options": {
        "awslogs-group": "/ecs/condominio-api",
        "awslogs-region": "$AWS_REGION",
        "awslogs-stream-prefix": "ecs"
      }
    },
    "healthCheck": {
      "command": ["CMD-SHELL", "curl -f http://localhost:8080/api/health || exit 1"],
      "interval": 30,
      "timeout": 5,
      "retries": 3,
      "startPeriod": 60
    }
  }]
}
"@

Set-Content -Path task-definition.json -Value $taskDefContent
aws ecs register-task-definition --cli-input-json file://task-definition.json --region $AWS_REGION >$null
Remove-Item task-definition.json
Write-Host "Task Definition registered" -ForegroundColor Green

# ============================================
# 12. CREATE ECS SERVICE
# ============================================

Write-Host "`n[12/12] Creating ECS Service..." -ForegroundColor Yellow

try {
    aws ecs create-service --cluster condominio-cluster --service-name condominio-api-service --task-definition condominio-api-task --desired-count 2 --launch-type FARGATE --platform-version LATEST --network-configuration "awsvpcConfiguration={subnets=[$SUBNET1,$SUBNET2],securityGroups=[$ECS_SG_ID],assignPublicIp=ENABLED}" --load-balancers "targetGroupArn=$TG_ARN,containerName=condominio-api,containerPort=8080" --health-check-grace-period-seconds 60 --region $AWS_REGION >$null
    Write-Host "ECS Service created" -ForegroundColor Green
    Write-Host "`nWaiting for service to stabilize (5-10 minutes)..." -ForegroundColor Gray
    aws ecs wait services-stable --cluster condominio-cluster --services condominio-api-service --region $AWS_REGION
    Write-Host "Service is stable and running!" -ForegroundColor Green
}
catch {
    Write-Host "ECS Service already exists or is being created" -ForegroundColor Green
}

# ============================================
# DEPLOYMENT COMPLETE
# ============================================

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "DEPLOYMENT COMPLETE!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Your API is now running on AWS!" -ForegroundColor Green
Write-Host ""
Write-Host "API URL:     http://$ALB_DNS" -ForegroundColor Yellow
Write-Host "Swagger UI:  http://$ALB_DNS/swagger" -ForegroundColor Yellow
Write-Host "Health:      http://$ALB_DNS/api/health" -ForegroundColor Yellow
Write-Host ""
Write-Host "Database:    $DB_ENDPOINT" -ForegroundColor Yellow
Write-Host ""
Write-Host "IMPORTANT: Initialize database schema!" -ForegroundColor Red
Write-Host "See DeployAWS.md Section 7.7 for instructions" -ForegroundColor Yellow
Write-Host ""
Write-Host "To view logs:" -ForegroundColor Cyan
Write-Host "  aws logs tail /ecs/condominio-api --follow --region $AWS_REGION" -ForegroundColor Gray
Write-Host ""
Write-Host "To check service status:" -ForegroundColor Cyan
Write-Host "  aws ecs describe-services --cluster condominio-cluster --services condominio-api-service --region $AWS_REGION" -ForegroundColor Gray
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
