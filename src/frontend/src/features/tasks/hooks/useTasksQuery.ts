import { useCallback, useEffect, useState } from "react";
import { getTasks } from "../api/tasksApi";
import type { TaskDto } from "../types/taskTypes";

type QueryStatus = "idle" | "loading" | "success" | "error";

/**
 * Loads tasks from the backend and exposes loading/error state + a reload function.
 */
export function useTasksQuery() {
  const [status, setStatus] = useState<QueryStatus>("idle");
  const [error, setError] = useState<string | null>(null);
  const [tasks, setTasks] = useState<TaskDto[]>([]);

  const reload = useCallback(async () => {
    const controller = new AbortController();

    setStatus("loading");
    setError(null);

    try {
      const result = await getTasks(controller.signal);
      setTasks(result);
      setStatus("success");
    } catch (e) {
      if (e instanceof DOMException && e.name === "AbortError") {
        return;
      }

      const message = e instanceof Error ? e.message : "Unknown error.";
      setError(message);
      setStatus("error");
    }

    return () => controller.abort();
  }, []);

  useEffect(() => {
    void reload();
  }, [reload]);

  return { status, error, tasks, reload };
}
