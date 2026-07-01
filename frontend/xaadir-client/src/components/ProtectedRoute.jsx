import { Navigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import Loading from "./Loading";

export default function ProtectedRoute({ children, roles }) {
  const { user, checking, isAuthenticated } = useAuth();

  if (checking) return <Loading text="Checking session..." />;
  if (!isAuthenticated) return <Navigate to="/login" replace />;

  if (roles?.length && !roles.includes(user.role)) {
    return <Navigate to={user.role === "Admin" ? "/admin-dashboard" : "/teacher-dashboard"} replace />;
  }

  return children;
}
