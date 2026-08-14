export enum SubmissionStatus {
  Submitted = 0,
  UnderReview = 1,
  Graded = 2,
  Returned = 3,
}


export interface Submission {
  id: number;

  assignmentId: number;
  assignmentCode: string;
  assignmentTitle: string;

  teacherId: number;

  studentId: number;
  studentName: string;
  studentEmail: string;

  answer: string;

  status: SubmissionStatus;

  marks: number | null;
  feedback: string | null;

  submittedAt: string;
  updatedAt: string | null;
  gradedAt: string | null;
}


export interface CreateSubmissionRequest {
  answer: string;
}


export interface UpdateSubmissionRequest {
  answer: string;
}


export interface GradeSubmissionRequest {
  marks: number;
  feedback?: string;
}


export interface UpdateSubmissionStatusRequest {
  status: SubmissionStatus;
}