import { BarChart3, BookOpen, CalendarCheck, GraduationCap, Layers3, Users } from "lucide-react";
import { useEffect, useState } from "react";
import apiClient from "../api/apiClient";
import Badge from "../components/Badge";
import DataTable from "../components/DataTable";
import Loading from "../components/Loading";
import PageHeader from "../components/PageHeader";
import StatCard from "../components/StatCard";

export default function AdminDashboard() {
  const [summary, setSummary] = useState(null);
  const [attendance, setAttendance] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([apiClient.get("/Reports/admin/summary"), apiClient.get("/Reports/admin/attendance")])
      .then(([sum, att]) => {
        setSummary(sum.data);
        setAttendance(att.data.slice(0, 10));
      })
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <Loading />;

  return (
    <>
      <PageHeader title="Admin Dashboard" subtitle="Complete system overview for users, classes, subjects, students, attendance, and reports." />
      <div className="mb-7 grid gap-5 md:grid-cols-2 xl:grid-cols-4">
        <StatCard title="Users" value={summary?.totalUsers} icon={Users} tone="blue" />
        <StatCard title="Classes" value={summary?.totalClasses} icon={Layers3} tone="orange" />
        <StatCard title="Subjects" value={summary?.totalSubjects} icon={BookOpen} tone="green" />
        <StatCard title="Students" value={summary?.totalStudents} icon={GraduationCap} tone="blue" />
        <StatCard title="Attendance" value={summary?.totalAttendance} icon={CalendarCheck} tone="orange" />
        <StatCard title="Present" value={summary?.presentCount} icon={BarChart3} tone="green" />
        <StatCard title="Absent" value={summary?.absentCount} icon={BarChart3} tone="red" />
        <StatCard title="Late" value={summary?.lateCount} icon={BarChart3} tone="orange" />
      </div>

      <section className="card">
        <h2 className="mb-4 text-2xl font-black tracking-tight">Recent Attendance</h2>
        <DataTable
          data={attendance}
          rowKey="attendanceId"
          columns={[
            { key: "studentName", label: "Student" },
            { key: "className", label: "Class" },
            { key: "subjectName", label: "Subject" },
            { key: "status", label: "Status", render: (r) => <Badge tone={r.status === "Present" ? "green" : r.status === "Absent" ? "red" : "orange"}>{r.status}</Badge> },
            { key: "markedByName", label: "Marked By" }
          ]}
        />
      </section>
    </>
  );
}
