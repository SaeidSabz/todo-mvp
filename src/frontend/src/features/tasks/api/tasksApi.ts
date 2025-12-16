import { config } from "../../../app/config";
import type { CreateTaskRequest, TaskDto, UpdateTaskRequest } from "../types/taskTypes";

/**
 * Fetches all tasks from the backend.
 * @throws Error when configuration is missing, network fails, or response is not OK/expected.
 */
export async function getTasks(signal?: AbortSignal): Promise<TaskDto[]> {
  const baseUrl = requireApiBaseUrl();

  const response = await fetch(`${baseUrl}/api/tasks`, {
    method: "GET",
    headers: { Accept: "application/json" },
    signal,
  });

  if (!response.ok) {
    throw new Error(`HTTP ${response.status} while calling /api/tasks.`);
  }

  const data: unknown = await response.json();
  if (!Array.isArray(data)) {
    throw new Error("Unexpected API response: expected an array of tasks.");
  }

  return data as TaskDto[];
}

export async function createTask(request: CreateTaskRequest, signal?: AbortSignal): Promise<TaskDto> {
  const baseUrl = requireApiBaseUrl();

  const response = await fetch(`${baseUrl}/api/tasks`, {
    method: "POST",
    headers: { "Content-Type": "application/json", Accept: "application/json" },
    body: JSON.stringify(request),
    signal,
  });

  if (!response.ok) {
    throw new Error(`HTTP ${response.status} while calling POST /api/tasks.`);
  }

  // Support both: 201 with body OR 204 no body (rare for POST but possible)
  if (response.status === 204) {
    throw new Error("API returned 204 No Content for create. Expected task payload.");
  }

  return (await response.json()) as TaskDto;
}

/**
 * Updates a task. Supports APIs that return 200 + body OR 204 No Content.
 * Returns the updated task when available, otherwise null.
 */
export async function updateTask(id: number, request: UpdateTaskRequest, signal?: AbortSignal): Promise<TaskDto | null> {
  const baseUrl = requireApiBaseUrl();

  const response = await fetch(`${baseUrl}/api/tasks/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json", Accept: "application/json" },
    body: JSON.stringify(request),
    signal,
  });

  if (!response.ok) {
    throw new Error(`HTTP ${response.status} while calling PUT /api/tasks/${id}.`);
  }

  if (response.status === 204) {
    return null;
  }

  // If API returns a body, parse it
  const contentType = response.headers.get("content-type") ?? "";
  if (!contentType.includes("application/json")) {
    return null;
  }

  return (await response.json()) as TaskDto;
}

/**
 * Deletes a task. Returns true if deleted, false if not found (only if API returns 404).
 */
export async function deleteTask(id: number, signal?: AbortSignal): Promise<boolean> {
  const baseUrl = requireApiBaseUrl();

  const response = await fetch(`${baseUrl}/api/tasks/${id}`, {
    method: "DELETE",
    headers: { Accept: "application/json" },
    signal,
  });

  if (response.status === 404) {
    return false;
  }

  if (!response.ok) {
    throw new Error(`HTTP ${response.status} while calling DELETE /api/tasks/${id}.`);
  }

  return true;
}

/**
 * Ensures API base URL is configured.
 */
function requireApiBaseUrl(): string {
  const baseUrl = config.apiBaseUrl;
  if (!baseUrl) {
    throw new Error("VITE_API_BASE_URL is not configured.");
  }
  return baseUrl;
}