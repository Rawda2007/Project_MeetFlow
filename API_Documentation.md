# MeetFlow API Documentation

This document provides an overview of the RESTful APIs available in the MeetFlow backend.

**Base URL**

```
https://localhost:7084/api
```

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
