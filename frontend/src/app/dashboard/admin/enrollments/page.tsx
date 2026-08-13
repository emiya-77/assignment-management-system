"use client";

import {
  FormEvent,
  useEffect,
  useState,
} from "react";

import {
  Plus,
  Trash2,
} from "lucide-react";

import { api } from "@/lib/api";

import {
  CreateEnrollmentRequest,
  Enrollment,
} from "@/types/enrollment";

import {
  User,
  UserRole,
} from "@/types/user";

import { Course } from "@/types/course";

import { Button } from "@/components/ui/button";

import {
  Card,
  CardContent,
} from "@/components/ui/card";

import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";

import { Label } from "@/components/ui/label";

import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";


const initialForm = {
  studentId: "",
  courseId: "",
};


export default function EnrollmentsPage() {
  const [enrollments, setEnrollments] =
    useState<Enrollment[]>([]);

  const [students, setStudents] =
    useState<User[]>([]);

  const [courses, setCourses] =
    useState<Course[]>([]);

  const [isLoading, setIsLoading] =
    useState(true);

  const [error, setError] =
    useState("");

  const [isCreateOpen, setIsCreateOpen] =
    useState(false);

  const [deletingEnrollment, setDeletingEnrollment] =
    useState<Enrollment | null>(null);

  const [form, setForm] =
    useState(initialForm);

  const [isSubmitting, setIsSubmitting] =
    useState(false);


  useEffect(() => {
    loadData();
  }, []);


  async function loadData() {
    try {
      setError("");
      setIsLoading(true);

      const [
        enrollmentsResponse,
        usersResponse,
        coursesResponse,
      ] = await Promise.all([
        api<Enrollment[]>("/Enrollments"),
        api<User[]>("/Users"),
        api<Course[]>("/Courses"),
      ]);

      setEnrollments(enrollmentsResponse);

      setStudents(
        usersResponse.filter(
          (user) =>
            user.role === UserRole.Student
        )
      );

      setCourses(coursesResponse);

    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Failed to load enrollments."
      );
    } finally {
      setIsLoading(false);
    }
  }


  function openCreateDialog() {
    setForm(initialForm);
    setError("");
    setIsCreateOpen(true);
  }


  async function handleCreate(
    event: FormEvent<HTMLFormElement>
  ) {
    event.preventDefault();

    try {
      setError("");
      setIsSubmitting(true);

      const request: CreateEnrollmentRequest = {
        studentId: Number(form.studentId),
        courseId: Number(form.courseId),
      };

      await api<Enrollment>(
        "/Enrollments",
        {
          method: "POST",
          body: JSON.stringify(request),
        }
      );

      setIsCreateOpen(false);

      await loadData();

    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Failed to enroll student."
      );
    } finally {
      setIsSubmitting(false);
    }
  }


  async function handleDelete() {
    if (!deletingEnrollment) {
      return;
    }

    try {
      setError("");
      setIsSubmitting(true);

      await api<void>(
        `/Enrollments/${deletingEnrollment.studentId}/${deletingEnrollment.courseId}`,
        {
          method: "DELETE",
        }
      );

      setDeletingEnrollment(null);

      await loadData();

    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Failed to remove enrollment."
      );
    } finally {
      setIsSubmitting(false);
    }
  }


  return (
    <div className="space-y-6">

      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">

        <div>
          <h1 className="text-3xl font-bold tracking-tight">
            Student Enrollments
          </h1>

          <p className="mt-1 text-muted-foreground">
            Enroll students in courses.
          </p>
        </div>

        <Button onClick={openCreateDialog}>
          <Plus />
          Enroll Student
        </Button>

      </div>


      {error && (
        <div className="rounded-lg border border-destructive/50 bg-destructive/10 p-4 text-sm text-destructive">
          {error}
        </div>
      )}


      <Card>
        <CardContent className="p-0">

          {isLoading ? (

            <div className="flex min-h-64 items-center justify-center text-sm text-muted-foreground">
              Loading enrollments...
            </div>

          ) : (

            <Table>

              <TableHeader>
                <TableRow>
                  <TableHead>Student</TableHead>
                  <TableHead>Course</TableHead>
                  <TableHead>Enrolled</TableHead>

                  <TableHead className="text-right">
                    Actions
                  </TableHead>
                </TableRow>
              </TableHeader>


              <TableBody>

                {enrollments.length === 0 ? (

                  <TableRow>
                    <TableCell
                      colSpan={4}
                      className="h-32 text-center text-muted-foreground"
                    >
                      No enrollments found.
                    </TableCell>
                  </TableRow>

                ) : (

                  enrollments.map(
                    (enrollment) => (

                      <TableRow
                        key={`${enrollment.studentId}-${enrollment.courseId}`}
                      >

                        <TableCell className="font-medium">
                          {enrollment.studentName}
                        </TableCell>


                        <TableCell>
                          <div>
                            <p className="font-medium">
                              {enrollment.courseCode}
                            </p>

                            <p className="text-sm text-muted-foreground">
                              {enrollment.courseName}
                            </p>
                          </div>
                        </TableCell>


                        <TableCell>
                          {new Date(
                            enrollment.enrolledAt
                          ).toLocaleDateString()}
                        </TableCell>


                        <TableCell className="text-right">

                          <Button
                            variant="ghost"
                            size="icon"
                            onClick={() =>
                              setDeletingEnrollment(
                                enrollment
                              )
                            }
                          >
                            <Trash2 />
                          </Button>

                        </TableCell>

                      </TableRow>

                    )
                  )

                )}

              </TableBody>

            </Table>

          )}

        </CardContent>
      </Card>


      {/* Create Enrollment Dialog */}

      <Dialog
        open={isCreateOpen}
        onOpenChange={(open) => {
          setIsCreateOpen(open);

          if (!open) {
            setForm(initialForm);
          }
        }}
      >

        <DialogContent>

          <DialogHeader>

            <DialogTitle>
              Enroll Student
            </DialogTitle>

            <DialogDescription>
              Select a student and the course they
              should be enrolled in.
            </DialogDescription>

          </DialogHeader>


          <form
            onSubmit={handleCreate}
            className="space-y-4"
          >

            <div className="space-y-2">

              <Label>Student</Label>

              <Select
                value={form.studentId}
                onValueChange={(value) =>
                  setForm((current) => ({
                    ...current,
                    studentId: value,
                  }))
                }
              >

                <SelectTrigger>
                  <SelectValue placeholder="Select a student" />
                </SelectTrigger>

                <SelectContent>

                  {students.map((student) => (

                    <SelectItem
                      key={student.id}
                      value={String(student.id)}
                    >
                      {student.firstName}{" "}
                      {student.lastName}
                    </SelectItem>

                  ))}

                </SelectContent>

              </Select>

            </div>


            <div className="space-y-2">

              <Label>Course</Label>

              <Select
                value={form.courseId}
                onValueChange={(value) =>
                  setForm((current) => ({
                    ...current,
                    courseId: value,
                  }))
                }
              >

                <SelectTrigger>
                  <SelectValue placeholder="Select a course" />
                </SelectTrigger>

                <SelectContent>

                  {courses.map((course) => (

                    <SelectItem
                      key={course.id}
                      value={String(course.id)}
                    >
                      {course.code} - {course.name}
                    </SelectItem>

                  ))}

                </SelectContent>

              </Select>

            </div>


            <Button
              type="submit"
              className="w-full"
              disabled={
                isSubmitting ||
                !form.studentId ||
                !form.courseId
              }
            >
              {isSubmitting
                ? "Enrolling..."
                : "Enroll Student"}
            </Button>

          </form>

        </DialogContent>

      </Dialog>


      {/* Delete Confirmation */}

      <Dialog
        open={!!deletingEnrollment}
        onOpenChange={(open) => {
          if (!open) {
            setDeletingEnrollment(null);
          }
        }}
      >

        <DialogContent>

          <DialogHeader>

            <DialogTitle>
              Remove Enrollment
            </DialogTitle>

            <DialogDescription>
              Are you sure you want to remove{" "}

              <span className="font-medium">
                {deletingEnrollment?.studentName}
              </span>

              {" "}from{" "}

              <span className="font-medium">
                {deletingEnrollment?.courseName}
              </span>

              ?
            </DialogDescription>

          </DialogHeader>


          <DialogFooter>

            <Button
              variant="outline"
              onClick={() =>
                setDeletingEnrollment(null)
              }
              disabled={isSubmitting}
            >
              Cancel
            </Button>


            <Button
              variant="destructive"
              onClick={handleDelete}
              disabled={isSubmitting}
            >
              {isSubmitting
                ? "Removing..."
                : "Remove Enrollment"}
            </Button>

          </DialogFooter>

        </DialogContent>

      </Dialog>

    </div>
  );
}