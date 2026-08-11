export type UserRole =
  | "Admin"
  | "Teacher"
  | "Student";

export interface User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: UserRole;
  isActive: boolean;
  createdAt: string;
}

export interface CreateUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  role: UserRole;
}

export interface UpdateUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  role: UserRole;
}

export interface UpdateUserStatusRequest {
  isActive: boolean;
}