export type UserRole = "Admin" | "Teacher" | "Student";

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  userId: number;
  firstName: string;
  lastName: string;
  email: string;
  role: UserRole;
}