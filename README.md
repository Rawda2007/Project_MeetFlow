# MeetFlow

> **Meeting Management & Follow-up Platform**

MeetFlow is a meeting management and follow-up platform designed to help teams organize meetings, document important discussions, track decisions, and turn meeting outcomes into actionable tasks.

The platform aims to simplify the complete meeting lifecycle — from creating and managing meetings to documenting outcomes and following up on assigned tasks.

---

## 📌 Project Overview

Meetings often generate important decisions, tasks, and responsibilities, but these outcomes can easily be lost or forgotten after the meeting ends.

MeetFlow addresses this problem by providing a centralized platform where teams can:

* Organize workspaces and team members
* Create and manage meetings
* Document meeting notes
* Record important decisions
* Create and manage follow-up tasks
* Assign tasks to workspace members
* Use AI to extract actionable tasks from meeting notes
* Track meeting outcomes and follow-up activities

The project is being developed incrementally through multiple phases, with additional collaboration, notification, and AI-powered features planned for future phases.

---

## 🎯 Main Objectives

* Centralize meeting-related information in one platform
* Improve team collaboration and meeting organization
* Make meeting outcomes clear and actionable
* Reduce the risk of losing important decisions and tasks
* Automate task extraction using AI
* Provide a scalable and maintainable backend architecture

---

## ✨ Current Features

### 🔐 Authentication & User Management

* User registration
* User login
* JWT-based authentication
* Refresh token support
* Refresh token rotation
* Logout
* Logout from all sessions
* Forgot password
* Reset password
* Secure password hashing using BCrypt

---

### 🏢 Workspace Management

* Create workspaces
* View workspace details
* Update workspaces
* Delete workspaces
* Manage workspace members
* Workspace roles and permissions
* Owner and member management

---

### 📅 Meeting Management

* Create meetings
* View meetings
* View meeting details
* Update meetings
* Delete meetings
* Associate meetings with workspaces

> **Planned for a future phase:** meeting participants, invitations, and online meeting integration.

---

### 📝 Meeting Notes

* Create meeting notes
* View meeting notes
* Update notes
* Delete notes
* Associate notes with meetings

---

### ✅ Task Management

* Create tasks
* View tasks
* Update tasks
* Delete tasks
* Assign tasks to workspace members
* Manage task status and follow-up activities

---

### 🧠 AI-Powered Task Extraction

MeetFlow integrates AI capabilities to help extract actionable tasks from meeting notes.

The planned workflow is:

```text
Meeting Notes
      ↓
AI Processing
      ↓
Extracted Task Suggestions
      ↓
User Review & Confirmation
      ↓
Task Creation
      ↓
Task Assignment
```

The AI suggests actionable tasks, while the user remains responsible for reviewing and confirming the extracted results.

---

### 📌 Decision Management

* Create meeting decisions
* View decisions
* Update decisions
* Delete decisions
* Associate decisions with meetings

---

## 🏗️ Project Architecture

MeetFlow follows a layered architecture to ensure separation of concerns, maintainability, and scalability.

```text
MeetFlow
│
├── Backend
│   │
│   ├── MeetFlow_API
│   │   └── Controllers
│   │
│   ├── MeetFlow.BLL
│   │   ├── Services
│   │   ├── DTOs
│   │   └── Interfaces
│   │
│   ├── MeetFlow_DAL
│   │   ├── Entities
│   │   ├── Data
│   │   ├── Configurations
│   │   └── Repositories
│   │
│   └── MeetFlow.Core
│       ├── Common
│       ├── Interfaces
│       └── Shared Components
│
└── Frontend
    └── Frontend Application
```

### Backend Request Flow

```text
Client
   ↓
API Controller
   ↓
BLL Service
   ↓
Repository / Unit of Work
   ↓
Entity Framework Core
   ↓
SQL Server Database
```

This structure keeps API endpoints, business logic, and data access responsibilities separated.

---

## 🛠️ Technology Stack

### Backend

* C#
* .NET 8
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* LINQ
* JWT Authentication
* BCrypt Password Hashing

### API & Development Tools

* Swagger / OpenAPI
* Git
* GitHub
* Visual Studio / Visual Studio Code

### AI Integration

* Google Gemini API

### Integrations

* Email Service
* WhatsApp Integration *(planned / optional depending on project phase)*

### Frontend

* Frontend application *(under development)*

---

## 🔐 Authentication

MeetFlow uses JWT-based authentication to secure API endpoints.

The authentication flow includes:

```text
Register
   ↓
Login
   ↓
Access Token + Refresh Token
   ↓
Authenticated API Requests
   ↓
Refresh Token Rotation
```

Sensitive configuration values such as:

* JWT secrets
* Database connection strings
* Email credentials
* External API keys

should be stored using environment-specific configuration and should never be committed to the repository.

---

## ⚙️ Environment Setup

### Prerequisites

Before running the project, make sure you have:

* .NET 8 SDK
* SQL Server
* Visual Studio 2022/2026 or Visual Studio Code
* Git
* A configured Gemini API key *(for AI features)*

---

### 1. Clone the Repository

```bash
git clone <https://github.com/Rawda2007/Project_MeetFlow.git>


### 2. Configure the Database

Update the database connection string in the appropriate environment configuration.
---

### 3. Configure Application Secrets

Configure the required environment-specific settings, such as:

* Database connection string
* JWT configuration
* Email settings
* Gemini API key
* WhatsApp credentials

For local development, use:

* .NET User Secrets
* Environment Variables

---

### 4. Apply Database Migrations

From the backend project directory, run:

```bash
dotnet ef database update
```

If migrations are not available in the current environment, make sure the required EF Core tools and database configuration are properly set up.

---

### 5. Run the Backend

```bash
dotnet run
```

The API can then be accessed through the configured local URL.

---

### 6. Explore the API

After running the application, open the Swagger UI using the URL displayed by the application.

Swagger provides an interactive interface for testing the available API endpoints.

---

## 📂 Repository Structure

```text
MeetFlow
│
├── Backend
│   ├── MeetFlow_API
│   ├── MeetFlow.BLL
│   ├── MeetFlow_DAL
│   └── MeetFlow.Core
│
├── Frontend
│   └── Frontend Project
│
├── .gitignore
├── README.md
└── MeetFlow.sln
```

---

## 🔄 Development Roadmap

### Phase 1 — Project Planning & System Design

* Problem Statement
* Value Proposition Canvas
* Business Model Canvas
* System Blueprint
* Technical Planning
* Documentation Repository

### Phase 2 — Initial Backend & Core Features

* Project Architecture
* Database Design
* Authentication
* User Management
* Workspace Management
* Meeting Management
* Meeting Notes
* Decisions
* Tasks
* AI Task Extraction

### Phase 3 — Collaboration & Meeting Experience

Planned features include:

* Meeting Participants
* Meeting Invitations
* Join Meeting Flow
* Meeting Links
* Meeting Lifecycle Management
* Notifications and Reminders
* Enhanced AI-powered meeting follow-up
* Additional integrations

---

## 🔒 Security Guidelines

Never commit the following information to GitHub:

* Passwords
* JWT secrets
* API keys
* Database credentials
* Email passwords
* WhatsApp credentials
* Any other sensitive configuration

Use environment variables or .NET User Secrets for local development and secure environment configuration for production.

---

## 🤝 Contribution Guidelines

When contributing to MeetFlow:

1. Create a feature branch.
2. Implement the feature following the existing architecture.
3. Test the changes locally.
4. Use clear and meaningful commit messages.
5. Create a Pull Request for review.

### Commit Message Examples

```text
feat: add workspace member management
feat: implement meeting CRUD operations
feat: add AI task extraction service
fix: resolve workspace authorization issue
fix: handle duplicate workspace members
refactor: improve meeting service structure
docs: update environment setup instructions
chore: update project dependencies
```

---

## 📜 License

This project is currently developed as a team project for educational and development purposes.

---

## 👥 Team

**MeetFlow Development Team**

A collaborative project focused on improving meeting organization, decision tracking, and post-meeting follow-up through modern backend technologies and AI-powered automation.

