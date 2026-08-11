"use client";

import { ReactNode, useEffect, useState } from "react";
import { usePathname, useRouter } from "next/navigation";
import {
  BookOpen,
  ClipboardList,
  GraduationCap,
  LayoutDashboard,
  LogOut,
  Menu,
  Users,
  UserCheck,
  X,
} from "lucide-react";

import { getAuth, logout } from "@/lib/auth";
import { getDashboardRoute } from "@/lib/routes";
import { LoginResponse } from "@/types/auth";
import { Button } from "@/components/ui/button";

interface DashboardLayoutProps {
  children: ReactNode;
  requiredRole: "Admin" | "Teacher" | "Student";
}

interface NavigationItem {
  label: string;
  href: string;
  icon: ReactNode;
}

export function DashboardLayout({
  children,
  requiredRole,
}: DashboardLayoutProps) {
  const router = useRouter();
  const pathname = usePathname();

  const [auth, setAuth] =
    useState<LoginResponse | null>(null);

  const [isLoading, setIsLoading] =
    useState(true);

  const [isSidebarOpen, setIsSidebarOpen] =
    useState(false);

  useEffect(() => {
    const storedAuth = getAuth();

    if (!storedAuth) {
      router.replace("/login");
      return;
    }

    if (storedAuth.role !== requiredRole) {
      router.replace(
        getDashboardRoute(storedAuth.role)
      );
      return;
    }

    setAuth(storedAuth);
    setIsLoading(false);
  }, [requiredRole, router]);

  function handleLogout() {
    logout();
    router.push("/login");
  }

  if (isLoading || !auth) {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <div className="text-sm text-muted-foreground">
          Loading...
        </div>
      </div>
    );
  }

  const navigationItems =
    getNavigationItems(requiredRole);

  return (
    <div className="min-h-screen bg-muted/30">
      {isSidebarOpen && (
        <div
          className="fixed inset-0 z-40 bg-black/50 lg:hidden"
          onClick={() =>
            setIsSidebarOpen(false)
          }
        />
      )}

      <aside
        className={`
          fixed inset-y-0 left-0 z-50 flex w-64
          flex-col border-r bg-background
          transition-transform duration-200
          lg:translate-x-0
          ${
            isSidebarOpen
              ? "translate-x-0"
              : "-translate-x-full"
          }
        `}
      >
        <div className="flex h-16 items-center justify-between border-b px-6">
          <div>
            <h1 className="font-semibold">
              AssignmentMS
            </h1>

            <p className="text-xs text-muted-foreground">
              {requiredRole} Portal
            </p>
          </div>

          <Button
            variant="ghost"
            size="icon"
            className="lg:hidden"
            onClick={() =>
              setIsSidebarOpen(false)
            }
          >
            <X className="size-5" />
          </Button>
        </div>

        <nav className="flex-1 space-y-1 p-3">
          {navigationItems.map((item) => {
            const isActive =
              pathname === item.href;

            return (
              <button
                key={item.href}
                onClick={() => {
                  router.push(item.href);
                  setIsSidebarOpen(false);
                }}
                className={`
                  flex w-full items-center gap-3
                  rounded-lg px-3 py-2 text-sm
                  transition-colors
                  ${
                    isActive
                      ? "bg-primary text-primary-foreground"
                      : "text-muted-foreground hover:bg-muted hover:text-foreground"
                  }
                `}
              >
                {item.icon}

                {item.label}
              </button>
            );
          })}
        </nav>

        <div className="border-t p-3">
          <div className="mb-3 px-3">
            <p className="text-sm font-medium">
              {auth.firstName} {auth.lastName}
            </p>

            <p className="truncate text-xs text-muted-foreground">
              {auth.email}
            </p>
          </div>

          <Button
            variant="ghost"
            className="w-full justify-start gap-3"
            onClick={handleLogout}
          >
            <LogOut className="size-4" />
            Logout
          </Button>
        </div>
      </aside>

      <main className="min-h-screen lg:pl-64">
        <header className="flex h-16 items-center border-b bg-background px-4 lg:px-8">
          <Button
            variant="ghost"
            size="icon"
            className="mr-2 lg:hidden"
            onClick={() =>
              setIsSidebarOpen(true)
            }
          >
            <Menu className="size-5" />
          </Button>

          <div>
            <h2 className="font-semibold">
              {requiredRole} Dashboard
            </h2>
          </div>
        </header>

        <div className="p-4 md:p-6 lg:p-8">
          {children}
        </div>
      </main>
    </div>
  );
}

function getNavigationItems(
  role: "Admin" | "Teacher" | "Student"
): NavigationItem[] {
  if (role === "Admin") {
    return [
      {
        label: "Dashboard",
        href: "/dashboard/admin",
        icon: <LayoutDashboard className="size-4" />,
      },
      {
        label: "Users",
        href: "/dashboard/admin/users",
        icon: <Users className="size-4" />,
      },
      {
        label: "Courses",
        href: "/dashboard/admin/courses",
        icon: <GraduationCap className="size-4" />,
      },
      {
        label: "Subjects",
        href: "/dashboard/admin/subjects",
        icon: <BookOpen className="size-4" />,
      },
      {
        label: "Teacher Assignments",
        href: "/dashboard/admin/teacher-assignments",
        icon: <UserCheck className="size-4" />,
      },
      {
        label: "Enrollments",
        href: "/dashboard/admin/enrollments",
        icon: <ClipboardList className="size-4" />,
      },
    ];
  }

  if (role === "Teacher") {
    return [
      {
        label: "Dashboard",
        href: "/dashboard/teacher",
        icon: <LayoutDashboard className="size-4" />,
      },
      {
        label: "Assignments",
        href: "/dashboard/teacher/assignments",
        icon: <ClipboardList className="size-4" />,
      },
    ];
  }

  return [
    {
      label: "Dashboard",
      href: "/dashboard/student",
      icon: <LayoutDashboard className="size-4" />,
    },
    {
      label: "Assignments",
      href: "/dashboard/student/assignments",
      icon: <ClipboardList className="size-4" />,
    },
    {
      label: "My Submissions",
      href: "/dashboard/student/submissions",
      icon: <BookOpen className="size-4" />,
    },
  ];
}