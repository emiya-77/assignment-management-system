"use client";

import {
  FormEvent,
  useEffect,
  useState,
} from "react";

import {
  Pencil,
  Plus,
  Send,
  Trash2,
} from "lucide-react";

import { api } from "@/lib/api";

import {
  Assignment,
  CreateAssignmentRequest,
  UpdateAssignmentRequest,
} from "@/types/assignment";

import {
  TeacherAssignment,
} from "@/types/teacher-assignment";

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

import { Input } from "@/components/ui/input";

import { Label } from "@/components/ui/label";

import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import { Switch } from "@/components/ui/switch";

import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

import { Textarea } from "@/components/ui/textarea";

import { Badge } from "@/components/ui/badge";


const initialForm = {
  code: "",
  title: "",
  description: "",
  deadline: "",
  maximumMarks: "",
  allowSubmissionUpdate: true,
  teacherAssignmentId: "",
};


export default function TeacherAssignmentsPage() {
  const [assignments, setAssignments] =
    useState<Assignment[]>([]);

  const [teacherAssignments, setTeacherAssignments] =
    useState<TeacherAssignment[]>([]);

  const [isLoading, setIsLoading] =
    useState(true);

  const [error, setError] =
    useState("");

  const [isCreateOpen, setIsCreateOpen] =
    useState(false);

  const [editingAssignment, setEditingAssignment] =
    useState<Assignment | null>(null);

  const [deletingAssignment, setDeletingAssignment] =
    useState<Assignment | null>(null);

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
        teacherAssignmentsResponse,
        ] = await Promise.all([
        api<Assignment[]>("/Assignments"),

        api<TeacherAssignment[]>(
            "/TeacherAssignments/my-assignments"
        ),
        ]);

        setAssignments(assignmentsResponse);

        setTeacherAssignments(
        teacherAssignmentsResponse
        );

    } catch (error) {
        setError(
        error instanceof Error
            ? error.message
            : "Failed to load assignments."
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


  function openEditDialog(
    assignment: Assignment
  ) {
    setForm({
      code: assignment.code,
      title: assignment.title,
      description: assignment.description,

      deadline: formatDateTimeLocal(
        assignment.deadline
      ),

      maximumMarks:
        String(assignment.maximumMarks),

      allowSubmissionUpdate:
        assignment.allowSubmissionUpdate,

      teacherAssignmentId:
        String(assignment.teacherAssignmentId),
    });

    setEditingAssignment(assignment);
    setError("");
  }


  async function handleCreate(
    event: FormEvent<HTMLFormElement>
  ) {
    event.preventDefault();

    try {
      setError("");
      setIsSubmitting(true);

      const request: CreateAssignmentRequest = {
        code: form.code,
        title: form.title,
        description: form.description,

        deadline: new Date(
          form.deadline
        ).toISOString(),

        maximumMarks:
          Number(form.maximumMarks),

        allowSubmissionUpdate:
          form.allowSubmissionUpdate,

        teacherAssignmentId:
          Number(form.teacherAssignmentId),
      };

      await api<Assignment>(
        "/Assignments",
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
          : "Failed to create assignment."
      );
    } finally {
      setIsSubmitting(false);
    }
  }


  async function handleUpdate(
    event: FormEvent<HTMLFormElement>
  ) {
    event.preventDefault();

    if (!editingAssignment) {
      return;
    }

    try {
      setError("");
      setIsSubmitting(true);

      const request: UpdateAssignmentRequest = {
        code: form.code,
        title: form.title,
        description: form.description,

        deadline: new Date(
          form.deadline
        ).toISOString(),

        maximumMarks:
          Number(form.maximumMarks),

        allowSubmissionUpdate:
          form.allowSubmissionUpdate,
      };

      await api<Assignment>(
        `/Assignments/${editingAssignment.id}`,
        {
          method: "PUT",
          body: JSON.stringify(request),
        }
      );

      setEditingAssignment(null);

      await loadData();

    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Failed to update assignment."
      );
    } finally {
      setIsSubmitting(false);
    }
  }


  async function handlePublish(
    assignment: Assignment
  ) {
    try {
      setError("");
      setIsSubmitting(true);

      await api<Assignment>(
        `/Assignments/${assignment.id}/publish`,
        {
          method: "PATCH",
        }
      );

      await loadData();

    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Failed to publish assignment."
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
        `/Assignments/${deletingAssignment.id}`,
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
          : "Failed to delete assignment."
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
            My Assignments
          </h1>

          <p className="mt-1 text-muted-foreground">
            Create, manage, and publish assignments.
          </p>
        </div>

        <Button onClick={openCreateDialog}>
          <Plus />
          Create Assignment
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
              Loading assignments...
            </div>

          ) : (

            <Table>

              <TableHeader>

                <TableRow>
                  <TableHead>
                    Assignment
                  </TableHead>

                  <TableHead>
                    Course & Subject
                  </TableHead>

                  <TableHead>
                    Deadline
                  </TableHead>

                  <TableHead>
                    Marks
                  </TableHead>

                  <TableHead>
                    Status
                  </TableHead>

                  <TableHead className="text-right">
                    Actions
                  </TableHead>
                </TableRow>

              </TableHeader>


              <TableBody>

                {assignments.length === 0 ? (

                  <TableRow>

                    <TableCell
                      colSpan={6}
                      className="h-32 text-center text-muted-foreground"
                    >
                      No assignments found.
                    </TableCell>

                  </TableRow>

                ) : (

                  assignments.map(
                    (assignment) => (

                      <TableRow
                        key={assignment.id}
                      >

                        <TableCell>

                          <div>

                            <p className="font-medium">
                              {assignment.title}
                            </p>

                            <p className="text-sm text-muted-foreground">
                              {assignment.code}
                            </p>

                          </div>

                        </TableCell>


                        <TableCell>

                          <div>

                            <p className="font-medium">
                              {assignment.courseCode}
                            </p>

                            <p className="text-sm text-muted-foreground">
                              {assignment.subjectName}
                            </p>

                          </div>

                        </TableCell>


                        <TableCell>
                          {new Date(
                            assignment.deadline
                          ).toLocaleString()}
                        </TableCell>


                        <TableCell>
                          {assignment.maximumMarks}
                        </TableCell>


                        <TableCell>

                          {assignment.isPublished ? (

                            <Badge>
                              Published
                            </Badge>

                          ) : (

                            <Badge variant="secondary">
                              Draft
                            </Badge>

                          )}

                        </TableCell>


                        <TableCell className="text-right">

                          <div className="flex justify-end gap-1">

                            {!assignment.isPublished && (

                              <Button
                                variant="ghost"
                                size="icon"
                                onClick={() =>
                                  handlePublish(
                                    assignment
                                  )
                                }
                                title="Publish assignment"
                              >
                                <Send />
                              </Button>

                            )}


                            <Button
                              variant="ghost"
                              size="icon"
                              onClick={() =>
                                openEditDialog(
                                  assignment
                                )
                              }
                            >
                              <Pencil />
                            </Button>


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

                          </div>

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

        <DialogContent className="sm:max-w-2xl max-h-[90vh] overflow-y-auto">

          <DialogHeader>

            <DialogTitle>
              Create Assignment
            </DialogTitle>

            <DialogDescription>
              Create a new assignment for one of your
              assigned courses and subjects.
            </DialogDescription>

          </DialogHeader>


          <AssignmentForm
            form={form}
            setForm={setForm}
            teacherAssignments={teacherAssignments}
            onSubmit={handleCreate}
            isSubmitting={isSubmitting}
            submitLabel="Create Assignment"
            showTeacherAssignment
          />

        </DialogContent>

      </Dialog>


      {/* Edit Dialog */}

      <Dialog
        open={!!editingAssignment}
        onOpenChange={(open) => {
          if (!open) {
            setEditingAssignment(null);
          }
        }}
      >

        <DialogContent className="sm:max-w-2xl max-h-[90vh] overflow-y-auto">

          <DialogHeader>

            <DialogTitle>
              Edit Assignment
            </DialogTitle>

            <DialogDescription>
              Update the assignment details.
            </DialogDescription>

          </DialogHeader>


          <AssignmentForm
            form={form}
            setForm={setForm}
            teacherAssignments={teacherAssignments}
            onSubmit={handleUpdate}
            isSubmitting={isSubmitting}
            submitLabel="Save Changes"
          />

        </DialogContent>

      </Dialog>


      {/* Delete Dialog */}

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
              Delete Assignment
            </DialogTitle>

            <DialogDescription>

              Are you sure you want to delete{" "}

              <span className="font-medium">
                {deletingAssignment?.title}
              </span>

              ? This action cannot be undone.

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
                ? "Deleting..."
                : "Delete Assignment"}
            </Button>

          </DialogFooter>

        </DialogContent>

      </Dialog>

    </div>
  );
}


interface AssignmentFormProps {
  form: typeof initialForm;

  setForm: React.Dispatch<
    React.SetStateAction<typeof initialForm>
  >;

  teacherAssignments:
    TeacherAssignment[];

  onSubmit: (
    event: FormEvent<HTMLFormElement>
  ) => void;

  isSubmitting: boolean;

  submitLabel: string;

  showTeacherAssignment?: boolean;
}


function AssignmentForm({
  form,
  setForm,
  teacherAssignments,
  onSubmit,
  isSubmitting,
  submitLabel,
  showTeacherAssignment = false,
}: AssignmentFormProps) {
  return (
    <form
      onSubmit={onSubmit}
      className="space-y-4"
    >

      {showTeacherAssignment && (

        <div className="space-y-2">

          <Label>
            Course & Subject
          </Label>

          <Select
            value={form.teacherAssignmentId}
            onValueChange={(value) =>
              setForm((current) => ({
                ...current,
                teacherAssignmentId: value,
              }))
            }
          >

            <SelectTrigger>
              <SelectValue placeholder="Select course and subject" />
            </SelectTrigger>

            <SelectContent>

              {teacherAssignments.map(
                (teacherAssignment) => (

                  <SelectItem
                    key={teacherAssignment.id}
                    value={String(
                      teacherAssignment.id
                    )}
                  >
                    {teacherAssignment.courseCode}
                    {" - "}
                    {teacherAssignment.courseName}
                    {" | "}
                    {teacherAssignment.subjectCode}
                    {" - "}
                    {teacherAssignment.subjectName}
                  </SelectItem>

                )
              )}

            </SelectContent>

          </Select>

        </div>

      )}


      <div className="space-y-2">

        <Label htmlFor="code">
          Assignment Code
        </Label>

        <Input
          id="code"
          value={form.code}
          onChange={(event) =>
            setForm((current) => ({
              ...current,
              code: event.target.value,
            }))
          }
          placeholder="ASSIGN-001"
          required
        />

      </div>


      <div className="space-y-2">

        <Label htmlFor="title">
          Title
        </Label>

        <Input
          id="title"
          value={form.title}
          onChange={(event) =>
            setForm((current) => ({
              ...current,
              title: event.target.value,
            }))
          }
          required
        />

      </div>


      <div className="space-y-2">

        <Label htmlFor="description">
          Description
        </Label>

        <Textarea
          id="description"
          value={form.description}
          onChange={(event) =>
            setForm((current) => ({
              ...current,
              description: event.target.value,
            }))
          }
          required
        />

      </div>


      <div className="grid gap-4 sm:grid-cols-2">

        <div className="space-y-2">

          <Label htmlFor="deadline">
            Deadline
          </Label>

          <Input
            id="deadline"
            type="datetime-local"
            value={form.deadline}
            onChange={(event) =>
              setForm((current) => ({
                ...current,
                deadline: event.target.value,
              }))
            }
            required
          />

        </div>


        <div className="space-y-2">

          <Label htmlFor="maximumMarks">
            Maximum Marks
          </Label>

          <Input
            id="maximumMarks"
            type="number"
            min="0.01"
            step="0.01"
            value={form.maximumMarks}
            onChange={(event) =>
              setForm((current) => ({
                ...current,
                maximumMarks:
                  event.target.value,
              }))
            }
            required
          />

        </div>

      </div>


      <div className="flex items-center justify-between rounded-lg border p-3">

        <div>

          <p className="text-sm font-medium">
            Allow submission updates
          </p>

          <p className="text-xs text-muted-foreground">
            Students can update their submission before
            the deadline.
          </p>

        </div>


        <Switch
          checked={form.allowSubmissionUpdate}
          onCheckedChange={(checked) =>
            setForm((current) => ({
              ...current,
              allowSubmissionUpdate: checked,
            }))
          }
        />

      </div>


      <Button
        type="submit"
        className="w-full"
        disabled={
          isSubmitting ||
          (showTeacherAssignment &&
            !form.teacherAssignmentId)
        }
      >
        {isSubmitting
          ? "Saving..."
          : submitLabel}
      </Button>

    </form>
  );
}


function formatDateTimeLocal(
  date: string
) {
  const value = new Date(date);

  const year = value.getFullYear();

  const month = String(
    value.getMonth() + 1
  ).padStart(2, "0");

  const day = String(
    value.getDate()
  ).padStart(2, "0");

  const hours = String(
    value.getHours()
  ).padStart(2, "0");

  const minutes = String(
    value.getMinutes()
  ).padStart(2, "0");

  return `${year}-${month}-${day}T${hours}:${minutes}`;
}