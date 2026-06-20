"use client";

import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api";
import { getToken } from "@/lib/auth";
import type { User } from "@/lib/types";

export function useMe() {
  return useQuery({
    queryKey: ["me"],
    queryFn: async () => {
      const { data } = await api.get<User>("/auth/me");
      return data;
    },
    enabled: !!getToken(),
    staleTime: 5 * 60 * 1000,
    retry: false,
  });
}
