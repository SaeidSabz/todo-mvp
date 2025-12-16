# Product Vision — To-Do Task Management MVP

## 1. Overview

This project is a **To-Do Task Management** application with a **.NET 10 backend API** and a **React frontend**.  
The goal of this MVP is to deliver a **simple, clean, and extensible** task manager that demonstrates:

- Clear separation between frontend and backend
- Good architecture and code organization
- Reasonable test coverage
- A foundation that can be evolved into a real production product

This document defines **what we are building and why**, at a product level (not a technical specification).

---

## 2. Problem Statement

Knowledge workers, students, and small teams often need a **lightweight, focused tool** to track tasks without the complexity of full project management suites.

Common problems with existing tools:

- Too complex or heavy for simple personal or small-team workflows
- Not easily adaptable or extendable for specific needs
- Hard to reason about or extend from a developer’s perspective (for interview or demo purposes)

This MVP aims to be a **simple, opinionated** task manager, optimized for clarity and extensibility rather than feature parity with large incumbents.

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
  - A .NET 10 REST API with EF Core
  - A React frontend consuming that API
  - Automated unit and component tests (NUnit and Vitest)

---

## 4. Target Users

For this MVP, we primarily focus on:

1. **Single user / individual contributor**
   - Needs a simple list of tasks
   - Wants to track what needs doing and what is completed

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
  - Optional due date
- **View** a list of all tasks
- **Update** task fields (title, description, due date)
- **Change** task status between:
  - Open
  - Completed
- **Delete** a task
- **Filter** tasks based on status (e.g., All, Open, Completed)

In addition to functionality, the MVP must demonstrate:

- **Clean architecture** on the backend and frontend
- **RESTful API design** using JSON
- **Responsive UI** that works on desktop and mobile
- **Automated tests**:
  - Backend unit tests (NUnit)
  - Frontend unit and component tests (Vitest)

---

## 6. Non-Goals (For This MVP)

The following are explicitly **out of scope** for this MVP, but may be added later:

- Authentication and authorization (login, roles, JWT, OAuth)
- Multi-tenant or multi-user data separation
- Advanced task hierarchies (subtasks, projects, boards)
- Real-time collaboration or live updates
- Complex analytics or reporting
- Integration with external services
- Native mobile applications (iOS / Android)

These non-goals help keep the MVP **focused** while leaving room for future expansion.

---

## 7. High-Level Feature Set

For the MVP, the product feature set includes:

1. **Task Management**
   - Create, edit, and delete tasks
   - Basic status workflow: Open ↔ Completed
   - Optional due date

2. **Task List View**
   - List of tasks with key information
   - Filtering by status

3. **Task Editing**
   - Inline or modal-based editing of task details

4. **User Experience**
   - Responsive layout (desktop, tablet, mobile)
   - Simple, intuitive interactions

5. **System & Technical**
   - REST API with clear CRUD endpoints
   - Frontend consuming API via HTTP
   - Automated tests across backend and frontend

---

## 8. Constraints & Assumptions (Product Level)

- MVP uses an **in-memory database** via EF Core:
  - Data is not persistent across restarts
- Designed for a **single-user context**
- Not optimized for large datasets
- Primary usage is:
  - Local development
  - Demonstration and code review

Technical constraints are covered in architecture and README documentation.

---

## 9. Production-MVP Mindset

Although limited in scope, the MVP follows **production-minded principles**:

- Clear separation of concerns
- Predictable API contracts
- Centralized error handling
- Testability and maintainability from day one
- A clear path to:
  - Persistent storage
  - Authentication and user context
  - Feature expansion without major rewrites

---

## 10. Success Criteria

The MVP is considered successful if:

- A user can complete the full task lifecycle:
  - Create → Read → Update → Complete → Delete
- The system is easy to set up using the README
- Reviewers can quickly understand:
  - Architecture
  - Data model
  - API surface
  - Testing approach
- The codebase clearly shows how it could evolve into a production-ready system

This document is the **product-level guide**.  
Technical details are covered in `Architecture-Overview.md`, `API-Spec.md`, `Data-Model.md`, and related documents.
