export interface TeacherAssignment {
  id: number;

  teacherId: number;
  teacherName: string;

  courseId: number;
  courseCode: string;
  courseName: string;

  subjectId: number;
  subjectCode: string;
  subjectName: string;

  assignedAt: string;
}

export interface CreateTeacherAssignmentRequest {
  teacherId: number;
  courseId: number;
  subjectId: number;
}