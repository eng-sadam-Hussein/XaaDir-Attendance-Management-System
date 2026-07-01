import { LockKeyhole, ShieldCheck, UserCheck } from "lucide-react";
import { useState } from "react";
import { Navigate, useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import logo from "../assets/xaadir-logo.svg";
import { useAuth } from "../context/AuthContext";

export default function Login() {
  const { login, isAuthenticated, user } = useAuth();
  const navigate = useNavigate();
  const [form, setForm] = useState({ username: "admin", password: "admin123" });
  const [loading, setLoading] = useState(false);

  if (isAuthenticated) {
    return <Navigate to={user.role === "Admin" ? "/admin-dashboard" : "/teacher-dashboard"} replace />;
  }

  const submit = async (e) => {
    e.preventDefault();
    if (!form.username || !form.password) {
      toast.error("Username and password are required");
      return;
    }

    try {
      setLoading(true);
      const logged = await login(form.username, form.password);
      navigate(logged.role === "Admin" ? "/admin-dashboard" : "/teacher-dashboard");
    } catch (error) {
      toast.error(error.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="grid min-h-screen items-center gap-8 p-4 lg:grid-cols-[1.15fr_.85fr] lg:p-12">
      <section className="card bg-gradient-to-br from-blue-50 via-white to-green-50 p-8 md:p-14">
        <img className="mb-10 w-full max-w-lg" src={logo} alt="XaaDir App" />
        <p className="eyebrow">React JS + Tailwind CSS</p>
        <h1 className="mt-3 max-w-4xl text-5xl font-black leading-[0.95] tracking-[-0.06em] text-slate-950 md:text-7xl">
          Smart Attendance Made Simple.
        </h1>
        <p className="mt-6 max-w-2xl text-lg leading-8 text-slate-600">
          Modern full-stack attendance dashboard connected to ASP.NET Core Web API,
          ADO.NET, and SQL Server XaaDirDB.
        </p>
        <div className="mt-8 flex flex-wrap gap-3">
          <span className="flex items-center gap-2 rounded-full bg-white px-4 py-3 text-sm font-black text-xaadirBlue shadow-lg"><ShieldCheck size={18} /> Role access</span>
          <span className="flex items-center gap-2 rounded-full bg-white px-4 py-3 text-sm font-black text-xaadirGreen shadow-lg"><UserCheck size={18} /> Teacher filter</span>
          <span className="flex items-center gap-2 rounded-full bg-white px-4 py-3 text-sm font-black text-xaadirOrange shadow-lg"><LockKeyhole size={18} /> Auth ready</span>
        </div>
      </section>

      <form onSubmit={submit} className="card p-7 md:p-9">
        <p className="eyebrow">Welcome Back</p>
        <h2 className="mt-2 text-3xl font-black tracking-tight">Login to XaaDir</h2>
        <p className="muted mt-2">Use admin or teacher credentials.</p>

        <label className="label mt-7">
          Username
          <input className="input" value={form.username} onChange={(e) => setForm({ ...form, username: e.target.value })} />
        </label>
        <label className="label mt-4">
          Password
          <input className="input" type="password" value={form.password} onChange={(e) => setForm({ ...form, password: e.target.value })} />
        </label>

        <button className="btn btn-primary mt-6 w-full" disabled={loading}>
          {loading ? "Signing in..." : "Login"}
        </button>

        <div className="mt-6 rounded-3xl bg-slate-50 p-4 text-sm leading-7 text-slate-600">
          <b className="text-slate-900">Demo Accounts</b><br />
          Admin: admin / admin123<br />
          Teacher: sadam / teacher123
        </div>
      </form>
    </div>
  );
}
