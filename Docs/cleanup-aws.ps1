# Condominio API - AWS Cleanup Script
# This script deletes ALL AWS resources created for the Condominio API
# 
# WARNING: This will permanently delete all resources and data!
#
# Usage: .\cleanup-aws.ps1

$AWS_REGION = "us-east-1"

Write-Host "========================================" -ForegroundColor Red
Write-Host "AWS RESOURCE CLEANUP - CONDOMINIO API" -ForegroundColor Red
Write-Host "WARNING: This will DELETE all resources!" -ForegroundColor Red
Write-Host "========================================" -ForegroundColor Red
Write-Host ""

$confirmation = Read-Host "Are you sure you want to delete all resources? Type 'YES' to confirm"
if ($confirmation -ne "YES") {
    Write-Host "Cleanup cancelled." -ForegroundColor Yellow
    exit 0
}

Write-Host "`nStarting cleanup process..." -ForegroundColor Yellow
Write-Host ""

# ============================================
# 1. DELETE ECS SERVICE
# ============================================

Write-Host "[1/13] Deleting ECS Service..." -ForegroundColor Yellow
try {
    aws ecs update-service --cluster condominio-cluster --service condominio-api-service --desired-count 0 --region $AWS_REGION 2>$null
    Start-Sleep -Seconds 10
    aws ecs delete-service --cluster condominio-cluster --service condominio-api-service --force --region $AWS_REGION 2>$null
    Write-Host "ECS Service deleted" -ForegroundColor Green
}
catch {
    Write-Host "ECS Service not found or already deleted" -ForegroundColor Gray
}

# ============================================
# 2. DELETE ECS CLUSTER
# ============================================

Write-Host "`n[2/13] Deleting ECS Cluster..." -ForegroundColor Yellow
try {
    aws ecs delete-cluster --cluster condominio-cluster --region $AWS_REGION 2>$null
    Write-Host "ECS Cluster deleted" -ForegroundColor Green
}
catch {
    Write-Host "ECS Cluster not found or already deleted" -ForegroundColor Gray
}

# ============================================
# 3. DELETE APPLICATION LOAD BALANCER
# ============================================

Write-Host "`n[3/13] Deleting Application Load Balancer..." -ForegroundColor Yellow
try {
    $ALB_ARN = (aws elbv2 describe-load-balancers --names condominio-alb --query 'LoadBalancers[0].LoadBalancerArn' --output text --region $AWS_REGION 2>$null)
    if ($ALB_ARN -and $ALB_ARN -ne "None") {
        aws elbv2 delete-load-balancer --load-balancer-arn $ALB_ARN --region $AWS_REGION 2>$null
        Write-Host "ALB deleted (waiting 30 seconds...)" -ForegroundColor Green
        Start-Sleep -Seconds 30
    }
    else {
        Write-Host "ALB not found" -ForegroundColor Gray
    }
}
catch {
    Write-Host "ALB not found or already deleted" -ForegroundColor Gray
}

# ============================================
# 4. DELETE TARGET GROUP
# ============================================

Write-Host "`n[4/13] Deleting Target Group..." -ForegroundColor Yellow
try {
    $TG_ARN = (aws elbv2 describe-target-groups --names condominio-tg --query 'TargetGroups[0].TargetGroupArn' --output text --region $AWS_REGION 2>$null)
    if ($TG_ARN -and $TG_ARN -ne "None") {
        aws elbv2 delete-target-group --target-group-arn $TG_ARN --region $AWS_REGION 2>$null
        Write-Host "Target Group deleted" -ForegroundColor Green
    }
    else {
        Write-Host "Target Group not found" -ForegroundColor Gray
    }
}
catch {
    Write-Host "Target Group not found or already deleted" -ForegroundColor Gray
}

# ============================================
# 5. DELETE RDS INSTANCE
# ============================================

Write-Host "`n[5/13] Deleting RDS Instance..." -ForegroundColor Yellow
try {
    aws rds delete-db-instance --db-instance-identifier condominio-db --skip-final-snapshot --region $AWS_REGION 2>$null
    Write-Host "RDS instance deletion started (takes 5-10 minutes)" -ForegroundColor Green
    Write-Host "Waiting for deletion to complete..." -ForegroundColor Gray
    aws rds wait db-instance-deleted --db-instance-identifier condominio-db --region $AWS_REGION 2>$null
    Write-Host "RDS instance deleted" -ForegroundColor Green
}
catch {
    Write-Host "RDS instance not found or already deleted" -ForegroundColor Gray
}

# ============================================
# 6. DELETE RDS SUBNET GROUP
# ============================================

Write-Host "`n[6/13] Deleting RDS Subnet Group..." -ForegroundColor Yellow
try {
    aws rds delete-db-subnet-group --db-subnet-group-name condominio-db-subnet-group --region $AWS_REGION 2>$null
    Write-Host "RDS Subnet Group deleted" -ForegroundColor Green
}
catch {
    Write-Host "RDS Subnet Group not found or already deleted" -ForegroundColor Gray
}

# ============================================
# 7. DELETE SECURITY GROUPS
# ============================================

Write-Host "`n[7/13] Deleting Security Groups..." -ForegroundColor Yellow

$ALB_SG_ID = (aws ec2 describe-security-groups --filters "Name=group-name,Values=condominio-alb-sg" --query "SecurityGroups[0].GroupId" --output text --region $AWS_REGION 2>$null)
$ECS_SG_ID = (aws ec2 describe-security-groups --filters "Name=group-name,Values=condominio-ecs-sg" --query "SecurityGroups[0].GroupId" --output text --region $AWS_REGION 2>$null)
$DB_SG_ID = (aws ec2 describe-security-groups --filters "Name=group-name,Values=condominio-db-sg" --query "SecurityGroups[0].GroupId" --output text --region $AWS_REGION 2>$null)

if ($ALB_SG_ID -and $ALB_SG_ID -ne "None") {
    aws ec2 delete-security-group --group-id $ALB_SG_ID --region $AWS_REGION 2>$null
    Write-Host "ALB Security Group deleted" -ForegroundColor Green
}

Start-Sleep -Seconds 5

if ($ECS_SG_ID -and $ECS_SG_ID -ne "None") {
    aws ec2 delete-security-group --group-id $ECS_SG_ID --region $AWS_REGION 2>$null
    Write-Host "ECS Security Group deleted" -ForegroundColor Green
}

Start-Sleep -Seconds 5

if ($DB_SG_ID -and $DB_SG_ID -ne "None") {
    aws ec2 delete-security-group --group-id $DB_SG_ID --region $AWS_REGION 2>$null
    Write-Host "DB Security Group deleted" -ForegroundColor Green
}

if (-not $ALB_SG_ID -and -not $ECS_SG_ID -and -not $DB_SG_ID) {
    Write-Host "No Security Groups found" -ForegroundColor Gray
}

# ============================================
# 8. DELETE ECR REPOSITORY
# ============================================

Write-Host "`n[8/13] Deleting ECR Repository..." -ForegroundColor Yellow
try {
    aws ecr delete-repository --repository-name condominio-api --force --region $AWS_REGION 2>$null
    Write-Host "ECR Repository deleted (with all images)" -ForegroundColor Green
}
catch {
    Write-Host "ECR Repository not found or already deleted" -ForegroundColor Gray
}

# ============================================
# 9. DELETE SECRETS MANAGER SECRETS
# ============================================

Write-Host "`n[9/13] Deleting Secrets Manager Secrets..." -ForegroundColor Yellow
try {
    aws secretsmanager delete-secret --secret-id condominio/db/credentials --force-delete-without-recovery --region $AWS_REGION 2>$null
    Write-Host "Database credentials secret deleted" -ForegroundColor Green
}
catch {
    Write-Host "Database credentials secret not found" -ForegroundColor Gray
}

try {
    aws secretsmanager delete-secret --secret-id condominio/jwt/secret --force-delete-without-recovery --region $AWS_REGION 2>$null
    Write-Host "JWT secret deleted" -ForegroundColor Green
}
catch {
    Write-Host "JWT secret not found" -ForegroundColor Gray
}

# ============================================
# 10. DELETE CLOUDWATCH LOG GROUP
# ============================================

Write-Host "`n[10/13] Deleting CloudWatch Log Group..." -ForegroundColor Yellow
try {
    aws logs delete-log-group --log-group-name /ecs/condominio-api --region $AWS_REGION 2>$null
    Write-Host "CloudWatch Log Group deleted" -ForegroundColor Green
}
catch {
    Write-Host "CloudWatch Log Group not found or already deleted" -ForegroundColor Gray
}

# ============================================
# 11. DELETE IAM ROLE
# ============================================

Write-Host "`n[11/13] Deleting IAM Execution Role..." -ForegroundColor Yellow
try {
    aws iam detach-role-policy --role-name condominioECSExecutionRole --policy-arn arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy 2>$null
    aws iam delete-role-policy --role-name condominioECSExecutionRole --policy-name SecretsManagerAccess 2>$null
    aws iam delete-role --role-name condominioECSExecutionRole 2>$null
    Write-Host "IAM Execution Role deleted" -ForegroundColor Green
}
catch {
    Write-Host "IAM Execution Role not found or already deleted" -ForegroundColor Gray
}

# ============================================
# 12. DEREGISTER TASK DEFINITIONS
# ============================================

Write-Host "`n[12/13] Deregistering Task Definitions..." -ForegroundColor Yellow
try {
    $TASK_DEFS = (aws ecs list-task-definitions --family-prefix condominio-api-task --query 'taskDefinitionArns' --output text --region $AWS_REGION 2>$null)
    if ($TASK_DEFS) {
        $TASK_DEFS -split '\s+' | ForEach-Object {
            aws ecs deregister-task-definition --task-definition $_ --region $AWS_REGION 2>$null
        }
        Write-Host "Task Definitions deregistered" -ForegroundColor Green
    }
    else {
        Write-Host "No Task Definitions found" -ForegroundColor Gray
    }
}
catch {
    Write-Host "No Task Definitions found" -ForegroundColor Gray
}

# ============================================
# 13. CLEAN UP LOCAL FILES
# ============================================

Write-Host "`n[13/13] Cleaning up local files..." -ForegroundColor Yellow
Remove-Item task-definition.json -ErrorAction SilentlyContinue
Remove-Item trust-policy.json -ErrorAction SilentlyContinue
Remove-Item secrets-policy.json -ErrorAction SilentlyContinue
Write-Host "Local files cleaned up" -ForegroundColor Green

# ============================================
# CLEANUP COMPLETE
# ============================================

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "CLEANUP COMPLETE!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "All Condominio API resources have been deleted from AWS." -ForegroundColor Green
Write-Host ""
Write-Host "You can now run deploy-to-aws.ps1 to redeploy with a clean slate." -ForegroundColor Cyan
Write-Host ""
