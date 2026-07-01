import { Edit, Plus, Trash2 } from "lucide-react";
import { useEffect, useMemo, useState } from "react";
import { toast } from "react-toastify";
import apiClient from "../api/apiClient";
import Badge from "../components/Badge";
import Button from "../components/Button";
import ConfirmDialog from "../components/ConfirmDialog";
import DataTable from "../components/DataTable";
import ErrorMessage from "../components/ErrorMessage";
import FormInput from "../components/FormInput";
import Loading from "../components/Loading";
import Modal from "../components/Modal";
import PageHeader from "../components/PageHeader";
import SearchBox from "../components/SearchBox";
import { useAuth } from "../context/AuthContext";
import { validateAttendance } from "../utils/validation";

const today = () => new Date().toISOString().slice(0, 10);

export default function Attendance() {
  const { user, isAdmin } = useAuth();
  const [records, setRecords] = useState([]);
  const [students, setStudents] = useState([]);
  const [classes, setClasses] = useState([]);
  const [subjects, setSubjects] = useState([]);
  const [query, setQuery] = useState("");
  const [form, setForm] = useState({ studentId: "", classId: "", subjectId: "", attendanceDate: today(), status: "Present", remarks: "", markedByUserId: user.userId });
  const [editing, setEditing] = useState(null);
  const [deleting, setDeleting] = useState(null);
  const [open, setOpen] = useState(false);
  const [errors, setErrors] = useState([]);
  const [loading, setLoading] = useState(true);

  const load = async () => {
    setLoading(true);
    const attendanceEndpoint = isAdmin ? "/Attendance" : `/Attendance/teacher/${user.userId}`;
    const subjectsEndpoint = isAdmin ? "/Subjects" : `/Subjects/teacher/${user.userId}`;
    const [att, studentsRes, classesRes, subjectsRes] = await Promise.all([
      apiClient.get(attendanceEndpoint),
      apiClient.get("/Students"),
      apiClient.get("/Classes"),
      apiClient.get(subjectsEndpoint)
    ]);
    setRecords(att.data);
    setStudents(studentsRes.data);
    setClasses(classesRes.data);
    setSubjects(subjectsRes.data);
    setLoading(false);
  };

  useEffect(() => { load(); }, [isAdmin, user.userId]);

  const filtered = useMemo(() => {
    const q = query.toLowerCase();
    return records.filter(x => [x.studentName, x.className, x.subjectName, x.status, x.markedByName].some(v => String(v || "").toLowerCase().includes(q)));
  }, [records, query]);

  const chooseStudent = (studentId) => {
    const student = students.find(s => Number(s.studentId) === Number(studentId));
    setForm({ ...form, studentId, classId: student?.classId || "" });
  };

  const add = () => { setForm({ studentId: "", classId: "", subjectId: "", attendanceDate: today(), status: "Present", remarks: "", markedByUserId: user.userId }); setEditing(null); setErrors([]); setOpen(true); };
  const edit = (row) => { setForm({ ...row, attendanceDate: row.attendanceDate?.slice(0, 10), markedByUserId: row.markedByUserId || user.userId }); setEditing(row); setErrors([]); setOpen(true); };

  const save = async (e) => {
    e.preventDefault();
    const payload = {
      ...form,
      studentId: Number(form.studentId),
      classId: Number(form.classId),
      subjectId: Number(form.subjectId),
      markedByUserId: Number(form.markedByUserId || user.userId)
    };
    const validation = validateAttendance(payload);
    setErrors(validation);
    if (validation.length) return;

    try {
      if (editing) await apiClient.put(`/Attendance/${editing.attendanceId}`, payload);
      else await apiClient.post("/Attendance/mark", payload);
      toast.success("Attendance saved successfully");
      setOpen(false);
      load();
    } catch (error) { toast.error(error.message); }
  };

  const remove = async () => {
    try {
      await apiClient.delete(`/Attendance/${deleting.attendanceId}`);
      toast.success("Attendance deleted");
      setDeleting(null);
      load();
    } catch (error) { toast.error(error.message); }
  };

  if (loading) return <Loading />;

  return (
    <>
      <PageHeader title={isAdmin ? "Attendance" : "My Attendance"} subtitle={isAdmin ? "Manage all attendance records." : "Mark attendance only for your assigned subjects."} actions={<Button onClick={add}><Plus size={18} /> Mark Attendance</Button>} />
      <section className="card">
        <SearchBox value={query} onChange={setQuery} placeholder="Search attendance..." />
        <DataTable data={filtered} rowKey="attendanceId" columns={[
          { key: "studentName", label: "Student" },
          { key: "className", label: "Class" },
          { key: "subjectName", label: "Subject" },
          { key: "attendanceDate", label: "Date", render: r => r.attendanceDate?.slice(0, 10) },
          { key: "status", label: "Status", render: r => <Badge tone={r.status === "Present" ? "green" : r.status === "Absent" ? "red" : "orange"}>{r.status}</Badge> },
          { key: "markedByName", label: "Marked By" }
        ]} actions={row => <>
          <button className="icon-btn" onClick={() => edit(row)}><Edit size={17} /></button>
          {isAdmin && <button className="icon-btn text-red-500" onClick={() => setDeleting(row)}><Trash2 size={17} /></button>}
        </>} />
      </section>

      <Modal open={open} title={editing ? "Update Attendance" : "Mark Attendance"} onClose={() => setOpen(false)}>
        <form onSubmit={save} className="grid gap-4">
          <ErrorMessage errors={errors} />
          <FormInput label="Student"><select className="input" value={form.studentId} onChange={e => chooseStudent(e.target.value)}><option value="">Select student</option>{students.map(s => <option key={s.studentId} value={s.studentId}>{s.fullName} - {s.className}</option>)}</select></FormInput>
          <FormInput label="Class"><select className="input" value={form.classId} onChange={e => setForm({ ...form, classId: e.target.value })}><option value="">Select class</option>{classes.map(c => <option key={c.classId} value={c.classId}>{c.className}</option>)}</select></FormInput>
          <FormInput label="Subject"><select className="input" value={form.subjectId} onChange={e => setForm({ ...form, subjectId: e.target.value })}><option value="">Select subject</option>{subjects.map(s => <option key={s.subjectId} value={s.subjectId}>{s.subjectName}</option>)}</select></FormInput>
          <FormInput label="Date" type="date" value={form.attendanceDate} onChange={e => setForm({ ...form, attendanceDate: e.target.value })} />
          <FormInput label="Status"><select className="input" value={form.status} onChange={e => setForm({ ...form, status: e.target.value })}><option>Present</option><option>Absent</option><option>Late</option></select></FormInput>
          <FormInput label="Remarks" value={form.remarks || ""} onChange={e => setForm({ ...form, remarks: e.target.value })} />
          <div className="flex justify-end gap-3"><Button type="button" variant="ghost" onClick={() => setOpen(false)}>Cancel</Button><Button type="submit">Save</Button></div>
        </form>
      </Modal>

      <ConfirmDialog open={Boolean(deleting)} message="Delete this attendance record?" onCancel={() => setDeleting(null)} onConfirm={remove} />
    </>
  );
}
