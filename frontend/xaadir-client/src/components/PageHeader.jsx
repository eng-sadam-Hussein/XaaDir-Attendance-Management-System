export default function PageHeader({ title, subtitle, actions }) {
  return (
    <div className="mb-7 flex flex-col justify-between gap-4 md:flex-row md:items-end">
      <div>
        <p className="eyebrow">XaaDir App</p>
        <h1 className="page-title">{title}</h1>
        {subtitle && <p className="muted mt-2 max-w-3xl">{subtitle}</p>}
      </div>
      {actions && <div className="flex flex-wrap gap-2">{actions}</div>}
    </div>
  );
}
