# To-Do Task Management MVP

A small but production-minded **Task Management** application built with a **.NET Core backend** and a **React frontend**.  
The goal is to demonstrate **clean architecture, clear API design, and automated testing** while keeping the feature set intentionally minimal.

---  

[![CI](https://github.com/SaeidSabz/todo-mvp/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/SaeidSabz/todo-mvp/actions/workflows/ci.yml)
![.NET](https://img.shields.io/badge/.NET-10-blue)
![React](https://img.shields.io/badge/React-19-blue)
![TypeScript](https://img.shields.io/badge/TypeScript-5-blue)

---  

## 1. Overview

This repository contains:

- A **backend API** (C# / .NET / EF Core InMemory) exposing RESTful JSON endpoints for managing tasks.
- A **frontend** (React) that consumes the API and provides a simple UI for:
  - Creating tasks  
  - Viewing and filtering tasks  
  - Updating tasks (including status)  
  - Deleting tasks  

The system is designed as an **MVP** but with a clear path toward a more production-ready application (real database, authentication, multi-user support, etc.).

---

## 2. Tech Stack (Planned)

**Backend**

- .NET Core Web API (e.g., .NET 8 or similar LTS)
- EF Core with InMemory provider (for MVP)
- NUnit for backend unit tests

**Frontend**

- React (with JSX; TypeScript)
- Jest (and React Testing Library) for unit/component tests
- Playwright for end-to-end tests

---

## 3. Repository Structure

```text
todo-mvp/
├─ README.md
├─ docs/
│  ├─ 01-Product-Vision.md
│  ├─ 02-Architecture-Overview.md
│  ├─ 03-Functional-Requirements.md
│  ├─ 04-NonFunctional-Requirements.md
│  ├─ 05-API-Spec.md
│  ├─ 06-Data-Model.md
│  ├─ 07-Frontend-Design.md
│  ├─ 08-Testing-Strategy.md
│  └─ 09-Future-Scalability.md
└─ src/
   ├─ backend/
   └─ frontend/
```
- ```docs/``` – Design and requirements documents (see links below).
- ```src/backend/``` – .NET Core API project (to be implemented).
- ```src/frontend/``` – React application (to be implemented).

## 4. Documentation

All key decisions and requirements are documented in `/docs`:

- [01 – Product Vision](docs/01-Product-Vision.md)  
- [02 – Architecture Overview](docs/02-Architecture-Overview.md)  
- [03 – Functional Requirements](docs/03-Functional-Requirements.md)  
- [04 – Non-Functional Requirements](docs/04-NonFunctional-Requirements.md)  
- [05 – API Specification](docs/05-API-Spec.md)  
- [06 – Data Model](docs/06-Data-Model.md)  
- [07 – Frontend Design](docs/07-Frontend-Design.md)  
- [08 – Testing Strategy](docs/08-Testing-Strategy.md)  
- [09 – Future & Scalability Notes](docs/09-Future-Scalability.md)  

If anything in the code appears unclear, these documents should be the first place to look.

---

## 5. Getting Started (High-Level)

> Note: Commands below are placeholders and may be refined once implementation is in place.

### 5.1 Prerequisites

- .NET SDK installed (matching the chosen version for the backend).
- Node.js and a package manager (npm or yarn) for the frontend.

### 5.2 Clone the Repository

Clone the project:
```yaml
git clone <repo-url> todo-mvp
cd todo-mvp
```


### 5.3 Run the Backend (TodoMvp.Api)

The backend is a .NET 10 Web API project located under `src/backend/TodoMvp.Api`.

#### Prerequisites

- .NET SDK 10 installed

You can verify your installation with:

    dotnet --version

#### How to run the backend

From the repository root:

    cd src/backend/TodoMvp.Api
    dotnet run

You should see console output indicating that the application is listening on HTTP/HTTPS ports, for example:

    Now listening on: https://localhost:7157
    Now listening on: http://localhost:5157

#### Health Check Endpoint

To verify the backend is running, call the health endpoint in a browser or with a tool like curl/Postman:

    GET https://localhost:<port>/api/health

Expected JSON response shape:

    {
      "status": "ok",
      "timestampUtc": "2025-01-01T12:00:00Z"
    }

(The exact `timestampUtc` value will vary.)

The API will listen on a local port (e.g., ```http://localhost:5000```) depending on configuration.

---  
## 6. Testing (Planned)
### Backend Tests (NUnit)
```bash
cd src/backend
# dotnet test
```

### Frontend Tests (Jest)
```bash
cd src/frontend
# npm test
```

### End-to-End Tests (Playwright)
```bash
cd src/frontend
# npx playwright test
```

Exact commands and test project locations will be defined once the implementation is added.  
See [08 – Testing Strategy](docs/08-Testing-Strategy.md) for detailed testing plans.

---

## 7. Assumptions, Trade-offs, and Future Work

Key assumptions and trade-offs for this MVP:

- Single-user context (no authentication) for simplicity.
- EF Core InMemory is used to avoid database setup overhead.
- Frontend and backend are separate projects but share a stable REST contract.
- Focus is on clarity, testability, and clean architecture, rather than feature breadth.

Planned future improvements (beyond MVP) are described in:

- [09 – Future & Scalability Notes](docs/09-Future-Scalability.md)

---

## 8. Contribution and Review

This repository is structured to be easy to **review** (for interviews, code reviews, or demos):

- Design documents first (in `docs/`).
- Clean separation between backend and frontend (in `src/`).
- Tests and tooling integrated from the start.

Feedback, comments, and improvements should ideally reference the relevant design documents to keep code and documentation aligned.
