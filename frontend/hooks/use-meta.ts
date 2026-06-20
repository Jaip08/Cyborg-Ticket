"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { api } from "@/lib/api";
import type { Category, ManagedUser, Role } from "@/lib/types";

export function useCategories() {
  return useQuery({
    queryKey: ["categories"],
    queryFn: async () => {
      const { data } = await api.get<Category[]>("/categories");
      return data;
    },
    staleTime: 5 * 60 * 1000,
  });
}

export function useCreateCategory() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (input: { name: string; description?: string }) => {
      const { data } = await api.post<Category>("/categories", input);
      return data;
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: ["categories"] }),
  });
}

export function useUsers(enabled = true) {
  return useQuery({
    queryKey: ["users"],
    queryFn: async () => {
      const { data } = await api.get<ManagedUser[]>("/users");
      return data;
    },
    enabled,
    staleTime: 60 * 1000,
  });
}

export function useUpdateUserRole() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async ({ id, role }: { id: string; role: Role }) => {
      await api.put(`/users/${id}/role`, { role });
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: ["users"] }),
  });
}
