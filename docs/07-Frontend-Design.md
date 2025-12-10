# Frontend Design — To-Do Task Management MVP

## 1. Purpose of This Document

This document describes the **frontend design** of the To-Do Task Management MVP.

It covers:

- Overall structure of the React application
- Pages and components
- State management and data flow
- Communication with the backend API
- UX, responsiveness, and accessibility considerations
- Testing approach for the frontend

This is a conceptual design, not an implementation guide.

---

## 2. High-Level Overview

- The frontend is a **single-page application (SPA)** built with **React**.
- It interacts with the backend via **HTTP/JSON**.
- It focuses on a **simple, fast, and clean UI** for:
  - Viewing tasks
  - Creating tasks
  - Updating tasks (including status)
  - Deleting tasks
  - Filtering by status

The frontend is structured to be:

- Easy to understand for reviewers
- Easy to extend (e.g., adding more views or features later)
- Easy to test (component tests and end-to-end tests)

---

## 3. Main User Flows

The frontend must support these primary flows:

1. **View tasks**
   - User lands on a main page showing the list of tasks.
   - User can see status and basic details at a glance.

2. **Create a task**
   - User opens a form to add a new task.
   - User fills title (required) and optional fields (description, status, due date, priority).
   - On submit, the task is created and appears in the list.

3. **Update a task**
   - User selects a task to edit.
   - User can modify fields and save.
   - Changes are reflected in the list view.

4. **Change task status**
   - User can change status (e.g., Todo → In Progress → Done) via edit form or quick action.
   - Status change is visual and persisted via the API.

5. **Delete a task**
   - User can delete a task from the list or detail view.
   - The list updates and the task disappears.

6. **Filter by status**
   - User can filter tasks by status (e.g., All, Todo, In Progress, Done).
   - Filter affects only the view; actual data remains unchanged on the server.

---

## 4. Page and Layout Structure

For the MVP, a **single main page** is sufficient, possibly with simple routing.

- **App Shell / Layout**
  - Header area
    - Application title (e.g., “Task Manager”)
    - Optional: simple navigation or future menu placeholder.
  - Main content area
    - Task list
    - Filters
    - Create/edit form (inline, modal, or a dedicated section)

- **Primary Page**
  - `TasksPage` (or equivalent):
    - Fetches tasks from the backend.
    - Holds the main state for the task list.
    - Coordinates components:
      - Filter controls
      - Task list view
      - Task create/edit form
      - Loading and error states

Routing (e.g., React Router) is optional for MVP. If used, `/` can map to `TasksPage`.

---

## 5. Component Design

### 5.1 Core Components

The main components can be:

- `App`
  - Root component.
  - Sets up global context providers (if any).
  - Renders layout and main page.

- `TasksPage`
  - Fetches tasks from the API.
  - Stores tasks, loading, error, and filter state.
  - Passes data and callbacks to child components.

- `TaskList`
  - Receives an array of tasks to display.
  - Responsible for rendering each `TaskItem`.
  - Shows an “empty state” message if no tasks.

- `TaskItem`
  - Renders a single task row/card:
    - Title, status, optional due date/priority.
  - Provides actions:
    - Edit task
    - Change status (optional quick action)
    - Delete task

- `TaskForm`
  - Used for creating or editing tasks.
  - Contains inputs for:
    - Title (required)
    - Description
    - Status
    - Due date
    - Priority
  - Handles local form state and validation.
  - Calls a callback on submit (e.g., `onSubmit(taskData)`).

- `StatusFilter`
  - Allows user to filter tasks by status (e.g., All, Todo, In Progress, Done).
  - Could be implemented as:
    - Buttons
    - Tabs
    - Dropdown

- `SearchBar` (optional)
  - Allows simple text search on title (and optionally description).

### 5.2 Support Components

- `LoadingIndicator`
  - Displays when data is being loaded from the API.

- `ErrorBanner` / `ErrorMessage`
  - Shows user-friendly error messages when:
    - API requests fail
    - Validation fails in a way that should be visible globally

- `EmptyState`
  - Used when there are no tasks to show.
  - Encourages user to create their first task.

- `Modal` (optional)
  - For edit/create dialogs if the design uses modals instead of inline forms.

---

## 6. State Management and Data Flow

### 6.1 Where State Lives

For the MVP:

- **Task list state** (array of tasks):
  - Stored near the top of the page, e.g., in `TasksPage`.
  - Passed down via props to `TaskList`, `TaskForm`, etc.

- **UI state**:
  - `isLoading` (boolean) for API calls.
  - `error` (string or object) for failures.
  - `selectedTask` for edit mode (nullable).
  - `filterStatus` to track current filter (e.g., "All", "Todo").

- Local form state:
  - Inside `TaskForm`, using local state hooks.

### 6.2 Data Flow

A typical data flow:

- On initial load:
  - `TasksPage` calls the API to fetch tasks.
  - Sets `tasks` state from the response.
  - Handles loading and error states.

- Create task:
  - User opens `TaskForm` in “create” mode.
  - On submit, `TasksPage` calls the API (POST).
  - On success, `TasksPage` updates its `tasks` state:
    - Either by refetching all tasks, or
    - By appending the new task to the existing list.

- Edit task:
  - User selects a task (e.g., clicks an edit button in `TaskItem`).
  - `TasksPage` sets `selectedTask`.
  - `TaskForm` shows existing values.
  - On submit, `TasksPage` calls API (PUT or PATCH).
  - On success, `TasksPage` updates the corresponding task in `tasks`.

- Delete task:
  - User triggers delete from `TaskItem`.
  - `TasksPage` calls API (DELETE).
  - On success, `TasksPage` removes the task from `tasks`.

- Filter tasks:
  - User changes `filterStatus`.
  - Derived state: `visibleTasks` = `tasks` filtered by `filterStatus`.
  - Only the view changes; underlying tasks in state remain unchanged.

### 6.3 Context or Global Store

For the MVP:

- **React Context** can be used if:
  - There is a desire to avoid prop drilling through multiple layers.
- A dedicated global store (Redux, Zustand, etc.) is **not required**.
- The design should not prevent introducing a global store later, but it is intentionally kept simple initially.

---

## 7. API Integration Layer

The frontend should use a small **API client layer** to encapsulate HTTP calls.

### 7.1 API Client Responsibilities

- Expose functions such as:
  - `fetchTasks(filters)`
  - `createTask(taskPayload)`
  - `updateTask(id, taskPayload)`
  - `deleteTask(id)`
  - Optional: `updateTaskStatus(id, status)`

- Handle low-level concerns:
  - Base URL configuration
  - HTTP methods and headers
  - Parsing JSON responses
  - Handling non-2xx responses by throwing or returning standardized error objects

### 7.2 Error Handling Strategy

- API client should:
  - Normalize error shapes where possible.
  - Surface enough information for UI to display a friendly message.

- UI should:
  - Display contextual error messages (e.g., near form fields for validation).
  - Show a global or banner error for larger failures (e.g., server down).

---

## 8. UX and Interaction Design

### 8.1 Core UX Principles

- **Simple and clean**
  - Minimal visual clutter.
  - Focus on tasks and core actions.

- **Inline feedback**
  - Validation errors shown right under or next to the fields.
  - Visual cues on success (e.g., reset form, new task appears).

- **Clear call-to-action**
  - Prominent “Add Task” button or section.

### 8.2 Loading and Empty States

- On initial load:
  - Show a loading indicator while tasks are fetched.
- If no tasks:
  - Show a friendly message and an invitation to create a first task.

### 8.3 Responsiveness

- **Desktop**
  - Use a layout that shows:
    - Task list
    - Clear “Add task” entry point
  - Optional: two-pane layout (list + details) if implemented.

- **Mobile**
  - Stack content vertically:
    - Header
    - Filter controls
    - Task list
    - Add button / form
  - Avoid horizontal scroll; use columns and lists that reflow naturally.

---

## 9. Accessibility Considerations

- Use semantic HTML elements where possible:
  - `<button>` for actions.
  - `<form>`, `<label>`, `<input>`, `<textarea>` for forms.
  - Headings for structure.

- Ensure:
  - Each input has an associated label.
  - Focus order is logical.
  - Keyboard navigation can reach all important actions.

- Use appropriate ARIA attributes only when needed, not as a replacement for semantic HTML.

Accessibility is “best effort” at MVP level but should avoid obviously inaccessible patterns.

---

## 10. Frontend Testing Strategy

### 10.1 Unit and Component Tests (Jest + React Testing Library)

- Test key components:
  - `TaskList`:
    - Renders tasks correctly.
    - Shows empty state when there are none.
  - `TaskItem`:
    - Displays correct title, status, and optional fields.
    - Calls callbacks when actions (edit/delete/status change) are clicked.
  - `TaskForm`:
    - Shows validation errors when required fields are empty.
    - Calls `onSubmit` with correct data on valid input.

- Test view/container logic in `TasksPage` where feasible, using mocking for the API client.

### 10.2 End-to-End Tests (Playwright)

- Validate critical user flows end-to-end:
  - Load application and see an empty state.
  - Create a new task and see it in the list.
  - Update a task’s fields and status.
  - Delete a task and confirm it disappears.
  - Apply a status filter and verify visible tasks match the filter.

These tests confirm that frontend and backend integrate correctly.

---

## 11. Extensibility and Future Enhancements

The frontend design is structured to accommodate future features:

- **Additional views**
  - e.g., “Today” view, “Overdue” view, or “Completed tasks” view.
- **More complex filtering and sorting**
  - Multi-criteria filters
  - Server-side pagination and sorting
- **User accounts and multi-user support**
  - Login/logout views
  - Showing tasks per user
- **Board/Kanban-style layout**
  - Multiple columns for each status.
  - Drag-and-drop between columns.

By keeping components focused and state management simple but well-organized, these enhancements can be added without a major redesign.

---
