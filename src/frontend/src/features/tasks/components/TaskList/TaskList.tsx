import styles from "./TaskList.module.css";
import type { TaskDto } from "../../types/taskTypes";
import { TaskCard } from "../TaskCard/TaskCard";

/**
 * Renders a responsive grid of task cards.
 */
export function TaskList(props: {
  tasks: TaskDto[];
  onEdit: (task: TaskDto) => void;
  onDelete: (task: TaskDto) => void;
  isDeleting: boolean;
}) {
  const { tasks, onEdit, onDelete, isDeleting } = props;

  return (
    <div className={styles.grid}>
      {tasks.map((t) => (
        <TaskCard key={t.id} task={t} onEdit={onEdit} onDelete={onDelete} isDeleting={isDeleting} />
      ))}
    </div>
  );
}