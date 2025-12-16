import { useMemo, useState } from "react";
import type { TaskDto } from "./types/taskTypes";
import { TaskList } from "./components/TaskList/TaskList";
import { TaskForm } from "./components/TaskForm/TaskForm";
import { useTasksQuery } from "./hooks/useTasksQuery";
import { useTaskMutations } from "./hooks/useTaskMutations";

/**
 * Page that loads and displays tasks with loading/error/empty states.
 */
export function TasksPage() {
  const { status, error, tasks, reload } = useTasksQuery();
   const { isSaving, saveError, isDeleting, deleteError, create, update, remove } = useTaskMutations();

   const [editingTask, setEditingTask] = useState<TaskDto | null>(null);
  const isFormOpen = useMemo(() => editingTask !== null, [editingTask]);

  async function handleCreate(request: { title: string; description: string | null; dueDate: string | null }) {
    await create(request);
    setEditingTask(null);
    await reload();
  }

  async function handleUpdate(id: number, request: { title: string; description: string | null; isCompleted: boolean; dueDate: string | null }) {
    await update(id, request);
    setEditingTask(null);
    await reload();
  }

  async function handleDelete(task: TaskDto) {
    const confirmed = window.confirm(`Delete "${task.title}"?`);
    if (!confirmed) return;

    const deleted = await remove(task.id);
    if (deleted) {
      await reload();
    } else {
      // Not found: reload anyway so UI reflects reality
      await reload();
    }
  }

  return (
    <section style={{ marginTop: "1.5rem" }}>
      <header style={styles.header}>
        <h2 style={{ margin: 0 }}>Tasks</h2>

        <div style={{ display: "flex", gap: "0.75rem" }}>
          <button
            type="button"
            onClick={() => setEditingTask({} as TaskDto)} // sentinel for "create mode"
            style={styles.button}
          >
            + New Task
          </button>

          <button type="button" onClick={() => void reload()} disabled={status === "loading"} style={styles.button}>
            {status === "loading" ? "Loading..." : "Refresh"}
          </button>
        </div>
      </header>

      {isFormOpen && (
        <div style={{ marginTop: "1rem" }}>
          <TaskForm
            mode={editingTask && editingTask.id ? "edit" : "create"}
            initialTask={editingTask && editingTask.id ? editingTask : undefined}
            isSaving={isSaving}
            errorMessage={saveError}
            onCancel={() => setEditingTask(null)}
            onCreate={handleCreate}
            onUpdate={handleUpdate}
          />
        </div>
      )}

      {deleteError && (
        <p style={{ color: "crimson", marginTop: "1rem" }}>
          Delete error: {deleteError}
        </p>
      )}

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

      {status === "success" && tasks.length === 0 && <p style={{ marginTop: "1rem" }}>No tasks yet.</p>}

      {status === "success" && tasks.length > 0 && (
        <TaskList
          tasks={tasks}
          onEdit={(t) => setEditingTask(t)}
          onDelete={(t) => void handleDelete(t)}
          isDeleting={isDeleting}
        />
      )}
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
