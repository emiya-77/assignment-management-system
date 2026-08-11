"use client";

import { FormEvent, useEffect, useState } from "react";
import { Pencil, Plus, Trash2 } from "lucide-react";

import { api } from "@/lib/api";

import {
  Course,
  CreateCourseRequest,
  UpdateCourseRequest,
} from "@/types/course";

import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
} from "@/components/ui/card";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Badge } from "@/components/ui/badge";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

const initialForm = {
  code: "",
  name: "",
  description: "",
};

export default function CoursesPage() {
  const [courses, setCourses] = useState<Course[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [editingCourse, setEditingCourse] = useState<Course | null>(null);

  const [form, setForm] = useState(initialForm);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [deletingId, setDeletingId] = useState<number | null>(null);

  useEffect(() => {
    loadCourses();
  }, []);

  async function loadCourses() {
    try {
      setError("");
      setIsLoading(true);

      const response = await api<Course[]>("/Courses");

      setCourses(response);
    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Failed to load courses."
      );
    } finally {
      setIsLoading(false);
    }
  }

  function openCreateDialog() {
    setForm(initialForm);
    setEditingCourse(null);
    setIsCreateOpen(true);
  }

  function openEditDialog(course: Course) {
    setForm({
      code: course.code,
      name: course.name,
      description: course.description ?? "",
    });

    setEditingCourse(course);
  }

  async function handleCreate(
    event: FormEvent<HTMLFormElement>
  ) {
    event.preventDefault();

    try {
      setIsSubmitting(true);
      setError("");

      const request: CreateCourseRequest = {
        code: form.code,
        name: form.name,
        description: form.description || null,
      };

      await api<Course>("/Courses", {
        method: "POST",
        body: JSON.stringify(request),
      });

      setIsCreateOpen(false);
      await loadCourses();
    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Failed to create course."
      );
    } finally {
      setIsSubmitting(false);
    }
  }

  async function handleUpdate(
    event: FormEvent<HTMLFormElement>
  ) {
    event.preventDefault();

    if (!editingCourse) {
      return;
    }

    try {
      setIsSubmitting(true);
      setError("");

      const request: UpdateCourseRequest = {
        code: form.code,
        name: form.name,
        description: form.description || null,
      };

      await api<Course>(
        `/Courses/${editingCourse.id}`,
        {
          method: "PUT",
          body: JSON.stringify(request),
        }
      );

      setEditingCourse(null);
      await loadCourses();
    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Failed to update course."
      );
    } finally {
      setIsSubmitting(false);
    }
  }

  async function handleDelete(course: Course) {
    const confirmed = window.confirm(
      `Are you sure you want to delete "${course.name}"?`
    );

    if (!confirmed) {
      return;
    }

    try {
      setDeletingId(course.id);
      setError("");

      await api<null>(
        `/Courses/${course.id}`,
        {
          method: "DELETE",
        }
      );

      await loadCourses();
    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Failed to delete course."
      );
    } finally {
      setDeletingId(null);
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">
            Courses
          </h1>

          <p className="mt-1 text-muted-foreground">
            Create and manage courses for your institution.
          </p>
        </div>

        <Button onClick={openCreateDialog}>
          <Plus />
          Add Course
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
              Loading courses...
            </div>
          ) : (
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Code</TableHead>
                  <TableHead>Course</TableHead>
                  <TableHead>Description</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Created</TableHead>
                  <TableHead className="text-right">
                    Actions
                  </TableHead>
                </TableRow>
              </TableHeader>

              <TableBody>
                {courses.length === 0 ? (
                  <TableRow>
                    <TableCell
                      colSpan={6}
                      className="h-32 text-center text-muted-foreground"
                    >
                      No courses found.
                    </TableCell>
                  </TableRow>
                ) : (
                  courses.map((course) => (
                    <TableRow key={course.id}>
                      <TableCell>
                        <Badge variant="outline">
                          {course.code}
                        </Badge>
                      </TableCell>

                      <TableCell>
                        <p className="font-medium">
                          {course.name}
                        </p>
                      </TableCell>

                      <TableCell>
                        <p className="max-w-md truncate text-sm text-muted-foreground">
                          {course.description || "—"}
                        </p>
                      </TableCell>

                      <TableCell>
                        <Badge
                          variant={
                            course.isActive
                              ? "default"
                              : "secondary"
                          }
                        >
                          {course.isActive
                            ? "Active"
                            : "Inactive"}
                        </Badge>
                      </TableCell>

                      <TableCell>
                        {new Date(
                          course.createdAt
                        ).toLocaleDateString()}
                      </TableCell>

                      <TableCell className="text-right">
                        <div className="flex justify-end gap-1">
                          <Button
                            variant="ghost"
                            size="icon"
                            onClick={() =>
                              openEditDialog(course)
                            }
                          >
                            <Pencil />
                          </Button>

                          <Button
                            variant="ghost"
                            size="icon"
                            className="text-destructive hover:text-destructive"
                            disabled={
                              deletingId === course.id
                            }
                            onClick={() =>
                              handleDelete(course)
                            }
                          >
                            <Trash2 />
                          </Button>
                        </div>
                      </TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          )}
        </CardContent>
      </Card>

      <Dialog
        open={isCreateOpen}
        onOpenChange={setIsCreateOpen}
      >
        <DialogContent>
          <DialogHeader>
            <DialogTitle>
              Add Course
            </DialogTitle>

            <DialogDescription>
              Create a new course.
            </DialogDescription>
          </DialogHeader>

          <CourseForm
            form={form}
            setForm={setForm}
            onSubmit={handleCreate}
            isSubmitting={isSubmitting}
            submitLabel="Create Course"
          />
        </DialogContent>
      </Dialog>

      <Dialog
        open={!!editingCourse}
        onOpenChange={(open) => {
          if (!open) {
            setEditingCourse(null);
          }
        }}
      >
        <DialogContent>
          <DialogHeader>
            <DialogTitle>
              Edit Course
            </DialogTitle>

            <DialogDescription>
              Update this course's information.
            </DialogDescription>
          </DialogHeader>

          <CourseForm
            form={form}
            setForm={setForm}
            onSubmit={handleUpdate}
            isSubmitting={isSubmitting}
            submitLabel="Save Changes"
          />
        </DialogContent>
      </Dialog>
    </div>
  );
}

interface CourseFormProps {
  form: typeof initialForm;

  setForm: React.Dispatch<
    React.SetStateAction<typeof initialForm>
  >;

  onSubmit: (
    event: FormEvent<HTMLFormElement>
  ) => void;

  isSubmitting: boolean;
  submitLabel: string;
}

function CourseForm({
  form,
  setForm,
  onSubmit,
  isSubmitting,
  submitLabel,
}: CourseFormProps) {
  return (
    <form
      onSubmit={onSubmit}
      className="space-y-4"
    >
      <div className="space-y-2">
        <Label htmlFor="course-code">
          Course Code
        </Label>

        <Input
          id="course-code"
          placeholder="e.g. CSE101"
          maxLength={50}
          value={form.code}
          onChange={(event) =>
            setForm((current) => ({
              ...current,
              code: event.target.value,
            }))
          }
          required
        />
      </div>

      <div className="space-y-2">
        <Label htmlFor="course-name">
          Course Name
        </Label>

        <Input
          id="course-name"
          placeholder="e.g. Introduction to Programming"
          maxLength={200}
          value={form.name}
          onChange={(event) =>
            setForm((current) => ({
              ...current,
              name: event.target.value,
            }))
          }
          required
        />
      </div>

      <div className="space-y-2">
        <Label htmlFor="course-description">
          Description
        </Label>

        <Textarea
          id="course-description"
          placeholder="Describe the course..."
          maxLength={1000}
          value={form.description}
          onChange={(event) =>
            setForm((current) => ({
              ...current,
              description: event.target.value,
            }))
          }
        />
      </div>

      <Button
        type="submit"
        className="w-full"
        disabled={isSubmitting}
      >
        {isSubmitting
          ? "Saving..."
          : submitLabel}
      </Button>
    </form>
  );
}