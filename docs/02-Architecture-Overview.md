# Architecture Overview — To-Do Task Management MVP

## 1. Purpose of This Document

This document describes the **high-level architecture** of the To-Do Task Management MVP:

- How the system is structured (frontend + backend)
- How components communicate
- Key patterns and design decisions
- How this MVP can grow into a more production-ready system

Implementation-level details are intentionally kept high-level, but this document is aligned with the current codebase structure.

---

## 2. High-Level System Architecture

The system consists of two main parts:

- A **.NET 10 backend API** exposing **RESTful JSON endpoints**
- A **React 19 frontend** (TypeScript) that consumes those endpoints and renders the UI

At a high level:

```text
+-------------------------+          +------------------------------+
|       React App         |  HTTP    |          .NET 10 API         |
|  (frontend, TypeScript) | <------> | (REST, EF Core InMemory)     |
+-------------------------+          +------------------------------+
                                              |
                                              v
                                        In-Memory Data
                                     (EF Core DbContext)

```

- All user interactions happen in the **browser** (React).
- The frontend communicates with the backend via **HTTP/JSON**.
- The backend manages the **Task** domain model using **EF Core InMemory** for this MVP.

---

## 3. Solution Layout

Current repository structure (simplified):
```text
todo-mvp/src
├─ backend/
│  └─ TodoMvp/
│     ├─ TodoMvp.slnx
│     ├─ TodoMvp.Api/
│     ├─ TodoMvp.Api.Tests/
│     ├─ TodoMvp.Application/
│     ├─ TodoMvp.Domain/
│     └─ TodoMvp.Persistence/
└─ frontend/

```

- Backend is a multi-project solution (```TodoMvp.slnx```) with clear separation of layers.
- Frontend is a standalone React application with its own tooling and tests.
- The two sides are tied together by a stable **API contract** (documented in ```05-API-Spec.md```).

---

## 4. Backend Architecture (.NET Core API)  

### 4.1. Architectural Layers
The backend follows a layered structure consistent with Clean Architecture principles:
```text
[ API Layer ]          → Controllers, HTTP concerns, DTOs
[ Application Layer ]  → Use cases/services, validation, orchestration
[ Domain Layer ]       → Entities, invariants, domain rules
[ Persistence Layer ]  → EF Core DbContext, repositories, data access

```

#### API Layer (TodoMvp.Api)
- Expose RESTful endpoints (e.g., ```/api/tasks```).
- Handle HTTP concerns:
    - Status codes
    - Routing
    - Model binding
    - Request validation (model validation + application validation)
- Delegates business logic to Application layer.

#### Application Layer (TodoMvp.Application)
- Contains **use case logic** such as:
    - Create task
    - Update task
    - Set status (Open/Completed)
    - Delete task
    - Query tasks
- Owns input validation and business rules (e.g., required title, max length).
- Depends on abstractions (e.g., repository interfaces), not EF Core directly.

#### Domain Layer (TodoMvp.Domain)
- Defines core entities and their invariants.
- Central entity for MVP is ```TaskItem``` (or equivalent).
- For MVP, the central entity is ```Task``` (name may differ at implementation time), with fields like:
    - ```Id```
    - ```Title```
    - ```Description``` (optional)
    - ```IsCompleted``` (Open vs Completed)
    - ```CreatedAt```
    - ```UpdatedAt``` (optional)
    - ```DueDate``` (optional)

#### Persistence Layer (TodoMvp.Persistence)
- Implements data access using **EF Core InMemory** for MVP.
- Contains:
    - ```DbContext``` (InMemory provider)
    - Repository implementations (e.g., ```ITaskRepository``` → ```TaskRepository```)
- Designed so storage can be swapped later (SQL Server/PostgreSQL) with minimal impact to Application/API layers.


### 4.2. Technology & Framework Choices
- **.NET 10 Web API**
- **EF Core** with **InMemory** provider for convenience and simplicity.
- Validation via:
    - Data annotations and/or
    - Application-layer validation for business rules
- Error handling:
    - Centralized exception handling (e.g., ```IExceptionHandler```) returning a consistent error response    
- Automated tests:
    - **NUnit** for unit tests
    - API-level tests in ```TodoMvp.Api.Tests```

---

## 5. Frontend Architecture (React 19 + TypeScript)

### 5.1. Overall Structure

The frontend is a React single-page application structured into:
```text
[ Pages / Features ]    → Task page (list + CRUD + filter)
[ UI Components ]       → TaskCard, TaskForm, Filter dropdown
[ API Client Layer ]    → Functions calling backend endpoints
[ Styles ]              → CSS Modules (component-scoped styles)

```

#### Pages / Features
- Example: ```TasksPage``` responsible for:
    - Loading tasks
    - Handling loading/error states
    - Creating/editing/deleting tasks
    - Applying status filter (All/Open/Completed)

#### UI Components
- SReusable components focused on rendering and interactions:
    - ```TaskCard```
    - ```TaskForm```
    - ```StatusFilter```
- Focused on rendering and user interaction, not on API details.

#### API Client Layer
- Small module or set of hooks encapsulating HTTP calls:
    - ```getTasks()```
    - ```createTask(task)```
    - ```updateTask(task)```
    - ```deleteTask(id)```
- Encapsulates:
    - Base URL
    - Request/response handling
    - Error mapping to UI-friendly results

### 5.2. Technology & Framework Choices
- **React 19**
- **TypeScript**
- **Vitest** for dev/build tooling
- **Vitest + React Testing Library** for unit/component tests

---

## 6. Communication Between Frontend and Backend  

### 6.1. Protocol & Format
- **Protocol**: HTTP/HTTPS
- **Data format**: JSON
- **API style**: RESTful, resource-based (e.g., ```/api/tasks```)

Backend returns:
    - Successful responses with appropriate HTTP status codes and JSON bodies.
    - Error responses with a **standard error shape** (e.g., ```{ error, message }```).

### 6.2. CORS & Security
- Backend configured with CORS for local development.
- No authentication in MVP:
    - Requests are anonymous or treated as a single user context.
- Security considerations:
    - Do not expose stack traces in responses
    - Validate inputs on the server

---

## 7. Request Flow (End-to-End)  

Example: “Create Task” flow:

1. User fills the “New Task” form in the frontend.
2. Frontend performs basic validation for UX.
3. Frontend sends POST ```/api/tasks``` with JSON payload.
4. Backend controller binds request DTO and validates model state.
5. Application layer validates business rules and creates the entity.
6. Persistence layer stores it in EF Core InMemory.
7. Backend returns 201 Created with the created task. 
8. Frontend reloads or updates local state and re-renders the task list.

---

## 8. Cross-Cutting Concerns  

### 8.1. Logging
- Backend:
    - Built-in .NET logging abstractions (```ILogger<T>```)
    - Centralized exception logging
- Frontend:
    - Minimal logging and user-friendly error messages

### 8.2. Error Handling
- Backend:
    - Centralized exception handling
    - Consistent error response contract
- Frontend:
    - Distinguish network vs API errors
    - Display user-friendly messages

### 8.3. Validation
- Frontend:
    - UX-focused validation
- Backend:
    - Authoritative validation

---

## 9. Environments & Configuration  
- **Local Development**
    - Backend and frontend run on different ports.
    - Frontend uses VITE_API_BASE_URL (e.g., ```https://localhost:7157```).
    - EF Core InMemory used by default.
- **Future Environments**
    - Swap InMemory provider for real database (e.g., SQL Server, PostgreSQL).
    - Configure environment-specific settings via appsettings/environment variables.
---

## 10. Testing Strategy (Architecture View)  
Tests are distributed across layers but share the same architectural goal: **confidence with minimal coupling**.

- **Backend (NUnit)**:
    - Application services
    - Repository behavior
    - Controller/API tests
- **Frontend (Vitest)**:
    - Loading states
    - Rendering tasks
    - CRUD interactions
    - Filtering behavior
---

## 11. Evolution Path  
- Replace InMemory storage with a real database
- Add authentication and multi-user support
- Introduce pagination and server-side filtering
- Enhance frontend state management if complexity grows

---  
