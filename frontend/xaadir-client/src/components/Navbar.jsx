import { LogOut, Menu, ShieldCheck } from "lucide-react";
import { useAuth } from "../context/AuthContext";
import logo from "../assets/xaadir-logo.svg";

export default function Navbar({ onMenu }) {
  const { user, logout } = useAuth();

  return (
    <header className="sticky top-0 z-30 flex h-20 items-center justify-between border-b border-slate-200 bg-white/80 px-4 backdrop-blur-xl md:px-8">
      <div className="flex items-center gap-3">
        <button className="icon-btn lg:hidden" onClick={onMenu}>
          <Menu size={22} />
        </button>
        <img className="h-14 w-44 object-contain md:w-56" src={logo} alt="XaaDir App" />
      </div>

      <div className="flex items-center gap-3">
        <span className="hidden items-center gap-2 rounded-full bg-green-50 px-4 py-2 text-sm font-black text-xaadirGreen md:inline-flex">
          <ShieldCheck size={16} />
          {user?.role}
        </span>
        <span className="hidden font-black text-slate-700 md:block">{user?.fullName}</span>
        <button className="icon-btn" onClick={logout} title="Logout">
          <LogOut size={20} />
        </button>
      </div>
    </header>
  );
}
