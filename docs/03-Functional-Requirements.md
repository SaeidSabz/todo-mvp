# Functional Requirements — To-Do Task Management MVP

## 1. Purpose and Scope

This document defines the **functional behavior** of the To-Do Task Management MVP:

- What the system must do from a user’s perspective
- How tasks are created, viewed, updated, and deleted
- What validations and rules apply
- What the user can expect from the UI and API

Technical implementation details are covered in `Architecture-Overview.md` and other docs.

---

## 2. Actors and Roles

For this MVP we assume a **single, anonymous user context**:

- **User**
  - Interacts with the system through the React frontend.
  - Can manage tasks (create, read, update, delete).
- **System**
  - Backend API that processes requests, applies business rules, and stores tasks in memory.

No authentication, roles, or multi-user separation are implemented in this MVP.

---

## 3. Core User Stories

### 3.1 Create Task

- As a user, I want to **create a new task** so that I can track something I need to do.

**Acceptance criteria:**

- The user can open a “New Task” form from the main UI.
- The user can input:
  - Title (required)
  - Optional description
  - Optional status (default is “Todo” if omitted)
  - Optional due date
  - Optional priority (e.g., Low / Medium / High or similar)
- When the user submits:
  - If the input is valid, the task is created and visible in the list.
  - If the input is invalid, the user is shown clear validation messages.
- The backend returns appropriate responses (success or error) through the API.

---

### 3.2 View Task List

- As a user, I want to **see a list of my tasks** so that I can understand what I need to do.

**Acceptance criteria:**

- The main screen shows a list of tasks with at least:
  - Task title
  - Status
  - Optional: due date and/or priority
- On initial load, the system retrieves all tasks from the backend.
- If there are no tasks:
  - An “empty state” message is displayed (e.g., “No tasks yet”).
- The list should update after:
  - Creating a new task
  - Updating an existing task
  - Deleting a task
  - Changing task status

---

### 3.3 View Task Details

- As a user, I want to **view the full details of a task** so that I can see all relevant information.

**Acceptance criteria:**

- The user can select a task from the list.
- The system shows its full details:
  - Title
  - Description
  - Status
  - Created/updated timestamps (if displayed)
  - Optional: due date and priority
- The details view may be:
  - A dedicated page, or
  - A modal / inline panel, as long as the information is accessible.

---

### 3.4 Update Task

- As a user, I want to **edit an existing task** so that I can keep information up to date.

**Acceptance criteria:**

- The user can open an “Edit Task” form for a specific task.
- The user can modify:
  - Title
  - Description
  - Status
  - Due date (optional)
  - Priority (optional)
- When the user submits:
  - If the input is valid, changes are saved and reflected in the list.
  - If the input is invalid, the user is shown validation messages.
- A task that is being edited should not be duplicated; the original record should be updated.

---

### 3.5 Change Task Status

- As a user, I want to **change a task’s status** so that I can track its progress.

**Acceptance criteria:**

- Each task has a status (at minimum): Todo, In Progress, Done.  
  Exact labels may be adjusted, but there must be a small, clear set of statuses.
- The user can change status via:
  - Editing the task, or
  - A quick action (e.g., dropdown, button) in the list.
- The UI is updated to reflect the new status.
- The backend stores the new status and returns it in subsequent reads.

---

### 3.6 Delete Task

- As a user, I want to **delete a task** so that I can remove items I no longer need.

**Acceptance criteria:**

- The user can trigger delete from:
  - The task list, or
  - The task detail view.
- The system may ask for confirmation (optional, recommended).
- Once deleted:
  - The task no longer appears in the list.
  - Subsequent attempts to fetch the task by id return “not found” from the API.

---

### 3.7 Filter and/or Group Tasks by Status

- As a user, I want to **filter or view tasks by status** so that I can focus on what matters now.

**Acceptance criteria:**

- The user can filter tasks to show:
  - All tasks
  - Only Todo tasks
  - Only In Progress tasks
  - Only Done tasks
- Filtering can be implemented as:
  - Tabs
  - Dropdown filter
  - Separate columns (Kanban-style)
- The filter state is clearly visible to the user.

---

### 3.8 Optional: Sort or Basic Search

(Production-minded but optional for the strict MVP.)

- As a user, I may want to **sort or search tasks** by title, due date, or priority.

**Acceptance criteria (if implemented):**

- The user can:
  - Sort by at least one field (e.g., due date or created date), or
  - Perform a simple text search by title.
- The list updates to show only matching or sorted tasks.
- The default view is consistent (e.g., most recently created tasks first).

---

## 4. API-Level Functional Requirements

The backend exposes REST endpoints to support the above user actions. Details of routes and payloads are in `API-Spec.md`, but functionally:

- The API must support:
  - Create task (POST)
  - Get all tasks (GET)
  - Get task by id (GET)
  - Update task (PUT or PATCH)
  - Delete task (DELETE)
- The API must:
  - Validate requests
  - Return clear error messages for invalid input
  - Use appropriate HTTP status codes (e.g., 200, 201, 400, 404)

---

## 5. Validation Rules

The system must enforce at least the following rules:

- **Title**
  - Required.
  - Non-empty string (must contain at least one non-whitespace character).
  - Max length: a reasonable limit (e.g., 200 characters).

- **Description**
  - Optional.
  - If provided, has a maximum length (e.g., 2000 characters) to avoid overly large payloads.

- **Status**
  - Must be one of the allowed values (e.g., Todo, InProgress, Done).
  - If omitted on creation, default is Todo.

- **Due Date (if used)**
  - Optional.
  - If provided, must be a valid date.
  - MVP may not enforce “must be in the future”, but that rule can be documented as a future enhancement.

- **Priority (if used)**
  - Optional.
  - Must be one of the allowed values (e.g., Low, Medium, High).

When validation fails:

- The backend returns a 400 Bad Request with validation details.
- The frontend displays user-friendly messages near the relevant fields.

---

## 6. Error Handling & Feedback

From the user’s perspective, the system must:

- Display clear messages if:
  - A network error occurs.
  - The server is unavailable.
  - Validation fails.
- Avoid crashing the UI on unexpected errors.
- Prevent duplicate actions where possible:
  - For example, disabling submit buttons while a request is in progress.

From API perspective:

- Invalid resource id (not found) → 404 Not Found.
- Invalid input data → 400 Bad Request with error details.
- Unexpected server error → 500 Internal Server Error with a generic safe message.

---

## 7. UI Behavior & Interaction

Functionally, the UI must:

- Allow the user to:
  - Add new tasks quickly from the main screen.
  - See immediate feedback after create/update/delete.
- Keep the task list in sync with the backend:
  - After any successful operation, the displayed list reflects server state.
- Display loading indicators (or some feedback) when:
  - Fetching tasks on initial load.
  - Submitting forms.

---

## 8. Testing-Related Functional Expectations

The functional behavior above must be verifiable by automated tests:

- **Backend (NUnit)**
  - Verify core business rules:
    - Valid/invalid task creation.
    - Status transitions.
    - Updates and deletions.
- **Frontend (Vitest (Jest-compatible API) for unit tests)**
  - Verify UI behavior:
    - Forms show validation messages.
    - Lists update after operations.
- **End-to-End (Playwright)**
  - At least one full flow:
    - Create → View → Update (or change status) → Delete.
  - Optional: Filter by status.

---

## 9. Out-of-Scope Functionalities (For MVP)

The following are explicitly **not** part of this MVP’s functional requirements:

- User registration, login, logout, and password flows.
- User-specific task separation (all tasks belong to the same implicit user).
- Sharing tasks between users.
- Real-time updates (e.g., websockets).
- Exporting tasks (CSV, PDF, etc.).
- Reminders, notifications, or calendar integration.
- Complex task hierarchy (subtasks, projects, tags).
- Bulk operations (e.g., bulk complete, bulk delete).

These features can be considered and designed later, and they may be referenced in `Future-Scalability.md`.

---
