export interface Task {
  id: number;
  employeeId: number;
  employeeName: string;
  department: string;
  title: string;
  description?: string | null;
  isDone: boolean;
  createdAt: string;
  completedAt?: string | null;
}

export interface CreateTaskRequest {
  employeeId: number;
  title: string;
  description?: string | null;
}
