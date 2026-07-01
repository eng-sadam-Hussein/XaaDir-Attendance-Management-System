export default function ErrorMessage({ errors = [] }) {
  if (!errors.length) return null;

  return (
    <div className="rounded-2xl border border-red-200 bg-red-50 p-4 text-sm font-bold text-red-600">
      {errors.map((error, index) => (
        <div key={index}>{error}</div>
      ))}
    </div>
  );
}
