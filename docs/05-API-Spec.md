# API Specification — To-Do Task Management MVP

## 1. Purpose and Scope

This document defines the **public REST API** for the To-Do Task Management MVP.

- Describes available endpoints, HTTP methods, and expected behavior.
- Defines JSON request and response formats.
- Specifies status codes and error conventions.
- Targets a simple **single-user** MVP using **EF Core InMemory** on the backend.

This spec is implementation-oriented but not tied to specific internal class names.

---

## 2. Base URL and General Conventions

- Base URL (local development example):  
  - Backend: https://localhost:7157 (actual port may differ)
  - All endpoints are relative to `/api`.

- API endpoints for tasks:
  - `/api/tasks`
  - `/api/tasks/{id}`

- General conventions:
  - Data format: JSON for both requests and responses.
  - Character encoding: UTF-8.
  - Path parameter `{id}` refers to the **task identifier** (e.g., integer).
  - HTTP status codes follow REST conventions (200, 201, 400, 404, 500, etc.).

---

## 3. Task Resource Model

### 3.1 Task Fields

The **Task** resource represents a to-do item:

- `id`  
  - Type: integer  
  - Description: Unique identifier of the task (assigned by the server).

- `title`  
  - Type: string  
  - Description: Short description of the task.  
  - Constraints: required, non-empty, reasonable max length (e.g., 200 characters).

- `description`  
  - Type: string or null  
  - Description: Longer description or notes.  
  - Constraints: optional, reasonable max length (e.g., 2000 characters).

- `status`  
  - Type: string  
  - Description: Task status.  
  - Allowed values (MVP):  
    - `"Todo"`  
    - `"InProgress"`  
    - `"Done"`  
  - Default: `"Todo"` if not provided on creation.

- `priority` (optional for MVP)  
  - Type: string or null  
  - Example allowed values: `"Low"`, `"Medium"`, `"High"`.

- `dueDate` (optional)  
  - Type: string (ISO 8601 date, e.g., `"2025-12-31"`), or null.  
  - Description: Optional due date.

- `createdAt`  
  - Type: string (ISO 8601 date-time)  
  - Description: Timestamp when the task was created (set by server).

- `updatedAt`  
  - Type: string (ISO 8601 date-time)  
  - Description: Timestamp of last update (set by server).

The exact set of optional fields (`priority`, `dueDate`) can be reduced or expanded as needed, but `id`, `title`, `status`, `createdAt` and `updatedAt` should be consistently present in responses.

---

## 4. Standard Error Response Format

Errors follow a consistent JSON structure:

- `errorCode`  
  - Short machine-friendly string identifier (e.g., `"ValidationError"`, `"NotFound"`, `"ServerError"`).

- `message`  
  - Human-readable description of the error (for developers and possibly users).

- `details`  
  - Optional array with additional context (e.g., per-field validation errors).

Example error shape (conceptual):

    {
      "errorCode": "ValidationError",
      "message": "One or more validation errors occurred.",
      "details": [
        { "field": "title", "message": "Title is required." }
      ]
    }

---

## 5. Endpoints

### 5.1 List Tasks

**Endpoint:**  
- `GET /api/tasks`

**Description:**  
Retrieve a list of tasks.

**Query parameters (all optional):**

- `status`  
  - Type: string  
  - Allowed values: `"Todo"`, `"InProgress"`, `"Done"`  
  - If provided, filters tasks by status.

- `search`  
  - Type: string  
  - If provided, performs a simple search on title (and optionally description).

- `sortBy`  
  - Type: string  
  - Example allowed values: `"createdAt"`, `"dueDate"`, `"title"` (depending on implementation).  
  - Optional for MVP.

- `sortDirection`  
  - Type: string  
  - Allowed values: `"asc"`, `"desc"`  
  - Optional for MVP.

- `page` / `pageSize` (optional, for future)  
  - Pagination parameters. MVP may initially return all tasks without pagination.

**Responses:**

- `200 OK`  
  - Body: array of Task resources.

- `500 Internal Server Error`  
  - Body: error object (see standard error format).

---

### 5.2 Get Task by Id

**Endpoint:**  
- `GET /api/tasks/{id}`

**Description:**  
Retrieve a single task by its identifier.

**Path parameters:**

- `id` (integer) – task identifier.

**Responses:**

- `200 OK`  
  - Body: Task resource.

- `404 Not Found`  
  - Body: error object with `errorCode` `"NotFound"`.

- `500 Internal Server Error`  
  - Body: error object.

---

### 5.3 Create Task

**Endpoint:**  
- `POST /api/tasks`

**Description:**  
Create a new task.

**Request body (JSON):**

Expected fields:

- `title` (required, string)
- `description` (optional, string)
- `status` (optional, string; defaults to `"Todo"` if omitted or invalid)
- `priority` (optional, string)
- `dueDate` (optional, ISO 8601 date string)

Fields like `id`, `createdAt`, and `updatedAt` are **ignored** if provided in the request and are set by the server.

**Responses:**

- `201 Created`  
  - Headers:
    - `Location`: URL of the created task (e.g., `/api/tasks/123`).
  - Body: Task resource with server-assigned `id`, `createdAt`, `updatedAt`.

- `400 Bad Request`  
  - Body: error object with `errorCode` `"ValidationError"` when:
    - `title` is missing or invalid.
    - Other fields violate allowed values.

- `500 Internal Server Error`  
  - Body: error object.

---

### 5.4 Update Task (Full Update)

**Endpoint:**  
- `PUT /api/tasks/{id}`

**Description:**  
Replace an existing task with new data (full update).

**Path parameters:**

- `id` (integer) – identifier of the task to update.

**Request body (JSON):**

Expected fields (similar to create, but representing the new desired state):

- `title` (required, string)
- `description` (optional, string)
- `status` (required or optional based on design; if optional, defaulting rules apply)
- `priority` (optional)
- `dueDate` (optional)

Fields like `id`, `createdAt`, `updatedAt` in the payload are ignored or validated according to implementation; typically, the server uses the path `id` and internal timestamps.

**Responses:**

- `200 OK`  
  - Body: updated Task resource.

- `400 Bad Request`  
  - Body: error object for validation errors.

- `404 Not Found`  
  - Body: error object when task with given `id` does not exist.

- `500 Internal Server Error`  
  - Body: error object.

---

### 5.5 Partial Update: Change Task Status (Optional but Recommended)

**Endpoint:**  
- `PATCH /api/tasks/{id}/status`

**Description:**  
Change only the status of a task without modifying other fields.

**Path parameters:**

- `id` (integer) – identifier of the task.

**Request body (JSON):**

- `status` (required, string)  
  - One of `"Todo"`, `"InProgress"`, `"Done"`.

**Responses:**

- `200 OK`  
  - Body: updated Task resource.

- `400 Bad Request`  
  - Body: error object if the new status is invalid.

- `404 Not Found`  
  - Body: error object if the task does not exist.

- `500 Internal Server Error`  
  - Body: error object.

Note: If this endpoint is not implemented, equivalent behavior must be supported via `PUT /api/tasks/{id}`.

---

### 5.6 Delete Task

**Endpoint:**  
- `DELETE /api/tasks/{id}`

**Description:**  
Delete a task permanently.

**Path parameters:**

- `id` (integer) – identifier of the task to delete.

**Responses:**

- `204 No Content`  
  - Body: empty.  
  - Indicates the task has been successfully deleted.

- `404 Not Found`  
  - Body: error object when task with given `id` does not exist.

- `500 Internal Server Error`  
  - Body: error object.

---

## 6. Validation Rules (API Perspective)

The API enforces the following validation logic:

- `title`
  - Must be present and non-empty after trimming whitespace.
  - Must not exceed the maximum length (e.g., 200 characters).

- `description` (if provided)
  - Must not exceed the maximum length (e.g., 2000 characters).

- `status` (if provided)
  - Must be one of: `"Todo"`, `"InProgress"`, `"Done"`.

- `priority` (if used)
  - Must be one of allowed values (e.g., `"Low"`, `"Medium"`, `"High"`), or omitted.

- `dueDate` (if provided)
  - Must be a valid date in ISO 8601 format.
  - MVP may not enforce “future date only”, but can document it as a potential future rule.

On validation failure, the API returns:

- HTTP status: `400 Bad Request`
- Body: error object with details about failing fields.

---

## 7. Status Codes Summary

The following status codes are used across endpoints:

- `200 OK`  
  - Successful retrieval or update.

- `201 Created`  
  - Successful creation of a resource; `Location` header points to new resource.

- `204 No Content`  
  - Successful deletion with no body.

- `400 Bad Request`  
  - The request cannot be processed due to client-side issues (validation errors, malformed JSON, etc.).

- `404 Not Found`  
  - The requested resource does not exist.

- `500 Internal Server Error`  
  - An unexpected error occurred on the server.

---

## 8. Versioning Strategy (MVP)

- The MVP may initially expose endpoints without an explicit version:  
  - Example: `/api/tasks`

- Future versions can introduce explicit versioning, such as:  
  - `/api/v1/tasks` or versioning via headers.

- The API specification and code should be structured so that:
  - New versions can be introduced without breaking existing clients.
  - Old versions can be deprecated gracefully.

---

## 9. CORS and Frontend Integration Notes

- The backend should be configured to allow CORS from the frontend origin during development.
- The frontend will:
  - Use the base API URL (configurable, e.g., via environment variables).
  - Handle error responses based on the standard error format.
  - Map API data to its internal models and UI components.

---

## 10. Future Enhancements (API Perspective)

Future changes that may extend this spec include:

- Adding **authentication** and associating tasks with user accounts.
- Adding **pagination** and more advanced sorting/filtering on `GET /api/tasks`.
- Adding endpoints for:
  - Task tags or categories.
  - Task comments or activity history.
- Supporting partial updates for other fields via `PATCH /api/tasks/{id}`.
- Introducing API versioning (e.g., `/api/v1`, `/api/v2`) when breaking changes are necessary.

These enhancements should be possible without a complete redesign due to the current resource-based REST structure.

---
