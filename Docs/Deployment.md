# Condominio API - AWS Container Deployment Guide

## Table of Contents

1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Understanding the Architecture](#understanding-the-architecture)
4. [Step 1: Build the Solution](#step-1-build-the-solution)
5. [Step 2: Create Docker Configuration](#step-2-create-docker-configuration)
6. [Step 3: Build and Test Locally](#step-3-build-and-test-locally)
7. [Step 4: Prepare for AWS Deployment](#step-4-prepare-for-aws-deployment)
8. [Step 5: Deploy to AWS ECS](#step-5-deploy-to-aws-ecs)
9. [Step 6: Expose API to the Internet](#step-6-expose-api-to-the-internet)
10. [Monitoring and Troubleshooting](#monitoring-and-troubleshooting)
11. [Cost Estimation](#cost-estimation)

---

## Overview

This guide will walk you through deploying your .NET 8 Condominio API with MySQL database to AWS using Docker containers. You'll learn:

- How to containerize your .NET application
- How to run MySQL in a container
- How to deploy to AWS Elastic Container Service (ECS)
- How to expose your API securely to the internet

**Architecture**: We'll use AWS ECS (Elastic Container Service) with Fargate (serverless compute) + RDS MySQL + Application Load Balancer.

---

## Prerequisites

### Software to Install

1. **Docker Desktop** - [Download here](https://www.docker.com/products/docker-desktop/)
2. **Visual Studio 2022** (already installed)
3. **AWS CLI** - [Installation guide](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html)
4. **AWS Account** - [Sign up here](https://aws.amazon.com/)

### AWS Account Setup

```powershell
# After installing AWS CLI, configure your credentials
aws configure
# You'll be prompted for:
# - AWS Access Key ID
# - AWS Secret Access Key
# - Default region (e.g., us-east-1)
# - Default output format (json)
```

**Where to get AWS credentials:**

1. Log into AWS Console
2. Go to IAM → Users → Your User → Security Credentials
3. Create Access Key → CLI usage
4. Save the Access Key ID and Secret Access Key

---

## Understanding the Architecture

### Why Docker?

Docker packages your application and all its dependencies into a standardized unit (container) that runs consistently across different environments.

### Why AWS ECS?

- **Managed Service**: AWS handles infrastructure management
- **Scalable**: Automatically scales based on demand
- **Integrated**: Works seamlessly with other AWS services
- **Cost-effective**: Pay only for what you use

### Architecture Diagram

```
Internet
    ↓
Application Load Balancer (ALB)
    ↓
ECS Service (Fargate)
    ├─→ API Container 1 (your .NET app)
    ├─→ API Container 2 (auto-scaled)
    └─→ API Container N
    ↓
RDS MySQL Database
```

---

## Step 1: Build the Solution

### 1.1 Build in Visual Studio

1. Open `CondominioAPI.sln` in Visual Studio
2. Right-click solution → **Restore NuGet Packages**
3. Build → **Build Solution** (Ctrl+Shift+B)
4. Verify no build errors

### 1.2 Build via Command Line (Alternative)

```powershell
# Navigate to solution directory
cd C:\Jose\code\FullStackMaster\MSD_CondoAdminBE\CondominioAPI

# Restore dependencies
dotnet restore

# Build the solution
dotnet build --configuration Release

# Verify build succeeded
dotnet build --configuration Release --no-incremental
```

**Expected Output:**

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

---

## Step 2: Create Docker Configuration

### 2.1 Understanding Docker Files

We need two main files:

- **Dockerfile**: Instructions to build the .NET API container
- **docker-compose.yml**: Orchestrates multiple containers (API + MySQL)

### 2.2 Create Dockerfile for .NET API

Create `Dockerfile` in `C:\Jose\code\FullStackMaster\MSD_CondoAdminBE\CondominioAPI\`

```dockerfile
# Dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file and project files
COPY CondominioAPI.sln ./
COPY CondominioAPI/CondominioAPI.csproj CondominioAPI/
COPY Condominio.Data.Mysql/Condominio.Data.Mysql.csproj Condominio.Data.Mysql/
COPY Condominio.DTOs/Condominio.DTOs.csproj Condominio.DTOs/
COPY Condominio.Models/Condominio.Models.csproj Condominio.Models/
COPY Condominio.Repository/Condominio.Repository.csproj Condominio.Repository/
COPY Condominio.Utils/Condominio.Utils.csproj Condominio.Utils/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . .

# Build the application
WORKDIR /src/CondominioAPI
RUN dotnet build -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Copy database scripts
COPY Database/ /app/Database/

# Expose port
EXPOSE 8080

# Set environment variable for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080

# Entry point
ENTRYPOINT ["dotnet", "CondominioAPI.dll"]
```

**What this does:**

- **Stage 1**: Uses SDK image to build the application
- **Stage 2**: Publishes the application (creates deployment-ready files)
- **Stage 3**: Uses smaller runtime image for final container
- **Multi-stage build**: Keeps final image small (~200MB vs ~800MB)

### 2.3 Create .dockerignore

Create `.dockerignore` in the same directory:

```
# .dockerignore
**/bin/
**/obj/
**/publish/
**/Logs/
**/.vs/
**/.vscode/
**/.git/
**/*.user
**/.env
**/appsettings.Development.json
```

### 2.4 Create docker-compose.yml for Local Development

Create `docker-compose.yml` in `C:\Jose\code\FullStackMaster\MSD_CondoAdminBE\CondominioAPI\`

```yaml
version: "3.8"

services:
  # MySQL Database Service
  mysql:
    image: mysql:8.0
    container_name: condominio-mysql
    environment:
      MYSQL_ROOT_PASSWORD: ${DB_PASSWORD:-RootPass123!}
      MYSQL_DATABASE: ${DB_NAME:-Condominio2}
    ports:
      - "3306:3306"
    volumes:
      - mysql-data:/var/lib/mysql
      - ./Database/Db_Schema.sql:/docker-entrypoint-initdb.d/01-schema.sql:ro
    networks:
      - condominio-network
    healthcheck:
      test:
        [
          "CMD",
          "mysqladmin",
          "ping",
          "-h",
          "localhost",
          "-u",
          "root",
          "-p${DB_PASSWORD:-RootPass123!}",
        ]
      interval: 10s
      timeout: 5s
      retries: 5
      start_period: 30s

  # .NET API Service
  api:
    build:
      context: .
      dockerfile: Dockerfile
    container_name: condominio-api
    environment:
      DB_SERVER: mysql
      DB_NAME: ${DB_NAME:-Condominio2}
      DB_USER: ${DB_USER:-root}
      DB_PASSWORD: ${DB_PASSWORD:-RootPass123!}
      JWT_SECRET_KEY: ${JWT_SECRET_KEY:-YourSuperSecretKeyHere123456789012345}
      CORS_ALLOWED_ORIGIN: ${CORS_ALLOWED_ORIGIN:-http://localhost:3000}
      LOG_PATH: /app/Logs/log-.txt
      ASPNETCORE_ENVIRONMENT: Production
    ports:
      - "5000:8080"
    depends_on:
      mysql:
        condition: service_healthy
    networks:
      - condominio-network
    volumes:
      - api-logs:/app/Logs

networks:
  condominio-network:
    driver: bridge

volumes:
  mysql-data:
  api-logs:
```

**What this does:**

- **mysql service**: Runs MySQL 8.0, automatically executes Db_Schema.sql on first start
- **api service**: Builds and runs your .NET application
- **healthcheck**: Ensures MySQL is ready before starting API
- **volumes**: Persists database data and logs
- **networks**: Allows containers to communicate

### 2.5 Create .env File for Local Development

Create `.env` in the same directory:

```bash
# .env - Local Development Environment Variables
DB_SERVER=mysql
DB_NAME=Condominio2
DB_USER=root
DB_PASSWORD=RootPass123!
JWT_SECRET_KEY=YourSuperSecretKeyHere123456789012345CHANGETHIS
CORS_ALLOWED_ORIGIN=http://localhost:3000
```

**⚠️ IMPORTANT**: Never commit `.env` to Git! Add it to `.gitignore`

---

## Step 3: Build and Test Locally

### 3.1 Build Docker Images

```powershell
# Navigate to the CondominioAPI directory
cd C:\Jose\code\FullStackMaster\MSD_CondoAdminBE\CondominioAPI

# Build the images (first time will take 5-10 minutes)
docker-compose build

# Verify images were created
docker images | Select-String "condominio"
```

**Expected Output:**

```
condominioapi-api     latest    abc123def456   2 minutes ago   220MB
```

### 3.2 Start Containers

```powershell
# Start both containers (MySQL + API)
docker-compose up -d

# Check container status
docker-compose ps
```

**Expected Output:**

```
NAME                STATUS       PORTS
condominio-mysql    Up 30s       0.0.0.0:3306->3306/tcp
condominio-api      Up 10s       0.0.0.0:5000->8080/tcp
```

### 3.3 Verify Database Initialization

```powershell
# View MySQL logs to confirm schema was created
docker-compose logs mysql | Select-String "Creating"

# Connect to MySQL and verify tables
docker exec -it condominio-mysql mysql -uroot -pRootPass123! -e "USE Condominio2; SHOW TABLES;"
```

**Expected Output:**

```
+-------------------------+
| Tables_in_Condominio2   |
+-------------------------+
| Expenses                |
| Expense_Categories      |
| Expense_Payments        |
| Incidents               |
| Incident_Costs          |
| Incident_Types          |
| Payments                |
| Payment_Status          |
| Property                |
| Property_Owners         |
| Property_Type           |
| Resources               |
| Resource_Bookings       |
| Resource_Costs          |
| Roles                   |
| Service_Expenses        |
| Service_Expense_Payments|
| Service_Payments        |
| Service_Types           |
| Users                   |
| User_Roles              |
| Versions                |
+-------------------------+
```

### 3.4 Test API Endpoints

```powershell
# Test health check endpoint
Invoke-WebRequest -Uri http://localhost:5000/health -Method GET

# Test Swagger UI (open in browser)
Start-Process "http://localhost:5000/swagger"

# Test login endpoint
$loginBody = @{
    login = "usa"
    password = "sa"
} | ConvertTo-Json

Invoke-RestMethod -Uri http://localhost:5000/api/Login -Method POST -Body $loginBody -ContentType "application/json"
```

**Expected Login Response:**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": 1,
  "userName": "sa"
}
```

### 3.5 View Logs

```powershell
# View API logs (real-time)
docker-compose logs -f api

# View MySQL logs
docker-compose logs mysql

# View all logs
docker-compose logs

# Stop following logs: Ctrl+C
```

### 3.6 Stop Containers

```powershell
# Stop containers (preserves data)
docker-compose down

# Stop and remove volumes (deletes database data)
docker-compose down -v
```

---

## Step 4: Prepare for AWS Deployment

### 4.1 Architecture Decision: AWS Managed Services

Instead of running MySQL in a container on ECS, we'll use **Amazon RDS** (Relational Database Service) because:

- ✅ Automated backups
- ✅ High availability (Multi-AZ)
- ✅ Automatic patching
- ✅ Better performance
- ✅ Point-in-time recovery

**Updated Architecture:**

```
Internet → ALB → ECS Fargate (API Containers) → RDS MySQL
```

### 4.2 Create ECR Repository (Elastic Container Registry)

ECR is AWS's Docker registry (like Docker Hub but private).

```powershell
# Create repository for your API
aws ecr create-repository `
    --repository-name condominio-api `
    --region us-east-1

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
    "repositoryUri": "123456789012.dkr.ecr.us-east-1.amazonaws.com/condominio-api"
  }
}
```

### 4.3 Build and Push Docker Image to ECR

```powershell
# Set variables (replace with your account ID and region)
$AWS_ACCOUNT_ID = "123456789012"
$AWS_REGION = "us-east-1"
$ECR_REPO = "$AWS_ACCOUNT_ID.dkr.ecr.$AWS_REGION.amazonaws.com/condominio-api"

# Authenticate Docker to ECR
aws ecr get-login-password --region $AWS_REGION | docker login --username AWS --password-stdin $ECR_REPO

# Build the image with ECR tag
cd C:\Jose\code\FullStackMaster\MSD_CondoAdminBE\CondominioAPI
docker build -t condominio-api:latest .
docker tag condominio-api:latest ${ECR_REPO}:latest
docker tag condominio-api:latest ${ECR_REPO}:v1.0.0

# Push to ECR
docker push ${ECR_REPO}:latest
docker push ${ECR_REPO}:v1.0.0

# Verify image in ECR
aws ecr describe-images --repository-name condominio-api --region $AWS_REGION
```

### 4.4 Create RDS MySQL Database

```powershell
# Create DB subnet group (requires at least 2 subnets in different AZs)
# First, get your default VPC ID
$VPC_ID = (aws ec2 describe-vpcs --filters "Name=isDefault,Values=true" --query "Vpcs[0].VpcId" --output text)

# Get subnet IDs from your VPC
$SUBNET_IDS = (aws ec2 describe-subnets --filters "Name=vpc-id,Values=$VPC_ID" --query "Subnets[0:2].SubnetId" --output text)
$SUBNET1 = ($SUBNET_IDS -split '\s+')[0]
$SUBNET2 = ($SUBNET_IDS -split '\s+')[1]

# Create DB subnet group
aws rds create-db-subnet-group `
    --db-subnet-group-name condominio-db-subnet `
    --db-subnet-group-description "Subnet group for Condominio DB" `
    --subnet-ids $SUBNET1 $SUBNET2 `
    --region $AWS_REGION

# Create security group for RDS
$DB_SG_ID = (aws ec2 create-security-group `
    --group-name condominio-db-sg `
    --description "Security group for Condominio RDS" `
    --vpc-id $VPC_ID `
    --region $AWS_REGION `
    --query 'GroupId' --output text)

# Allow MySQL access from within VPC only
aws ec2 authorize-security-group-ingress `
    --group-id $DB_SG_ID `
    --protocol tcp `
    --port 3306 `
    --cidr 172.31.0.0/16 `
    --region $AWS_REGION

# Create RDS MySQL instance
aws rds create-db-instance `
    --db-instance-identifier condominio-db `
    --db-instance-class db.t3.micro `
    --engine mysql `
    --engine-version 8.0.35 `
    --master-username admin `
    --master-user-password "YourStrongPassword123!" `
    --allocated-storage 20 `
    --storage-type gp3 `
    --vpc-security-group-ids $DB_SG_ID `
    --db-subnet-group-name condominio-db-subnet `
    --backup-retention-period 7 `
    --preferred-backup-window "03:00-04:00" `
    --publicly-accessible false `
    --region $AWS_REGION

# Wait for DB to be available (takes 5-10 minutes)
aws rds wait db-instance-available --db-instance-identifier condominio-db --region $AWS_REGION

# Get DB endpoint
$DB_ENDPOINT = (aws rds describe-db-instances `
    --db-instance-identifier condominio-db `
    --query "DBInstances[0].Endpoint.Address" `
    --output text `
    --region $AWS_REGION)

Write-Host "Database endpoint: $DB_ENDPOINT"
```

### 4.5 Initialize Database Schema

Since RDS doesn't have access to your SQL file, we'll use a temporary EC2 instance or Cloud9.

**Option A: Using Cloud9 (Recommended for beginners)**

1. Go to AWS Console → Cloud9 → Create Environment
2. Name: `condominio-setup`, Instance type: `t2.micro`
3. Click Create
4. Once ready, open the Cloud9 IDE
5. Upload `Db_Schema.sql` using File → Upload Local Files
6. Run in terminal:

```bash
# Install MySQL client
sudo yum install mysql -y

# Connect to RDS (replace with your endpoint and password)
mysql -h $DB_ENDPOINT -u admin -p

# In MySQL prompt:
CREATE DATABASE Condominio2;
USE Condominio2;
source Db_Schema.sql;
SHOW TABLES;
exit;
```

**Option B: Using MySQL Workbench (If you prefer GUI)**

1. Download MySQL Workbench
2. Create SSH tunnel through a bastion host (more complex, not recommended for beginners)

### 4.6 Store Secrets in AWS Secrets Manager

Instead of hardcoding passwords, use AWS Secrets Manager:

```powershell
# Create secret for database credentials
aws secretsmanager create-secret `
    --name condominio/db/credentials `
    --description "Database credentials for Condominio API" `
    --secret-string '{\"username\":\"admin\",\"password\":\"YourStrongPassword123!\",\"host\":\"'$DB_ENDPOINT'\",\"database\":\"Condominio2\"}' `
    --region $AWS_REGION

# Create secret for JWT key
aws secretsmanager create-secret `
    --name condominio/jwt/secret `
    --description "JWT secret key for Condominio API" `
    --secret-string '{\"key\":\"YourSuperSecretJWTKeyChangeMeToSomethingSecure12345\"}' `
    --region $AWS_REGION

# Verify secrets
aws secretsmanager list-secrets --region $AWS_REGION
```

---

## Step 5: Deploy to AWS ECS

### 5.1 Create ECS Cluster

```powershell
# Create ECS cluster (Fargate serverless mode)
aws ecs create-cluster `
    --cluster-name condominio-cluster `
    --region $AWS_REGION

# Verify cluster
aws ecs describe-clusters --clusters condominio-cluster --region $AWS_REGION
```

### 5.2 Create IAM Role for ECS Tasks

ECS tasks need permission to:

- Pull images from ECR
- Write logs to CloudWatch
- Read secrets from Secrets Manager

```powershell
# Create trust policy file
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

# Create execution role
aws iam create-role `
    --role-name condominioECSExecutionRole `
    --assume-role-policy-document file://trust-policy.json

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
        "arn:aws:secretsmanager:$AWS_REGION:${AWS_ACCOUNT_ID}:secret:condominio/*"
      ]
    }
  ]
}
"@ | Out-File -FilePath secrets-policy.json -Encoding utf8

aws iam put-role-policy `
    --role-name condominioECSExecutionRole `
    --policy-name SecretsManagerAccess `
    --policy-document file://secrets-policy.json

# Get role ARN
$EXECUTION_ROLE_ARN = (aws iam get-role --role-name condominioECSExecutionRole --query 'Role.Arn' --output text)
```

### 5.3 Create CloudWatch Log Group

```powershell
# Create log group for API logs
aws logs create-log-group `
    --log-group-name /ecs/condominio-api `
    --region $AWS_REGION

# Set retention to 7 days (optional, saves costs)
aws logs put-retention-policy `
    --log-group-name /ecs/condominio-api `
    --retention-in-days 7 `
    --region $AWS_REGION
```

### 5.4 Create ECS Task Definition

This defines how your container should run:

```powershell
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
      "image": "${ECR_REPO}:latest",
      "essential": true,
      "portMappings": [
        {
          "containerPort": 8080,
          "protocol": "tcp"
        }
      ],
      "environment": [
        {
          "name": "ASPNETCORE_ENVIRONMENT",
          "value": "Production"
        },
        {
          "name": "CORS_ALLOWED_ORIGIN",
          "value": "*"
        },
        {
          "name": "LOG_PATH",
          "value": "/app/Logs/log-.txt"
        }
      ],
      "secrets": [
        {
          "name": "DB_SERVER",
          "valueFrom": "arn:aws:secretsmanager:$AWS_REGION:${AWS_ACCOUNT_ID}:secret:condominio/db/credentials:host::"
        },
        {
          "name": "DB_NAME",
          "valueFrom": "arn:aws:secretsmanager:$AWS_REGION:${AWS_ACCOUNT_ID}:secret:condominio/db/credentials:database::"
        },
        {
          "name": "DB_USER",
          "valueFrom": "arn:aws:secretsmanager:$AWS_REGION:${AWS_ACCOUNT_ID}:secret:condominio/db/credentials:username::"
        },
        {
          "name": "DB_PASSWORD",
          "valueFrom": "arn:aws:secretsmanager:$AWS_REGION:${AWS_ACCOUNT_ID}:secret:condominio/db/credentials:password::"
        },
        {
          "name": "JWT_SECRET_KEY",
          "valueFrom": "arn:aws:secretsmanager:$AWS_REGION:${AWS_ACCOUNT_ID}:secret:condominio/jwt/secret:key::"
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
        "command": ["CMD-SHELL", "curl -f http://localhost:8080/health || exit 1"],
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
aws ecs register-task-definition `
    --cli-input-json file://task-definition.json `
    --region $AWS_REGION
```

**What this does:**

- **CPU/Memory**: 0.5 vCPU and 1 GB RAM (adjust based on load)
- **Secrets**: Loads from Secrets Manager (secure!)
- **Logs**: Sends to CloudWatch
- **Health Check**: Calls /health endpoint every 30 seconds

### 5.5 Create Application Load Balancer

The ALB distributes traffic across containers:

```powershell
# Get default VPC subnets
$SUBNETS = (aws ec2 describe-subnets `
    --filters "Name=vpc-id,Values=$VPC_ID" `
    --query "Subnets[*].SubnetId" `
    --output text)

# Create security group for ALB
$ALB_SG_ID = (aws ec2 create-security-group `
    --group-name condominio-alb-sg `
    --description "Security group for Condominio ALB" `
    --vpc-id $VPC_ID `
    --region $AWS_REGION `
    --query 'GroupId' --output text)

# Allow HTTP and HTTPS from anywhere
aws ec2 authorize-security-group-ingress `
    --group-id $ALB_SG_ID `
    --protocol tcp --port 80 `
    --cidr 0.0.0.0/0 `
    --region $AWS_REGION

aws ec2 authorize-security-group-ingress `
    --group-id $ALB_SG_ID `
    --protocol tcp --port 443 `
    --cidr 0.0.0.0/0 `
    --region $AWS_REGION

# Create ALB
$ALB_ARN = (aws elbv2 create-load-balancer `
    --name condominio-alb `
    --subnets $SUBNETS.Split() `
    --security-groups $ALB_SG_ID `
    --scheme internet-facing `
    --type application `
    --ip-address-type ipv4 `
    --region $AWS_REGION `
    --query 'LoadBalancers[0].LoadBalancerArn' --output text)

# Create target group
$TG_ARN = (aws elbv2 create-target-group `
    --name condominio-tg `
    --protocol HTTP `
    --port 8080 `
    --vpc-id $VPC_ID `
    --target-type ip `
    --health-check-path /health `
    --health-check-interval-seconds 30 `
    --health-check-timeout-seconds 5 `
    --healthy-threshold-count 2 `
    --unhealthy-threshold-count 3 `
    --region $AWS_REGION `
    --query 'TargetGroups[0].TargetGroupArn' --output text)

# Create listener (forwards HTTP:80 to containers)
aws elbv2 create-listener `
    --load-balancer-arn $ALB_ARN `
    --protocol HTTP `
    --port 80 `
    --default-actions Type=forward,TargetGroupArn=$TG_ARN `
    --region $AWS_REGION

# Get ALB DNS name
$ALB_DNS = (aws elbv2 describe-load-balancers `
    --load-balancer-arns $ALB_ARN `
    --query 'LoadBalancers[0].DNSName' `
    --output text `
    --region $AWS_REGION)

Write-Host "Load Balancer DNS: $ALB_DNS"
```

### 5.6 Create Security Group for ECS Tasks

```powershell
# Create security group for ECS tasks
$ECS_SG_ID = (aws ec2 create-security-group `
    --group-name condominio-ecs-sg `
    --description "Security group for Condominio ECS tasks" `
    --vpc-id $VPC_ID `
    --region $AWS_REGION `
    --query 'GroupId' --output text)

# Allow traffic from ALB only
aws ec2 authorize-security-group-ingress `
    --group-id $ECS_SG_ID `
    --protocol tcp --port 8080 `
    --source-group $ALB_SG_ID `
    --region $AWS_REGION

# Update DB security group to allow ECS tasks
aws ec2 authorize-security-group-ingress `
    --group-id $DB_SG_ID `
    --protocol tcp --port 3306 `
    --source-group $ECS_SG_ID `
    --region $AWS_REGION
```

### 5.7 Create ECS Service

The service maintains the desired number of running tasks:

```powershell
# Get public subnets (for Fargate tasks with public IPs)
$PUBLIC_SUBNETS = (aws ec2 describe-subnets `
    --filters "Name=vpc-id,Values=$VPC_ID" `
    --query "Subnets[*].SubnetId" `
    --output text)

# Create service
aws ecs create-service `
    --cluster condominio-cluster `
    --service-name condominio-api-service `
    --task-definition condominio-api-task `
    --desired-count 2 `
    --launch-type FARGATE `
    --platform-version LATEST `
    --network-configuration "awsvpcConfiguration={subnets=[$PUBLIC_SUBNETS],securityGroups=[$ECS_SG_ID],assignPublicIp=ENABLED}" `
    --load-balancers "targetGroupArn=$TG_ARN,containerName=condominio-api,containerPort=8080" `
    --health-check-grace-period-seconds 60 `
    --region $AWS_REGION

# Wait for service to stabilize (2-5 minutes)
aws ecs wait services-stable `
    --cluster condominio-cluster `
    --services condominio-api-service `
    --region $AWS_REGION

Write-Host "Service created successfully!"
```

**What this does:**

- **desired-count 2**: Runs 2 containers for high availability
- **assignPublicIp**: Required for Fargate to pull from ECR
- **load-balancers**: Connects to ALB target group

### 5.8 Verify Deployment

```powershell
# Check service status
aws ecs describe-services `
    --cluster condominio-cluster `
    --services condominio-api-service `
    --region $AWS_REGION `
    --query 'services[0].{Status:status,Running:runningCount,Desired:desiredCount}'

# Check tasks
aws ecs list-tasks `
    --cluster condominio-cluster `
    --service-name condominio-api-service `
    --region $AWS_REGION

# View task logs
aws logs tail /ecs/condominio-api --follow --region $AWS_REGION
```

### 5.9 Test API

```powershell
# Test API through ALB
$API_URL = "http://$ALB_DNS"

# Health check
Invoke-WebRequest -Uri "$API_URL/health" -Method GET

# Swagger UI
Start-Process "$API_URL/swagger"

# Login test
$loginBody = @{
    login = "usa"
    password = "sa"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$API_URL/api/Login" -Method POST -Body $loginBody -ContentType "application/json"
```

---

## Step 6: Expose API to the Internet

### 6.1 Load Balancer vs API Gateway

| Feature           | Application Load Balancer (ALB) | API Gateway                                |
| ----------------- | ------------------------------- | ------------------------------------------ |
| **Purpose**       | Layer 7 load balancing          | API management platform                    |
| **Cost**          | ~$16/month + data transfer      | Pay per request (~$3.50/million)           |
| **Features**      | Basic routing, health checks    | Throttling, caching, API keys, usage plans |
| **WebSockets**    | ✅ Yes                          | ✅ Yes                                     |
| **Custom Domain** | ✅ Yes (Route 53 + ACM)         | ✅ Yes (easier setup)                      |
| **SSL/TLS**       | ✅ Free with ACM                | ✅ Free with ACM                           |
| **Best For**      | Direct container access         | API monetization, complex routing          |

**Recommendation for your case:** Start with **ALB** (simpler, already configured). Add API Gateway later if you need:

- Rate limiting per API key
- Request/response transformation
- API monetization
- Complex request validation

### 6.2 Add HTTPS Support (Recommended)

**Prerequisites:** You need a domain name (e.g., `api.yourcondominio.com`)

#### Step 1: Request SSL Certificate

```powershell
# Request certificate in ACM
$CERT_ARN = (aws acm request-certificate `
    --domain-name api.yourcondominio.com `
    --validation-method DNS `
    --region $AWS_REGION `
    --query 'CertificateArn' --output text)

# Get validation CNAME record
aws acm describe-certificate `
    --certificate-arn $CERT_ARN `
    --region $AWS_REGION `
    --query 'Certificate.DomainValidationOptions[0].ResourceRecord'
```

#### Step 2: Add DNS Record

1. Go to your domain registrar (GoDaddy, Namecheap, etc.)
2. Add the CNAME record from above
3. Wait for validation (5-30 minutes)

```powershell
# Check validation status
aws acm describe-certificate `
    --certificate-arn $CERT_ARN `
    --region $AWS_REGION `
    --query 'Certificate.Status'
```

#### Step 3: Add HTTPS Listener to ALB

```powershell
# Create HTTPS listener
aws elbv2 create-listener `
    --load-balancer-arn $ALB_ARN `
    --protocol HTTPS `
    --port 443 `
    --certificates CertificateArn=$CERT_ARN `
    --default-actions Type=forward,TargetGroupArn=$TG_ARN `
    --region $AWS_REGION

# Optional: Redirect HTTP to HTTPS
aws elbv2 modify-listener `
    --listener-arn <HTTP_LISTENER_ARN> `
    --default-actions Type=redirect,RedirectConfig='{Protocol=HTTPS,Port=443,StatusCode=HTTP_301}' `
    --region $AWS_REGION
```

#### Step 4: Point Domain to ALB

In Route 53 or your DNS provider:

```
Type: A (Alias)
Name: api.yourcondominio.com
Value: [ALB DNS name]
```

### 6.3 Optional: Add API Gateway (Advanced)

If you decide to add API Gateway later:

```powershell
# Create HTTP API (simpler than REST API)
$API_ID = (aws apigatewayv2 create-api `
    --name condominio-api `
    --protocol-type HTTP `
    --target $ALB_DNS `
    --region $AWS_REGION `
    --query 'ApiId' --output text)

# Get API endpoint
aws apigatewayv2 get-api `
    --api-id $API_ID `
    --region $AWS_REGION `
    --query 'ApiEndpoint'
```

This creates a managed API endpoint that forwards to your ALB.

---

## Monitoring and Troubleshooting

### View Logs

```powershell
# Real-time logs
aws logs tail /ecs/condominio-api --follow --region $AWS_REGION

# Last 100 lines
aws logs tail /ecs/condominio-api --since 1h --region $AWS_REGION

# Filter errors
aws logs tail /ecs/condominio-api --filter-pattern "ERROR" --follow --region $AWS_REGION
```

### Check Task Status

```powershell
# List tasks
$TASK_ARN = (aws ecs list-tasks `
    --cluster condominio-cluster `
    --service-name condominio-api-service `
    --region $AWS_REGION `
    --query 'taskArns[0]' --output text)

# Describe task
aws ecs describe-tasks `
    --cluster condominio-cluster `
    --tasks $TASK_ARN `
    --region $AWS_REGION
```

### Common Issues

#### Issue 1: Task keeps restarting

```powershell
# Check task stopped reason
aws ecs describe-tasks `
    --cluster condominio-cluster `
    --tasks $TASK_ARN `
    --region $AWS_REGION `
    --query 'tasks[0].stoppedReason'

# Common causes:
# - Database connection failed (check DB security group)
# - Secret not found (verify Secrets Manager ARNs)
# - Image not found (verify ECR image exists)
```

#### Issue 2: ALB returns 503 Service Unavailable

```powershell
# Check target health
aws elbv2 describe-target-health `
    --target-group-arn $TG_ARN `
    --region $AWS_REGION

# If unhealthy, check:
# - ECS task is running
# - Security group allows ALB → ECS traffic
# - Health check endpoint /health returns 200
```

#### Issue 3: Cannot connect to database

```powershell
# Verify security groups
aws ec2 describe-security-groups `
    --group-ids $DB_SG_ID `
    --region $AWS_REGION

# Test from ECS task:
aws ecs execute-command `
    --cluster condominio-cluster `
    --task $TASK_ARN `
    --container condominio-api `
    --interactive `
    --command "/bin/bash"

# In container:
# apt-get update && apt-get install -y mysql-client
# mysql -h $DB_ENDPOINT -u admin -p
```

### Enable Container Insights (Monitoring)

```powershell
# Enable CloudWatch Container Insights
aws ecs update-cluster-settings `
    --cluster condominio-cluster `
    --settings name=containerInsights,value=enabled `
    --region $AWS_REGION

# View metrics in CloudWatch Console:
# Services → CloudWatch → Container Insights
```

---

## Scaling and Auto-Scaling

### Manual Scaling

```powershell
# Scale to 5 tasks
aws ecs update-service `
    --cluster condominio-cluster `
    --service condominio-api-service `
    --desired-count 5 `
    --region $AWS_REGION
```

### Auto-Scaling (Based on CPU)

```powershell
# Register scalable target
aws application-autoscaling register-scalable-target `
    --service-namespace ecs `
    --resource-id service/condominio-cluster/condominio-api-service `
    --scalable-dimension ecs:service:DesiredCount `
    --min-capacity 2 `
    --max-capacity 10 `
    --region $AWS_REGION

# Create scaling policy (scale when CPU > 70%)
aws application-autoscaling put-scaling-policy `
    --service-namespace ecs `
    --resource-id service/condominio-cluster/condominio-api-service `
    --scalable-dimension ecs:service:DesiredCount `
    --policy-name cpu-scaling-policy `
    --policy-type TargetTrackingScaling `
    --target-tracking-scaling-policy-configuration '{
        "TargetValue": 70.0,
        "PredefinedMetricSpecification": {
            "PredefinedMetricType": "ECSServiceAverageCPUUtilization"
        },
        "ScaleInCooldown": 300,
        "ScaleOutCooldown": 60
    }' `
    --region $AWS_REGION
```

---

## CI/CD Pipeline (Optional - For Future)

### Using GitHub Actions

Create `.github/workflows/deploy.yml`:

```yaml
name: Deploy to AWS ECS

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Configure AWS credentials
        uses: aws-actions/configure-aws-credentials@v2
        with:
          aws-access-key-id: ${{ secrets.AWS_ACCESS_KEY_ID }}
          aws-secret-access-key: ${{ secrets.AWS_SECRET_ACCESS_KEY }}
          aws-region: us-east-1

      - name: Login to Amazon ECR
        id: login-ecr
        uses: aws-actions/amazon-ecr-login@v1

      - name: Build and push image
        env:
          ECR_REGISTRY: ${{ steps.login-ecr.outputs.registry }}
          ECR_REPOSITORY: condominio-api
          IMAGE_TAG: ${{ github.sha }}
        run: |
          docker build -t $ECR_REGISTRY/$ECR_REPOSITORY:$IMAGE_TAG .
          docker push $ECR_REGISTRY/$ECR_REPOSITORY:$IMAGE_TAG

      - name: Deploy to ECS
        uses: aws-actions/amazon-ecs-deploy-task-definition@v1
        with:
          task-definition: task-definition.json
          service: condominio-api-service
          cluster: condominio-cluster
          wait-for-service-stability: true
```

---

## Cost Estimation

### Monthly Costs (Approximate)

| Service             | Configuration                      | Cost           |
| ------------------- | ---------------------------------- | -------------- |
| **ECS Fargate**     | 2 tasks × 0.5 vCPU × 1GB × 730 hrs | $30            |
| **RDS MySQL**       | db.t3.micro × 730 hrs              | $15            |
| **ALB**             | 1 ALB + data transfer              | $18            |
| **ECR**             | < 500 MB storage                   | $0.50          |
| **CloudWatch Logs** | 5 GB/month                         | $2.50          |
| **Data Transfer**   | 10 GB out to internet              | $1             |
| **Secrets Manager** | 2 secrets                          | $0.80          |
| **Total**           |                                    | **~$68/month** |

**Cost Optimization Tips:**

1. Use **Savings Plans** for ECS (20-30% discount)
2. Use **Reserved Instances** for RDS (40% discount)
3. Reduce log retention (currently 7 days)
4. Use **Fargate Spot** for non-production (70% discount)

---

## Cleanup (Delete Everything)

```powershell
# Delete ECS service
aws ecs delete-service --cluster condominio-cluster --service condominio-api-service --force --region $AWS_REGION

# Delete ECS cluster
aws ecs delete-cluster --cluster condominio-cluster --region $AWS_REGION

# Delete ALB
aws elbv2 delete-load-balancer --load-balancer-arn $ALB_ARN --region $AWS_REGION
aws elbv2 delete-target-group --target-group-arn $TG_ARN --region $AWS_REGION

# Delete RDS
aws rds delete-db-instance --db-instance-identifier condominio-db --skip-final-snapshot --region $AWS_REGION

# Delete security groups (wait for resources to be deleted first)
aws ec2 delete-security-group --group-id $ECS_SG_ID --region $AWS_REGION
aws ec2 delete-security-group --group-id $ALB_SG_ID --region $AWS_REGION
aws ec2 delete-security-group --group-id $DB_SG_ID --region $AWS_REGION

# Delete ECR repository
aws ecr delete-repository --repository-name condominio-api --force --region $AWS_REGION

# Delete secrets
aws secretsmanager delete-secret --secret-id condominio/db/credentials --force-delete-without-recovery --region $AWS_REGION
aws secretsmanager delete-secret --secret-id condominio/jwt/secret --force-delete-without-recovery --region $AWS_REGION

# Delete CloudWatch log group
aws logs delete-log-group --log-group-name /ecs/condominio-api --region $AWS_REGION

# Delete IAM role
aws iam detach-role-policy --role-name condominioECSExecutionRole --policy-arn arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy
aws iam delete-role-policy --role-name condominioECSExecutionRole --policy-name SecretsManagerAccess
aws iam delete-role --role-name condominioECSExecutionRole
```

---

## Next Steps

1. **Set up custom domain** with HTTPS
2. **Configure auto-scaling** based on traffic patterns
3. **Implement CI/CD** pipeline with GitHub Actions
4. **Add monitoring** and alerting (CloudWatch Alarms)
5. **Set up backups** and disaster recovery
6. **Implement API rate limiting** (via API Gateway or custom middleware)
7. **Add WAF** (Web Application Firewall) for security

---

## Additional Resources

- [AWS ECS Documentation](https://docs.aws.amazon.com/ecs/)
- [Docker Documentation](https://docs.docker.com/)
- [AWS Well-Architected Framework](https://aws.amazon.com/architecture/well-architected/)
- [ASP.NET Core Deployment](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/)

---

**Last Updated:** May 5, 2026  
**Version:** 1.0.0
