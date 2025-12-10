# Architecture Overview — To-Do Task Management MVP

## 1. Purpose of This Document

This document describes the **high-level architecture** of the To-Do Task Management MVP:

- How the system is structured (frontend + backend)
- How components communicate
- Key patterns and design decisions
- How this MVP can grow into a more production-ready system

Implementation-level details (e.g., exact class names, components, or code) are intentionally omitted or kept abstract.

---

## 2. High-Level System Architecture

The system consists of two main parts:

- A **.NET Core backend API** exposing **RESTful JSON endpoints**
- A **React frontend** that consumes those endpoints and renders the UI

At a high level:

```text
+-------------------------+          +---------------------------+
|       React App         |  HTTP    |        .NET Core API      |
|  (frontend, TypeScript) | <------> | (backend, EF Core InMem)  |
+-------------------------+          +---------------------------+
                                              |
                                              v
                                        In-Memory Data
                                     (EF Core DbContext)
```

- All user interactions happen in the **browser** (React).
- The frontend communicates with the backend via **HTTP/JSON**.
- The backend manages the **Task** domain model using **EF Core InMemory** for this MVP.

---

## 3. Solution Layout (High-Level)

Planned root structure:
```text
todo-mvp/src
├─ backend/      # .NET Core Web API
└─ frontend/     # React application
```

- Each side (backend/frontend) is self-contained with its own tooling and tests.
- Both are tied together by shared **API contracts** (documented in ```API-Spec.md```).

---

## 4. Backend Architecture (.NET Core API)  

### 4.1. Architectural Layers

The backend is organized in conventional, layered form:
```text
[ API Layer ]        → Controllers / Endpoints
[ Application Layer ]→ Services / Use Cases / Business Logic
[ Data Access Layer ]→ EF Core DbContext, Repositories (if used)
[ Domain Model ]      → Entities (Task, etc.)
```

#### API Layer (Controllers)
- Expose RESTful endpoints (e.g., ```/api/tasks```).
- Translate HTTP requests to **application-level commands** or **queries**.
- Handle HTTP concerns:
    - Status codes
    - Routing
    - Model binding
    - Basic validation (e.g., required fields)

#### Application Layer (Services)
- Contains **use case logic** such as:
    - Create task
    - Update task
    - Change task status
    - Delete task
- Applies validation and business rules (e.g., non-empty title).
- Coordinates between domain objects and data layer.
- Isolates domain logic from transport (HTTP) and persistence details.

#### Data Access Layer
- Uses **EF Core InMemory** to persist entities during runtime.
- Provides an abstraction over data storage (e.g., via a repository or direct DbContext usage).
- Supports swapping the **InMemory provider** with a real database (e.g., SQL Server) in the future, without changing the application layer.

#### Domain Model
- Defines core entities and their invariants.
- For MVP, the central entity is ```Task``` (name may differ at implementation time), with fields like:
    - ```Id```
    - ```Title```
    - ```Description```
    - ```Status```
    - ```CreatedAt```
    - ```UpdatedAt```
    - Optional: ```DueDate```, ```Priority```

### 4.2. Technology & Framework Choices
- **.NET Core Web API** (version to be decided, e.g., .NET 8).
- **EF Core** with **InMemory** provider for convenience and simplicity.
- Validation via:
    - Data annotations and/or
    - Application-level validation logic inside services.
- Automated tests:
    - **NUnit** for unit tests and possibly lightweight integration tests (e.g., in-memory test server).

---

## 5. Frontend Architecture (React)  

### 5.1. Overall Structure

The frontend is a **React** single-page application, likely with **TypeScript**, structured into:
```text
[ Pages / Views ]      → High-level screens (Task list, etc.)
[ UI Components ]      → Reusable building blocks (Task item, form, filters)
[ State / Hooks ]      → Local + shared state, data fetching
[ API Client Layer ]   → Functions or hooks that call the backend API
```

#### Pages / Views
- Responsible for:
    - Fetching and presenting data to the user.
    - Composing components (list, form, filters).
- Example page:
    - ```TasksPage``` — shows list of tasks and allows creating/updating them.

#### UI Components
- Stateless or lightly stateful components:
    - ```TaskList```
    - ```TaskItem```
    - ```TaskForm```
    - ```StatusFilter```
- Focused on rendering and user interaction, not on API details.

#### State Management
- Use **React hooks** for:

- Local state (```useState```, ```useReducer```)
    - Side effects (```useEffect```)
    - For MVP, **Context API or simple lifting state up** is sufficient.
- No heavy global state library (like Redux) initially, but structure should not block adding one later.

#### API Client Layer
- Small module or set of hooks encapsulating HTTP calls:
    - ```getTasks()```
    - ```createTask(task)```
    - ```updateTask(task)```
    - ```deleteTask(id)```
- Returns data in a consistent shape to the views.
- Centralizes error handling (e.g., mapping HTTP errors into UI-consumable states).

### 5.2. Technology & Framework Choices
- **React** (version to be decided, e.g., 18+).
- **TypeScript** (recommended, but optional if out-of-scope).
- **Vitest (Jest-compatible API)** for unit tests (and React Testing Library for component tests).
- **Playwright** for end-to-end browser tests.

---

## 6. Communication Between Frontend and Backend  

### 6.1. Protocol & Format
- **Protocol**: HTTP/HTTPS
- **Data format**: JSON
- **API style**: RESTful, resource-based (e.g., ```/api/tasks```)

Requests/Responses:
- Frontend sends JSON payloads to relevant endpoints.
- Backend returns:
    - Successful responses with appropriate HTTP status codes and JSON bodies.
    - Error responses with a **standard error shape** (e.g., ```{ errorCode, message, details }```).

### 6.2. CORS & Security
- Backend will be configured with **CORS** to allow calls from the frontend origin (during development).
- No complex authentication in MVP:
    - Requests are anonymous or treated as a single user context.
- Security considerations:
    - Avoid exposing stack traces or sensitive information in error responses.
    - Validate input to prevent obvious malformed data.

---

## 7. Request Flow (End-to-End)  

Example: “Create Task” flow:

1. User opens frontend and fills task form.
2. Frontend validates basic constraints (e.g., title not empty).
3. Frontend sends POST ```/api/tasks``` with task data in JSON.
4. Backend controller:
    - Binds JSON to a DTO or command model.
    - Calls application service (e.g., ```TaskService.CreateTask()```).
5. Application service:
    - Validates business rules.
    - Creates a domain entity.
    - Persists it via EF Core (InMemory DbContext).
6. Backend returns ```201 Created``` with the created task resource.
7. Frontend updates its state and re-renders the task list.  

This pattern is similar for **update, delete, and fetch** operations.

---

## 8. Cross-Cutting Concerns  

### 8.1. Logging
- Backend:
    - Use built-in .NET logging abstractions.
    - Log key events and errors (at least at MVP level).
- Frontend:
    - Minimal logging (console + error boundaries for critical failures if used).

### 8.2. Error Handling
- Backend:
    - Centralized exception handling middleware or filters.
    - Consistent error response format.
- Frontend:
    - Handle different error types (e.g., network error vs. validation error).
    - Display user-friendly error messages (e.g., toast or inline error).

### 8.3. Validation
- Frontend:
    - Basic client-side validation for UX.
- Backend:
    - Authoritative validation of input.
    - Avoids trusting client-side checks.

---

## 9. Environments & Configuration  
- **Local Development**
    - Backend and frontend run on different ports (e.g., ```https://localhost:5001``` and ```http://localhost:3000```).
    - CORS configured to allow local frontend.
    - EF Core InMemory used by default.
- **Future Environments (Planned)**
- Staging / production:
    - Swap InMemory provider for real database (e.g., SQL Server, PostgreSQL).
    - Configure environment-specific settings via appsettings/environment variables.

Configurable aspects:
- Backend:
    - Port, connection strings (future), logging levels.
- Frontend:
    - API base URL (via environment variables).

---

## 10. Testing Strategy (Architecture View)  
Tests are distributed across layers but share the same architectural goal: **confidence with minimal coupling**.

- **Backend (NUnit)**:
    - Unit tests for:
        - Application services
        - Domain logic (e.g., status transitions)
    - Optional integration tests for:
        - Controllers + EF Core InMemory
- **Frontend (Vitest (Jest-compatible API) for unit tests)**:
    - Unit/component tests for:
        - Components (rendering tasks, handling user input)
        - Hooks (data fetching, state logic)
- **End-to-End (Playwright)**:
    - Full-stack scenarios:
        - “User creates a task and sees it in the list”
        - “User updates task status”
        - “User deletes a task”
    - Validates that frontend and backend integrate correctly.

---

## 11. Evolution Path  
The architecture is intentionally kept **simple but extensible**. Future changes can include:
- Replacing InMemory DB with a real relational DB (minimal changes to upper layers).
- Adding an **authentication layer**:
    - JWT-based auth, external identity provider, etc.
- Introducing **multi-user support**:
    - Associate tasks with a user or workspace.
- Scaling out:
    - API behind a load balancer
    - Separate read/write concerns if needed
- Enhancing the frontend:
    - Global state management (Redux / Zustand, etc.) if complexity grows.
    - More sophisticated routing and layout.

These evolutions are enabled by the separation of concerns and layered design described above.

---  
