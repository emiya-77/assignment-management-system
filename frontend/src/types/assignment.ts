export interface Assignment {
  id: number;

  code: string;
  title: string;
  description: string;

  deadline: string;
  maximumMarks: number;

  isPublished: boolean;
  allowSubmissionUpdate: boolean;

  teacherAssignmentId: number;

  teacherId: number;
  teacherName: string;

  courseId: number;
  courseCode: string;
  courseName: string;

  subjectId: number;
  subjectCode: string;
  subjectName: string;

  createdAt: string;
  updatedAt: string | null;
}

export interface CreateAssignmentRequest {
  code: string;
  title: string;
  description: string;

  deadline: string;
  maximumMarks: number;

  allowSubmissionUpdate: boolean;

  teacherAssignmentId: number;
}

export interface UpdateAssignmentRequest {
  code: string;
  title: string;
  description: string;

  deadline: string;
  maximumMarks: number;

  allowSubmissionUpdate: boolean;
}