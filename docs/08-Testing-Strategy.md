# Testing Strategy — To-Do Task Management MVP

This document outlines the **testing approach** for the To-Do Task Management MVP, covering backend and frontend layers.  
The goal is to ensure reliability, maintainability, and confidence during iterative development, while keeping the approach lightweight and fast for an MVP.

---

## 1. Goals of the Testing Strategy

- Provide a **minimal but meaningful** test foundation.
- Enable **fast feedback** during development and CI.
- Ensure **core functionality is testable** and stable.
- Support **future scalability** toward integration and end-to-end testing.
- Use modern, well-supported tools:
  - NUnit (backend)
  - Vitest + React Testing Library (frontend)
  - Playwright (future E2E)

---

## 2. Overview of Test Types

### Unit Tests
- **Backend:** NUnit  
- **Frontend:** Vitest + React Testing Library  

Focus:
- Small, isolated units of logic
- Deterministic and fast-running tests
- No reliance on external services

---

### Integration Tests (Future)
Will verify:
- API endpoints end-to-end
- Database interactions (when moving from EF InMemory to a real DB)
- Cross-layer behavior (controller → service → persistence)

Not required for the MVP but supported by the architecture.

---

### End-to-End (E2E) Tests (Future)
Using **Playwright**, covering:
- Full user workflows
- Browser-level interactions
- Frontend ↔ backend integration

E2E tests will be added once the application evolves beyond MVP.

---

## 3. Backend Testing (NUnit)

### Current Scope (Implemented)

Backend tests focus on:

- Repository behavior (EF Core InMemory)
- Application/service logic
- API controller behavior
- Validation and error handling

Tests are written using **NUnit** with **Moq** where mocking is appropriate.

### Folder Structure

```
src/backend/TodoMvp.Api.Tests/
├─ Controllers/
├─ Services/
├─ Repositories/
└─ SmokeTests.cs
```

### Example Command

```bash
cd src/backend
dotnet test
```

### Why NUnit?

- Mature and well-supported in the .NET ecosystem
- Clean integration with .NET CLI and CI pipelines
- Works well with mocking libraries (Moq)
- Suitable for both unit and integration-style tests

---

## 4. Frontend Testing (Vitest + React Testing Library)

### Current Scope (Implemented)

Frontend tests cover:

- Loading and error states
- Rendering task lists
- Creating, editing, deleting tasks
- Filtering tasks by status
- Component-level interactions

Tests mock API calls to remain fast and deterministic.

### Tools

- **Vitest** — test runner (Vite-native, fast)
- **React Testing Library** — DOM-focused component testing
- **@testing-library/jest-dom** — extended matchers
- **happy-dom** — lightweight DOM environment

### Folder Structure

```
src/frontend/src/
├─ features/
│  └─ tasks/
│     ├─ TasksPage.test.tsx
│     ├─ TasksPage.crud.test.tsx
│     └─ TasksPage.filter.test.tsx
├─ setupTests.ts
```

### Example Command

```bash
cd src/frontend
npm test
```

### Why Vitest?

- First-class integration with Vite
- Excellent TypeScript support
- Jest-compatible API
- Much faster startup and execution than Jest

---

## 5. End-to-End Testing (Planned)

### Approach

End-to-end tests will be introduced using **Playwright** once the MVP stabilizes.

Planned scenarios:

- User creates a task and sees it in the list
- User updates a task
- User deletes a task
- User filters tasks by status

### Benefits

- High confidence in real user workflows
- Detects frontend ↔ backend integration issues
- Runs reliably in CI

---

## 6. CI Integration

Testing is integrated into **GitHub Actions CI**:

- Backend:
  - `dotnet build`
  - `dotnet test`
- Frontend:
  - `npm ci`
  - `npm run build`
  - `npm test`

Tests run automatically on pull requests and main branch updates.

---

## 7. Testing Philosophy

- **Test behavior, not implementation details**
- **Prefer clarity over cleverness**
- **Keep tests fast and deterministic**
- **Avoid over-mocking**
- **Grow coverage organically with features**
- **Fail fast in CI**

---

## 8. Summary

| Layer    | Tooling                          | Status |
|--------|----------------------------------|--------|
| Backend | NUnit + Moq                     | Implemented |
| Frontend | Vitest + React Testing Library | Implemented |
| E2E     | Playwright                      | Planned |
| CI      | GitHub Actions                  | Implemented |

This testing strategy provides a **solid, production-minded foundation** while keeping development velocity high for the MVP.
