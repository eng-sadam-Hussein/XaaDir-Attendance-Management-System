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
import { validateUser } from "../utils/validation";

const blank = { fullName: "", username: "", email: "", password: "", role: "Teacher", isActive: true };

export default function Users() {
  const [items, setItems] = useState([]);
  const [form, setForm] = useState(blank);
  const [query, setQuery] = useState("");
  const [open, setOpen] = useState(false);
  const [editing, setEditing] = useState(null);
  const [deleting, setDeleting] = useState(null);
  const [errors, setErrors] = useState([]);
  const [loading, setLoading] = useState(true);

  const load = async () => {
    setLoading(true);
    const res = await apiClient.get("/Users");
    setItems(res.data);
    setLoading(false);
  };

  useEffect(() => { load(); }, []);

  const filtered = useMemo(() => {
    const q = query.toLowerCase();
    return items.filter(x => [x.fullName, x.username, x.email, x.role].some(v => String(v).toLowerCase().includes(q)));
  }, [items, query]);

  const add = () => { setForm(blank); setEditing(null); setErrors([]); setOpen(true); };
  const edit = (row) => { setForm(row); setEditing(row); setErrors([]); setOpen(true); };

  const save = async (e) => {
    e.preventDefault();
    const validation = validateUser(form);
    setErrors(validation);
    if (validation.length) return;

    try {
      if (editing) await apiClient.put(`/Users/${editing.userId}`, form);
      else await apiClient.post("/Users", form);
      toast.success("User saved successfully");
      setOpen(false);
      load();
    } catch (error) { toast.error(error.message); }
  };

  const remove = async () => {
    try {
      await apiClient.delete(`/Users/${deleting.userId}`);
      toast.success("User deleted");
      setDeleting(null);
      load();
    } catch (error) { toast.error(error.message); }
  };

  if (loading) return <Loading />;

  return (
    <>
      <PageHeader title="Users" subtitle="Admin and teacher account management." actions={<Button onClick={add}><Plus size={18} /> Add User</Button>} />
      <section className="card">
        <SearchBox value={query} onChange={setQuery} placeholder="Search users..." />
        <DataTable
          data={filtered}
          rowKey="userId"
          columns={[
            { key: "fullName", label: "Full Name" },
            { key: "username", label: "Username" },
            { key: "email", label: "Email" },
            { key: "role", label: "Role", render: r => <Badge tone={r.role === "Admin" ? "orange" : "green"}>{r.role}</Badge> },
            { key: "isActive", label: "Active", render: r => <Badge tone={r.isActive ? "green" : "red"}>{r.isActive ? "Yes" : "No"}</Badge> }
          ]}
          actions={row => <>
            <button className="icon-btn" onClick={() => edit(row)}><Edit size={17} /></button>
            <button className="icon-btn text-red-500" onClick={() => setDeleting(row)}><Trash2 size={17} /></button>
          </>}
        />
      </section>

      <Modal open={open} title={editing ? "Update User" : "Add User"} onClose={() => setOpen(false)}>
        <form onSubmit={save} className="grid gap-4">
          <ErrorMessage errors={errors} />
          <FormInput label="Full Name" value={form.fullName} onChange={e => setForm({ ...form, fullName: e.target.value })} required />
          <FormInput label="Username" value={form.username} onChange={e => setForm({ ...form, username: e.target.value })} required />
          <FormInput label="Email" type="email" value={form.email} onChange={e => setForm({ ...form, email: e.target.value })} required />
          <FormInput label="Password" value={form.password} onChange={e => setForm({ ...form, password: e.target.value })} required />
          <FormInput label="Role"><select className="input" value={form.role} onChange={e => setForm({ ...form, role: e.target.value })}><option>Admin</option><option>Teacher</option></select></FormInput>
          <label className="flex items-center gap-2 font-bold"><input type="checkbox" checked={form.isActive} onChange={e => setForm({ ...form, isActive: e.target.checked })} /> Active account</label>
          <div className="flex justify-end gap-3"><Button type="button" variant="ghost" onClick={() => setOpen(false)}>Cancel</Button><Button type="submit">Save</Button></div>
        </form>
      </Modal>

      <ConfirmDialog open={Boolean(deleting)} message={`Delete ${deleting?.fullName}?`} onCancel={() => setDeleting(null)} onConfirm={remove} />
    </>
  );
}
