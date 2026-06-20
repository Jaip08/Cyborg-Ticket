"use client";

import { useCallback, useEffect, useState } from "react";
import type { Role, User } from "./types";

const TOKEN_KEY = "ticket_token";
const USER_KEY = "ticket_user";

export function getToken(): string | null {
  if (typeof window === "undefined") return null;
  return window.localStorage.getItem(TOKEN_KEY);
}

export function getStoredUser(): User | null {
  if (typeof window === "undefined") return null;
  const raw = window.localStorage.getItem(USER_KEY);
  if (!raw) return null;
  try {
    return JSON.parse(raw) as User;
  } catch {
    return null;
  }
}

export function saveSession(token: string, user: User) {
  window.localStorage.setItem(TOKEN_KEY, token);
  window.localStorage.setItem(USER_KEY, JSON.stringify(user));
  window.dispatchEvent(new Event("auth-change"));
}

export function clearSession() {
  window.localStorage.removeItem(TOKEN_KEY);
  window.localStorage.removeItem(USER_KEY);
  window.dispatchEvent(new Event("auth-change"));
}

export function hasRole(user: User | null, ...roles: Role[]) {
  return !!user && roles.includes(user.role);
}

export function isManagerOrAdmin(user: User | null) {
  return hasRole(user, "Manager", "Admin");
}

export function useAuth() {
  const [user, setUser] = useState<User | null>(null);
  const [ready, setReady] = useState(false);

  const sync = useCallback(() => {
    setUser(getStoredUser());
    setReady(true);
  }, []);

  useEffect(() => {
    sync();
    const handler = () => sync();
    window.addEventListener("auth-change", handler);
    window.addEventListener("storage", handler);
    return () => {
      window.removeEventListener("auth-change", handler);
      window.removeEventListener("storage", handler);
    };
  }, [sync]);

  const logout = useCallback(() => {
    clearSession();
    window.location.href = "/login";
  }, []);

  return {
    user,
    ready,
    isAuthenticated: !!user,
    isManagerOrAdmin: isManagerOrAdmin(user),
    isAdmin: user?.role === "Admin",
    logout,
  };
}
