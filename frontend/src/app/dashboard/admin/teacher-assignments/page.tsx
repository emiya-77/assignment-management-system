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
  CreateTeacherAssignmentRequest,
  TeacherAssignment,
} from "@/types/teacher-assignment";

import { User } from "@/types/user";
import { Course } from "@/types/course";
import { Subject } from "@/types/subject";

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
  teacherId: "",
  courseId: "",
  subjectId: "",
};


export default function TeacherAssignmentsPage() {
  const [teacherAssignments, setTeacherAssignments] =
    useState<TeacherAssignment[]>([]);

  const [teachers, setTeachers] =
    useState<User[]>([]);

  const [courses, setCourses] =
    useState<Course[]>([]);

  const [subjects, setSubjects] =
    useState<Subject[]>([]);

  const [isLoading, setIsLoading] =
    useState(true);

  const [error, setError] =
    useState("");

  const [isCreateOpen, setIsCreateOpen] =
    useState(false);

  const [deletingAssignment, setDeletingAssignment] =
    useState<TeacherAssignment | null>(null);

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
        assignmentsResponse,
        usersResponse,
        coursesResponse,
        subjectsResponse,
      ] = await Promise.all([
        api<TeacherAssignment[]>(
          "/TeacherAssignments"
        ),
        api<User[]>("/Users"),
        api<Course[]>("/Courses"),
        api<Subject[]>("/Subjects"),
      ]);

      setTeacherAssignments(
        assignmentsResponse
      );

      setTeachers(
        usersResponse.filter(
          (user) => user.role === "Teacher"
        )
      );

      setCourses(coursesResponse);
      setSubjects(subjectsResponse);

    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Failed to load teacher assignments."
      );
    } finally {
      setIsLoading(false);
    }
  }


  function openCreateDialog() {
    setForm(initialForm);
    setIsCreateOpen(true);
  }


  async function handleCreate(
    event: FormEvent<HTMLFormElement>
  ) {
    event.preventDefault();

    try {
      setError("");
      setIsSubmitting(true);

      const request: CreateTeacherAssignmentRequest = {
        teacherId: Number(form.teacherId),
        courseId: Number(form.courseId),
        subjectId: Number(form.subjectId),
      };

      await api<TeacherAssignment>(
        "/TeacherAssignments",
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
          : "Failed to create teacher assignment."
      );
    } finally {
      setIsSubmitting(false);
    }
  }


  async function handleDelete() {
    if (!deletingAssignment) {
      return;
    }

    try {
      setError("");
      setIsSubmitting(true);

      await api<void>(
        `/TeacherAssignments/${deletingAssignment.id}`,
        {
          method: "DELETE",
        }
      );

      setDeletingAssignment(null);

      await loadData();

    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Failed to delete teacher assignment."
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
            Teacher Assignments
          </h1>

          <p className="mt-1 text-muted-foreground">
            Assign teachers to courses and subjects.
          </p>
        </div>

        <Button onClick={openCreateDialog}>
          <Plus />
          Assign Teacher
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
              Loading teacher assignments...
            </div>

          ) : (

            <Table>

              <TableHeader>
                <TableRow>
                  <TableHead>Teacher</TableHead>
                  <TableHead>Course</TableHead>
                  <TableHead>Subject</TableHead>
                  <TableHead>Assigned</TableHead>

                  <TableHead className="text-right">
                    Actions
                  </TableHead>
                </TableRow>
              </TableHeader>


              <TableBody>

                {teacherAssignments.length === 0 ? (

                  <TableRow>
                    <TableCell
                      colSpan={5}
                      className="h-32 text-center text-muted-foreground"
                    >
                      No teacher assignments found.
                    </TableCell>
                  </TableRow>

                ) : (

                  teacherAssignments.map(
                    (assignment) => (

                      <TableRow
                        key={assignment.id}
                      >

                        <TableCell className="font-medium">
                          {assignment.teacherName}
                        </TableCell>


                        <TableCell>
                          <div>
                            <p className="font-medium">
                              {assignment.courseCode}
                            </p>

                            <p className="text-sm text-muted-foreground">
                              {assignment.courseName}
                            </p>
                          </div>
                        </TableCell>


                        <TableCell>
                          <div>
                            <p className="font-medium">
                              {assignment.subjectCode}
                            </p>

                            <p className="text-sm text-muted-foreground">
                              {assignment.subjectName}
                            </p>
                          </div>
                        </TableCell>


                        <TableCell>
                          {new Date(
                            assignment.assignedAt
                          ).toLocaleDateString()}
                        </TableCell>


                        <TableCell className="text-right">

                          <Button
                            variant="ghost"
                            size="icon"
                            onClick={() =>
                              setDeletingAssignment(
                                assignment
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


      {/* Create Dialog */}

      <Dialog
        open={isCreateOpen}
        onOpenChange={setIsCreateOpen}
      >

        <DialogContent>

          <DialogHeader>

            <DialogTitle>
              Assign Teacher
            </DialogTitle>

            <DialogDescription>
              Select a teacher, course, and subject.
            </DialogDescription>

          </DialogHeader>


          <form
            onSubmit={handleCreate}
            className="space-y-4"
          >

            <div className="space-y-2">

              <Label>Teacher</Label>

              <Select
                value={form.teacherId}
                onValueChange={(value) =>
                  setForm((current) => ({
                    ...current,
                    teacherId: value,
                  }))
                }
              >

                <SelectTrigger>
                  <SelectValue placeholder="Select a teacher" />
                </SelectTrigger>

                <SelectContent>

                  {teachers.map((teacher) => (

                    <SelectItem
                      key={teacher.id}
                      value={String(teacher.id)}
                    >
                      {teacher.firstName}{" "}
                      {teacher.lastName}
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


            <div className="space-y-2">

              <Label>Subject</Label>

              <Select
                value={form.subjectId}
                onValueChange={(value) =>
                  setForm((current) => ({
                    ...current,
                    subjectId: value,
                  }))
                }
              >

                <SelectTrigger>
                  <SelectValue placeholder="Select a subject" />
                </SelectTrigger>

                <SelectContent>

                  {subjects.map((subject) => (

                    <SelectItem
                      key={subject.id}
                      value={String(subject.id)}
                    >
                      {subject.code} - {subject.name}
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
                !form.teacherId ||
                !form.courseId ||
                !form.subjectId
              }
            >
              {isSubmitting
                ? "Assigning..."
                : "Assign Teacher"}
            </Button>

          </form>

        </DialogContent>

      </Dialog>


      {/* Delete Confirmation */}

      <Dialog
        open={!!deletingAssignment}
        onOpenChange={(open) => {
          if (!open) {
            setDeletingAssignment(null);
          }
        }}
      >

        <DialogContent>

          <DialogHeader>

            <DialogTitle>
              Remove Teacher Assignment
            </DialogTitle>

            <DialogDescription>
              Are you sure you want to remove{" "}
              <span className="font-medium">
                {deletingAssignment?.teacherName}
              </span>
              {" "}from this course and subject?
              This action cannot be undone.
            </DialogDescription>

          </DialogHeader>


          <DialogFooter>

            <Button
              variant="outline"
              onClick={() =>
                setDeletingAssignment(null)
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
                : "Remove Assignment"}
            </Button>

          </DialogFooter>

        </DialogContent>

      </Dialog>

    </div>
  );
}