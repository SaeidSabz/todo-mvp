import { useMemo, useState } from "react";
import type { TaskDto } from "./types/taskTypes";
import { TaskList } from "./components/TaskList/TaskList";
import { TaskForm } from "./components/TaskForm/TaskForm";
import { useTasksQuery } from "./hooks/useTasksQuery";
import { useTaskMutations } from "./hooks/useTaskMutations";
import { TaskFilter, type TaskStatusFilter } from "./components/TaskFilter/TaskFilter";
import { Button } from "../../ui/Button";
import { Callout } from "../../ui/Callout";
import styles from "./TasksPage.module.css";
import { ConfirmDialog } from "./components/ConfirmDialog/ConfirmDialog";

export function TasksPage() {
  const { status, error, tasks, reload } = useTasksQuery();
  const { isSaving, saveError, isDeleting, deleteError, create, update, remove } = useTaskMutations();

  const [editingTask, setEditingTask] = useState<TaskDto | null>(null);
  const [filter, setFilter] = useState<TaskStatusFilter>("all");

  const [pendingDelete, setPendingDelete] = useState<TaskDto | null>(null);

  const filteredTasks = useMemo(() => {
    if (filter === "all") return tasks;
    if (filter === "open") return tasks.filter((t) => !t.isCompleted);
    return tasks.filter((t) => t.isCompleted);
  }, [tasks, filter]);

  const isFormOpen = editingTask !== null;

  async function handleCreate(request: { title: string; description: string | null; dueDate: string | null }) {
    await create(request);
    setEditingTask(null);
    await reload();
  }

  async function handleUpdate(
    id: number,
    request: { title: string; description: string | null; isCompleted: boolean; dueDate: string | null }
  ) {
    await update(id, request);
    setEditingTask(null);
    await reload();
  }

  async function confirmDelete() {
    if (!pendingDelete) return;

    const deleted = await remove(pendingDelete.id);
    setPendingDelete(null);
    await reload();
  }

  return (
    <section className={styles.page}>
      <header className={styles.header}>
        <div className={styles.hgroup}>
          <h2 className={styles.h2}>Tasks</h2>
          <p className={styles.caption}>
            Create, filter, and manage tasks. Designed to stay simple but scalable.
          </p>

          <TaskFilter
            value={filter}
            onChange={setFilter}
            disabled={status === "loading" || status === "error"}
          />
        </div>

        <div className={styles.actions}>
          <Button type="button" variant="primary" onClick={() => setEditingTask({} as TaskDto)}>
            + New Task
          </Button>

          <Button type="button" onClick={() => void reload()} disabled={status === "loading"}>
            {status === "loading" ? "Loading..." : "Refresh"}
          </Button>
        </div>
      </header>

      {isFormOpen && (
        <TaskForm
          mode={editingTask && editingTask.id ? "edit" : "create"}
          initialTask={editingTask && editingTask.id ? editingTask : undefined}
          isSaving={isSaving}
          errorMessage={saveError}
          onCancel={() => setEditingTask(null)}
          onCreate={handleCreate}
          onUpdate={handleUpdate}
        />
      )}

      {deleteError && <Callout title="Delete failed">{deleteError}</Callout>}

      {status === "loading" && <Callout title="Loading tasks…">Please wait.</Callout>}

      {status === "error" && (
        <Callout
          title="Couldn’t load tasks"
          actions={<Button onClick={() => void reload()}>Retry</Button>}
        >
          {error ?? "Unknown error"}
        </Callout>
      )}

      {status === "success" && tasks.length === 0 && <Callout title="No tasks yet">Create your first task.</Callout>}

      {status === "success" && tasks.length > 0 && filteredTasks.length === 0 && (
        <Callout title="No matching tasks">Try another filter.</Callout>
      )}

      {status === "success" && filteredTasks.length > 0 && (
        <TaskList
          tasks={filteredTasks}
          onEdit={(t) => setEditingTask(t)}
          onDelete={(t) => setPendingDelete(t)}
          isDeleting={isDeleting}
        />
      )}

      <ConfirmDialog
        open={pendingDelete !== null}
        title="Delete task?"
        message={pendingDelete ? `This will permanently delete “${pendingDelete.title}”.` : ""}
        confirmText={isDeleting ? "Deleting..." : "Delete"}
        cancelText="Cancel"
        danger
        disableConfirm={isDeleting}
        onCancel={() => setPendingDelete(null)}
        onConfirm={() => void confirmDelete()}
      />
    </section>
  );
}
