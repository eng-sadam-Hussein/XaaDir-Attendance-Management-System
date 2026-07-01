import { createContext, useContext, useEffect, useMemo, useState } from "react";
import { toast } from "react-toastify";
import apiClient from "../api/apiClient";

const AuthContext = createContext(null);
const STORAGE_KEY = "xaadir_tailwind_user";

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [checking, setChecking] = useState(true);

  useEffect(() => {
    const saved = localStorage.getItem(STORAGE_KEY);
    if (saved) {
      try {
        setUser(JSON.parse(saved));
      } catch {
        localStorage.removeItem(STORAGE_KEY);
      }
    }
    setChecking(false);
  }, []);

  const login = async (username, password) => {
    const response = await apiClient.post("/Auth/login", { username, password });
    const loggedUser = response.data;
    setUser(loggedUser);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(loggedUser));
    toast.success(`Welcome ${loggedUser.fullName}`);
    return loggedUser;
  };

  const logout = () => {
    setUser(null);
    localStorage.removeItem(STORAGE_KEY);
    toast.info("Logged out successfully");
  };

  const value = useMemo(
    () => ({
      user,
      checking,
      isAuthenticated: Boolean(user),
      isAdmin: user?.role === "Admin",
      isTeacher: user?.role === "Teacher",
      login,
      logout
    }),
    [user, checking]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used inside AuthProvider");
  return context;
}
