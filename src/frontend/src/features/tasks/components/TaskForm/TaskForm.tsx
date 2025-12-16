import { useMemo, useState } from "react";
import type { CreateTaskRequest, TaskDto, UpdateTaskRequest } from "../../types/taskTypes";
import styles from "./TaskForm.module.css";

type Mode = "create" | "edit";

/**
 * Task form for create/edit.
 */
export function TaskForm(props: {
  mode: Mode;
  initialTask?: TaskDto;
  isSaving: boolean;
  errorMessage: string | null;
  onCancel: () => void;
  onCreate: (request: CreateTaskRequest) => Promise<void>;
  onUpdate: (id: number, request: UpdateTaskRequest) => Promise<void>;
}) {
  const { mode, initialTask, isSaving, errorMessage, onCancel, onCreate, onUpdate } = props;

  const [title, setTitle] = useState<string>(initialTask?.title ?? "");
  const [description, setDescription] = useState<string>(initialTask?.description ?? "");
  const [dueDateLocal, setDueDateLocal] = useState<string>(() =>
    initialTask?.dueDate ? toDateTimeLocal(initialTask.dueDate) : ""
  );
  const [isCompleted, setIsCompleted] = useState<boolean>(initialTask?.isCompleted ?? false);

  const canSubmit = useMemo(() => title.trim().length > 0 && !isSaving, [title, isSaving]);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!title.trim()) return;

    const dueDateIso = dueDateLocal ? new Date(dueDateLocal).toISOString() : null;
    const desc = description.trim().length > 0 ? description.trim() : null;

    if (mode === "create") {
      const request: CreateTaskRequest = {
        title: title.trim(),
        description: desc,
        dueDate: dueDateIso,
      };
      await onCreate(request);
      return;
    }

    if (!initialTask) return;

    const request: UpdateTaskRequest = {
      title: title.trim(),
      description: desc,
      isCompleted,
      dueDate: dueDateIso,
    };

    await onUpdate(initialTask.id, request);
  }

  return (
    <section className={styles.container}>
      <header className={styles.header}>
        <h3 className={styles.heading}>{mode === "create" ? "Create Task" : "Edit Task"}</h3>

        <button type="button" className={styles.secondaryButton} onClick={onCancel} disabled={isSaving}>
          Cancel
        </button>
      </header>

      {errorMessage && <p className={styles.error}>Error: {errorMessage}</p>}

      <form onSubmit={handleSubmit} className={styles.form}>
        <label className={styles.label}>
          Title <span className={styles.required}>*</span>
          <input
            className={styles.input}
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            maxLength={200}
            placeholder="e.g., Buy groceries"
            disabled={isSaving}
          />
        </label>

        <label className={styles.label}>
          Description
          <textarea
            className={styles.textarea}
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            maxLength={2000}
            placeholder="Optional"
            disabled={isSaving}
          />
        </label>

        <label className={styles.label}>
          Due date
          <input
            className={styles.input}
            type="datetime-local"
            value={dueDateLocal}
            onChange={(e) => setDueDateLocal(e.target.value)}
            disabled={isSaving}
          />
        </label>

        {mode === "edit" && (
          <label className={styles.checkboxRow}>
            <input
              type="checkbox"
              checked={isCompleted}
              onChange={(e) => setIsCompleted(e.target.checked)}
              disabled={isSaving}
            />
            Completed
          </label>
        )}

        <div className={styles.actions}>
          <button className={styles.primaryButton} type="submit" disabled={!canSubmit}>
            {isSaving ? "Saving..." : mode === "create" ? "Create" : "Save"}
          </button>
        </div>
      </form>
    </section>
  );
}

function toDateTimeLocal(iso: string): string {
  const d = new Date(iso);
  if (Number.isNaN(d.getTime())) return "";
  const pad = (n: number) => String(n).padStart(2, "0");
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
}
