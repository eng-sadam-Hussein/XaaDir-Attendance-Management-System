import { Link } from "react-router-dom";

export default function NotFound() {
  return (
    <div className="grid min-h-screen place-items-center p-6 text-center">
      <div className="card max-w-lg">
        <h1 className="text-7xl font-black text-xaadirBlue">404</h1>
        <p className="muted mt-3">Page not found.</p>
        <Link className="btn btn-primary mt-6" to="/">Back Home</Link>
      </div>
    </div>
  );
}
