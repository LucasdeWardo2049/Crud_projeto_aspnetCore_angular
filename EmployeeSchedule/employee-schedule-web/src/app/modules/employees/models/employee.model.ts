export interface Employee {
  id: number;
  name: string;
  registration: string;
  department: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreateEmployeeRequest {
  name: string;
  registration: string;
  department: string;
  isActive: boolean;
}
