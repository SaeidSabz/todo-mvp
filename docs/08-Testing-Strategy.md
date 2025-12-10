# Testing Strategy

This document outlines the testing approach for the To-Do MVP application, covering both backend and frontend layers. The goal is to ensure reliability, maintainability, and confidence during iterative development while keeping the initial implementation lightweight for an MVP.

---

## 1. Goals of the Testing Strategy

- Provide a **minimal but meaningful** test foundation.
- Enable **fast feedback** during development.
- Ensure **core functionality is testable** and stable.
- Support **future scalability** toward more comprehensive testing (unit, integration, and E2E).
- Maintain a testing approach aligned with modern tooling (NUnit, Vitest, Playwright).

---

## 2. Overview of Test Types

### ✓ Unit Tests  
- **Backend:** NUnit  
- **Frontend:** Vitest + React Testing Library  
Focus on small, isolated pieces of logic (functions, components, handlers).

### ✓ Integration Tests (Future)  
Will verify:
- API endpoints
- Database operations (when moved from in-memory EF to SQL)
- Cross-module flows

### ✓ End-to-End (E2E) Tests (Future)  
Using **Playwright**, covering:
- Full UI workflows
- API/UI interactions
- Browser behavior

E2E tests will be introduced once the MVP evolves into a feature-complete user experience.

---

## 3. Backend Testing (NUnit)

### Current Scope (MVP)
- A smoke test to verify the test pipeline runs.
- Additional small unit tests can be added around:
  - Domain model validation
  - Services (when introduced)
  - API endpoint behavior (via integration tests in the future)

### Folder Structure
```
src/backend/TodoMvp.Api.Tests/
SmokeTests.cs
```

### Example Command
```bash
cd src/backend
dotnet test
```


### Why NUnit?
- Well-supported in .NET ecosystem  
- Easy to extend into integration testing  
- Works cleanly with minimal configuration  

---

## 4. Frontend Testing (Vitest + React Testing Library)

### Current Scope (MVP)
- A smoke test confirming the app renders.
- Component-level tests as features are implemented:
  - Task item component
  - Task list rendering
  - Interactions (e.g., button clicks, input)

### Tools
- **Vitest** (test runner; Vite-native, fast)
- **React Testing Library** (DOM testing)
- **@testing-library/jest-dom/vitest** (matchers like `toBeInTheDocument()`)

### Folder Structure

```
src/frontend/src/setupTests.ts
src/frontend/src/App.test.tsx
```

### Example Command

```bash
cd src/frontend
npm test
```


### Why Vitest?
- Integrates directly with Vite  
- Zero-config TypeScript support  
- Jest-compatible APIs for familiarity  

---

## 5. End-to-End Testing (Planned for Post-MVP)

### Approach
Introduce **Playwright** after core user flows exist.

E2E coverage will focus on:
- Creating tasks
- Editing tasks
- Completing tasks
- Persisting changes (when DB added)
- Error scenarios

### Benefits
- Confidence in real user workflows  
- Detects UI/API integration issues  
- Runs in CI easily  

---

## 6. CI/CD Integration (Future)

The project will later include:
- GitHub Actions workflow for:
  - Running backend tests
  - Running frontend tests
  - Running E2E tests (optional matrix with browsers)
- Test reporting for pass/fail/error
- Automatic running on each PR

---

## 7. Test Philosophy

- **Keep tests simple and meaningful.**
- **Test behavior, not implementation details.**
- **Optimize for developer confidence and speed.**
- **Grow test coverage organically with features.**
- **Avoid premature complexity in the MVP.**

---

## 8. Summary

| Layer      | Tooling                              | Status     |
|------------|---------------------------------------|------------|
| Backend    | NUnit                                 | ✔ Implemented |
| Frontend   | Vitest + React Testing Library        | ✔ Implemented |
| E2E        | Playwright                            | ⏳ Planned |
| CI/CD      | GitHub Actions                        | ⏳ Planned |

The testing strategy is intentionally lightweight but scalable, ensuring a solid foundation while keeping development velocity high for the MVP phase.
