# Environment Setup

This guide explains how to configure and run the MeetFlow backend in a local development environment.

---

# Prerequisites

Before running the project, ensure the following software is installed on your machine:

* .NET 8 SDK
* SQL Server
* Visual Studio 2022/2026 or Visual Studio Code
* Git

---

# Clone the Repository

Clone the project from GitHub using the following commands:

```bash
git clone https://github.com/Rawda2007/Project_MeetFlow.git

cd Project_MeetFlow
```

---

# Configure Application Settings

Before running the application, configure the required environment-specific settings.

The following values should be provided:

* Database Connection String
* JWT Configuration (Secret Key, Issuer, Audience)
* Email Service Configuration
* Google Gemini API Key (AI Features)
* Any additional external service credentials

> **Security Note**
>
> Sensitive information such as passwords, API keys, JWT secrets, and database credentials should **never** be committed to the GitHub repository.

For local development, it is recommended to use:

* .NET User Secrets
* Environment Variables

---

# Configure the Database

Update the SQL Server connection string to point to your local database instance.

After configuring the connection string, apply the Entity Framework Core migrations.

```bash
dotnet ef database update
```

This command creates the required database schema if it does not already exist.

---

# Build and Run the Project

Run the backend application using:

```bash
dotnet run
```

Alternatively, you can open the solution in **Visual Studio** and start the project using **F5** or **Ctrl + F5**.

---

# Access the API

Once the application is running, the API will be available through the configured local URL.

For local development:

```
https://localhost:7084
```

---

# Swagger Documentation

MeetFlow provides interactive API documentation through Swagger.

Open the following URL in your browser:

```
https://localhost:7084/swagger
```

Swagger can be used to:

* Explore all available endpoints.
* Execute API requests.
* View request and response models.
* Test the backend without a frontend application.

---

# Authentication

Most API endpoints require authentication.

To access protected endpoints:

1. Register a new account using the **Register** endpoint.
2. Login using the **Login** endpoint.
3. Copy the generated **Access Token**.
4. Click the **Authorize** button in Swagger.
5. Enter the token using the following format:

```
Bearer <your_access_token>
```

After successful authorization, all protected endpoints can be tested directly from Swagger.

---

# Project Configuration Checklist

Before running the application, verify the following:

* .NET 8 SDK is installed.
* SQL Server is running.
* Database connection string is configured.
* Required application secrets are configured.
* Entity Framework migrations have been applied.
* Swagger is accessible.
* Authentication is working correctly.

---

# Troubleshooting

## Database Connection Error

* Verify that SQL Server is running.
* Check the connection string.
* Ensure the target database exists or run the migrations.

---

## Migration Errors

If database migrations fail:

```bash
dotnet ef migrations add InitialCreate

dotnet ef database update
```

---

## Authentication Issues

If protected endpoints return **401 Unauthorized**:

* Verify that you are logged in.
* Ensure the Access Token has not expired.
* Confirm that the Authorization header uses the following format:

```
Bearer <access_token>
```

---

# Local Development Workflow

A typical development workflow is:

1. Clone the repository.
2. Configure application settings.
3. Apply database migrations.
4. Run the backend.
5. Open Swagger.
6. Authenticate using JWT.
7. Test the available API endpoints.

