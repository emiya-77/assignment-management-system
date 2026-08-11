export default function AdminDashboard() {
  return (
    <div>
      <div className="mb-8">
        <h1 className="text-3xl font-bold tracking-tight">
          Dashboard
        </h1>

        <p className="mt-2 text-muted-foreground">
          Manage users, courses, subjects, and academic assignments.
        </p>
      </div>

      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        <DashboardCard
          title="Users"
          description="Manage administrators, teachers, and students."
        />

        <DashboardCard
          title="Courses"
          description="Create and manage available courses."
        />

        <DashboardCard
          title="Subjects"
          description="Manage subjects for your institution."
        />

        <DashboardCard
          title="Academic Setup"
          description="Assign teachers and enroll students."
        />
      </div>
    </div>
  );
}

function DashboardCard({
  title,
  description,
}: {
  title: string;
  description: string;
}) {
  return (
    <div className="rounded-xl border bg-background p-5 shadow-sm">
      <h2 className="font-semibold">
        {title}
      </h2>

      <p className="mt-2 text-sm text-muted-foreground">
        {description}
      </p>
    </div>
  );
}