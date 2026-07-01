import { Inbox } from "lucide-react";

export default function DataTable({ data = [], columns = [], rowKey = "id", actions }) {
  if (!data.length) {
    return (
      <div className="grid min-h-[220px] place-items-center rounded-3xl border border-dashed border-slate-300 bg-white p-8 text-center">
        <div>
          <Inbox className="mx-auto text-xaadirBlue" size={44} />
          <h3 className="mt-3 text-lg font-black">No records found</h3>
          <p className="muted">Try refreshing or adding new data.</p>
        </div>
      </div>
    );
  }

  return (
    <div className="table-wrap">
      <table className="data-table">
        <thead>
          <tr>
            {columns.map((col) => <th key={col.key}>{col.label}</th>)}
            {actions && <th>Actions</th>}
          </tr>
        </thead>
        <tbody>
          {data.map((row, index) => (
            <tr className="hover:bg-slate-50" key={row[rowKey] ?? index}>
              {columns.map((col) => <td key={col.key}>{col.render ? col.render(row) : row[col.key]}</td>)}
              {actions && <td><div className="flex gap-2">{actions(row)}</div></td>}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
