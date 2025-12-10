# Data Model — To-Do Task Management MVP

## 1. Purpose and Scope

This document describes the **data model** for the To-Do Task Management MVP.

- Defines core entities, their fields, and constraints.
- Explains relationships (current and planned).
- Describes how the model is represented in EF Core (InMemory for MVP, but designed to be compatible with a relational database later).

This is intentionally small and focused: the MVP has a **single primary entity** (`Task`) with clear room to grow.

---

## 2. Design Principles

- **Keep it minimal, but realistic**  
  Only model what is needed for the current feature set, but structure it as if it could be persisted in a relational database.

- **Stable identifiers**  
  Each entity has a primary key (`Id`) suitable for use in URLs, API contracts, and database keys.

- **Explicit optional vs required fields**  
  Required fields (e.g., `Title`) are clearly marked. Optional fields (e.g., `Description`, `DueDate`) are nullable.

- **Future-ready**  
  Even though the MVP uses EF Core InMemory, the entity design should:
  - Map naturally to a SQL table later.
  - Allow adding foreign keys (e.g., `UserId`) without breaking changes.

---

## 3. Core Entity: Task

The **Task** entity represents an individual to-do item. It is the central concept of this MVP.

### 3.1 Task Fields

Logical fields (conceptual model):

- Id  
  - Type: integer  
  - Required: yes  
  - Purpose: primary key and unique identifier for the task.

- Title  
  - Type: string  
  - Required: yes  
  - Constraints:
    - Non-empty (after trimming whitespace).
    - Maximum length (e.g., 200 characters).

- Description  
  - Type: string  
  - Required: no (nullable)  
  - Constraints:
    - Maximum length (e.g., 2000 characters) to avoid excessively large payloads.

- Status  
  - Type: string (enum-like)  
  - Required: yes  
  - Allowed values (MVP):
    - Todo  
    - InProgress  
    - Done  
  - Default: Todo when not explicitly set on creation.

- Priority (optional for MVP but included in the model)  
  - Type: string (enum-like)  
  - Required: no  
  - Example allowed values:
    - Low  
    - Medium  
    - High  
  - If not provided, may default to Medium or null depending on implementation.

- DueDate  
  - Type: date (or date-time in storage), typically represented as ISO 8601 string in API.  
  - Required: no  
  - Purpose: optional due date; MVP may not enforce “must be in future” but can document this as a future rule.

- CreatedAt  
  - Type: date-time (UTC), ISO 8601 in API.  
  - Required: yes (set by the system on creation).  
  - Purpose: audit when the task was first created.

- UpdatedAt  
  - Type: date-time (UTC), ISO 8601 in API.  
  - Required: yes (set by the system).  
  - Purpose: audit when the task was last changed.
  - Should be updated on every successful modification (including status changes).

### 3.2 Task Constraints and Invariants

- Title must be present and valid for all Task instances.
- Status must always be one of the allowed values; invalid statuses are rejected at the API and/or application layer.
- CreatedAt and UpdatedAt are controlled by the system, not the client.
- Id is unique within the task set and should not change once assigned.

### 3.3 Task Lifecycle

The data model does not directly encode workflow rules beyond Status, but the typical lifecycle is:

- New task is created with Status = Todo.
- Status may change to InProgress and eventually Done.
- Task can be updated (title, description, due date, priority) at any point.
- Task can be deleted (hard delete) from the system.

Soft deletes are not modelled in the MVP (no IsDeleted flag).

---

## 4. Enumerations (Conceptual)

Although EF Core can store enumerations as strings, this document treats them conceptually:

- Status values:
  - Todo  
  - InProgress  
  - Done  

- Priority values (if implemented):
  - Low  
  - Medium  
  - High  

In the database / EF model, these may be represented as:

- String fields with validation logic, or
- Enum types mapped to strings or integers (implementation detail).

For the MVP, a simple string with validation is acceptable and easier to evolve.

---

## 5. Relationships

### 5.1 Current MVP Relationships

In the MVP, there is only one primary entity:

- Task

No other entities (e.g., User, Project, Tag) are persisted. This means:

- No foreign keys.
- No join tables.
- All tasks belong implicitly to a single logical user context.

### 5.2 Future Relationships (Planned / Envisioned)

The model is designed to easily support additional entities later:

- User  
  - A User entity that owns many Tasks.  
  - Relationship: a User has many Tasks; a Task belongs to one User (UserId as foreign key).

- Project or List  
  - A Project or List entity to group tasks.  
  - Relationship: a Project has many Tasks; a Task belongs to one Project.

- Tag  
  - A Tag entity representing labels.  
  - Relationship: many-to-many between Task and Tag (TaskTag join table).

None of these are implemented in the MVP, but the Task entity and API contracts are structured so these relationships can be added with minimal breaking changes.

---

## 6. EF Core Representation

### 6.1 EF Core and InMemory Provider

- The MVP uses **EF Core InMemory** as the database provider.
- Despite being in-memory, the entity configuration should be treated as if mapping to a relational database:
  - Primary key on Id.
  - Required property configuration for Title, Status, CreatedAt, UpdatedAt.
  - Optional property configuration for Description, Priority, DueDate.
  - Reasonable max lengths on strings (enforced via configuration or annotations).

### 6.2 Entity Configuration Considerations

- Primary Key:
  - Id is the primary key.
  - Configured for identity/auto-increment semantics (even in memory) so the application does not manually assign Ids.

- Property Constraints:
  - Title: required, max length.
  - Description: optional, max length.
  - Status: required; optionally enforce allowed values at application or model level.
  - Priority: optional; allowed values enforced at application level.
  - DueDate: optional; valid date.
  - CreatedAt and UpdatedAt: required, set by code before SaveChanges.

- Indexes (optional but future-friendly):
  - Index on Status (e.g., for filtering by status).
  - Index on DueDate (e.g., for future date-based queries).
  - For MVP with InMemory provider, these may not significantly impact performance but help when migrating to a real database.

---

## 7. Mapping Between API and Data Model

The external API (described in `API-Spec.md`) uses JSON to represent Task resources. The mapping is:

- API `id` ↔ Entity `Id`
- API `title` ↔ Entity `Title`
- API `description` ↔ Entity `Description`
- API `status` ↔ Entity `Status`
- API `priority` ↔ Entity `Priority`
- API `dueDate` ↔ Entity `DueDate`
- API `createdAt` ↔ Entity `CreatedAt`
- API `updatedAt` ↔ Entity `UpdatedAt`

Notes:

- The API never trusts client-supplied Id, CreatedAt, or UpdatedAt values:
  - Id is determined by the database / EF.
  - CreatedAt and UpdatedAt are set/updated by server logic.

- The API may use DTOs or request/response models that are separate from the EF entities, but semantically they carry the same fields.

---

## 8. Data Persistence Characteristics in MVP

- Storage Provider:
  - EF Core InMemory.
  - Data is lost when the application stops or restarts.

- Isolation:
  - A single DbContext-type model holds the Tasks DbSet.
  - There is no multi-tenant separation in the MVP.

- Concurrency:
  - The MVP does not implement advanced concurrency control (e.g., row versioning).
  - If needed later, a concurrency token (e.g., RowVersion) can be added to the Task entity.

---

## 9. Future Data Model Enhancements

Planned or possible extensions to this data model include:

- **User entity and ownership**
  - Add User with one-to-many Tasks.
  - Add UserId foreign key to Task.

- **Soft deletes**
  - Add IsDeleted and DeletedAt fields to Task.
  - Replace hard delete with soft delete in the API.

- **Audit fields**
  - Add CreatedBy / UpdatedBy if authentication is introduced.

- **Task hierarchy and structure**
  - Subtasks with parent-child relationship between Task entities.
  - Projects or lists to group tasks.
  - Tags and many-to-many relationships.

- **Additional attributes**
  - ReminderDate, CompletedAt, or other lifecycle fields.

These enhancements can be added incrementally, building on the Task entity defined here without requiring a fundamental redesign.

---
