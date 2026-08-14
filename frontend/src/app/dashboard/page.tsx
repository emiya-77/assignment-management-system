"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { getAuth } from "@/lib/auth";

export default function DashboardPage() {
  const router = useRouter();

  useEffect(() => {
    const auth = getAuth();

    if (!auth) {
      router.replace("/login");
      return;
    }

    switch (auth.role) {
      case "Admin":
        router.replace("/dashboard/admin");
        break;

      case "Teacher":
        router.replace("/dashboard/teacher");
        break;

      case "Student":
        router.replace("/dashboard/student");
        break;

      default:
        router.replace("/login");
    }
  }, [router]);

  return null;
}