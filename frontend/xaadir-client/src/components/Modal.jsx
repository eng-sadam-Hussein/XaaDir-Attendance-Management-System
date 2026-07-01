import { X } from "lucide-react";

export default function Modal({ open, title, onClose, children }) {
  if (!open) return null;

  return (
    <div className="fixed inset-0 z-50 grid place-items-center bg-slate-950/50 p-4 backdrop-blur-sm">
      <div className="max-h-[92vh] w-full max-w-3xl overflow-y-auto rounded-4xl bg-white p-6 shadow-2xl">
        <div className="mb-5 flex items-center justify-between">
          <h2 className="text-2xl font-black tracking-tight">{title}</h2>
          <button className="icon-btn" onClick={onClose} type="button">
            <X size={20} />
          </button>
        </div>
        {children}
      </div>
    </div>
  );
}
