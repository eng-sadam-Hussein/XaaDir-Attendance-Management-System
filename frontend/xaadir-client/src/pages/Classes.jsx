import { Edit, Plus, Trash2 } from "lucide-react";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import apiClient from "../api/apiClient";
import Button from "../components/Button";
import ConfirmDialog from "../components/ConfirmDialog";
import DataTable from "../components/DataTable";
import ErrorMessage from "../components/ErrorMessage";
import FormInput from "../components/FormInput";
import Loading from "../components/Loading";
import Modal from "../components/Modal";
import PageHeader from "../components/PageHeader";
import { validateClass } from "../utils/validation";

const blank = { className: "", section: "A", description: "" };

export default function Classes() {
  const [items, setItems] = useState([]);
  const [form, setForm] = useState(blank);
  const [open, setOpen] = useState(false);
  const [editing, setEditing] = useState(null);
  const [deleting, setDeleting] = useState(null);
  const [errors, setErrors] = useState([]);
  const [loading, setLoading] = useState(true);

  const load = async () => {
    setLoading(true);
    const res = await apiClient.get("/Classes");
    setItems(res.data);
    setLoading(false);
  };

  useEffect(() => { load(); }, []);
  const add = () => { setForm(blank); setEditing(null); setErrors([]); setOpen(true); };
  const edit = (row) => { setForm(row); setEditing(row); setErrors([]); setOpen(true); };

  const save = async (e) => {
    e.preventDefault();
    const validation = validateClass(form);
    setErrors(validation);
    if (validation.length) return;
    try {
      if (editing) await apiClient.put(`/Classes/${editing.classId}`, form);
      else await apiClient.post("/Classes", form);
      toast.success("Class saved successfully");
      setOpen(false);
      load();
    } catch (error) { toast.error(error.message); }
  };

  const remove = async () => {
    try {
      await apiClient.delete(`/Classes/${deleting.classId}`);
      toast.success("Class deleted");
      setDeleting(null);
      load();
    } catch (error) { toast.error(error.message); }
  };

  if (loading) return <Loading />;

  return (
    <>
      <PageHeader title="Classes" subtitle="Manage Form 1, Form 2, Form 3, and Form 4." actions={<Button onClick={add}><Plus size={18} /> Add Class</Button>} />
      <section className="card">
        <DataTable data={items} rowKey="classId" columns={[
          { key: "className", label: "Class" },
          { key: "section", label: "Section" },
          { key: "description", label: "Description" }
        ]} actions={row => <>
          <button className="icon-btn" onClick={() => edit(row)}><Edit size={17} /></button>
          <button className="icon-btn text-red-500" onClick={() => setDeleting(row)}><Trash2 size={17} /></button>
        </>} />
      </section>

      <Modal open={open} title={editing ? "Update Class" : "Add Class"} onClose={() => setOpen(false)}>
        <form onSubmit={save} className="grid gap-4">
          <ErrorMessage errors={errors} />
          <FormInput label="Class Name" value={form.className} onChange={e => setForm({ ...form, className: e.target.value })} required />
          <FormInput label="Section" value={form.section || ""} onChange={e => setForm({ ...form, section: e.target.value })} />
          <FormInput label="Description" value={form.description || ""} onChange={e => setForm({ ...form, description: e.target.value })} />
          <div className="flex justify-end gap-3"><Button type="button" variant="ghost" onClick={() => setOpen(false)}>Cancel</Button><Button type="submit">Save</Button></div>
        </form>
      </Modal>

      <ConfirmDialog open={Boolean(deleting)} message={`Delete ${deleting?.className}?`} onCancel={() => setDeleting(null)} onConfirm={remove} />
    </>
  );
}
