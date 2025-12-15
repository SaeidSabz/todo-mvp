import type { TaskDto } from "../types/taskTypes";
import { TaskCard } from "./TaskCard";

/**
 * Renders a responsive grid of task cards.
 */
export function TaskList({ tasks }: { tasks: TaskDto[] }) {
  return (
    <div style={styles.grid}>
      {tasks.map((t) => (
        <TaskCard key={t.id} task={t} />
      ))}
    </div>
  );
}

const styles: Record<string, React.CSSProperties> = {
  grid: {
    marginTop: "1rem",
    display: "grid",
    gridTemplateColumns: "repeat(auto-fit, minmax(280px, 1fr))",
    gap: "1rem",
  },
};
