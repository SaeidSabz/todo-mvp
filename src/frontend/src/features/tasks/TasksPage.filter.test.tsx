import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { TasksPage } from "./TasksPage";
import { getTasks } from "./api/tasksApi";
import type { TaskDto } from "./types/taskTypes";

vi.mock("./api/tasksApi", () => ({
  getTasks: vi.fn(),
  createTask: vi.fn(),
  updateTask: vi.fn(),
  deleteTask: vi.fn(),
}));

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

describe("TasksPage filter", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("shows both open and completed tasks when filter is All", async () => {
    const tasks: TaskDto[] = [
      makeTask({ id: 1, title: "Open Task", isCompleted: false }),
      makeTask({ id: 2, title: "Completed Task", isCompleted: true }),
    ];

    vi.mocked(getTasks).mockResolvedValueOnce(tasks);

    render(<TasksPage />);

    expect(await screen.findByText("Open Task")).toBeInTheDocument();
    expect(screen.getByText("Completed Task")).toBeInTheDocument();
  });

  it("filters to Completed tasks when dropdown is changed", async () => {
    const user = userEvent.setup();

    const tasks: TaskDto[] = [
      makeTask({ id: 1, title: "Open Task", isCompleted: false }),
      makeTask({ id: 2, title: "Completed Task", isCompleted: true }),
    ];

    vi.mocked(getTasks).mockResolvedValueOnce(tasks);

    render(<TasksPage />);

    expect(await screen.findByText("Open Task")).toBeInTheDocument();
    expect(screen.getByText("Completed Task")).toBeInTheDocument();

    const filterSelect = screen.getByLabelText("Task status filter");
    await user.selectOptions(filterSelect, "completed");

    expect(screen.queryByText("Open Task")).not.toBeInTheDocument();
    expect(screen.getByText("Completed Task")).toBeInTheDocument();
  });
});