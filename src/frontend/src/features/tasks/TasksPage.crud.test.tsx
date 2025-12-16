import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { TasksPage } from "./TasksPage";
import {
  getTasks,
  createTask,
  updateTask,
  deleteTask,
} from "./api/tasksApi";
import type { TaskDto } from "./types/taskTypes";

vi.mock("./api/tasksApi", () => ({
  getTasks: vi.fn(),
  createTask: vi.fn(),
  updateTask: vi.fn(),
  deleteTask: vi.fn(),
}));

/**
 * Creates a sample task DTO.
 */
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

    // Initial load returns empty
    vi.mocked(getTasks).mockResolvedValueOnce([]);

    // Create returns a created task
    vi.mocked(createTask).mockResolvedValueOnce(
      makeTask({ id: 10, title: "New Task" })
    );

    // Reload after create returns one task
    vi.mocked(getTasks).mockResolvedValueOnce([
      makeTask({ id: 10, title: "New Task" }),
    ]);

    render(<TasksPage />);

    // Wait for initial empty state
    expect(await screen.findByText(/no tasks yet/i)).toBeInTheDocument();

    // Open create form
    await user.click(screen.getByRole("button", { name: /\+ new task/i }));

    // Fill Title
    const titleInput = screen.getByLabelText(/title/i);
    await user.clear(titleInput);
    await user.type(titleInput, "New Task");

    // Submit
    await user.click(screen.getByRole("button", { name: /^create$/i }));

    // Verify API calls
    await waitFor(() => {
      expect(createTask).toHaveBeenCalledTimes(1);
      expect(getTasks).toHaveBeenCalledTimes(2); // initial + reload
    });

    // Verify new task appears
    expect(await screen.findByText("New Task")).toBeInTheDocument();
  });

  it("edits a task and reloads the list", async () => {
    const user = userEvent.setup();

    const existing = makeTask({ id: 1, title: "Original Title" });
    const updated = makeTask({ id: 1, title: "Updated Title" });

    // Initial load shows one task
    vi.mocked(getTasks).mockResolvedValueOnce([existing]);

    // Update returns 204 or body; your api wrapper returns TaskDto | null
    vi.mocked(updateTask).mockResolvedValueOnce(null);

    // Reload shows updated task
    vi.mocked(getTasks).mockResolvedValueOnce([updated]);

    render(<TasksPage />);

    // Wait for task to render
    expect(await screen.findByText("Original Title")).toBeInTheDocument();

    // Click Edit on the card
    await user.click(screen.getByRole("button", { name: /edit/i }));

    // Update Title in form
    const titleInput = screen.getByLabelText(/title/i);
    await user.clear(titleInput);
    await user.type(titleInput, "Updated Title");

    // Save
    await user.click(screen.getByRole("button", { name: /^save$/i }));

    await waitFor(() => {
      expect(updateTask).toHaveBeenCalledTimes(1);
      expect(getTasks).toHaveBeenCalledTimes(2); // initial + reload
    });

    expect(await screen.findByText("Updated Title")).toBeInTheDocument();
  });

  it("deletes a task (confirmed) and reloads the list", async () => {
    const user = userEvent.setup();

    const task = makeTask({ id: 7, title: "To Delete" });

    vi.mocked(getTasks).mockResolvedValueOnce([task]);
    vi.mocked(deleteTask).mockResolvedValueOnce(true);
    vi.mocked(getTasks).mockResolvedValueOnce([]); // after delete reload

    // Confirm dialog: user confirms deletion
    vi.stubGlobal("confirm", vi.fn(() => true));

    render(<TasksPage />);

    expect(await screen.findByText("To Delete")).toBeInTheDocument();

    // Click Delete on the card
    await user.click(screen.getByRole("button", { name: /delete/i }));

    await waitFor(() => {
      expect(deleteTask).toHaveBeenCalledTimes(1);
      expect(deleteTask).toHaveBeenCalledWith(7);
      expect(getTasks).toHaveBeenCalledTimes(2); // initial + reload
    });

    expect(await screen.findByText(/no tasks yet/i)).toBeInTheDocument();

    vi.unstubAllGlobals();
  });
});
