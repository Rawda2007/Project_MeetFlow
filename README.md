# MeetFlow

> **ASP.NET Core Web API Backend for a Meeting Management & Follow-up Platform**

MeetFlow is a backend platform built with **ASP.NET Core Web API** to help teams organize meetings, document discussions, track decisions, and manage follow-up tasks. The platform also integrates **Google Gemini AI** to extract actionable tasks from meeting notes.

---

## ✨ Features

- 🔐 JWT Authentication & Refresh Tokens
- 👤 User Management
- 🏢 Workspace Management
- 📅 Meeting Management
- 📝 Meeting Notes
- 📌 Decision Management
- ✅ Task Management
- 🤖 AI Task Extraction using Google Gemini

---

## 🛠️ Technology Stack

- ASP.NET Core Web API (.NET 8)
- C#
- Entity Framework Core
- SQL Server
- LINQ
- JWT Authentication
- BCrypt Password Hashing
- Swagger / OpenAPI
- Google Gemini API

---

## ⚙️ Quick Start

### Prerequisites

- .NET 8 SDK
- SQL Server
- Visual Studio 2022/2026 or VS Code
- Git

Clone the repository:

```bash
git clone https://github.com/Rawda2007/Project_MeetFlow.git
```

Run the project:

```bash
dotnet ef database update
dotnet run
```

For the complete setup guide, see:

- [Environment Setup](docs/Environment_Setup.md)

---

## 📚 Documentation

Detailed project documentation is available in the **docs** folder:

- [API Documentation](docs/API_Documentation.md)
- [Database Documentation](docs/Database_Documentation.md)
- [Architecture](docs/Architecture.md)
- [Environment Setup](docs/Environment_Setup.md)

---

## 📂 Project Structure

```text
Project_MeetFlow
│
├── Backend
├── Frontend
├── docs
│   ├── API_Documentation.md
│   ├── Database_Documentation.md
│   ├── Architecture.md
│   ├── Environment_Setup.md
│   └── images
│
├── README.md
└── MeetFlow.sln
```

---

## 🔒 Security

Sensitive information such as database connection strings, JWT secrets, email credentials, and API keys is stored using **Environment Variables** or **.NET User Secrets** and is never committed to the repository.

---

## 🤝 Contributing

1. Create a feature branch.
2. Implement your changes.
3. Test the project.
4. Commit using meaningful messages.
5. Open a Pull Request.

Example:

```text
feat: add workspace management
fix: resolve authorization issue
docs: update API documentation
```

---

## 📜 License

This project was developed for educational purposes as a collaborative team project.
