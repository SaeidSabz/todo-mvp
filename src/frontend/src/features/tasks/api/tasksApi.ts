import { config } from "../../../app/config";
import type { TaskDto } from "../types/taskTypes";

/**
 * Fetches all tasks from the backend.
 * @throws Error when configuration is missing, network fails, or response is not OK/expected.
 */
export async function getTasks(signal?: AbortSignal): Promise<TaskDto[]> {
  if (!config.apiBaseUrl) {
    throw new Error("VITE_API_BASE_URL is not configured.");
  }

  const response = await fetch(`${config.apiBaseUrl}/api/tasks`, {
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