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
import { validateStudent } from "../utils/validation";

const blank = { fullName: "", gender: "Male", phone: "", email: "", classId: "", status: "Active" };

export default function Students() {
  const { isAdmin } = useAuth();
  const [items, setItems] = useState([]);
  const [classes, setClasses] = useState([]);
  const [query, setQuery] = useState("");
  const [form, setForm] = useState(blank);
  const [editing, setEditing] = useState(null);
  const [deleting, setDeleting] = useState(null);
  const [open, setOpen] = useState(false);
  const [errors, setErrors] = useState([]);
  const [loading, setLoading] = useState(true);

  const load = async () => {
    setLoading(true);
    const [studentsRes, classesRes] = await Promise.all([apiClient.get("/Students"), apiClient.get("/Classes")]);
    setItems(studentsRes.data);
    setClasses(classesRes.data);
    setLoading(false);
  };

  useEffect(() => { load(); }, []);

  const filtered = useMemo(() => {
    const q = query.toLowerCase();
    return items.filter(x => [x.fullName, x.phone, x.email, x.className, x.status].some(v => String(v || "").toLowerCase().includes(q)));
  }, [items, query]);

  const add = () => { setForm(blank); setEditing(null); setErrors([]); setOpen(true); };
  const edit = (row) => { setForm(row); setEditing(row); setErrors([]); setOpen(true); };

  const save = async (e) => {
    e.preventDefault();
    const payload = { ...form, classId: Number(form.classId) };
    const validation = validateStudent(payload);
    setErrors(validation);
    if (validation.length) return;
    try {
      if (editing) await apiClient.put(`/Students/${editing.studentId}`, payload);
      else await apiClient.post("/Students", payload);
      toast.success("Student saved successfully");
      setOpen(false);
      load();
    } catch (error) { toast.error(error.message); }
  };

  const remove = async () => {
    try {
      await apiClient.delete(`/Students/${deleting.studentId}`);
      toast.success("Student deleted");
      setDeleting(null);
      load();
    } catch (error) { toast.error(error.message); }
  };

  if (loading) return <Loading />;

  return (
    <>
      <PageHeader title="Students" subtitle={isAdmin ? "Manage student records." : "View students for attendance."} actions={isAdmin && <Button onClick={add}><Plus size={18} /> Add Student</Button>} />
      <section className="card">
        <SearchBox value={query} onChange={setQuery} placeholder="Search students..." />
        <DataTable data={filtered} rowKey="studentId" columns={[
          { key: "fullName", label: "Student" },
          { key: "gender", label: "Gender" },
          { key: "phone", label: "Phone" },
          { key: "email", label: "Email" },
          { key: "className", label: "Class" },
          { key: "status", label: "Status", render: r => <Badge tone={r.status === "Active" ? "green" : "red"}>{r.status}</Badge> }
        ]} actions={isAdmin ? row => <>
          <button className="icon-btn" onClick={() => edit(row)}><Edit size={17} /></button>
          <button className="icon-btn text-red-500" onClick={() => setDeleting(row)}><Trash2 size={17} /></button>
        </> : null} />
      </section>

      <Modal open={open} title={editing ? "Update Student" : "Add Student"} onClose={() => setOpen(false)}>
        <form onSubmit={save} className="grid gap-4">
          <ErrorMessage errors={errors} />
          <FormInput label="Full Name" value={form.fullName} onChange={e => setForm({ ...form, fullName: e.target.value })} required />
          <FormInput label="Gender"><select className="input" value={form.gender} onChange={e => setForm({ ...form, gender: e.target.value })}><option>Male</option><option>Female</option></select></FormInput>
          <FormInput label="Phone" value={form.phone} onChange={e => setForm({ ...form, phone: e.target.value })} required />
          <FormInput label="Email" value={form.email} onChange={e => setForm({ ...form, email: e.target.value })} required />
          <FormInput label="Class"><select className="input" value={form.classId} onChange={e => setForm({ ...form, classId: e.target.value })}><option value="">Select class</option>{classes.map(c => <option key={c.classId} value={c.classId}>{c.className}</option>)}</select></FormInput>
          <FormInput label="Status"><select className="input" value={form.status} onChange={e => setForm({ ...form, status: e.target.value })}><option>Active</option><option>Inactive</option></select></FormInput>
          <div className="flex justify-end gap-3"><Button type="button" variant="ghost" onClick={() => setOpen(false)}>Cancel</Button><Button type="submit">Save</Button></div>
        </form>
      </Modal>

      <ConfirmDialog open={Boolean(deleting)} message={`Delete ${deleting?.fullName}?`} onCancel={() => setDeleting(null)} onConfirm={remove} />
    </>
  );
}
