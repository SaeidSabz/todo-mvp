import styles from "./TaskFilter.module.css";

/**
 * Represents the supported task status filters.
 */
export type TaskStatusFilter = "all" | "open" | "completed";

/**
 * Renders a dropdown filter for task status (All / Open / Completed).
 */
export function TaskFilter(props: {
  value: TaskStatusFilter;
  onChange: (value: TaskStatusFilter) => void;
  disabled?: boolean;
}) {
  const { value, onChange, disabled } = props;

  return (
    <label className={styles.container}>
      <span className={styles.labelText}>Filter</span>

      <select
        className={styles.select}
        value={value}
        onChange={(e) => onChange(e.target.value as TaskStatusFilter)}
        disabled={disabled}
        aria-label="Task status filter"
      >
        <option value="all">All</option>
        <option value="open">Open</option>
        <option value="completed">Completed</option>
      </select>
    </label>
  );
}