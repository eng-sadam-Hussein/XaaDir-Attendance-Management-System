import { useState } from "react";
import Navbar from "./Navbar";
import Sidebar from "./Sidebar";

export default function Layout({ children }) {
  const [open, setOpen] = useState(false);

  return (
    <div className="flex min-h-screen">
      <Sidebar open={open} onClose={() => setOpen(false)} />
      {open && <div className="fixed inset-0 z-30 bg-slate-950/40 lg:hidden" onClick={() => setOpen(false)} />}
      <main className="min-w-0 flex-1">
        <Navbar onMenu={() => setOpen(true)} />
        <section className="p-4 md:p-8">{children}</section>
      </main>
    </div>
  );
}
