"use client";

import { FormEvent, useEffect, useState } from "react";
import {
  Pencil,
  Plus,
  Users,
} from "lucide-react";

import { api } from "@/lib/api";

import {
  CreateUserRequest,
  UpdateUserRequest,
  User,
  UserRole,
} from "@/types/user";

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
import { Badge } from "@/components/ui/badge";
import { Switch } from "@/components/ui/switch";

const roles: UserRole[] = [
  UserRole.Admin,
  UserRole.Teacher,
  UserRole.Student,
];

const initialForm = {
  firstName: "",
  lastName: "",
  email: "",
  password: "",
  role: UserRole.Student,
};

export default function UsersPage() {
  const [users, setUsers] =
    useState<User[]>([]);

  const [isLoading, setIsLoading] =
    useState(true);

  const [error, setError] =
    useState("");

  const [isCreateOpen, setIsCreateOpen] =
    useState(false);

  const [editingUser, setEditingUser] =
    useState<User | null>(null);

  const [form, setForm] =
    useState(initialForm);

  const [isSubmitting, setIsSubmitting] =
    useState(false);

  useEffect(() => {
    loadUsers();
  }, []);

  async function loadUsers() {
    try {
      setError("");
      setIsLoading(true);

      const response =
        await api<User[]>("/Users");

      setUsers(response);
    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Failed to load users."
      );
    } finally {
      setIsLoading(false);
    }
  }

  function openCreateDialog() {
    setForm(initialForm);
    setEditingUser(null);
    setIsCreateOpen(true);
  }

  function openEditDialog(user: User) {
    setForm({
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      password: "",
      role: user.role,
    });

    setEditingUser(user);
  }

  async function handleCreate(
    event: FormEvent<HTMLFormElement>
  ) {
    event.preventDefault();

    try {
      setIsSubmitting(true);

      const request: CreateUserRequest = {
        firstName: form.firstName,
        lastName: form.lastName,
        email: form.email,
        password: form.password,
        role: form.role,
      };

      await api<User>("/Users", {
        method: "POST",
        body: JSON.stringify(request),
      });

      setIsCreateOpen(false);
      await loadUsers();
    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Failed to create user."
      );
    } finally {
      setIsSubmitting(false);
    }
  }

  async function handleUpdate(
    event: FormEvent<HTMLFormElement>
  ) {
    event.preventDefault();

    if (!editingUser) {
      return;
    }

    try {
      setIsSubmitting(true);

      const request: UpdateUserRequest = {
        firstName: form.firstName,
        lastName: form.lastName,
        email: form.email,
        role: form.role,
      };

      await api<User>(
        `/Users/${editingUser.id}`,
        {
          method: "PUT",
          body: JSON.stringify(request),
        }
      );

      setEditingUser(null);

      await loadUsers();
    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Failed to update user."
      );
    } finally {
      setIsSubmitting(false);
    }
  }

  async function handleStatusChange(
    user: User,
    isActive: boolean
  ) {
    try {
      await api(
        `/Users/${user.id}/status`,
        {
          method: "PATCH",
          body: JSON.stringify({
            isActive,
          }),
        }
      );

      await loadUsers();
    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Failed to update user status."
      );
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">
            Users
          </h1>

          <p className="mt-1 text-muted-foreground">
            Manage administrators, teachers, and students.
          </p>
        </div>

        <Button
          onClick={openCreateDialog}
        >
          <Plus />
          Add User
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
              Loading users...
            </div>
          ) : (
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>User</TableHead>
                  <TableHead>Role</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Created</TableHead>
                  <TableHead className="text-right">
                    Actions
                  </TableHead>
                </TableRow>
              </TableHeader>

              <TableBody>
                {users.length === 0 ? (
                  <TableRow>
                    <TableCell
                      colSpan={5}
                      className="h-32 text-center text-muted-foreground"
                    >
                      No users found.
                    </TableCell>
                  </TableRow>
                ) : (
                  users.map((user) => (
                    <TableRow key={user.id}>
                      <TableCell>
                        <div>
                          <p className="font-medium">
                            {user.firstName}{" "}
                            {user.lastName}
                          </p>

                          <p className="text-sm text-muted-foreground">
                            {user.email}
                          </p>
                        </div>
                      </TableCell>

                      <TableCell>
                        <Badge variant="secondary">
                          {user.role}
                        </Badge>
                      </TableCell>

                      <TableCell>
                        <div className="flex items-center gap-3">
                          <Switch
                            checked={user.isActive}
                            onCheckedChange={(checked) =>
                              handleStatusChange(
                                user,
                                checked
                              )
                            }
                          />

                          <span className="text-sm text-muted-foreground">
                            {user.isActive
                              ? "Active"
                              : "Inactive"}
                          </span>
                        </div>
                      </TableCell>

                      <TableCell>
                        {new Date(
                          user.createdAt
                        ).toLocaleDateString()}
                      </TableCell>

                      <TableCell className="text-right">
                        <Button
                          variant="ghost"
                          size="icon"
                          onClick={() =>
                            openEditDialog(user)
                          }
                        >
                          <Pencil />
                        </Button>
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
              Add User
            </DialogTitle>

            <DialogDescription>
              Create a new administrator, teacher,
              or student.
            </DialogDescription>
          </DialogHeader>

          <UserForm
            form={form}
            setForm={setForm}
            onSubmit={handleCreate}
            isSubmitting={isSubmitting}
            showPassword
            submitLabel="Create User"
          />
        </DialogContent>
      </Dialog>

      <Dialog
        open={!!editingUser}
        onOpenChange={(open) => {
          if (!open) {
            setEditingUser(null);
          }
        }}
      >
        <DialogContent>
          <DialogHeader>
            <DialogTitle>
              Edit User
            </DialogTitle>

            <DialogDescription>
              Update this user's information.
            </DialogDescription>
          </DialogHeader>

          <UserForm
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

interface UserFormProps {
  form: typeof initialForm;

  setForm: React.Dispatch<
    React.SetStateAction<typeof initialForm>
  >;

  onSubmit: (
    event: FormEvent<HTMLFormElement>
  ) => void;

  isSubmitting: boolean;

  showPassword?: boolean;

  submitLabel: string;
}

function UserForm({
  form,
  setForm,
  onSubmit,
  isSubmitting,
  showPassword = false,
  submitLabel,
}: UserFormProps) {
  return (
    <form
      onSubmit={onSubmit}
      className="space-y-4"
    >
      <div className="grid gap-4 sm:grid-cols-2">
        <div className="space-y-2">
          <Label htmlFor="firstName">
            First Name
          </Label>

          <Input
            id="firstName"
            value={form.firstName}
            onChange={(event) =>
              setForm((current) => ({
                ...current,
                firstName: event.target.value,
              }))
            }
            required
          />
        </div>

        <div className="space-y-2">
          <Label htmlFor="lastName">
            Last Name
          </Label>

          <Input
            id="lastName"
            value={form.lastName}
            onChange={(event) =>
              setForm((current) => ({
                ...current,
                lastName: event.target.value,
              }))
            }
            required
          />
        </div>
      </div>

      <div className="space-y-2">
        <Label htmlFor="email">
          Email
        </Label>

        <Input
          id="email"
          type="email"
          value={form.email}
          onChange={(event) =>
            setForm((current) => ({
              ...current,
              email: event.target.value,
            }))
          }
          required
        />
      </div>

      {showPassword && (
        <div className="space-y-2">
          <Label htmlFor="password">
            Password
          </Label>

          <Input
            id="password"
            type="password"
            minLength={8}
            value={form.password}
            onChange={(event) =>
              setForm((current) => ({
                ...current,
                password: event.target.value,
              }))
            }
            required
          />
        </div>
      )}

      <div className="space-y-2">
        <Label>Role</Label>

        <Select
          value={form.role.toString()}
          onValueChange={(value) =>
            setForm((current) => ({
              ...current,
              role: Number(value) as UserRole,
            }))
          }
        >
          <SelectTrigger>
            <SelectValue />
          </SelectTrigger>

          <SelectContent>
            {roles.map((role) => (
              <SelectItem
                key={role}
                value={role.toString()}
              >
                {role}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
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