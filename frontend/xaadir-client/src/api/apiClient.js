import axios from "axios";

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || "http://localhost:5168/api",
  headers: {
    "Content-Type": "application/json"
  },
  timeout: 15000
});

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    const raw = error.response?.data || error.message || "API request failed";
    const message = typeof raw === "string" ? raw : JSON.stringify(raw);
    return Promise.reject(new Error(message));
  }
);

export default apiClient;
