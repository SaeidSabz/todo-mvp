# Future & Scalability Notes — To-Do Task Management MVP

## 1. Purpose of This Document

This document outlines **how the current MVP can evolve** into a more production-ready system over time.

It does **not** describe required MVP functionality. Instead, it captures:
- Architectural decisions that were made with growth in mind
- Clearly identified extension points
- Trade-offs taken in the MVP
- Concrete next steps for scaling functionality, performance, and reliability

This helps reviewers understand that the MVP is intentionally minimal, not incomplete.

---

## 2. MVP Constraints (Current State)

The current MVP intentionally includes the following constraints:

- **Single-user context**
  - No authentication or user separation
- **In-memory persistence**
  - EF Core InMemory provider
  - Data is lost on restart
- **Single backend instance**
  - No horizontal scaling
- **Simple REST API**
  - No versioning
  - No pagination
- **Basic UI**
  - One main page
  - Client-side filtering only

All future enhancements below assume starting from this baseline.

---

## 3. Data Persistence & Storage Evolution

### 3.1 Replace InMemory with Real Database

**Next step:**
- Replace EF Core InMemory with a relational database:
  - SQL Server
  - PostgreSQL
  - SQLite (lightweight option)

**Why this is easy now:**
- Repository and DbContext abstractions already exist
- No business logic depends directly on InMemory behavior

**Additional improvements:**
- Add database migrations
- Seed initial data for development/testing
- Add indexes for frequently queried fields (Status, DueDate)

---

## 4. Multi-User Support & Authentication

### 4.1 Add User Entity

Future model additions:
- `User`
  - Id
  - Email / Username
  - CreatedAt

Changes to Task:
- Add `UserId` foreign key
- Enforce per-user task isolation

### 4.2 Authentication

Potential approaches:
- JWT-based authentication
- OAuth / OpenID Connect (Auth0, Azure AD, etc.)

Backend changes:
- Add authentication middleware
- Protect task endpoints
- Extract user identity from token

Frontend changes:
- Login / logout flow
- Auth-aware API client
- Conditional UI rendering

---

## 5. API Scalability & Robustness

### 5.1 API Versioning

Introduce explicit versioning:
- `/api/v1/tasks`
- `/api/v2/tasks`

Allows:
- Backward compatibility
- Safe evolution of contracts

### 5.2 Pagination, Sorting, and Advanced Filtering

Enhancements to `GET /api/tasks`:
- Pagination (`page`, `pageSize`)
- Server-side filtering by status
- Sorting by:
  - Created date
  - Due date
  - Priority

Improves:
- Performance
- Scalability for large task sets

---

## 6. Domain & Feature Expansion

### 6.1 Task Enhancements

Possible additions:
- Subtasks (parent-child relationship)
- Task completion timestamp (`CompletedAt`)
- Soft deletes (`IsDeleted`, `DeletedAt`)
- Recurring tasks
- Reminders

### 6.2 Organization Features

Future entities:
- Projects / Lists
- Tags (many-to-many)
- Task comments / activity log

These can be layered on top of the current domain model without breaking existing APIs.

---

## 7. Frontend Scalability

### 7.1 State Management

If complexity grows:
- Introduce a global store:
  - Redux Toolkit
  - Zustand
  - React Query / TanStack Query for server state

### 7.2 Routing & Views

Add:
- Multiple pages (Today, Completed, Overdue)
- Deep links to tasks
- URL-driven filters

### 7.3 UI Enhancements

Potential improvements:
- Kanban-style board
- Drag-and-drop status changes
- Optimistic UI updates
- Offline support (PWA)

---

## 8. Performance & Scaling

### 8.1 Backend Scaling

- Stateless API instances
- Horizontal scaling behind a load balancer
- Externalized cache (Redis) if needed

### 8.2 Frontend Performance

- Code splitting
- Lazy-loaded routes
- Memoized components
- Virtualized task lists for large datasets

---

## 9. Observability & Operations

### 9.1 Logging & Monitoring

Future additions:
- Structured logging (Serilog)
- Centralized log aggregation
- Metrics (request latency, error rates)

### 9.2 Health & Readiness

- Liveness and readiness probes
- Deeper health checks (DB connectivity)

---

## 10. Testing & Quality Expansion

### 10.1 End-to-End Testing

Add Playwright tests for:
- Critical user flows
- Authenticated scenarios
- Regression coverage

### 10.2 CI Enhancements

- Test coverage reporting
- Linting as part of CI
- Build artifacts
- Deployment pipelines (if deployed)

---

## 11. Deployment & Distribution

Future deployment options:
- Dockerized backend and frontend
- Cloud hosting (Azure, AWS, GCP)
- Static frontend hosting + API backend
- Environment-specific configurations

---

## 12. Summary

This MVP is intentionally small but **structurally ready** for growth.

Key takeaways:
- Core architecture supports extension without rewrites
- Clear boundaries between layers
- Trade-offs are explicit and documented
- Each future step can be taken incrementally

This document ensures that the MVP is viewed not as a throwaway prototype, but as a **solid foundation** for a real-world system.
