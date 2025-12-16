import styles from "./TaskCard.module.css";
import type { TaskDto } from "../../types/taskTypes";

/**
 * Renders a single task as a card.
 */
export function TaskCard(props: {
  task: TaskDto;
  onEdit: (task: TaskDto) => void;
  onDelete: (task: TaskDto) => void;
  isDeleting: boolean;
}) {
  const { task, onEdit, onDelete, isDeleting } = props;
  const due = formatOptionalDate(task.dueDate);

  return (
    <article className={styles.card}>
      <div className={styles.header}>
        <h3 className={styles.title}>{task.title}</h3>

        <span className={`${styles.badge} ${task.isCompleted ? styles.badgeCompleted : styles.badgeOpen}`}>
          {task.isCompleted ? "Completed" : "Open"} 
        </span>

      </div>

      {task.description && <p className={styles.description}>{task.description}</p>}

      {due && (
        <p className={styles.meta}>
          <strong>Due:</strong> {due}
        </p>
      )}

    <div className={styles.actions}>
        <button type="button" className={styles.secondaryButton} onClick={() => onEdit(task)}>
          Edit
        </button>

        <button
          type="button"
          className={styles.dangerButton}
          onClick={() => onDelete(task)}
          disabled={isDeleting}
        >
          {isDeleting ? "Deleting..." : "Delete"}
        </button>
      </div>
    </article>
  );
}

/**
 * Formats a nullable ISO date string for display.
 */
function formatOptionalDate(value: string | null): string | null {
  if (!value) return null;
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return value;
  return date.toLocaleString();
}


