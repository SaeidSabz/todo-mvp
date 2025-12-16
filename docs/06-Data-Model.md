# Data Model — To-Do Task Management MVP

## 1. Purpose and Scope

This document describes the **data model** for the To-Do Task Management MVP.

- Defines core entities, their fields, and constraints.
- Explains relationships (current and planned).
- Describes how the model is represented in EF Core (InMemory for MVP, but designed to be compatible with a relational database later).

The MVP intentionally has a **single primary entity** (`Task`) with a clean and extensible structure.

---

## 2. Design Principles

- **Minimal but realistic**  
  Only model what is needed for the MVP while keeping the structure suitable for a real relational database.

- **Stable identifiers**  
  Each entity has a stable primary key (`Id`) suitable for API contracts and URLs.

- **Explicit required vs optional fields**  
  Required fields (e.g., `Title`, `Status`) are clearly distinguished from optional fields (e.g., `Description`, `DueDate`).

- **Future-ready**  
  Although EF Core InMemory is used for the MVP:
  - The model maps cleanly to a SQL database.
  - Additional foreign keys (e.g., `UserId`) can be added without breaking changes.

---

## 3. Core Entity: Task

The **Task** entity represents a single to-do item.

### 3.1 Task Fields

Logical (conceptual) fields:

- **Id**
  - Type: integer
  - Required: yes
  - Purpose: primary key and unique identifier.

- **Title**
  - Type: string
  - Required: yes
  - Constraints:
    - Non-empty after trimming whitespace.
    - Maximum length (e.g., 200 characters).

- **Description**
  - Type: string
  - Required: no (nullable)
  - Constraints:
    - Maximum length (e.g., 2000 characters).

- **IsCompleted**
  - Type: boolean
  - Required: yes
  - Purpose: represents task completion state.
  - Default: `false` on creation.

- **DueDate**
  - Type: date-time (UTC)
  - Required: no (nullable)
  - Purpose: optional deadline for the task.
  - MVP does not enforce future-only dates.

- **CreatedAt**
  - Type: date-time (UTC)
  - Required: yes
  - Purpose: timestamp when the task was created.
  - Set by the server.

- **UpdatedAt**
  - Type: date-time (UTC)
  - Required: no (nullable)
  - Purpose: timestamp of the last modification.
  - Null until the task is updated for the first time.

> Note: A `Priority` field is intentionally **not implemented** in the MVP code but is documented as a possible future extension.

---

## 3.2 Task Constraints and Invariants

- Title must always be present and valid.
- `IsCompleted` is always defined (`true` or `false`).
- `CreatedAt` is set once and never modified.
- `UpdatedAt` is updated on each successful modification.
- Client input must never control `Id`, `CreatedAt`, or `UpdatedAt`.

---

## 3.3 Task Lifecycle

- Task is created with:
  - `IsCompleted = false`
  - `CreatedAt = now (UTC)`
  - `UpdatedAt = null`
- Task may be updated:
  - Title, Description, DueDate
  - Completion status
- On update:
  - `UpdatedAt` is set to current UTC time.
- Task may be deleted permanently (hard delete).

Soft deletes are intentionally **out of scope** for the MVP.

---

## 4. Enumerations (Conceptual)

The MVP avoids complex enums in favor of simple primitives.

- Completion state:
  - `IsCompleted = false` → Open
  - `IsCompleted = true` → Completed

Future enhancements may introduce:
- Explicit status enums (`Todo`, `InProgress`, `Done`)
- Priority enums (`Low`, `Medium`, `High`)

---

## 5. Relationships

### 5.1 Current MVP Relationships

- Single entity: `Task`
- No foreign keys
- All tasks belong to a single implicit user context

---

### 5.2 Future Relationships (Planned)

The model is designed to support:

- **User**
  - One-to-many relationship (`User` → `Tasks`)
  - Add `UserId` foreign key to `Task`

- **Project / List**
  - Grouping tasks into collections

- **Tags**
  - Many-to-many relationship via join table

None of these are implemented in the MVP.

---

## 6. EF Core Representation

### 6.1 Provider

- EF Core **InMemory** provider
- Data is not persisted across restarts

### 6.2 Configuration Guidelines

- Primary key: `Id` (identity)
- Required properties:
  - `Title`
  - `IsCompleted`
  - `CreatedAt`
- Optional properties:
  - `Description`
  - `DueDate`
  - `UpdatedAt`

- String length constraints enforced via configuration or annotations.

---

## 7. API ↔ Data Model Mapping

| API Field     | Entity Property |
|----------------|----------------|
| `id`          | `Id`           |
| `title`       | `Title`        |
| `description` | `Description`  |
| `isCompleted` | `IsCompleted`  |
| `dueDate`     | `DueDate`      |
| `createdAt`   | `CreatedAt`    |
| `updatedAt`   | `UpdatedAt`    |

Notes:
- API ignores client-supplied `Id`, `CreatedAt`, `UpdatedAt`.
- DTOs may be used but mirror the same semantics.

---

## 8. Persistence Characteristics

- Storage: EF Core InMemory
- Isolation: single DbContext
- Concurrency:
  - No optimistic concurrency in MVP
  - Can add `RowVersion` later if needed

---

## 9. Future Enhancements

Possible extensions include:

- User ownership (`UserId`)
- Soft deletes (`IsDeleted`, `DeletedAt`)
- Audit fields (`CreatedBy`, `UpdatedBy`)
- Task hierarchy (subtasks)
- Priority, tags, reminders

All enhancements can be added incrementally without breaking the core model.

---
