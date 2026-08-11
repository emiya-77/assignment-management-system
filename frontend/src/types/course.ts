export interface Course {
  id: number;
  code: string;
  name: string;
  description: string | null;
  isActive: boolean;
  createdAt: string;
}

export interface CreateCourseRequest {
  code: string;
  name: string;
  description?: string | null;
}

export interface UpdateCourseRequest {
  code: string;
  name: string;
  description?: string | null;
}