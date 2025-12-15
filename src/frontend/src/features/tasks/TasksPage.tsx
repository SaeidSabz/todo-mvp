import { useTasksQuery } from "./hooks/useTasksQuery";
import { TaskList } from "./components/TaskList";

/**
 * Page that loads and displays tasks with loading/error/empty states.
 */
export function TasksPage() {
  const { status, error, tasks, reload } = useTasksQuery();

  return (
    <section style={{ marginTop: "1.5rem" }}>
      <header style={styles.header}>
        <h2 style={{ margin: 0 }}>Tasks</h2>

        <button
          type="button"
          onClick={() => void reload()}
          disabled={status === "loading"}
          style={styles.button}
        >
          {status === "loading" ? "Loading..." : "Refresh"}
        </button>
      </header>

      {status === "loading" && <p style={{ marginTop: "1rem" }}>Loading tasks…</p>}

      {status === "error" && (
        <div style={{ marginTop: "1rem" }}>
          <p style={{ color: "crimson", marginBottom: "0.75rem" }}>
            Couldn’t load tasks: {error ?? "Unknown error"}
          </p>
          <button type="button" onClick={() => void reload()} style={styles.button}>
            Retry
          </button>
        </div>
      )}

      {status === "success" && tasks.length === 0 && (
        <p style={{ marginTop: "1rem" }}>No tasks yet.</p>
      )}

      {status === "success" && tasks.length > 0 && <TaskList tasks={tasks} />}
    </section>
  );
}

const styles: Record<string, React.CSSProperties> = {
  header: {
    display: "flex",
    alignItems: "center",
    justifyContent: "space-between",
    gap: "1rem",
    flexWrap: "wrap",
  },
  button: {
    padding: "0.5rem 0.9rem",
    borderRadius: 8,
    border: "1px solid #ccc",
    cursor: "pointer",
    background: "white",
  },
};
