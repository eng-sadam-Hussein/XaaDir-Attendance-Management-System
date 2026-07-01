import { Navigate, Route, Routes } from "react-router-dom";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import Layout from "./components/Layout";
import ProtectedRoute from "./components/ProtectedRoute";
import { useAuth } from "./context/AuthContext";
import About from "./pages/About";
import AdminDashboard from "./pages/AdminDashboard";
import Attendance from "./pages/Attendance";
import Classes from "./pages/Classes";
import Login from "./pages/Login";
import NotFound from "./pages/NotFound";
import Reports from "./pages/Reports";
import Students from "./pages/Students";
import Subjects from "./pages/Subjects";
import TeacherDashboard from "./pages/TeacherDashboard";
import Users from "./pages/Users";

function HomeRedirect() {
  const { user, isAuthenticated } = useAuth();
  if (!isAuthenticated) return <Navigate to="/login" replace />;
  return <Navigate to={user.role === "Admin" ? "/admin-dashboard" : "/teacher-dashboard"} replace />;
}

function ProtectedLayout({ children, roles }) {
  return (
    <ProtectedRoute roles={roles}>
      <Layout>{children}</Layout>
    </ProtectedRoute>
  );
}

export default function App() {
  return (
    <>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/" element={<HomeRedirect />} />

        <Route path="/admin-dashboard" element={<ProtectedLayout roles={["Admin"]}><AdminDashboard /></ProtectedLayout>} />
        <Route path="/teacher-dashboard" element={<ProtectedLayout roles={["Teacher"]}><TeacherDashboard /></ProtectedLayout>} />

        <Route path="/users" element={<ProtectedLayout roles={["Admin"]}><Users /></ProtectedLayout>} />
        <Route path="/classes" element={<ProtectedLayout roles={["Admin"]}><Classes /></ProtectedLayout>} />
        <Route path="/subjects" element={<ProtectedLayout><Subjects /></ProtectedLayout>} />
        <Route path="/students" element={<ProtectedLayout><Students /></ProtectedLayout>} />
        <Route path="/attendance" element={<ProtectedLayout><Attendance /></ProtectedLayout>} />
        <Route path="/reports" element={<ProtectedLayout><Reports /></ProtectedLayout>} />
        <Route path="/about" element={<ProtectedLayout><About /></ProtectedLayout>} />

        <Route path="*" element={<NotFound />} />
      </Routes>
      <ToastContainer position="top-right" autoClose={2500} theme="colored" />
    </>
  );
}
