import { BarChart3, CheckCircle2, Clock, XCircle } from "lucide-react";
import { useEffect, useState } from "react";
import apiClient from "../api/apiClient";
import Badge from "../components/Badge";
import DataTable from "../components/DataTable";
import Loading from "../components/Loading";
import PageHeader from "../components/PageHeader";
import StatCard from "../components/StatCard";
import { useAuth } from "../context/AuthContext";

export default function Reports() {
  const { user, isAdmin } = useAuth();
  const [summary, setSummary] = useState(null);
  const [byClass, setByClass] = useState([]);
  const [byStatus, setByStatus] = useState([]);
  const [byTeacher, setByTeacher] = useState([]);
  const [teacherAttendance, setTeacherAttendance] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function load() {
      setLoading(true);
      if (isAdmin) {
        const [sum, cls, stat, teacher] = await Promise.all([
          apiClient.get("/Reports/admin/summary"),
          apiClient.get("/Reports/admin/by-class"),
          apiClient.get("/Reports/admin/by-status"),
          apiClient.get("/Reports/admin/by-teacher")
        ]);
        setSummary(sum.data); setByClass(cls.data); setByStatus(stat.data); setByTeacher(teacher.data);
      } else {
        const [sum, att] = await Promise.all([
          apiClient.get(`/Reports/teacher/${user.userId}/summary`),
          apiClient.get(`/Reports/teacher/${user.userId}/attendance`)
        ]);
        setSummary(sum.data); setTeacherAttendance(att.data);
      }
      setLoading(false);
    }
    load();
  }, [isAdmin, user.userId]);

  if (loading) return <Loading />;

  return (
    <>
      <PageHeader title={isAdmin ? "Admin Reports" : "My Reports"} subtitle={isAdmin ? "System-wide report analytics." : "Reports filtered by your assigned subjects."} />

      <div className="mb-7 grid gap-5 md:grid-cols-2 xl:grid-cols-4">
        <StatCard title={isAdmin ? "Total Records" : "My Records"} value={isAdmin ? summary?.totalAttendance : summary?.myAttendanceRecords} icon={BarChart3} tone="blue" />
        <StatCard title="Present" value={summary?.presentCount} icon={CheckCircle2} tone="green" />
        <StatCard title="Absent" value={summary?.absentCount} icon={XCircle} tone="red" />
        <StatCard title="Late" value={summary?.lateCount} icon={Clock} tone="orange" />
      </div>

      {isAdmin ? (
        <div className="grid gap-5 xl:grid-cols-2">
          <section className="card">
            <h2 className="mb-4 text-2xl font-black">Attendance by Class</h2>
            <DataTable data={byClass} rowKey="name" columns={[{ key: "name", label: "Class" }, { key: "total", label: "Total" }]} />
          </section>
          <section className="card">
            <h2 className="mb-4 text-2xl font-black">Attendance by Status</h2>
            <DataTable data={byStatus} rowKey="name" columns={[
              { key: "name", label: "Status", render: r => <Badge tone={r.name === "Present" ? "green" : r.name === "Absent" ? "red" : "orange"}>{r.name}</Badge> },
              { key: "total", label: "Total" }
            ]} />
          </section>
          <section className="card xl:col-span-2">
            <h2 className="mb-4 text-2xl font-black">Attendance by Teacher</h2>
            <DataTable data={byTeacher} rowKey="name" columns={[{ key: "name", label: "Teacher" }, { key: "total", label: "Total Records" }]} />
          </section>
        </div>
      ) : (
        <section className="card">
          <h2 className="mb-4 text-2xl font-black">My Attendance Records</h2>
          <DataTable data={teacherAttendance} rowKey="attendanceId" columns={[
            { key: "studentName", label: "Student" },
            { key: "className", label: "Class" },
            { key: "subjectName", label: "Subject" },
            { key: "attendanceDate", label: "Date", render: r => r.attendanceDate?.slice(0, 10) },
            { key: "status", label: "Status", render: r => <Badge tone={r.status === "Present" ? "green" : r.status === "Absent" ? "red" : "orange"}>{r.status}</Badge> }
          ]} />
        </section>
      )}
    </>
  );
}
