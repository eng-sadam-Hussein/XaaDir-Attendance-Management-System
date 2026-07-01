import { BookOpen, CalendarCheck, CheckCircle2, Clock, XCircle } from "lucide-react";
import { useEffect, useState } from "react";
import apiClient from "../api/apiClient";
import Loading from "../components/Loading";
import PageHeader from "../components/PageHeader";
import StatCard from "../components/StatCard";
import { useAuth } from "../context/AuthContext";

export default function TeacherDashboard() {
  const { user } = useAuth();
  const [summary, setSummary] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    apiClient.get(`/Reports/teacher/${user.userId}/summary`)
      .then((res) => setSummary(res.data))
      .finally(() => setLoading(false));
  }, [user.userId]);

  if (loading) return <Loading />;

  return (
    <>
      <PageHeader title="Teacher Dashboard" subtitle={`Welcome ${user.fullName}. Only your assigned subjects and reports are visible.`} />
      <div className="mb-7 grid gap-5 md:grid-cols-2 xl:grid-cols-5">
        <StatCard title="My Subjects" value={summary?.mySubjects} icon={BookOpen} tone="blue" />
        <StatCard title="Records" value={summary?.myAttendanceRecords} icon={CalendarCheck} tone="orange" />
        <StatCard title="Present" value={summary?.presentCount} icon={CheckCircle2} tone="green" />
        <StatCard title="Absent" value={summary?.absentCount} icon={XCircle} tone="red" />
        <StatCard title="Late" value={summary?.lateCount} icon={Clock} tone="orange" />
      </div>
      <div className="card bg-gradient-to-br from-blue-50 to-green-50">
        <h2 className="text-2xl font-black">Teacher Permission Active</h2>
        <p className="muted mt-2 max-w-3xl">
          This dashboard is filtered by your UserId. Subjects and reports from other teachers are hidden.
        </p>
      </div>
    </>
  );
}
