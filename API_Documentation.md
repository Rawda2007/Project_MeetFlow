# MeetFlow API Documentation

This document provides an overview of the RESTful APIs available in the MeetFlow backend.

**Base URL**

```
https://localhost:7084/api
```

---

# API Modules

The MeetFlow backend is organized into the following functional modules:

| Module | Description |
|---------|-------------|
| Authentication | User authentication, authorization, session management, and password recovery. |
| User Management | Manage user profile, account information, and password. |
| Workspace Management | Create workspaces and manage members and roles. |
| Meeting Management | Create, update, and organize meetings. |
| Meeting Notes | Store and manage meeting notes. |
| Decision Management | Record and manage meeting decisions. |
| Task Management | Create, assign, update, and track meeting tasks with AI support. |

---

# Table of Contents

- [Authentication](#authentication)
- [User Management](#user-management)
- [Workspace Management](#workspace-management)
- [Meeting Management](#meeting-management)
- [Meeting Notes](#meeting-notes)
- [Decision Management](#decision-management)
- [Task Management](#task-management)

---


# Authentication

The Authentication module is responsible for user registration, login, session management, and password recovery.

| Method | Endpoint | Authentication | Description |
|--------|----------|---------------|-------------|
| POST | /Auth/register | No | Register a new user |
| POST | /Auth/login | No | Authenticate a user and return JWT & Refresh Token |
| POST | /Auth/refresh-token | No | Generate a new Access Token using a valid Refresh Token |
| POST | /Auth/logout | Yes | Logout from the current session |
| POST | /Auth/logout-all | Yes | Logout from all active sessions |
| POST | /Auth/forgot-password | No | Send password reset code to user's email |
| POST | /Auth/reset-password | No | Reset the user password using the verification code |

---

## Register

**POST** `/api/Auth/register`

Registers a new user.

### Request Body

| Field | Type | Required |
|------|------|----------|
| fullName | string | Yes |
| email | string | Yes |
| password | string | Yes |

### Response

Returns:

- User Id
- Full Name
- Email
- Access Token
- Refresh Token
- Token Expiration Date

---

## Login

**POST** `/api/Auth/login`

Authenticates an existing user.

### Request Body

| Field | Type | Required |
|------|------|----------|
| email | string | Yes |
| password | string | Yes |

### Response

Returns a JWT Access Token and Refresh Token.

---

## Refresh Token

**POST** `/api/Auth/refresh-token`

Generates a new Access Token using a valid Refresh Token.

---

## Logout

**POST** `/api/Auth/logout`

Logs out the current session.

---

## Logout All

**POST** `/api/Auth/logout-all`

Terminates all active sessions for the authenticated user.

---

## Forgot Password

**POST** `/api/Auth/forgot-password`

Sends a password reset code to the registered email address.

---

## Reset Password

**POST** `/api/Auth/reset-password`

Resets the user's password using the verification code.

---

# Decisions

Decision APIs are used to manage meeting decisions.

| Method | Endpoint | Authentication | Description |
|--------|----------|---------------|-------------|
| GET | /meetings/{meetingId}/decisions | Yes | Retrieve all decisions for a meeting |
| POST | /meetings/{meetingId}/decisions | Yes | Create a new decision |
| PUT | /meetings/{meetingId}/decisions/{decisionId} | Yes | Update an existing decision |
| DELETE | /meetings/{meetingId}/decisions/{decisionId} | Yes | Delete a decision |

---

## Get Decisions

**GET** `/api/meetings/{meetingId}/decisions`

Returns all decisions associated with the specified meeting.

---

## Create Decision

**POST** `/api/meetings/{meetingId}/decisions`

### Request Body

| Field | Type | Required |
|------|------|----------|
| description | string | Yes |

Creates a new decision for the specified meeting.

---

## Update Decision

**PUT** `/api/meetings/{meetingId}/decisions/{decisionId}`

Updates the description of an existing decision.

---

## Delete Decision

**DELETE** `/api/meetings/{meetingId}/decisions/{decisionId}`

Deletes the specified decision.

# Meeting Management

The Meeting module provides APIs for creating, managing, and organizing meetings within workspaces. Each meeting can contain notes, decisions, and follow-up tasks.

| Method | Endpoint | Authentication | Description |
|--------|----------|---------------|-------------|
| GET | `/api/Meetings/workspace/{workspaceId}` | Yes | Retrieve all meetings for a specific workspace. |
| POST | `/api/Meetings` | Yes | Create a new meeting. |
| GET | `/api/Meetings/{id}` | Yes | Retrieve meeting details by ID. |
| PUT | `/api/Meetings/{id}` | Yes | Update an existing meeting. |
| DELETE | `/api/Meetings/{id}` | Yes | Delete a meeting. |

---

## Create Meeting

**POST** `/api/Meetings`

Creates a new meeting within a workspace.

### Request Body

| Field | Type | Required | Description |
|------|------|----------|-------------|
| workspaceId | integer | Yes | Identifier of the workspace. |
| title | string | Yes | Meeting title. |
| description | string | No | Meeting description. |
| meetingDate | datetime | Yes | Scheduled meeting date and time. |

### Response

Returns the created meeting with its details, including:

- Meeting ID
- Workspace ID
- Title
- Description
- Meeting Date
- Creator information
- Creation date
- Number of meeting notes

---

## Get Meetings by Workspace

**GET** `/api/Meetings/workspace/{workspaceId}`

Returns all meetings that belong to the specified workspace.

---

## Get Meeting Details

**GET** `/api/Meetings/{id}`

Returns detailed information about a specific meeting.

---

## Update Meeting

**PUT** `/api/Meetings/{id}`

Updates the meeting title, description, or scheduled date.

### Request Body

| Field | Type | Required |
|------|------|----------|
| title | string | Yes |
| description | string | No |
| meetingDate | datetime | Yes |

---

## Delete Meeting

**DELETE** `/api/Meetings/{id}`

Deletes the specified meeting.

---

# Meeting Notes

Meeting Notes APIs allow users to record, update, retrieve, and delete notes associated with meetings.

| Method | Endpoint | Authentication | Description |
|--------|----------|---------------|-------------|
| GET | `/api/Meetings/{id}/notes` | Yes | Retrieve all notes for a meeting. |
| POST | `/api/Meetings/{id}/notes` | Yes | Add a new note to a meeting. |
| PUT | `/api/Meetings/{id}/notes/{noteId}` | Yes | Update an existing meeting note. |
| DELETE | `/api/Meetings/{id}/notes/{noteId}` | Yes | Delete a meeting note. |

---

## Add Meeting Note

**POST** `/api/Meetings/{id}/notes`

Creates a new note for the specified meeting.

### Request Body

| Field | Type | Required | Description |
|------|------|----------|-------------|
| content | string | Yes | The content of the meeting note. |

### Response

Returns the created note, including:

- Note ID
- Meeting ID
- Note content
- Creator information
- Creation date

---

## Get Meeting Notes

**GET** `/api/Meetings/{id}/notes`

Returns all notes associated with the specified meeting.

---

## Update Meeting Note

**PUT** `/api/Meetings/{id}/notes/{noteId}`

Updates the content of an existing meeting note.

### Request Body

| Field | Type | Required |
|------|------|----------|
| content | string | Yes |

---

## Delete Meeting Note

**DELETE** `/api/Meetings/{id}/notes/{noteId}`

Deletes the specified meeting note.

---

# Task Management

The Task module enables users to create, assign, manage, and track follow-up tasks generated during meetings. It also supports AI-powered task extraction from meeting notes.

| Method | Endpoint | Authentication | Description |
|--------|----------|---------------|-------------|
| GET | `/api/meetings/{meetingId}/tasks` | Yes | Retrieve all tasks for a meeting. |
| POST | `/api/meetings/{meetingId}/tasks` | Yes | Create a new task. |
| PUT | `/api/meetings/{meetingId}/tasks/{taskId}` | Yes | Update an existing task. |
| DELETE | `/api/meetings/{meetingId}/tasks/{taskId}` | Yes | Delete a task. |
| PUT | `/api/meetings/{meetingId}/tasks/{taskId}/status` | Yes | Update the status of a task. |
| POST | `/api/meetings/{meetingId}/tasks/extract-from-notes` | Yes | Extract task suggestions from meeting notes using AI. |
| GET | `/api/tasks/my` | Yes | Retrieve tasks assigned to the authenticated user. |

---

## Create Task

**POST** `/api/meetings/{meetingId}/tasks`

Creates a new task associated with a meeting.

### Request Body

| Field | Type | Required | Description |
|------|------|----------|-------------|
| title | string | Yes | Task title. |
| description | string | No | Task description. |
| assignedTo | integer | Yes | Assigned user ID. |
| dueDate | datetime | No | Task due date. |
| priority | string | Yes | Task priority level. |

### Response

Returns the created task with assignment and status information.

---

## Get Meeting Tasks

**GET** `/api/meetings/{meetingId}/tasks`

Returns all tasks associated with the specified meeting.

---

## Update Task

**PUT** `/api/meetings/{meetingId}/tasks/{taskId}`

Updates an existing task.

---

## Update Task Status

**PUT** `/api/meetings/{meetingId}/tasks/{taskId}/status`

Updates only the task status (e.g., Pending, In Progress, Completed).

### Request Body

| Field | Type | Required |
|------|------|----------|
| status | string | Yes |

---

## Delete Task

**DELETE** `/api/meetings/{meetingId}/tasks/{taskId}`

Deletes the specified task.

---

## AI Task Extraction

**POST** `/api/meetings/{meetingId}/tasks/extract-from-notes`

Uses AI to analyze meeting notes and generate task suggestions that can later be reviewed and confirmed by the user.

### Request Body

| Field | Type | Required |
|------|------|----------|
| notesText | string | Yes |

---

## My Tasks

**GET** `/api/tasks/my`

Returns all tasks currently assigned to the authenticated user.

# User Management

The User module enables authenticated users to manage their personal account information, including viewing and updating their profile, changing their password, and deleting their account.

| Method | Endpoint | Authentication | Description |
|--------|----------|---------------|-------------|
| GET | `/api/User/me` | Yes | Retrieve the authenticated user's profile information. |
| PUT | `/api/User/me` | Yes | Update the authenticated user's profile information. |
| DELETE | `/api/User/me` | Yes | Delete the authenticated user's account. |
| PUT | `/api/User/change-password` | Yes | Change the authenticated user's password. |

---

## Get Current User

**GET** `/api/User/me`

Returns the profile information of the authenticated user.

### Response

Returns:

- User ID
- Full Name
- Email Address
- Phone Number
- Account Creation Date

---

## Update Profile

**PUT** `/api/User/me`

Updates the authenticated user's profile information.

### Request Body

| Field | Type | Required | Description |
|------|------|----------|-------------|
| fullName | string | Yes | User's full name. |
| phoneNumber | string | No | User's phone number. |

### Response

Returns the updated user profile.

---

## Change Password

**PUT** `/api/User/change-password`

Changes the authenticated user's password.

### Request Body

| Field | Type | Required | Description |
|------|------|----------|-------------|
| currentPassword | string | Yes | Current account password. |
| newPassword | string | Yes | New password. |

---

## Delete Account

**DELETE** `/api/User/me`

Permanently deletes the authenticated user's account.

---

# Workspace Management

The Workspace module allows users to create, manage, and collaborate within workspaces. It also provides APIs for managing workspace members and their roles.

| Method | Endpoint | Authentication | Description |
|--------|----------|---------------|-------------|
| GET | `/api/Workspaces` | Yes | Retrieve all workspaces accessible to the authenticated user. |
| POST | `/api/Workspaces` | Yes | Create a new workspace. |
| GET | `/api/Workspaces/{id}` | Yes | Retrieve workspace details. |
| PUT | `/api/Workspaces/{id}` | Yes | Update workspace information. |
| DELETE | `/api/Workspaces/{id}` | Yes | Delete a workspace. |
| GET | `/api/Workspaces/{id}/members` | Yes | Retrieve all workspace members. |
| POST | `/api/Workspaces/{id}/members` | Yes | Add a new member to the workspace. |
| PUT | `/api/Workspaces/{id}/members/{userId}/role` | Yes | Update a member's workspace role. |
| DELETE | `/api/Workspaces/{id}/members/{userId}` | Yes | Remove a member from the workspace. |

---

## Create Workspace

**POST** `/api/Workspaces`

Creates a new workspace.

### Request Body

| Field | Type | Required | Description |
|------|------|----------|-------------|
| name | string | Yes | Workspace name. |

### Response

Returns the created workspace, including:

- Workspace ID
- Workspace Name
- Creator Information
- Creation Date
- Member Count
- Current User Role

---

## Get Workspaces

**GET** `/api/Workspaces`

Returns all workspaces that the authenticated user belongs to.

---

## Get Workspace Details

**GET** `/api/Workspaces/{id}`

Returns detailed information about a specific workspace.

---

## Update Workspace

**PUT** `/api/Workspaces/{id}`

Updates the workspace name.

### Request Body

| Field | Type | Required |
|------|------|----------|
| name | string | Yes |

---

## Delete Workspace

**DELETE** `/api/Workspaces/{id}`

Deletes the specified workspace.

---

## Get Workspace Members

**GET** `/api/Workspaces/{id}/members`

Returns all members within the specified workspace, including their assigned roles.

---

## Add Workspace Member

**POST** `/api/Workspaces/{id}/members`

Adds a new member to the workspace using their email address.

### Request Body

| Field | Type | Required | Description |
|------|------|----------|-------------|
| email | string | Yes | Email address of the user to be invited or added. |

---

## Update Member Role

**PUT** `/api/Workspaces/{id}/members/{userId}/role`

Updates the role of an existing workspace member.

### Request Body

| Field | Type | Required | Description |
|------|------|----------|-------------|
| role | string | Yes | New role assigned to the workspace member. |

---

## Remove Workspace Member

**DELETE** `/api/Workspaces/{id}/members/{userId}`

Removes the specified member from the workspace.
