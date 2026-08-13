export interface Enrollment {
  studentId: number;
  studentName: string;

  courseId: number;
  courseCode: string;
  courseName: string;

  enrolledAt: string;
}

export interface CreateEnrollmentRequest {
  studentId: number;
  courseId: number;
}