import { useCallback, useState } from "react";
import { createTask, deleteTask, updateTask } from "../api/tasksApi";
import type { CreateTaskRequest, UpdateTaskRequest } from "../types/taskTypes";

/**
 * Provides task mutation operations (create/update/delete) and exposes operation states.
 */
export function useTaskMutations() {
  const [isSaving, setIsSaving] = useState(false);
  const [saveError, setSaveError] = useState<string | null>(null);

  const [isDeleting, setIsDeleting] = useState(false);
  const [deleteError, setDeleteError] = useState<string | null>(null);

  const create = useCallback(async (request: CreateTaskRequest) => {
    setIsSaving(true);
    setSaveError(null);

    try {
      return await createTask(request);
    } catch (e) {
      setSaveError(e instanceof Error ? e.message : "Unknown error.");
      throw e;
    } finally {
      setIsSaving(false);
    }
  }, []);

  const update = useCallback(async (id: number, request: UpdateTaskRequest) => {
    setIsSaving(true);
    setSaveError(null);

    try {
      return await updateTask(id, request);
    } catch (e) {
      setSaveError(e instanceof Error ? e.message : "Unknown error.");
      throw e;
    } finally {
      setIsSaving(false);
    }
  }, []);

  const remove = useCallback(async (id: number) => {
    setIsDeleting(true);
    setDeleteError(null);

    try {
      return await deleteTask(id);
    } catch (e) {
      setDeleteError(e instanceof Error ? e.message : "Unknown error.");
      throw e;
    } finally {
      setIsDeleting(false);
    }
  }, []);

  return { isSaving, saveError, isDeleting, deleteError, create, update, remove };
}
