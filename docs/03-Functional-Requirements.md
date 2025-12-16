# Functional Requirements — To-Do Task Management MVP

## 1. Purpose and Scope

This document defines the **functional behavior** of the To-Do Task Management MVP:

- What the system must do from a user’s perspective
- How tasks are created, viewed, updated, filtered, and deleted
- What validations and rules apply
- What the user can expect from the UI and API

Technical implementation details are covered in `02-Architecture-Overview.md` and related documents.

---

## 2. Actors and Roles

For this MVP, the system assumes a **single, anonymous user context**:

- **User**
  - Interacts with the system through the React frontend.
  - Can manage tasks (create, read, update, delete, filter).
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
  - Optional due date
- When the user submits:
  - If the input is valid, the task is created and visible in the list.
  - If the input is invalid, the user is shown clear validation messages.
- The backend returns appropriate success or error responses via the API.

---

### 3.2 View Task List

- As a user, I want to **see a list of my tasks** so that I can understand what needs to be done.

**Acceptance criteria:**

- The main screen shows a list of tasks with at least:
  - Task title
  - Status (Open or Completed)
  - Optional due date
- On initial load, the system retrieves all tasks from the backend.
- If there are no tasks:
  - An empty-state message is displayed (e.g., “No tasks yet”).
- The list updates after:
  - Creating a task
  - Updating a task
  - Deleting a task
  - Changing task status

---

### 3.3 View Task Details

- As a user, I want to **view the details of a task** so that I can see all relevant information.

**Acceptance criteria:**

- The user can see task details either inline or via a modal or dedicated section.
- Displayed information includes:
  - Title
  - Description (if present)
  - Status
  - Optional due date
- Created/updated timestamps may be shown but are not mandatory for the MVP UI.

---

### 3.4 Update Task

- As a user, I want to **edit an existing task** so that I can keep it up to date.

**Acceptance criteria:**

- The user can open an “Edit Task” interaction for a task.
- The user can modify:
  - Title
  - Description
  - Due date
- When the user submits:
  - Valid changes are saved and reflected in the task list.
  - Invalid input results in validation messages.
- The existing task record is updated (not duplicated).

---

### 3.5 Change Task Status

- As a user, I want to **change a task’s status** so that I can track completion.

**Acceptance criteria:**

- Each task has one of the following statuses:
  - Open
  - Completed
- The user can change status via:
  - Editing the task, or
  - A quick action in the task list.
- The UI immediately reflects the new status.
- The backend persists the new status and returns it in subsequent reads.

---

### 3.6 Delete Task

- As a user, I want to **delete a task** so that I can remove tasks I no longer need.

**Acceptance criteria:**

- The user can trigger delete from the task list.
- The system may ask for confirmation (recommended).
- After deletion:
  - The task no longer appears in the list.
  - Fetching the task by id returns “not found” from the API.

---

### 3.7 Filter Tasks by Status

- As a user, I want to **filter tasks by status** so that I can focus on relevant items.

**Acceptance criteria:**

- The user can filter tasks to show:
  - All tasks
  - Only Open tasks
  - Only Completed tasks
- Filtering is available via a visible UI control (e.g., dropdown).
- The active filter state is clear to the user.

---

## 4. API-Level Functional Requirements

The backend exposes REST endpoints to support the above user actions. Functionally:

- The API must support:
  - Create task (POST)
  - Get all tasks (GET)
  - Get task by id (GET)
  - Update task (PUT or PATCH)
  - Delete task (DELETE)
- The API must:
  - Validate incoming requests
  - Return clear error messages for invalid input
  - Use appropriate HTTP status codes (e.g., 200, 201, 400, 404)

Detailed routes and payloads are defined in `05-API-Spec.md`.

---

## 5. Validation Rules

The system must enforce at least the following rules:

- **Title**
  - Required.
  - Non-empty string.
  - Maximum length (e.g., 200 characters).

- **Description**
  - Optional.
  - Maximum length enforced (e.g., 2000 characters).

- **Status**
  - Must be one of the allowed values: Open or Completed.
  - Defaults to Open on creation.

- **Due Date**
  - Optional.
  - Must be a valid date if provided.

When validation fails:

- The backend returns `400 Bad Request` with validation details.
- The frontend displays user-friendly messages near relevant fields.

---

## 6. Error Handling & Feedback

From the user’s perspective, the system must:

- Display clear messages when:
  - Network errors occur
  - The server is unavailable
  - Validation fails
- Avoid crashing the UI on unexpected errors.
- Prevent duplicate actions where possible (e.g., disable submit buttons during requests).

From the API perspective:

- Invalid resource id → `404 Not Found`
- Invalid input → `400 Bad Request`
- Unexpected server error → `500 Internal Server Error` with a safe, generic message

---

## 7. UI Behavior & Interaction

Functionally, the UI must:

- Allow quick task creation from the main screen.
- Show immediate feedback after create, update, or delete actions.
- Keep the task list in sync with backend state after successful operations.
- Display loading indicators when:
  - Fetching tasks on initial load
  - Submitting create or update requests

---

## 8. Testing-Related Functional Expectations

The functional behavior above must be verifiable by automated tests:

- **Backend (NUnit)**
  - Valid and invalid task creation
  - Status changes
  - Updates and deletions
- **Frontend (Vitest + React Testing Library)**
  - Loading and error states
  - Rendering task lists
  - CRUD interactions
  - Filtering behavior

---

## 9. Out-of-Scope Functionalities (For MVP)

The following are explicitly **out of scope** for this MVP:

- User authentication and authorization
- User-specific task separation
- Sharing tasks between users
- Real-time updates
- Notifications or reminders
- Task hierarchy (subtasks, projects, tags)
- Bulk operations

These may be considered in future iterations and are referenced in `09-Future-Scalability.md`.
