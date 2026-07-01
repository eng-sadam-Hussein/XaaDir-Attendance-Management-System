export default function StatCard({ title, value, icon: Icon, tone = "blue" }) {
  const tones = {
    blue: "bg-blue-100 text-xaadirBlue",
    green: "bg-green-100 text-xaadirGreen",
    orange: "bg-orange-100 text-xaadirOrange",
    red: "bg-red-100 text-red-500"
  };

  return (
    <div className="card flex items-center justify-between">
      <div>
        <p className="text-sm font-black uppercase tracking-wide text-slate-500">{title}</p>
        <h2 className="mt-2 text-4xl font-black tracking-tight text-slate-950">{value ?? 0}</h2>
      </div>
      {Icon && (
        <div className={`grid h-14 w-14 place-items-center rounded-2xl ${tones[tone] || tones.blue}`}>
          <Icon size={27} />
        </div>
      )}
    </div>
  );
}
