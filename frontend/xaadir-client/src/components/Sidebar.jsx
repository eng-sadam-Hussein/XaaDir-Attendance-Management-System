import {
  BarChart3,
  BookOpen,
  CalendarCheck,
  GraduationCap,
  Home,
  Info,
  Layers3,
  LogOut,
  UserRoundCog
} from "lucide-react";
import { NavLink } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import icon from "../assets/logo-icon.svg";

const adminMenu = [
  { to: "/admin-dashboard", label: "Dashboard", icon: Home },
  { to: "/users", label: "Users", icon: UserRoundCog },
  { to: "/classes", label: "Classes", icon: Layers3 },
  { to: "/subjects", label: "Subjects", icon: BookOpen },
  { to: "/students", label: "Students", icon: GraduationCap },
  { to: "/attendance", label: "Attendance", icon: CalendarCheck },
  { to: "/reports", label: "Reports", icon: BarChart3 },
  { to: "/about", label: "About", icon: Info }
];

const teacherMenu = [
  { to: "/teacher-dashboard", label: "Dashboard", icon: Home },
  { to: "/subjects", label: "My Subjects", icon: BookOpen },
  { to: "/students", label: "Students", icon: GraduationCap },
  { to: "/attendance", label: "Mark Attendance", icon: CalendarCheck },
  { to: "/reports", label: "My Reports", icon: BarChart3 },
  { to: "/about", label: "About", icon: Info }
];

export default function Sidebar({ open, onClose }) {
  const { isAdmin, logout } = useAuth();
  const menu = isAdmin ? adminMenu : teacherMenu;

  return (
    <aside className={`fixed inset-y-0 left-0 z-40 w-72 border-r border-slate-200 bg-white p-5 shadow-soft transition lg:sticky lg:translate-x-0 ${open ? "translate-x-0" : "-translate-x-full"}`}>
      <div className="mb-8 flex items-center gap-3">
        <img className="h-14 w-14" src={icon} alt="XaaDir" />
        <div>
          <h2 className="font-black tracking-tight">XaaDir App</h2>
          <p className="text-xs font-bold text-slate-500">Attendance System</p>
        </div>
      </div>

      <nav className="grid gap-2">
        {menu.map((item) => {
          const Icon = item.icon;
          return (
            <NavLink
              key={item.to}
              to={item.to}
              onClick={onClose}
              className={({ isActive }) =>
                `flex items-center gap-3 rounded-2xl px-4 py-3 text-sm font-black transition ${
                  isActive
                    ? "bg-gradient-to-r from-xaadirBlue to-xaadirGreen text-white shadow-glow"
                    : "text-slate-600 hover:bg-slate-100 hover:text-xaadirBlue"
                }`
              }
            >
              <Icon size={19} />
              {item.label}
            </NavLink>
          );
        })}
      </nav>

      <button onClick={logout} className="absolute bottom-5 left-5 right-5 flex items-center justify-center gap-2 rounded-2xl bg-slate-950 px-4 py-3 text-sm font-black text-white">
        <LogOut size={18} />
        Logout
      </button>
    </aside>
  );
}
