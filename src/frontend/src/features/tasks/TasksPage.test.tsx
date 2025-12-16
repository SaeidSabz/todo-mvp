import { render, screen, within } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { TasksPage } from "./TasksPage";
import { getTasks } from "./api/tasksApi";
import type { TaskDto } from "./types/taskTypes";

// Mock the API boundary (recommended over mocking fetch everywhere)
vi.mock("./api/tasksApi", () => ({
  getTasks: vi.fn(),
}));

/**
 * Creates a deferred promise so we can keep the component in "loading" state
 * until we decide to resolve it.
 */
function createDeferred<T>() {
  let resolve!: (value: T) => void;
  let reject!: (reason?: unknown) => void;

  const promise = new Promise<T>((res, rej) => {
    resolve = res;
    reject = rej;
  });

  return { promise, resolve, reject };
}

describe("TasksPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("shows loading state initially (before tasks resolve)", async () => {
    const deferred = createDeferred<TaskDto[]>();

    vi.mocked(getTasks).mockReturnValueOnce(deferred.promise);

    render(<TasksPage />);

    // The effect runs after render; we expect the loading UI to appear quickly
    expect(await screen.findByText(/loading tasks/i)).toBeInTheDocument();

    // Finish the request so the component can settle
    deferred.resolve([]);

    // After resolving with empty list, we should see the empty state
    expect(await screen.findByText(/no tasks yet/i)).toBeInTheDocument();
  });

  it("renders tasks correctly when API returns data", async () => {
    const tasks: TaskDto[] = [
      {
        id: 1,
        title: "Buy groceries",
        description: "Eggs and bread",
        isCompleted: false,
        dueDate: null,
        createdAt: "2025-12-15T00:00:00Z",
        updatedAt: null,
      },
      {
        id: 2,
        title: "Pay bills",
        description: null,
        isCompleted: true,
        dueDate: "2025-12-20T12:00:00Z",
        createdAt: "2025-12-15T00:00:00Z",
        updatedAt: "2025-12-15T10:00:00Z",
      },
    ];

    vi.mocked(getTasks).mockResolvedValueOnce(tasks);

    render(<TasksPage />);

    // Wait for content to appear
    expect(await screen.findByText("Buy groceries")).toBeInTheDocument();
    expect(screen.getByText("Pay bills")).toBeInTheDocument();

    // Verify status badges (exact text depends on your TaskCard)
    expect(screen.getAllByText("Open").length).toBeGreaterThanOrEqual(1);
    expect(screen.getAllByText("Completed").length).toBeGreaterThanOrEqual(1);

    // Description renders only when present
    expect(screen.getByText("Eggs and bread")).toBeInTheDocument();
  });
});
