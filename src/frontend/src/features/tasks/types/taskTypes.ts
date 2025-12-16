export type TaskDto = {
  id: number;
  title: string;
  description: string | null;
  isCompleted: boolean;
  dueDate: string | null;
  createdAt: string;
  updatedAt: string | null;
};

export type CreateTaskRequest = {
  title: string;
  description: string | null;
  dueDate: string | null;
};

export type UpdateTaskRequest = {
  title: string;
  description: string | null;
  isCompleted: boolean;
  dueDate: string | null;
};