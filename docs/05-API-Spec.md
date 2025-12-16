# API Specification — To-Do Task Management MVP

## 1. Purpose and Scope

This document defines the **public REST API** for the To-Do Task Management MVP.

- Describes available endpoints, HTTP methods, and expected behavior.
- Defines JSON request and response formats.
- Specifies status codes and error conventions.
- Targets a simple **single-user** MVP using **EF Core InMemory** on the backend.

This specification reflects the **current implementation** and intentionally avoids documenting endpoints or query options that are not implemented yet.

---

## 2. Base URL and General Conventions

- Base URL (local development example):
  - `https://localhost:7157`
- All endpoints are relative to `/api`.

### Task Endpoints

- `/api/tasks`
- `/api/tasks/{id}`

### General Conventions

- Data format: JSON (request & response)
- Character encoding: UTF-8
- `{id}` is an integer task identifier
- HTTP status codes follow REST conventions

---

## 3. Task Resource Model

### 3.1 Task Fields

The **Task** resource represents a to-do item.

| Field        | Type      | Description |
|-------------|-----------|-------------|
| `id`        | integer   | Server-generated unique identifier |
| `title`     | string    | Required, non-empty task title |
| `description` | string \| null | Optional description |
| `isCompleted` | boolean | Task completion status |
| `createdAt` | string (ISO 8601 datetime) | Creation timestamp (server-set) |
| `updatedAt` | string (ISO 8601 datetime \| null) | Last update timestamp (null until first update) |

> Notes:
> - The current MVP uses a **boolean status model** (`isCompleted`) instead of multiple workflow states.
> - Optional fields like `priority` and `dueDate` are **not implemented yet** and intentionally omitted.

---

## 4. Standard Error Response Format

All error responses follow a consistent JSON structure:

```json
{
  "errorCode": "ValidationError",
  "message": "One or more validation errors occurred.",
  "details": [
    { "field": "title", "message": "Title is required." }
  ]
}
```

### Fields

- `errorCode` — Machine-readable identifier
- `message` — Human-readable summary
- `details` — Optional list of field-level errors

---

## 5. Endpoints

### 5.1 List Tasks

**GET `/api/tasks`**

Retrieve all tasks.

#### Responses

- `200 OK`
  - Body: array of Task resources
- `500 Internal Server Error`
  - Body: error object

> Filtering by status is currently handled on the **frontend**, not via query parameters.

---

### 5.2 Get Task by Id

**GET `/api/tasks/{id}`**

Retrieve a single task by its identifier.

#### Responses

- `200 OK`
  - Body: Task resource
- `404 Not Found`
  - Body: error object (`errorCode: "NotFound"`)

---

### 5.3 Create Task

**POST `/api/tasks`**

Create a new task.

#### Request Body

```json
{
  "title": "Buy groceries",
  "description": "Eggs and bread"
}
```

- `title` is required
- `description` is optional
- `isCompleted`, `id`, `createdAt`, `updatedAt` are ignored if provided

#### Responses

- `201 Created`
  - Header: `Location: /api/tasks/{id}`
  - Body: created Task resource
- `400 Bad Request`
  - Validation error
- `500 Internal Server Error`

---

### 5.4 Update Task

**PUT `/api/tasks/{id}`**

Update an existing task.

#### Request Body

```json
{
  "title": "Buy groceries",
  "description": "Eggs, bread, and milk",
  "isCompleted": true
}
```

#### Responses

- `200 OK`
  - Body: updated Task resource
- `400 Bad Request`
- `404 Not Found`
- `500 Internal Server Error`

---

### 5.5 Delete Task

**DELETE `/api/tasks/{id}`**

Delete a task permanently.

#### Responses

- `204 No Content`
- `404 Not Found`
- `500 Internal Server Error`

---

## 6. Validation Rules

### Title
- Required
- Non-empty after trimming
- Maximum length enforced (implementation-defined)

### Description
- Optional
- Maximum length enforced

### isCompleted
- Boolean only

On validation failure:
- HTTP `400 Bad Request`
- Error object with details

---

## 7. Status Codes Summary

| Code | Meaning |
|-----|--------|
| 200 | Success |
| 201 | Resource created |
| 204 | Resource deleted |
| 400 | Validation or client error |
| 404 | Resource not found |
| 500 | Server error |

---

## 8. Versioning Strategy

- MVP endpoints are **unversioned**:
  - `/api/tasks`
- Future versions may introduce:
  - `/api/v1/tasks`
- API is structured to allow versioning without breaking clients.

---

## 9. CORS and Frontend Integration

- Backend allows CORS from the frontend origin in development.
- Frontend:
  - Uses a configurable base URL (via environment variables)
  - Handles errors based on the standard error response format

---

## 10. Future Enhancements (API Perspective)

Planned or possible future additions:

- Task priority and due dates
- Server-side filtering and sorting
- Pagination
- Partial updates (`PATCH`)
- Authentication and user-scoped tasks
- API versioning

These enhancements can be added without redesigning the current API.

---
