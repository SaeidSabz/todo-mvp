# Frontend Design — To-Do Task Management MVP

## 1. Purpose of This Document

This document describes the **frontend design** of the To-Do Task Management MVP.

It covers:

- Overall structure of the React application
- Pages and components
- State management and data flow
- Communication with the backend API
- UX, responsiveness, and accessibility considerations
- Frontend testing strategy

This document is **conceptual**, but aligned with the current implementation.

---

## 2. High-Level Overview

- The frontend is a **single-page application (SPA)** built with **React + TypeScript**.
- It communicates with the backend via **HTTP/JSON**.
- The UI focuses on:
  - Viewing tasks
  - Creating tasks
  - Updating tasks
  - Deleting tasks
  - Filtering tasks by completion status

Design goals:

- Clear and readable structure
- Minimal but production-minded
- Easy to review and extend
- Strong automated test coverage

---

## 3. Main User Flows

The frontend supports the following flows:

1. **View tasks**
   - User opens the app and sees the task list.
   - Each task displays title, status, and optional metadata.

2. **Create a task**
   - User clicks “New Task”.
   - User enters title (required) and optional description / due date.
   - Task appears in the list after successful creation.

3. **Update a task**
   - User edits an existing task.
   - Changes are persisted and reflected in the list.

4. **Change task status**
   - User toggles task completion (Open ↔ Completed).
   - Status is updated visually and persisted.

5. **Delete a task**
   - User deletes a task from the list.
   - Confirmation is shown before deletion.
   - Task disappears after success.

6. **Filter tasks**
   - User selects a filter:
     - All
     - Open
     - Completed
   - Only visible tasks change; server data is unchanged.

---

## 4. Page and Layout Structure

### 4.1 Application Structure

The MVP uses a **single main page**.

- **App**
  - Root component
  - Renders the main Tasks page

- **TasksPage**
  - Fetches tasks from the API
  - Holds all page-level state:
    - Tasks
    - Loading / error states
    - Filter state
  - Coordinates all task-related components

No routing is required for the MVP.

---

## 5. Component Design

### 5.1 Core Components

- **TasksPage**
  - Container component
  - Owns task data and UI state
  - Calls API client functions
  - Applies filtering logic

- **TaskList**
  - Receives tasks as props
  - Renders a grid/list of `TaskCard` components
  - Shows empty state when no tasks are visible

- **TaskCard**
  - Displays a single task:
    - Title
    - Completion badge (Open / Completed)
    - Optional description and due date
  - Provides actions:
    - Edit
    - Delete
  - Uses CSS Modules for styling

- **TaskForm**
  - Used for create and edit
  - Manages local form state
  - Performs basic client-side validation
  - Calls callbacks on submit

- **StatusFilter**
  - Dropdown selector:
    - All
    - Open
    - Completed
  - Updates filter state in `TasksPage`

### 5.2 Supporting Components

- **Loading state**
  - Displayed while tasks are loading

- **Error message**
  - Displayed when API calls fail

- **Empty state**
  - Displayed when there are no tasks

---

## 6. State Management and Data Flow

### 6.1 State Ownership

- **TasksPage** owns:
  - `tasks: Task[]`
  - `isLoading: boolean`
  - `error: string | null`
  - `filter: "all" | "open" | "completed"`
  - `editingTask: Task | null`

- **TaskForm** owns:
  - Local form field state
  - Validation messages

No global store is used.

---

### 6.2 Data Flow

- **Initial load**
  - TasksPage calls `fetchTasks()`
  - Loading state shown
  - Tasks stored in state

- **Create**
  - TaskForm submits
  - API called
  - Tasks reloaded or updated locally

- **Update**
  - TaskForm submits changes
  - API called
  - Task updated in state

- **Delete**
  - User confirms deletion
  - API called
  - Task removed from state

- **Filter**
  - Filter state updated
  - Derived list computed:
    - No server call required

---

## 7. API Integration Layer

### 7.1 Responsibilities

The API client layer:

- Centralizes HTTP calls
- Handles:
  - Base URL configuration via environment variables
  - JSON parsing
  - Error normalization

Exposed functions include:

- `getTasks()`
- `createTask(payload)`
- `updateTask(id, payload)`
- `deleteTask(id)`

---

### 7.2 Error Handling

- API errors are surfaced as user-friendly messages
- UI displays:
  - Inline errors for forms
  - Global error message for fetch failures

---

## 8. Styling and UX

### 8.1 Styling Approach

- **CSS Modules**
  - Each component owns its styles
  - Prevents global CSS leakage
  - Encourages encapsulation

### 8.2 UX Principles

- Clear call-to-action buttons
- Immediate visual feedback
- Loading and disabled states during async operations
- Minimal visual noise

---

## 9. Responsiveness and Accessibility

- Responsive layout using flexible grids
- Semantic HTML elements:
  - `button`, `form`, `label`, `input`
- Keyboard-accessible interactions
- Accessible labels and ARIA attributes where needed

Accessibility is best-effort but avoids obvious issues.

---

## 10. Frontend Testing Strategy

### 10.1 Unit & Component Tests

Tools:
- **Vitest**
- **React Testing Library**
- **happy-dom**

Coverage includes:
- Loading state
- Rendering task data
- Create / update / delete flows
- Filter behavior

API calls are mocked in tests.

---

### 10.2 End-to-End Tests (Future)

Planned with Playwright:
- Create task
- Update task
- Delete task
- Filter tasks

Not required for MVP completion.

---

## 11. Extensibility

The frontend is structured to support:

- Additional filters and sorting
- Routing and multiple pages
- Authentication and user context
- Global state management if needed
- More complex task views (e.g., Kanban)

The current design intentionally keeps complexity low while remaining extensible.

---
