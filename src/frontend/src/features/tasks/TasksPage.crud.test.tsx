import { render, screen, waitFor, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { TasksPage } from "./TasksPage";
import type { TaskDto } from "./types/taskTypes";

/* ===============================
   MOCK THE HOOKS (NOT API)
================================ */

vi.mock("./hooks/useTasksQuery", () => ({
  useTasksQuery: vi.fn(),
}));

vi.mock("./hooks/useTaskMutations", () => ({
  useTaskMutations: vi.fn(),
}));

import { useTasksQuery } from "./hooks/useTasksQuery";
import { useTaskMutations } from "./hooks/useTaskMutations";

/* ===============================
   HELPERS
================================ */

function makeTask(overrides?: Partial<TaskDto>): TaskDto {
  return {
    id: 1,
    title: "Task 1",
    description: null,
    isCompleted: false,
    dueDate: null,
    createdAt: "2025-12-15T00:00:00Z",
    updatedAt: null,
    ...overrides,
  };
}

describe("TasksPage CRUD", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("creates a task and reloads the list", async () => {
    const user = userEvent.setup();

    const reload = vi.fn().mockResolvedValue(undefined);
    const create = vi.fn().mockResolvedValue(undefined);

    (useTasksQuery as any).mockReturnValue({
      status: "success",
      error: null,
      tasks: [],
      reload,
    });

    (useTaskMutations as any).mockReturnValue({
      isSaving: false,
      saveError: null,
      isDeleting: false,
      deleteError: null,
      create,
      update: vi.fn(),
      remove: vi.fn(),
    });

    render(<TasksPage />);

    expect(screen.getByText(/no tasks yet/i)).toBeInTheDocument();

    await user.click(screen.getByRole("button", { name: /\+ new task/i }));

    const titleInput = screen.getByLabelText(/^title/i);
    await user.type(titleInput, "New Task");

    await user.click(screen.getByRole("button", { name: /create task/i }));

    await waitFor(() => {
      expect(create).toHaveBeenCalledTimes(1);
      expect(reload).toHaveBeenCalledTimes(1);
    });
  });

  it("edits a task and reloads the list", async () => {
    const user = userEvent.setup();

    const existing = makeTask({ id: 1, title: "Original Title" });
    const reload = vi.fn().mockResolvedValue(undefined);
    const update = vi.fn().mockResolvedValue(undefined);

    (useTasksQuery as any).mockReturnValue({
      status: "success",
      error: null,
      tasks: [existing],
      reload,
    });

    (useTaskMutations as any).mockReturnValue({
      isSaving: false,
      saveError: null,
      isDeleting: false,
      deleteError: null,
      create: vi.fn(),
      update,
      remove: vi.fn(),
    });

    render(<TasksPage />);

    const cardTitle = screen.getByText("Original Title");
    const card = cardTitle.closest("article");
    expect(card).not.toBeNull();

    await user.click(within(card!).getByRole("button", { name: /edit/i }));

    const titleInput = screen.getByLabelText(/^title/i);
    await user.clear(titleInput);
    await user.type(titleInput, "Updated Title");

    await user.click(screen.getByRole("button", { name: /save changes/i }));

    await waitFor(() => {
      expect(update).toHaveBeenCalledTimes(1);
      expect(update).toHaveBeenCalledWith(
        1,
        expect.objectContaining({ title: "Updated Title" })
      );
      expect(reload).toHaveBeenCalledTimes(1);
    });
  });

  it("deletes a task (confirmed) and reloads the list", async () => {
    const user = userEvent.setup();

    const existing = makeTask({ id: 7, title: "Task To Delete" });
    const reload = vi.fn().mockResolvedValue(undefined);
    const remove = vi.fn().mockResolvedValue(true);

    (useTasksQuery as any).mockReturnValue({
      status: "success",
      error: null,
      tasks: [existing],
      reload,
    });

    (useTaskMutations as any).mockReturnValue({
      isSaving: false,
      saveError: null,
      isDeleting: false,
      deleteError: null,
      create: vi.fn(),
      update: vi.fn(),
      remove,
    });

    render(<TasksPage />);

    const cardTitle = screen.getByText("Task To Delete");
    const card = cardTitle.closest("article");
    expect(card).not.toBeNull();

    await user.click(within(card!).getByRole("button", { name: /delete/i }));

    // ConfirmDialog confirm button
    const dialog = screen.getByRole("dialog", { name: /delete task\?/i });
    await user.click(within(dialog).getByRole("button", { name: /^delete$/i }));

    await waitFor(() => {
      expect(remove).toHaveBeenCalledTimes(1);
      expect(remove).toHaveBeenCalledWith(7);
      expect(reload).toHaveBeenCalledTimes(1);
    });
  });
});
