import { useMemo, useState } from "react";
import type { CreateTaskRequest, TaskDto, UpdateTaskRequest } from "../../types/taskTypes";
import styles from "./TaskForm.module.css";
import { Button } from "../../../../ui/Button";

type Mode = "create" | "edit";

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
      await onCreate({ title: title.trim(), description: desc, dueDate: dueDateIso });
      return;
    }

    if (!initialTask) return;

    await onUpdate(initialTask.id, {
      title: title.trim(),
      description: desc,
      isCompleted,
      dueDate: dueDateIso,
    });
  }

const titleId = useMemo(() => `task-title-${mode}-${initialTask?.id ?? "new"}`, [mode, initialTask?.id]);
const dueId = useMemo(() => `task-due-${mode}-${initialTask?.id ?? "new"}`, [mode, initialTask?.id]);
const descId = useMemo(() => `task-desc-${mode}-${initialTask?.id ?? "new"}`, [mode, initialTask?.id]);


  return (
    <section className={styles.card} aria-label={mode === "create" ? "Create task form" : "Edit task form"}>
      <header className={styles.header}>
        <div className={styles.headerText}>
          <h3 className={styles.heading}>{mode === "create" ? "New task" : "Edit task"}</h3>
          <p className={styles.subheading}>
            {mode === "create" ? "Add details to keep yourself on track." : "Update details and status."}
          </p>
        </div>

        <Button type="button" onClick={onCancel} disabled={isSaving}>
          Close
        </Button>
      </header>

      {errorMessage && (
        <div className={styles.errorBox} role="alert">
          <strong>Couldn’t save:</strong> {errorMessage}
        </div>
      )}

      <form onSubmit={handleSubmit} className={styles.form}>
        {/* Title */}
        <div className={styles.fieldFull}>
          <label className={styles.label} htmlFor={titleId}>
            Title <span className={styles.required}>*</span>
          </label>
          <input
            id={titleId}
            className={styles.input}
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            maxLength={200}
            placeholder="e.g., Buy groceries"
            disabled={isSaving}
            autoFocus
          />
          <div className={styles.helpRow}>
            <span className={styles.helpText}>Keep it short and action-oriented.</span>
            <span className={styles.counter}>{title.length}/200</span>
          </div>
        </div>

        {/* Due date */}
        <div className={styles.field}>
          <label className={styles.label} htmlFor={dueId}>Due date</label>
          <input
            id={dueId}
            className={styles.input}
            type="datetime-local"
            value={dueDateLocal}
            onChange={(e) => setDueDateLocal(e.target.value)}
            disabled={isSaving}
          />
          <span className={styles.helpText}>Optional. Leave blank if not needed.</span>
        </div>

        {/* Completed (edit only) */}
        {mode === "edit" ? (
          <div className={styles.field}>
            <label className={styles.label}>Status</label>

            <label className={styles.toggleRow}>
              <input
                className={styles.checkbox}
                type="checkbox"
                checked={isCompleted}
                onChange={(e) => setIsCompleted(e.target.checked)}
                disabled={isSaving}
              />
              <span className={styles.toggleText}>
                Mark as <strong>Completed</strong>
              </span>
            </label>

            <span className={styles.helpText}>
              Completed tasks remain visible but will show as Completed.
            </span>
          </div>
        ) : (
          <div className={styles.field}>
            <label className={styles.label}>Status</label>
            <div className={styles.readonlyPill}>Open</div>
            <span className={styles.helpText}>New tasks start as Open.</span>
          </div>
        )}

        {/* Description */}
        <div className={styles.fieldFull}>
          <label className={styles.label} htmlFor={descId}>Description</label>
          <textarea
            id={descId}
            className={styles.textarea}
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            maxLength={2000}
            placeholder="Optional notes, steps, or context…"
            disabled={isSaving}
          />
          <div className={styles.helpRow}>
            <span className={styles.helpText}>Optional, but useful for larger tasks.</span>
            <span className={styles.counter}>{description.length}/2000</span>
          </div>
        </div>

        {/* Actions */}
        <div className={styles.actions}>
          <Button type="button" onClick={onCancel} disabled={isSaving}>
            Cancel
          </Button>

          <Button type="submit" variant="primary" disabled={!canSubmit}>
            {isSaving ? "Saving..." : mode === "create" ? "Create task" : "Save changes"}
          </Button>
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
