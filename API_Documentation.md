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
