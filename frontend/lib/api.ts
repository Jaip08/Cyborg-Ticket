import axios from "axios";
import { clearSession, getToken } from "./auth";

export const API_URL =
  process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5080/api";

export const api = axios.create({
  baseURL: API_URL,
});

api.interceptors.request.use((config) => {
  const token = getToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (res) => res,
  (error) => {
    if (error.response?.status === 401 && typeof window !== "undefined") {
      const onLogin = window.location.pathname.startsWith("/login");
      clearSession();
      if (!onLogin) {
        window.location.href = "/login";
      }
    }
    return Promise.reject(error);
  }
);

export function apiError(error: unknown, fallback = "Something went wrong") {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as
      | { message?: string; title?: string; errors?: Record<string, string[]> }
      | undefined;
    if (data?.errors) {
      const first = Object.values(data.errors)[0];
      if (first?.length) return first[0];
    }
    return data?.message ?? data?.title ?? error.message ?? fallback;
  }
  return fallback;
}
