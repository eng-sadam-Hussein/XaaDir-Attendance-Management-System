export default function FormInput({ label, required, children, ...props }) {
  return (
    <label className="label">
      <span>{label} {required && <b className="text-red-500">*</b>}</span>
      {children || <input className="input" {...props} />}
    </label>
  );
}
