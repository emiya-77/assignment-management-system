import { DashboardLayout } from "@/components/dashboard/dashboard-layout";

export default function TeacherLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <DashboardLayout requiredRole="Teacher">
      {children}
    </DashboardLayout>
  );
}