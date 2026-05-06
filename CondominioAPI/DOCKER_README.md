# Quick Start - Docker Local Development

This guide helps you quickly get started with Docker locally before deploying to AWS.

## Two Build Modes Available

### 1. Git-Based Build (Default - Production Ready)

- **Dockerfile**: Clones from GitHub repository
- **Use when**: Building for production, CI/CD, or testing specific commits
- **Command**: `docker-compose up -d --build`

### 2. Local Build (Faster for Development)

- **Dockerfile.local**: Copies local code
- **Use when**: Actively developing and testing changes
- **Command**: `docker-compose -f docker-compose.local.yml up -d --build`

---

## Prerequisites

1. Install Docker Desktop for Windows
2. Ensure Docker is running (check system tray)

## Quick Start - Git-Based Build (5 minutes)

This builds directly from the GitHub repository.

### Step 1: Create Environment File

```powershell
cd C:\Jose\code\FullStackMaster\MSD_CondoAdminBE\CondominioAPI

# Copy the example environment file
Copy-Item .env.example .env

# (Optional) Edit .env to customize values
notepad .env
```

### Step 2: Build and Run from GitHub

```powershell
# Build from GitHub main branch (default)
docker-compose up -d --build

# Or build from a specific branch
docker-compose build --build-arg GIT_BRANCH=develop api
docker-compose up -d

# Or build from a specific commit/tag
docker-compose build --build-arg GIT_BRANCH=v1.0.0 api
docker-compose up -d

# Check status
docker-compose ps

# Expected output:
# NAME                STATUS       PORTS
# condominio-mysql    Up          0.0.0.0:3306->3306/tcp
# condominio-api      Up          0.0.0.0:5000->8080/tcp
```

## Quick Start - Local Development (Faster Rebuilds)

Use this when actively developing - it copies your local code instead of cloning from Git.

### Step 1: Same as above (Create .env file)

### Step 2: Build and Run from Local Code

```powershell
# Build from local code (faster for development)
docker-compose -f docker-compose.local.yml up -d --build

# Check status
docker-compose -f docker-compose.local.yml ps
```

**When to use local mode:**

- ✅ Testing uncommitted changes
- ✅ Faster rebuild times (only copies changed files)
- ✅ No internet required

**When to use Git mode:**

- ✅ Ensure clean build from repository
- ✅ Test specific commits/tags/branches
- ✅ Consistent with CI/CD pipeline
- ✅ Building on servers without local code

---

## Step 3: Test the API

Both modes use the same commands:

```powershell
# Test health endpoint
Invoke-WebRequest -Uri http://localhost:5000/health -Method GET

# Open Swagger UI in browser
Start-Process "http://localhost:5000/swagger"

# Test login
$body = @{ login = "usa"; password = "sa" } | ConvertTo-Json
Invoke-RestMethod -Uri http://localhost:5000/api/Login -Method POST -Body $body -ContentType "application/json"
```

## Common Commands

### View Logs

```powershell
# All logs
docker-compose logs

# Only API logs
docker-compose logs api

# Follow logs (real-time)
docker-compose logs -f api

# Press Ctrl+C to stop following
```

### Database Access

```powershell
# Connect to MySQL
docker exec -it condominio-mysql mysql -uroot -pRootPass123!

# In MySQL:
USE Condominio2;
SHOW TABLES;
SELECT * FROM Users;
exit;
```

### Rebuild After Code Changes

```powershell
# Stop containers
docker-compose down

# Rebuild and start
docker-compose up -d --build
```

### Stop Everything

```powershell
# Stop containers (keeps data)
docker-compose down

# Stop and remove all data
docker-compose down -v
```

## Troubleshooting

### Port Already in Use

If port 5000 or 3306 is already in use:

**Option 1: Change port in docker-compose.yml**

```yaml
api:
  ports:
    - "5001:8080" # Changed from 5000 to 5001
```

**Option 2: Stop conflicting service**

```powershell
# Find what's using port 5000
Get-NetTCPConnection -LocalPort 5000 -State Listen | Select-Object -Property OwningProcess
```

### Container Won't Start

```powershell
# Check detailed logs
docker-compose logs api

# Check MySQL health
docker-compose logs mysql | Select-String "ready for connections"

# Restart services
docker-compose restart
```

### Database Schema Not Created

```powershell
# The schema runs only on FIRST start with empty database
# To re-run schema:

# 1. Stop and remove volumes
docker-compose down -v

# 2. Start again
docker-compose up -d

# 3. Wait for MySQL to be ready (30 seconds)
Start-Sleep -Seconds 30

# 4. Check tables
docker exec -it condominio-mysql mysql -uroot -pRootPass123! -e "USE Condominio2; SHOW TABLES;"
```

### API Can't Connect to Database

```powershell
# Verify MySQL is healthy
docker-compose ps

# If mysql shows "unhealthy":
docker-compose logs mysql

# Common fix: Restart MySQL
docker-compose restart mysql
```

## Next Steps

Once you've tested locally and everything works:

1. Read the full [Deployment.md](Deployment.md) guide
2. Set up your AWS account
3. Follow the AWS deployment steps

## API Endpoints

After starting, these endpoints are available:

- **Swagger UI**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/health
- **API Base**: http://localhost:5000/api/

### Default Test Users

The database includes these test users:

| Username   | Password  | Role      |
| ---------- | --------- | --------- |
| usa        | sa        | super     |
| uadmin     | admin     | admin     |
| udirector  | director  | director  |
| uhabitante | habitante | habitante |
| uauxiliar  | auxiliar  | auxiliar  |
| useguridad | seguridad | seguridad |

## Docker Commands Cheat Sheet

```powershell
# Build images
docker-compose build

# Start containers
docker-compose up -d

# Stop containers
docker-compose down

# View logs
docker-compose logs -f

# Restart specific service
docker-compose restart api

# View running containers
docker-compose ps

# Execute command in container
docker exec -it condominio-api /bin/bash

# Remove all containers and volumes
docker-compose down -v

# Remove unused Docker resources
docker system prune -a
```

## Understanding docker-compose.yml

The `docker-compose.yml` file defines two services:

1. **mysql**: MySQL database server
   - Automatically creates database on first run
   - Runs `Db_Schema.sql` script to create tables
   - Data persists in `mysql-data` volume

2. **api**: Your .NET application
   - Builds from Dockerfile
   - Waits for MySQL to be healthy before starting
   - Logs saved to `api-logs` volume

## Environment Variables

Edit `.env` to customize:

```bash
DB_PASSWORD=YourSecurePassword123!
JWT_SECRET_KEY=YourVerySecureJWTKey...
CORS_ALLOWED_ORIGIN=http://your-frontend-domain.com
```

**Security Note**: Never commit `.env` to Git! It's already in `.gitignore`.
