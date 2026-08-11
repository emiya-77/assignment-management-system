import { UserRole } from "@/types/auth";

export function getDashboardRoute(
  role: UserRole
) {
  switch (role) {
    case "Admin":
      return "/dashboard/admin";

    case "Teacher":
      return "/dashboard/teacher";

    case "Student":
      return "/dashboard/student";
  }
}