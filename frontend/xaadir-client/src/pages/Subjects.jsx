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
import { validateSubject } from "../utils/validation";

const blank = { subjectName: "", teacherUserId: "", description: "" };

export default function Subjects() {
  const { user, isAdmin } = useAuth();
  const [items, setItems] = useState([]);
  const [teachers, setTeachers] = useState([]);
  const [query, setQuery] = useState("");
  const [form, setForm] = useState(blank);
  const [editing, setEditing] = useState(null);
  const [deleting, setDeleting] = useState(null);
  const [open, setOpen] = useState(false);
  const [errors, setErrors] = useState([]);
  const [loading, setLoading] = useState(true);

  const load = async () => {
    setLoading(true);
    const res = await apiClient.get(isAdmin ? "/Subjects" : `/Subjects/teacher/${user.userId}`);
    setItems(res.data);
    if (isAdmin) {
      const users = await apiClient.get("/Users");
      setTeachers(users.data.filter(x => x.role === "Teacher"));
    }
    setLoading(false);
  };

  useEffect(() => { load(); }, [isAdmin, user.userId]);

  const filtered = useMemo(() => {
    const q = query.toLowerCase();
    return items.filter(x => [x.subjectName, x.teacherName, x.description].some(v => String(v || "").toLowerCase().includes(q)));
  }, [items, query]);

  const add = () => { setForm(blank); setEditing(null); setErrors([]); setOpen(true); };
  const edit = (row) => { setForm(row); setEditing(row); setErrors([]); setOpen(true); };

  const save = async (e) => {
    e.preventDefault();
    const payload = { ...form, teacherUserId: Number(form.teacherUserId) };
    const validation = validateSubject(payload);
    setErrors(validation);
    if (validation.length) return;
    try {
      if (editing) await apiClient.put(`/Subjects/${editing.subjectId}`, payload);
      else await apiClient.post("/Subjects", payload);
      toast.success("Subject saved successfully");
      setOpen(false);
      load();
    } catch (error) { toast.error(error.message); }
  };

  const remove = async () => {
    try {
      await apiClient.delete(`/Subjects/${deleting.subjectId}`);
      toast.success("Subject deleted");
      setDeleting(null);
      load();
    } catch (error) { toast.error(error.message); }
  };

  if (loading) return <Loading />;

  return (
    <>
      <PageHeader title={isAdmin ? "Subjects" : "My Subjects"} subtitle={isAdmin ? "Manage subjects and teacher assignments." : "Only your assigned subjects are displayed."} actions={isAdmin && <Button onClick={add}><Plus size={18} /> Add Subject</Button>} />
      <section className="card">
        <SearchBox value={query} onChange={setQuery} placeholder="Search subjects..." />
        <DataTable data={filtered} rowKey="subjectId" columns={[
          { key: "subjectName", label: "Subject" },
          { key: "teacherName", label: "Teacher" },
          { key: "description", label: "Description" },
          { key: "access", label: "Access", render: () => <Badge tone={isAdmin ? "orange" : "green"}>{isAdmin ? "Admin View" : "My Subject"}</Badge> }
        ]} actions={isAdmin ? row => <>
          <button className="icon-btn" onClick={() => edit(row)}><Edit size={17} /></button>
          <button className="icon-btn text-red-500" onClick={() => setDeleting(row)}><Trash2 size={17} /></button>
        </> : null} />
      </section>

      <Modal open={open} title={editing ? "Update Subject" : "Add Subject"} onClose={() => setOpen(false)}>
        <form onSubmit={save} className="grid gap-4">
          <ErrorMessage errors={errors} />
          <FormInput label="Subject Name" value={form.subjectName} onChange={e => setForm({ ...form, subjectName: e.target.value })} required />
          <FormInput label="Teacher">
            <select className="input" value={form.teacherUserId} onChange={e => setForm({ ...form, teacherUserId: e.target.value })}>
              <option value="">Select teacher</option>
              {teachers.map(t => <option key={t.userId} value={t.userId}>{t.fullName}</option>)}
            </select>
          </FormInput>
          <FormInput label="Description" value={form.description || ""} onChange={e => setForm({ ...form, description: e.target.value })} />
          <div className="flex justify-end gap-3"><Button type="button" variant="ghost" onClick={() => setOpen(false)}>Cancel</Button><Button type="submit">Save</Button></div>
        </form>
      </Modal>

      <ConfirmDialog open={Boolean(deleting)} message={`Delete ${deleting?.subjectName}?`} onCancel={() => setDeleting(null)} onConfirm={remove} />
    </>
  );
}
