# AWS Deployment Guide - Condominio API

Complete step-by-step guide to deploy your Condominio API to AWS using ECS Fargate with RDS MySQL.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [AWS Account Setup](#aws-account-setup)
3. [Create IAM User for Condominio API](#create-iam-user-for-condominio-api)
4. [Install and Configure AWS CLI](#install-and-configure-aws-cli)
5. [Create ECR Repository](#create-ecr-repository)
6. [Build and Push Docker Image](#build-and-push-docker-image)
7. [Create RDS MySQL Database](#create-rds-mysql-database)
8. [Setup AWS Secrets Manager](#setup-aws-secrets-manager)
9. [Create ECS Cluster](#create-ecs-cluster)
10. [Setup Networking](#setup-networking)
11. [Create Application Load Balancer](#create-application-load-balancer)
12. [Deploy ECS Service](#deploy-ecs-service)
13. [Testing and Verification](#testing-and-verification)
14. [Monitoring](#monitoring)
15. [Cost Optimization](#cost-optimization)
16. [Cleanup Resources](#cleanup-resources)

---

## Prerequisites

### Software Requirements

1. **Docker Desktop** - [Download](https://www.docker.com/products/docker-desktop/)
2. **AWS CLI v2** - [Download](https://aws.amazon.com/cli/)
3. **Git** - Already installed
4. **PowerShell** - Already available on Windows

### Knowledge Requirements

- Basic understanding of Docker containers
- AWS account with billing enabled
- Credit card for AWS (required even for free tier)

---

## AWS Account Setup

### Step 1: Create AWS Account (If not done)

1. Go to [aws.amazon.com](https://aws.amazon.com/)
2. Click "Create an AWS Account"
3. Follow the registration process
4. Verify your email and phone number
5. Add payment method (required)

### Step 2: Enable MFA on Root Account

**⚠️ CRITICAL SECURITY STEP - DO NOT SKIP**

1. Log in to AWS Console as root user
2. Click your account name (top right) → Security Credentials
3. Scroll to "Multi-factor authentication (MFA)"
4. Click "Assign MFA device"
5. Choose "Virtual MFA device"
6. Use Google Authenticator or Microsoft Authenticator app
7. Scan QR code and enter two consecutive codes

**Why?** Root account has unlimited access. MFA protects against unauthorized access.

---

## Create IAM User for Condominio API

**Best Practice:** Never use the root account for daily operations. Create dedicated IAM users.

### Step 1: Create IAM User via Console

1. **Login to AWS Console** (as root or existing admin user)
2. **Navigate to IAM**:
   - Search for "IAM" in the search bar
   - Click "IAM" (Identity and Access Management)

3. **Create User**:

   ```
   Left menu → Users → Create user

   User details:
   - User name: condominio-deployer
   - ✅ Provide user access to the AWS Management Console (optional, for GUI access)
   - Choose "I want to create an IAM user"
   - ✅ Autogenerate password (or custom password)
   - ✅ Users must create a new password at next sign-in

   Click "Next"
   ```

4. **Set Permissions**:

   ```
   Choose: "Attach policies directly"

   Search and select these policies:
   ✅ AmazonEC2ContainerRegistryFullAccess
   ✅ AmazonECS_FullAccess
   ✅ AmazonRDSFullAccess
   ✅ AmazonVPCFullAccess
   ✅ IAMFullAccess (needed to create ECS task execution roles)
   ✅ SecretsManagerReadWrite
   ✅ CloudWatchLogsFullAccess
   ✅ ElasticLoadBalancingFullAccess

   Click "Next"
   ```

5. **Review and Create**:

   ```
   Review the user details
   Click "Create user"

   ⚠️ IMPORTANT: Download or save:
   - User name: condominio-deployer
   - Password (if console access enabled)
   - Console sign-in URL
   ```

### Step 2: Create Access Keys for CLI

1. **Navigate to the user**:

   ```
   IAM → Users → condominio-deployer
   ```

2. **Create access key**:

   ```
   Click "Security credentials" tab
   Scroll to "Access keys"
   Click "Create access key"

   Use case: Command Line Interface (CLI)
   ✅ Check "I understand the above recommendation..."
   Click "Next"

   Description tag (optional): "Condominio API deployment from local machine"
   Click "Create access key"
   ```

3. **Save Credentials Securely**:

   ```
   ⚠️ CRITICAL - Save these immediately, you cannot retrieve them later:

   Access key ID: AKIAIOSFODNN7EXAMPLE
   Secret access key: wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY

   Options:
   - Download .csv file
   - Copy to password manager (recommended)
   - Write down securely

   Click "Done"
   ```

### Step 3: Enable MFA for IAM User (Recommended)

1. Still in the user's "Security credentials" tab
2. Scroll to "Multi-factor authentication (MFA)"
3. Click "Assign MFA device"
4. Follow same steps as root account MFA setup

---

## Install and Configure AWS CLI

### Step 1: Install AWS CLI v2

```powershell
# Download AWS CLI installer for Windows
# Visit: https://aws.amazon.com/cli/

# Or use winget (if available)
winget install Amazon.AWSCLI

# Verify installation
aws --version
# Expected: aws-cli/2.x.x Python/3.x.x Windows/x.x.x
```

### Step 2: Configure AWS CLI

```powershell
# Configure with your IAM user credentials
aws configure

# You will be prompted for:
AWS Access Key ID [None]: AKIAIOSFODNN7EXAMPLE
AWS Secret Access Key [None]: wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY
Default region name [None]: us-east-1
Default output format [None]: json
```

**Region Selection:**

- `us-east-1` (North Virginia) - Cheapest, most services
- `us-west-2` (Oregon) - Good alternative
- Choose based on your location/requirements

### Step 3: Test Configuration

```powershell
# Test credentials
aws sts get-caller-identity

# Expected output:
# {
#     "UserId": "AIDAJEXAMPLEID",
#     "Account": "123456789012",
#     "Arn": "arn:aws:iam::123456789012:user/condominio-deployer"
# }
```

**If you see errors:**

- "Unable to locate credentials" → Run `aws configure` again
- "Access Denied" → Check IAM permissions
- "Invalid security token" → Credentials might be wrong

---

## Create ECR Repository

ECR (Elastic Container Registry) is AWS's Docker registry where we'll store our container images.

### Step 1: Create Repository

```powershell
# Set your AWS region
$AWS_REGION = "us-east-1"

# Create ECR repository
aws ecr create-repository `
    --repository-name condominio-api `
    --region $AWS_REGION `
    --image-scanning-configuration scanOnPush=true `
    --encryption-configuration encryptionType=AES256

# Save the repository URI from output
# Example: 123456789012.dkr.ecr.us-east-1.amazonaws.com/condominio-api
```

**Expected Output:**

```json
{
  "repository": {
    "repositoryArn": "arn:aws:ecr:us-east-1:123456789012:repository/condominio-api",
    "registryId": "123456789012",
    "repositoryName": "condominio-api",
    "repositoryUri": "123456789012.dkr.ecr.us-east-1.amazonaws.com/condominio-api",
    "createdAt": "2026-05-06T10:00:00-05:00"
  }
}
```

### Step 2: Save Repository Details

```powershell
# Get your AWS Account ID
$AWS_ACCOUNT_ID = (aws sts get-caller-identity --query Account --output text)

# Construct ECR URI
$ECR_REPO_URI = "$AWS_ACCOUNT_ID.dkr.ecr.$AWS_REGION.amazonaws.com/condominio-api"

# Display for verification
Write-Host "Account ID: $AWS_ACCOUNT_ID"
Write-Host "ECR Repository URI: $ECR_REPO_URI"

# Save to environment variable for later use
[Environment]::SetEnvironmentVariable("ECR_REPO_URI", $ECR_REPO_URI, "User")
```

---

## Build and Push Docker Image

### Step 1: Navigate to Project

```powershell
cd C:\Jose\code\FullStackMaster\MSD_CondoAdminBE\CondominioAPI
```

### Step 2: Authenticate Docker to ECR

```powershell
# Login to ECR
aws ecr get-login-password --region $AWS_REGION | docker login --username AWS --password-stdin $ECR_REPO_URI

# Expected: "Login Succeeded"
```

### Step 3: Build Docker Image

```powershell
# Build image with production tag
# This will clone from GitHub main branch
docker build `
    --build-arg GIT_BRANCH=main `
    -t condominio-api:latest `
    -f Dockerfile .

# Build takes 5-10 minutes on first run
```

**If you want to deploy from a specific branch:**

```powershell
# Deploy from feature branch
docker build `
    --build-arg GIT_BRANCH=feature/Backend_AWS_Deployment `
    -t condominio-api:latest `
    -f Dockerfile .
```

### Step 4: Tag Image for ECR

```powershell
# Tag with latest
docker tag condominio-api:latest ${ECR_REPO_URI}:latest

# Tag with version (good practice)
docker tag condominio-api:latest ${ECR_REPO_URI}:v1.0.0

# Verify tags
docker images | Select-String "condominio"
```

### Step 5: Push to ECR

```powershell
# Push latest tag
docker push ${ECR_REPO_URI}:latest

# Push version tag
docker push ${ECR_REPO_URI}:v1.0.0

# Push takes 2-5 minutes depending on internet speed
```

### Step 6: Verify Image in ECR

```powershell
# List images in repository
aws ecr describe-images `
    --repository-name condominio-api `
    --region $AWS_REGION

# Should show both tags: latest and v1.0.0
```

**Via Console (Optional):**

1. Go to AWS Console → ECR
2. Click on "condominio-api" repository
3. Verify you see your images with tags

---

## Create RDS MySQL Database

### Step 1: Get VPC Information

```powershell
# Get default VPC ID
$VPC_ID = (aws ec2 describe-vpcs `
    --filters "Name=isDefault,Values=true" `
    --query "Vpcs[0].VpcId" `
    --output text `
    --region $AWS_REGION)

Write-Host "VPC ID: $VPC_ID"

# Get subnet IDs (need at least 2 in different AZs for RDS)
$SUBNETS = (aws ec2 describe-subnets `
    --filters "Name=vpc-id,Values=$VPC_ID" `
    --query "Subnets[*].SubnetId" `
    --output text `
    --region $AWS_REGION)

$SUBNET_ARRAY = $SUBNETS -split '\s+'
Write-Host "Subnets: $SUBNET_ARRAY"
```

### Step 2: Create DB Subnet Group

```powershell
# Create subnet group (RDS requires at least 2 subnets in different AZs)
aws rds create-db-subnet-group `
    --db-subnet-group-name condominio-db-subnet-group `
    --db-subnet-group-description "Subnet group for Condominio API database" `
    --subnet-ids $SUBNET_ARRAY[0] $SUBNET_ARRAY[1] `
    --region $AWS_REGION
```

### Step 3: Create Security Group for RDS

```powershell
# Create security group
$DB_SG_ID = (aws ec2 create-security-group `
    --group-name condominio-db-sg `
    --description "Security group for Condominio MySQL database" `
    --vpc-id $VPC_ID `
    --region $AWS_REGION `
    --query 'GroupId' `
    --output text)

Write-Host "DB Security Group ID: $DB_SG_ID"

# Note: We'll add ingress rules later after creating ECS security group
```

### Step 4: Create RDS MySQL Instance

```powershell
# Set database credentials (CHANGE THESE!)
$DB_USERNAME = "admin"
$DB_PASSWORD = "CondominioSecurePass123!"  # Must be 8+ chars, include uppercase, lowercase, numbers
$DB_NAME = "Condominio2"

# Create RDS instance
aws rds create-db-instance `
    --db-instance-identifier condominio-db `
    --db-instance-class db.t3.micro `
    --engine mysql `
    --engine-version 8.0.35 `
    --master-username $DB_USERNAME `
    --master-user-password $DB_PASSWORD `
    --allocated-storage 20 `
    --storage-type gp3 `
    --db-subnet-group-name condominio-db-subnet-group `
    --vpc-security-group-ids $DB_SG_ID `
    --backup-retention-period 7 `
    --preferred-backup-window "03:00-04:00" `
    --preferred-maintenance-window "mon:04:00-mon:05:00" `
    --no-publicly-accessible `
    --region $AWS_REGION

Write-Host "Creating RDS instance... This takes 5-10 minutes"
```

**RDS Configuration Explained:**

- `db.t3.micro` - Free tier eligible, 1 vCPU, 1 GB RAM
- `gp3` - Latest generation SSD storage
- `20 GB` - Minimum storage (can auto-scale)
- `no-publicly-accessible` - Security best practice
- `backup-retention-period 7` - Keep backups for 7 days

### Step 5: Wait for Database to be Available

```powershell
# Wait for database (this command blocks until ready)
Write-Host "Waiting for database to be available (5-10 minutes)..."
aws rds wait db-instance-available `
    --db-instance-identifier condominio-db `
    --region $AWS_REGION

Write-Host "Database is now available!"
```

### Step 6: Get Database Endpoint

```powershell
# Get database endpoint
$DB_ENDPOINT = (aws rds describe-db-instances `
    --db-instance-identifier condominio-db `
    --query 'DBInstances[0].Endpoint.Address' `
    --output text `
    --region $AWS_REGION)

Write-Host "Database Endpoint: $DB_ENDPOINT"
Write-Host "Database Port: 3306"
Write-Host "Database Name: $DB_NAME"

# Save for later use
[Environment]::SetEnvironmentVariable("DB_ENDPOINT", $DB_ENDPOINT, "User")
```

### Step 7: Initialize Database Schema

Since RDS doesn't have direct access to your SQL file, we'll use a temporary method:

**Option A: Using MySQL Workbench (Recommended)**

1. **Install MySQL Workbench** - [Download](https://dev.mysql.com/downloads/workbench/)

2. **Setup SSH Tunnel** (Required because RDS is not publicly accessible):

   ```powershell
   # You'll need a bastion host/jump server in the VPC
   # For now, we'll temporarily make RDS publicly accessible

   # TEMPORARY - Make RDS publicly accessible for initial setup
   aws rds modify-db-instance `
       --db-instance-identifier condominio-db `
       --publicly-accessible `
       --apply-immediately `
       --region $AWS_REGION

   # Wait for modification
   aws rds wait db-instance-available `
       --db-instance-identifier condominio-db `
       --region $AWS_REGION

   # Update security group to allow your IP
   $MY_IP = (Invoke-RestMethod -Uri "https://api.ipify.org?format=text")

   aws ec2 authorize-security-group-ingress `
       --group-id $DB_SG_ID `
       --protocol tcp `
       --port 3306 `
       --cidr "$MY_IP/32" `
       --region $AWS_REGION
   ```

3. **Connect with MySQL Workbench**:

   ```
   Connection Name: Condominio AWS RDS
   Hostname: [Your DB_ENDPOINT]
   Port: 3306
   Username: admin
   Password: [Your DB_PASSWORD]

   Click "Test Connection"
   Click "OK" to save
   ```

4. **Run SQL Script**:

   ```
   Open connection
   File → Open SQL Script
   Navigate to: C:\Jose\code\FullStackMaster\MSD_CondoAdminBE\CondominioAPI\Database\Db_Schema.sql
   Execute (⚡ button or Ctrl+Shift+Enter)
   Verify: SHOW TABLES;
   ```

5. **Verify Tables Created**:

   ```sql
   USE Condominio2;
   SHOW TABLES;

   -- Should show all tables in lowercase:
   -- users, roles, user_roles, payment_status, etc.
   ```

6. **IMPORTANT - Secure RDS Again**:

   ```powershell
   # Remove public access
   aws rds modify-db-instance `
       --db-instance-identifier condominio-db `
       --no-publicly-accessible `
       --apply-immediately `
       --region $AWS_REGION

   # Remove your IP from security group
   aws ec2 revoke-security-group-ingress `
       --group-id $DB_SG_ID `
       --protocol tcp `
       --port 3306 `
       --cidr "$MY_IP/32" `
       --region $AWS_REGION
   ```

**Option B: Using AWS Cloud9 (Alternative)**

1. Create Cloud9 environment in same VPC
2. Install MySQL client: `sudo yum install mysql -y`
3. Upload Db_Schema.sql
4. Connect: `mysql -h $DB_ENDPOINT -u admin -p`
5. Run: `source Db_Schema.sql`

---

## Setup AWS Secrets Manager

Store sensitive credentials securely instead of hardcoding.

### Step 1: Create Database Credentials Secret

```powershell
# Create secret for database credentials
$DB_SECRET_ARN = (aws secretsmanager create-secret `
    --name condominio/db/credentials `
    --description "Database credentials for Condominio API" `
    --secret-string "{`"username`":`"$DB_USERNAME`",`"password`":`"$DB_PASSWORD`",`"host`":`"$DB_ENDPOINT`",`"port`":3306,`"dbname`":`"$DB_NAME`"}" `
    --region $AWS_REGION `
    --query 'ARN' `
    --output text)

Write-Host "Database Secret ARN: $DB_SECRET_ARN"
```

### Step 2: Create JWT Secret

```powershell
# Generate a secure JWT secret (or use your own)
$JWT_SECRET = "clave_super_secreta_para_jwt_1234567890_PRODUCTION"

# Create secret
$JWT_SECRET_ARN = (aws secretsmanager create-secret `
    --name condominio/jwt/secret `
    --description "JWT secret key for Condominio API" `
    --secret-string $JWT_SECRET `
    --region $AWS_REGION `
    --query 'ARN' `
    --output text)

Write-Host "JWT Secret ARN: $JWT_SECRET_ARN"
```

### Step 3: Verify Secrets

```powershell
# List secrets
aws secretsmanager list-secrets --region $AWS_REGION

# View secret value (for verification)
aws secretsmanager get-secret-value `
    --secret-id condominio/db/credentials `
    --region $AWS_REGION `
    --query 'SecretString' `
    --output text
```

---

## Create ECS Cluster

### Step 1: Create Cluster

```powershell
# Create ECS cluster (Fargate mode - serverless)
aws ecs create-cluster `
    --cluster-name condominio-cluster `
    --region $AWS_REGION

Write-Host "ECS Cluster created successfully"
```

### Step 2: Create CloudWatch Log Group

```powershell
# Create log group for API logs
aws logs create-log-group `
    --log-group-name /ecs/condominio-api `
    --region $AWS_REGION

# Set retention to 7 days (saves costs)
aws logs put-retention-policy `
    --log-group-name /ecs/condominio-api `
    --retention-in-days 7 `
    --region $AWS_REGION

Write-Host "CloudWatch log group created"
```

### Step 3: Create IAM Execution Role

ECS needs permission to pull images from ECR and read secrets.

```powershell
# Create trust policy for ECS
@"
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Principal": {
        "Service": "ecs-tasks.amazonaws.com"
      },
      "Action": "sts:AssumeRole"
    }
  ]
}
"@ | Out-File -FilePath trust-policy.json -Encoding utf8

# Create role
aws iam create-role `
    --role-name condominioECSExecutionRole `
    --assume-role-policy-document file://trust-policy.json `
    --region $AWS_REGION

# Attach AWS managed policy for ECS task execution
aws iam attach-role-policy `
    --role-name condominioECSExecutionRole `
    --policy-arn arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy

# Create custom policy for Secrets Manager access
@"
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "secretsmanager:GetSecretValue"
      ],
      "Resource": [
        "$DB_SECRET_ARN",
        "$JWT_SECRET_ARN"
      ]
    }
  ]
}
"@ | Out-File -FilePath secrets-policy.json -Encoding utf8

# Attach custom policy
aws iam put-role-policy `
    --role-name condominioECSExecutionRole `
    --policy-name SecretsManagerAccess `
    --policy-document file://secrets-policy.json

# Get role ARN
$EXECUTION_ROLE_ARN = (aws iam get-role `
    --role-name condominioECSExecutionRole `
    --query 'Role.Arn' `
    --output text)

Write-Host "Execution Role ARN: $EXECUTION_ROLE_ARN"

# Clean up temp files
Remove-Item trust-policy.json, secrets-policy.json
```

---

## Setup Networking

### Step 1: Create Security Group for ECS Tasks

```powershell
# Create security group for ECS tasks
$ECS_SG_ID = (aws ec2 create-security-group `
    --group-name condominio-ecs-sg `
    --description "Security group for Condominio ECS tasks" `
    --vpc-id $VPC_ID `
    --region $AWS_REGION `
    --query 'GroupId' `
    --output text)

Write-Host "ECS Security Group ID: $ECS_SG_ID"
```

### Step 2: Allow Database Access from ECS

```powershell
# Allow ECS tasks to connect to RDS
aws ec2 authorize-security-group-ingress `
    --group-id $DB_SG_ID `
    --protocol tcp `
    --port 3306 `
    --source-group $ECS_SG_ID `
    --region $AWS_REGION

Write-Host "Database security group updated to allow ECS access"
```

---

## Create Application Load Balancer

### Step 1: Create Security Group for ALB

```powershell
# Create security group for ALB
$ALB_SG_ID = (aws ec2 create-security-group `
    --group-name condominio-alb-sg `
    --description "Security group for Condominio Application Load Balancer" `
    --vpc-id $VPC_ID `
    --region $AWS_REGION `
    --query 'GroupId' `
    --output text)

# Allow HTTP from anywhere
aws ec2 authorize-security-group-ingress `
    --group-id $ALB_SG_ID `
    --protocol tcp `
    --port 80 `
    --cidr 0.0.0.0/0 `
    --region $AWS_REGION

# Allow HTTPS from anywhere (for future SSL)
aws ec2 authorize-security-group-ingress `
    --group-id $ALB_SG_ID `
    --protocol tcp `
    --port 443 `
    --cidr 0.0.0.0/0 `
    --region $AWS_REGION

Write-Host "ALB Security Group ID: $ALB_SG_ID"
```

### Step 2: Allow ALB to Access ECS Tasks

```powershell
# Allow ALB to connect to ECS tasks on port 8080
aws ec2 authorize-security-group-ingress `
    --group-id $ECS_SG_ID `
    --protocol tcp `
    --port 8080 `
    --source-group $ALB_SG_ID `
    --region $AWS_REGION
```

### Step 3: Create Application Load Balancer

```powershell
# Get public subnets (need at least 2 in different AZs)
$PUBLIC_SUBNETS = (aws ec2 describe-subnets `
    --filters "Name=vpc-id,Values=$VPC_ID" "Name=default-for-az,Values=true" `
    --query "Subnets[*].SubnetId" `
    --output text `
    --region $AWS_REGION)

$SUBNET1 = ($PUBLIC_SUBNETS -split '\s+')[0]
$SUBNET2 = ($PUBLIC_SUBNETS -split '\s+')[1]

# Create ALB
$ALB_ARN = (aws elbv2 create-load-balancer `
    --name condominio-alb `
    --subnets $SUBNET1 $SUBNET2 `
    --security-groups $ALB_SG_ID `
    --scheme internet-facing `
    --type application `
    --ip-address-type ipv4 `
    --region $AWS_REGION `
    --query 'LoadBalancers[0].LoadBalancerArn' `
    --output text)

Write-Host "ALB ARN: $ALB_ARN"
```

### Step 4: Create Target Group

```powershell
# Create target group for ECS tasks
$TG_ARN = (aws elbv2 create-target-group `
    --name condominio-tg `
    --protocol HTTP `
    --port 8080 `
    --vpc-id $VPC_ID `
    --target-type ip `
    --health-check-enabled `
    --health-check-protocol HTTP `
    --health-check-path /api/health `
    --health-check-interval-seconds 30 `
    --health-check-timeout-seconds 5 `
    --healthy-threshold-count 2 `
    --unhealthy-threshold-count 3 `
    --region $AWS_REGION `
    --query 'TargetGroups[0].TargetGroupArn' `
    --output text)

Write-Host "Target Group ARN: $TG_ARN"
```

### Step 5: Create Listener

```powershell
# Create HTTP listener (forwards traffic to target group)
aws elbv2 create-listener `
    --load-balancer-arn $ALB_ARN `
    --protocol HTTP `
    --port 80 `
    --default-actions Type=forward,TargetGroupArn=$TG_ARN `
    --region $AWS_REGION

Write-Host "Listener created"
```

### Step 6: Get ALB DNS Name

```powershell
# Get ALB DNS name (this is your API URL)
$ALB_DNS = (aws elbv2 describe-load-balancers `
    --load-balancer-arns $ALB_ARN `
    --query 'LoadBalancers[0].DNSName' `
    --output text `
    --region $AWS_REGION)

Write-Host "========================================="
Write-Host "ALB DNS Name: $ALB_DNS"
Write-Host "API will be available at: http://$ALB_DNS"
Write-Host "========================================="
```

---

## Deploy ECS Service

### Step 1: Create Task Definition

```powershell
# Get account ID
$AWS_ACCOUNT_ID = (aws sts get-caller-identity --query Account --output text)

# Create task definition JSON
@"
{
  "family": "condominio-api-task",
  "networkMode": "awsvpc",
  "requiresCompatibilities": ["FARGATE"],
  "cpu": "512",
  "memory": "1024",
  "executionRoleArn": "$EXECUTION_ROLE_ARN",
  "containerDefinitions": [
    {
      "name": "condominio-api",
      "image": "${ECR_REPO_URI}:latest",
      "essential": true,
      "portMappings": [
        {
          "containerPort": 8080,
          "protocol": "tcp"
        }
      ],
      "environment": [
        {
          "name": "DB_SERVER",
          "value": "$DB_ENDPOINT"
        },
        {
          "name": "DB_NAME",
          "value": "$DB_NAME"
        },
        {
          "name": "DB_USER",
          "value": "$DB_USERNAME"
        },
        {
          "name": "CORS_ALLOWED_ORIGIN",
          "value": "http://$ALB_DNS"
        },
        {
          "name": "LOG_PATH",
          "value": "/app/Logs/log-.txt"
        },
        {
          "name": "ASPNETCORE_ENVIRONMENT",
          "value": "Production"
        }
      ],
      "secrets": [
        {
          "name": "DB_PASSWORD",
          "valueFrom": "$DB_SECRET_ARN:password::"
        },
        {
          "name": "JWT_SECRET_KEY",
          "valueFrom": "$JWT_SECRET_ARN"
        }
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
    }
  ]
}
"@ | Out-File -FilePath task-definition.json -Encoding utf8

# Register task definition
$TASK_DEF_ARN = (aws ecs register-task-definition `
    --cli-input-json file://task-definition.json `
    --region $AWS_REGION `
    --query 'taskDefinition.taskDefinitionArn' `
    --output text)

Write-Host "Task Definition ARN: $TASK_DEF_ARN"

# Clean up
Remove-Item task-definition.json
```

### Step 2: Create ECS Service

```powershell
# Create ECS service
aws ecs create-service `
    --cluster condominio-cluster `
    --service-name condominio-api-service `
    --task-definition condominio-api-task `
    --desired-count 2 `
    --launch-type FARGATE `
    --platform-version LATEST `
    --network-configuration "awsvpcConfiguration={subnets=[$SUBNET1,$SUBNET2],securityGroups=[$ECS_SG_ID],assignPublicIp=ENABLED}" `
    --load-balancers "targetGroupArn=$TG_ARN,containerName=condominio-api,containerPort=8080" `
    --health-check-grace-period-seconds 60 `
    --region $AWS_REGION

Write-Host "ECS Service created. Deployment in progress..."
```

**Configuration Explained:**

- `desired-count 2` - Runs 2 containers for high availability
- `FARGATE` - Serverless (no EC2 instances to manage)
- `assignPublicIp=ENABLED` - Required for Fargate to pull from ECR
- `health-check-grace-period` - Time before health checks start

### Step 3: Wait for Service to Stabilize

```powershell
Write-Host "Waiting for service to become stable (5-10 minutes)..."
Write-Host "This means:"
Write-Host "  - Tasks are running"
Write-Host "  - Containers are healthy"
Write-Host "  - Target group shows healthy targets"
Write-Host ""

aws ecs wait services-stable `
    --cluster condominio-cluster `
    --services condominio-api-service `
    --region $AWS_REGION

Write-Host "Service is stable!"
```

---

## Testing and Verification

### Step 1: Check Service Status

```powershell
# Check service status
aws ecs describe-services `
    --cluster condominio-cluster `
    --services condominio-api-service `
    --region $AWS_REGION `
    --query 'services[0].{Status:status,Running:runningCount,Desired:desiredCount,Pending:pendingCount}'
```

**Expected:**

```json
{
  "Status": "ACTIVE",
  "Running": 2,
  "Desired": 2,
  "Pending": 0
}
```

### Step 2: Check Target Health

```powershell
# Check if targets are healthy
aws elbv2 describe-target-health `
    --target-group-arn $TG_ARN `
    --region $AWS_REGION
```

**Expected:** All targets should show `State: healthy`

### Step 3: Test API Endpoints

```powershell
# Test health check
Invoke-WebRequest -Uri "http://$ALB_DNS/api/health" -Method GET

# Test Swagger UI
Start-Process "http://$ALB_DNS/swagger"

# Test login endpoint
$loginBody = @{
    login = "usa"
    password = "sa"
} | ConvertTo-Json

$response = Invoke-RestMethod `
    -Uri "http://$ALB_DNS/api/Login" `
    -Method POST `
    -Body $loginBody `
    -ContentType "application/json"

# Display response
$response | ConvertTo-Json -Depth 10
```

**Expected Login Response:**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": 1,
  "userName": "sa"
}
```

### Step 4: View Logs

```powershell
# View real-time logs
aws logs tail /ecs/condominio-api --follow --region $AWS_REGION

# View last 100 lines
aws logs tail /ecs/condominio-api --since 1h --region $AWS_REGION

# Filter for errors
aws logs tail /ecs/condominio-api --filter-pattern "ERROR" --follow --region $AWS_REGION
```

---

## Monitoring

### CloudWatch Dashboards

1. **Go to CloudWatch Console**
2. **Create Dashboard**:

   ```
   Dashboard name: Condominio-API-Dashboard

   Add widgets:
   - ECS Service CPU Utilization
   - ECS Service Memory Utilization
   - ALB Request Count
   - ALB Target Response Time
   - RDS CPU Utilization
   - RDS Database Connections
   ```

### Set Up Alarms

```powershell
# CPU utilization alarm
aws cloudwatch put-metric-alarm `
    --alarm-name condominio-high-cpu `
    --alarm-description "Alert when CPU > 80%" `
    --metric-name CPUUtilization `
    --namespace AWS/ECS `
    --statistic Average `
    --period 300 `
    --threshold 80 `
    --comparison-operator GreaterThanThreshold `
    --evaluation-periods 2 `
    --dimensions Name=ServiceName,Value=condominio-api-service Name=ClusterName,Value=condominio-cluster `
    --region $AWS_REGION

# Unhealthy targets alarm
aws cloudwatch put-metric-alarm `
    --alarm-name condominio-unhealthy-targets `
    --alarm-description "Alert when targets are unhealthy" `
    --metric-name UnHealthyHostCount `
    --namespace AWS/ApplicationELB `
    --statistic Average `
    --period 60 `
    --threshold 1 `
    --comparison-operator GreaterThanOrEqualToThreshold `
    --evaluation-periods 2 `
    --dimensions Name=TargetGroup,Value=$(Split-Path -Leaf $TG_ARN) Name=LoadBalancer,Value=$(Split-Path -Leaf $ALB_ARN) `
    --region $AWS_REGION
```

---

## Cost Optimization

### Current Cost Estimate (Monthly)

```
RDS db.t3.micro (20GB gp3):        ~$15-20/month
ECS Fargate (2 tasks, 0.5 vCPU):   ~$30-40/month
ALB:                                ~$20-25/month
Data Transfer:                      ~$5-10/month (depends on usage)
CloudWatch Logs (7 day retention):  ~$2-5/month
Secrets Manager:                    ~$1/month
---------------------------------------------------
TOTAL:                              ~$73-101/month
```

### Free Tier (First 12 Months)

- RDS: 750 hours/month of db.t2.micro (not t3.micro)
- Fargate: Limited free tier
- ALB: Not included in free tier
- **Estimated with Free Tier: ~$50-70/month**

### Cost Reduction Tips

1. **Reduce Task Count**:

   ```powershell
   # Run only 1 task instead of 2
   aws ecs update-service `
       --cluster condominio-cluster `
       --service condominio-api-service `
       --desired-count 1 `
       --region $AWS_REGION
   ```

2. **Use Smaller RDS Instance** (if performance allows):

   ```powershell
   # Change to db.t3.micro (smallest) or db.t2.micro (free tier)
   aws rds modify-db-instance `
       --db-instance-identifier condominio-db `
       --db-instance-class db.t2.micro `
       --apply-immediately `
       --region $AWS_REGION
   ```

3. **Stop Resources When Not in Use**:

   ```powershell
   # Stop ECS service (no charges)
   aws ecs update-service `
       --cluster condominio-cluster `
       --service condominio-api-service `
       --desired-count 0 `
       --region $AWS_REGION

   # Stop RDS (backup retained, small storage charge)
   aws rds stop-db-instance `
       --db-instance-identifier condominio-db `
       --region $AWS_REGION
   ```

4. **Use Spot Instances** (Advanced):
   - Not available for Fargate
   - Switch to EC2 launch type with Spot for ~70% savings

---

## Cleanup Resources

### ⚠️ WARNING: This will delete all resources and data!

```powershell
# Delete ECS service
aws ecs update-service `
    --cluster condominio-cluster `
    --service condominio-api-service `
    --desired-count 0 `
    --region $AWS_REGION

aws ecs delete-service `
    --cluster condominio-cluster `
    --service condominio-api-service `
    --force `
    --region $AWS_REGION

# Delete ECS cluster
aws ecs delete-cluster `
    --cluster condominio-cluster `
    --region $AWS_REGION

# Delete ALB
aws elbv2 delete-load-balancer `
    --load-balancer-arn $ALB_ARN `
    --region $AWS_REGION

# Delete target group (wait 30 seconds after deleting ALB)
Start-Sleep -Seconds 30
aws elbv2 delete-target-group `
    --target-group-arn $TG_ARN `
    --region $AWS_REGION

# Delete RDS instance (no final snapshot)
aws rds delete-db-instance `
    --db-instance-identifier condominio-db `
    --skip-final-snapshot `
    --region $AWS_REGION

# Delete RDS subnet group (after RDS is deleted)
aws rds delete-db-subnet-group `
    --db-subnet-group-name condominio-db-subnet-group `
    --region $AWS_REGION

# Delete security groups
aws ec2 delete-security-group --group-id $ALB_SG_ID --region $AWS_REGION
aws ec2 delete-security-group --group-id $ECS_SG_ID --region $AWS_REGION
aws ec2 delete-security-group --group-id $DB_SG_ID --region $AWS_REGION

# Delete ECR repository (and all images)
aws ecr delete-repository `
    --repository-name condominio-api `
    --force `
    --region $AWS_REGION

# Delete secrets
aws secretsmanager delete-secret `
    --secret-id condominio/db/credentials `
    --force-delete-without-recovery `
    --region $AWS_REGION

aws secretsmanager delete-secret `
    --secret-id condominio/jwt/secret `
    --force-delete-without-recovery `
    --region $AWS_REGION

# Delete CloudWatch log group
aws logs delete-log-group `
    --log-group-name /ecs/condominio-api `
    --region $AWS_REGION

# Delete IAM role
aws iam detach-role-policy `
    --role-name condominioECSExecutionRole `
    --policy-arn arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy

aws iam delete-role-policy `
    --role-name condominioECSExecutionRole `
    --policy-name SecretsManagerAccess

aws iam delete-role `
    --role-name condominioECSExecutionRole

Write-Host "All resources deleted"
```

---

## Troubleshooting Common Issues

### Issue 1: ECS Tasks Fail to Start

**Check:**

```powershell
# Get task ID
$TASK_ARN = (aws ecs list-tasks `
    --cluster condominio-cluster `
    --service-name condominio-api-service `
    --desired-status STOPPED `
    --region $AWS_REGION `
    --query 'taskArns[0]' `
    --output text)

# Get stopped reason
aws ecs describe-tasks `
    --cluster condominio-cluster `
    --tasks $TASK_ARN `
    --region $AWS_REGION `
    --query 'tasks[0].{Reason:stoppedReason,Containers:containers[0].reason}'
```

**Common Causes:**

- Cannot pull image from ECR (check execution role permissions)
- Cannot read secrets (check secrets ARNs and permissions)
- Health check failing (check /api/health endpoint)

### Issue 2: Targets Unhealthy in ALB

```powershell
# Check target health
aws elbv2 describe-target-health `
    --target-group-arn $TG_ARN `
    --region $AWS_REGION

# Common fixes:
# 1. Check security group allows ALB → ECS on port 8080
# 2. Verify health check path /api/health returns 200
# 3. Check ECS task logs for errors
```

### Issue 3: Cannot Connect to Database

```powershell
# Verify security group rules
aws ec2 describe-security-group-rules `
    --filters "Name=group-id,Values=$DB_SG_ID" `
    --region $AWS_REGION

# Should show ingress rule allowing port 3306 from ECS security group
```

**Fix:**

```powershell
# Add rule if missing
aws ec2 authorize-security-group-ingress `
    --group-id $DB_SG_ID `
    --protocol tcp `
    --port 3306 `
    --source-group $ECS_SG_ID `
    --region $AWS_REGION
```

### Issue 4: "Access Denied" Errors

**Common causes:**

- IAM user lacks permissions
- Execution role lacks permissions
- Secrets Manager permissions not configured

**Check IAM user permissions:**

```powershell
aws iam list-attached-user-policies --user-name condominio-deployer
```

---

## Next Steps

### 1. Setup Custom Domain (Optional)

1. **Register domain** (Route 53 or external)
2. **Request SSL certificate** (AWS Certificate Manager)
3. **Add HTTPS listener** to ALB
4. **Create Route 53 alias** pointing to ALB

### 2. Setup CI/CD Pipeline

Use AWS CodePipeline to auto-deploy on git push:

1. Create CodePipeline
2. Connect to GitHub repository
3. Build with CodeBuild
4. Deploy to ECS

### 3. Enable Auto-Scaling

```powershell
# Auto-scale based on CPU
aws application-autoscaling register-scalable-target `
    --service-namespace ecs `
    --resource-id service/condominio-cluster/condominio-api-service `
    --scalable-dimension ecs:service:DesiredCount `
    --min-capacity 2 `
    --max-capacity 10 `
    --region $AWS_REGION
```

### 4. Backup Strategy

- RDS automated backups (already enabled, 7 days)
- Consider RDS snapshots before major changes
- Export ECR images for disaster recovery

---

## Summary

You've successfully deployed your Condominio API to AWS! 🎉

**Your API is now:**

- ✅ Running on ECS Fargate (serverless containers)
- ✅ Using RDS MySQL (managed database)
- ✅ Behind Application Load Balancer (high availability)
- ✅ Secured with Secrets Manager (no hardcoded passwords)
- ✅ Monitored with CloudWatch (logs and metrics)
- ✅ Accessible via HTTP at: `http://[YOUR-ALB-DNS]`

**Access your API:**

- Swagger: `http://[YOUR-ALB-DNS]/swagger`
- Health: `http://[YOUR-ALB-DNS]/api/health`
- Login: `http://[YOUR-ALB-DNS]/api/Login`

**Monthly Cost:** ~$73-101 USD (can be reduced to ~$50-70 with optimizations)

**Support Resources:**

- AWS Documentation: https://docs.aws.amazon.com/
- AWS Support: Available in AWS Console
- Community: AWS Forums, Stack Overflow
