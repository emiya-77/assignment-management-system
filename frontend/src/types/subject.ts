export interface Subject {
  id: number;
  code: string;
  name: string;
  description: string | null;
  createdAt: string;
}

export interface CreateSubjectRequest {
  code: string;
  name: string;
  description?: string | null;
}

export interface UpdateSubjectRequest {
  code: string;
  name: string;
  description?: string | null;
}