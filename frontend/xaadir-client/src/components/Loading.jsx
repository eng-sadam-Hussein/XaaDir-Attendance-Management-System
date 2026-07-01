export default function Loading({ text = "Loading..." }) {
  return (
    <div className="grid min-h-[320px] place-items-center">
      <div className="flex items-center gap-3 rounded-3xl bg-white px-6 py-5 shadow-soft">
        <span className="h-7 w-7 animate-spin rounded-full border-4 border-blue-100 border-t-xaadirBlue" />
        <span className="font-bold text-slate-600">{text}</span>
      </div>
    </div>
  );
}
