# Dockerfile for Condominio API
# Multi-stage build to keep final image small
# Clones from GitHub repository

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Install git
RUN apt-get update && apt-get install -y git && rm -rf /var/lib/apt/lists/*

WORKDIR /repo

# Build arguments for Git configuration
ARG GIT_REPO=https://github.com/joseluisdaza/MSD_CondoAdminBE.git
#ARG GIT_BRANCH=main
ARG GIT_BRANCH=feature/Backend_AWS_Deployment

# Clone the repository
RUN git clone --branch ${GIT_BRANCH} --depth 1 ${GIT_REPO} .

# Navigate to the CondominioAPI folder where .sln is located
WORKDIR /repo/CondominioAPI

# Restore dependencies
RUN dotnet restore

# Build the application
WORKDIR /repo/CondominioAPI/CondominioAPI
RUN dotnet build -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Copy database scripts from the cloned repository
COPY --from=build /repo/CondominioAPI/Database/ /app/Database/

# Expose port
EXPOSE 8080

# Set environment variable for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080

# Entry point
ENTRYPOINT ["dotnet", "CondominioAPI.dll"]
