const tones = {
  blue: "bg-blue-100 text-xaadirBlue",
  green: "bg-green-100 text-xaadirGreen",
  orange: "bg-orange-100 text-xaadirOrange",
  red: "bg-red-100 text-red-600",
  dark: "bg-slate-900 text-white"
};

export default function Badge({ children, tone = "blue" }) {
  return <span className={`inline-flex rounded-full px-3 py-1 text-xs font-black ${tones[tone] || tones.blue}`}>{children}</span>;
}
