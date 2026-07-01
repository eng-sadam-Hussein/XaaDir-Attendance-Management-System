import { Search } from "lucide-react";

export default function SearchBox({ value, onChange, placeholder = "Search..." }) {
  return (
    <div className="mb-5 flex items-center gap-3 rounded-2xl border border-slate-200 bg-white px-4">
      <Search className="text-slate-400" size={18} />
      <input
        className="w-full bg-transparent py-3 text-sm outline-none"
        value={value}
        onChange={(e) => onChange(e.target.value)}
        placeholder={placeholder}
      />
    </div>
  );
}
