# Product Vision — To-Do Task Management MVP

## 1. Overview

This project is a **To-Do Task Management** application with a **.NET Core backend API** and a **React frontend**. The goal of this MVP is to deliver a **simple, clean, and extensible** task manager that demonstrates:

- Clear separation between frontend and backend
- Good architecture and code organization
- Reasonable test coverage
- A foundation that can be evolved into a real production product

This document defines **what we are building and why**, at a product level (not a technical spec).

---

## 2. Problem Statement

Knowledge workers, students, and small teams often need a **lightweight, focused tool** to track tasks without the complexity of full project management suites.

Common problems with existing tools:

- Too complex or heavy for simple personal / small-team workflows
- Not easily adaptable or extendable for specific needs
- Hard to reason about or extend from a developer’s perspective (for interview/demo purposes)

This MVP aims to be a **simple, opinionated** task manager, optimized for clarity and extensibility, not for feature parity with large incumbents.

---

## 3. Vision

Create a **minimal but production-minded** task management app that:

- Is **fast** and **pleasant** to use on desktop and mobile
- Has a **clean, understandable architecture** suitable for code review
- Can be realistically extended into a production system with:
  - Authentication and multi-user support
  - Persistent storage
  - Additional task organization features (tags, projects, due dates, etc.)

The application should serve as:

- A practical tool for basic daily task management
- A **reference implementation** of:
  - .NET Core REST API with EF Core
  - React frontend consuming that API
  - Automated tests across layers (NUnit, Jest, Playwright)

---

## 4. Target Users

For this MVP, we primarily focus on:

1. **Single user / individual contributor**
   - Needs a simple list of tasks
   - Wants to track what needs doing today and what is done

2. **Small, informal teams (future)**
   - Might share tasks in the same instance
   - Need light coordination without heavy project management

3. **Technical reviewers / interviewers**
   - Need to quickly assess design, architecture, testing approach, and code quality

---

## 5. MVP Goals

The MVP should allow a user to:

- **Create** a task with:
  - Title (required)
  - Optional description
  - Optional status, due date, and priority (if included in MVP scope)
- **View** a list of all tasks
- **Update** task fields (e.g., title, description, status)
- **Change** task status (e.g., Todo → In Progress → Done)
- **Delete** a task
- **Filter or view** tasks based on status (e.g., show only “Open” or “Completed” tasks)

In addition to functionality, the MVP must demonstrate:

- **Clean architecture** on the backend and frontend
- **RESTful API design** using JSON
- **Responsive UI** that works on desktop and mobile
- **Automated tests**:
  - Backend (NUnit)
  - Frontend unit tests (Jest)
  - At least one end-to-end scenario (Playwright)

---

## 6. Non-Goals (For This MVP)

The following are explicitly **out of scope** for this MVP, but may be added later:

- Full **authentication and authorization** (e.g., login, roles, JWT, OAuth)
- Multi-tenant support and complex user management
- Advanced task hierarchy:
  - Subtasks
  - Projects / boards / swimlanes
- Real-time collaboration (e.g., websockets, live updates)
- Complex analytics, reporting, or dashboards
- Integration with external services (email, calendars, chat tools)
- Mobile native applications (iOS / Android)

These non-goals help keep the MVP **focused** while allowing room for clearly documented future improvements.

---

## 7. High-Level Feature Set

For the MVP, the product feature set includes:

1. **Task Management**
   - Create/edit/delete tasks
   - Basic status workflow (e.g., Todo, In Progress, Done)
   - Optional: due date and priority

2. **Task List View**
   - List of tasks with key information (title, status, optional due date/priority)
   - Simple filtering or grouping by status (e.g., separate columns or filters)

3. **Task Details / Editing**
   - Ability to view and edit details of an existing task

4. **User Experience**
   - Responsive layout (desktop, tablet, mobile)
   - Simple, intuitive interactions with minimal clicks

5. **System & Technical**
   - REST API with clear endpoints for CRUD operations
   - Frontend consuming API using fetch/axios (or similar)
   - Automated tests across layers

---

## 8. Constraints & Assumptions (Product Level)

- MVP uses an **in-memory database** via EF Core:
  - Data is **not persistent** across application restarts
  - This is acceptable for demonstration and local development
- The MVP is designed for **a single user context**:
  - No per-user separation of data yet
- The system is not optimized for large datasets:
  - Intended for up to hundreds of tasks, not millions
- The primary environment is:
  - Local development (developer machine)
  - Potential simple deployment for demonstration

Technical constraints (e.g., .NET version, React version) are covered in the architecture and README docs.

---

## 9. “Production-MVP Mindset”

Although this is an MVP with limited scope, the design should **align with production-minded principles**:

- Clear separation of concerns and layers
- Predictable and consistent API contracts
- Error handling that can be evolved (e.g., centralized error responses)
- Testability and maintainability in mind from day one
- A path to:
  - Swap in-memory storage with a real database
  - Add authentication and user context
  - Support more complex features without massive rewrites

---

## 10. Success Criteria

The MVP is considered successful if:

- A user can reliably perform the full task lifecycle:
  - Create → Read → Update → Complete → Delete
- The system is **easy to set up and run** following the README instructions
- Reviewers can quickly understand:
  - Architectural decisions
  - Data model
  - API surface
  - Testing approach
- The codebase and documentation clearly show **how the system would evolve** into:
  - A multi-user, persistent, production-ready application

This document is the **product-level guide**. Technical implementation details are covered in `Architecture-Overview.md`, `API-Spec.md`, `Data-Model.md`, and related docs.
