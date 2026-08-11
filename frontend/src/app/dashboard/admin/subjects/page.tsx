"use client";

import {
  FormEvent,
  useEffect,
  useState,
} from "react";

import {
  Pencil,
  Plus,
  Trash2,
} from "lucide-react";

import { api } from "@/lib/api";

import {
  CreateSubjectRequest,
  Subject,
  UpdateSubjectRequest,
} from "@/types/subject";

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

import { Textarea } from "@/components/ui/textarea";

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


export default function SubjectsPage() {
  const [subjects, setSubjects] =
    useState<Subject[]>([]);

  const [isLoading, setIsLoading] =
    useState(true);

  const [error, setError] =
    useState("");

  const [isCreateOpen, setIsCreateOpen] =
    useState(false);

  const [editingSubject, setEditingSubject] =
    useState<Subject | null>(null);

  const [deletingSubject, setDeletingSubject] =
    useState<Subject | null>(null);

  const [form, setForm] =
    useState(initialForm);

  const [isSubmitting, setIsSubmitting] =
    useState(false);


  useEffect(() => {
    loadSubjects();
  }, []);


  async function loadSubjects() {
    try {
      setError("");
      setIsLoading(true);

      const response =
        await api<Subject[]>("/Subjects");

      setSubjects(response);
    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Failed to load subjects."
      );
    } finally {
      setIsLoading(false);
    }
  }


  function openCreateDialog() {
    setForm(initialForm);
    setEditingSubject(null);
    setIsCreateOpen(true);
  }


  function openEditDialog(
    subject: Subject
  ) {
    setForm({
      code: subject.code,
      name: subject.name,
      description:
        subject.description ?? "",
    });

    setEditingSubject(subject);
  }


  async function handleCreate(
    event: FormEvent<HTMLFormElement>
  ) {
    event.preventDefault();

    try {
      setError("");
      setIsSubmitting(true);

      const request: CreateSubjectRequest = {
        code: form.code,
        name: form.name,
        description:
          form.description.trim() || null,
      };

      await api<Subject>("/Subjects", {
        method: "POST",
        body: JSON.stringify(request),
      });

      setIsCreateOpen(false);

      await loadSubjects();
    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Failed to create subject."
      );
    } finally {
      setIsSubmitting(false);
    }
  }


  async function handleUpdate(
    event: FormEvent<HTMLFormElement>
  ) {
    event.preventDefault();

    if (!editingSubject) {
      return;
    }

    try {
      setError("");
      setIsSubmitting(true);

      const request: UpdateSubjectRequest = {
        code: form.code,
        name: form.name,
        description:
          form.description.trim() || null,
      };

      await api<Subject>(
        `/Subjects/${editingSubject.id}`,
        {
          method: "PUT",
          body: JSON.stringify(request),
        }
      );

      setEditingSubject(null);

      await loadSubjects();
    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Failed to update subject."
      );
    } finally {
      setIsSubmitting(false);
    }
  }


  async function handleDelete() {
    if (!deletingSubject) {
      return;
    }

    try {
      setError("");
      setIsSubmitting(true);

      await api<void>(
        `/Subjects/${deletingSubject.id}`,
        {
          method: "DELETE",
        }
      );

      setDeletingSubject(null);

      await loadSubjects();
    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Failed to delete subject."
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
            Subjects
          </h1>

          <p className="mt-1 text-muted-foreground">
            Manage subjects available in your institution.
          </p>
        </div>

        <Button onClick={openCreateDialog}>
          <Plus />
          Add Subject
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
              Loading subjects...
            </div>

          ) : (

            <Table>

              <TableHeader>
                <TableRow>
                  <TableHead>Code</TableHead>
                  <TableHead>Name</TableHead>
                  <TableHead>Description</TableHead>
                  <TableHead>Created</TableHead>

                  <TableHead className="text-right">
                    Actions
                  </TableHead>
                </TableRow>
              </TableHeader>


              <TableBody>

                {subjects.length === 0 ? (

                  <TableRow>
                    <TableCell
                      colSpan={5}
                      className="h-32 text-center text-muted-foreground"
                    >
                      No subjects found.
                    </TableCell>
                  </TableRow>

                ) : (

                  subjects.map((subject) => (

                    <TableRow key={subject.id}>

                      <TableCell className="font-medium">
                        {subject.code}
                      </TableCell>

                      <TableCell>
                        {subject.name}
                      </TableCell>

                      <TableCell className="max-w-xs truncate">
                        {subject.description || "-"}
                      </TableCell>

                      <TableCell>
                        {new Date(
                          subject.createdAt
                        ).toLocaleDateString()}
                      </TableCell>


                      <TableCell className="text-right">

                        <div className="flex justify-end gap-1">

                          <Button
                            variant="ghost"
                            size="icon"
                            onClick={() =>
                              openEditDialog(subject)
                            }
                          >
                            <Pencil />
                          </Button>


                          <Button
                            variant="ghost"
                            size="icon"
                            onClick={() =>
                              setDeletingSubject(subject)
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


      {/* Create Subject */}

      <Dialog
        open={isCreateOpen}
        onOpenChange={setIsCreateOpen}
      >

        <DialogContent>

          <DialogHeader>

            <DialogTitle>
              Add Subject
            </DialogTitle>

            <DialogDescription>
              Create a new subject for your institution.
            </DialogDescription>

          </DialogHeader>


          <SubjectForm
            form={form}
            setForm={setForm}
            onSubmit={handleCreate}
            isSubmitting={isSubmitting}
            submitLabel="Create Subject"
          />

        </DialogContent>

      </Dialog>


      {/* Edit Subject */}

      <Dialog
        open={!!editingSubject}
        onOpenChange={(open) => {
          if (!open) {
            setEditingSubject(null);
          }
        }}
      >

        <DialogContent>

          <DialogHeader>

            <DialogTitle>
              Edit Subject
            </DialogTitle>

            <DialogDescription>
              Update this subject's information.
            </DialogDescription>

          </DialogHeader>


          <SubjectForm
            form={form}
            setForm={setForm}
            onSubmit={handleUpdate}
            isSubmitting={isSubmitting}
            submitLabel="Save Changes"
          />

        </DialogContent>

      </Dialog>


      {/* Delete Confirmation */}

      <Dialog
        open={!!deletingSubject}
        onOpenChange={(open) => {
          if (!open) {
            setDeletingSubject(null);
          }
        }}
      >

        <DialogContent>

          <DialogHeader>

            <DialogTitle>
              Delete Subject
            </DialogTitle>

            <DialogDescription>
              Are you sure you want to delete{" "}
              <span className="font-medium">
                {deletingSubject?.name}
              </span>
              ? This action cannot be undone.
            </DialogDescription>

          </DialogHeader>


          <DialogFooter>

            <Button
              variant="outline"
              onClick={() =>
                setDeletingSubject(null)
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
                : "Delete Subject"}
            </Button>

          </DialogFooter>

        </DialogContent>

      </Dialog>

    </div>
  );
}


interface SubjectFormProps {
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


function SubjectForm({
  form,
  setForm,
  onSubmit,
  isSubmitting,
  submitLabel,
}: SubjectFormProps) {
  return (
    <form
      onSubmit={onSubmit}
      className="space-y-4"
    >

      <div className="space-y-2">

        <Label htmlFor="code">
          Subject Code
        </Label>

        <Input
          id="code"
          placeholder="e.g. CSE101"
          value={form.code}
          onChange={(event) =>
            setForm((current) => ({
              ...current,
              code: event.target.value,
            }))
          }
          required
          maxLength={50}
        />

      </div>


      <div className="space-y-2">

        <Label htmlFor="name">
          Subject Name
        </Label>

        <Input
          id="name"
          placeholder="e.g. Introduction to Programming"
          value={form.name}
          onChange={(event) =>
            setForm((current) => ({
              ...current,
              name: event.target.value,
            }))
          }
          required
          maxLength={200}
        />

      </div>


      <div className="space-y-2">

        <Label htmlFor="description">
          Description
        </Label>

        <Textarea
          id="description"
          placeholder="Optional description..."
          value={form.description}
          onChange={(event) =>
            setForm((current) => ({
              ...current,
              description: event.target.value,
            }))
          }
          maxLength={1000}
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