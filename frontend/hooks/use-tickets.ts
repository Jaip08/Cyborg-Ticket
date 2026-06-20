"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { api } from "@/lib/api";
import type {
  ActivityEntry,
  Attachment,
  Comment,
  CreateTicketInput,
  Paged,
  Ticket,
  TicketFilters,
  TicketStatus,
  UpdateTicketInput,
} from "@/lib/types";

function buildParams(filters: TicketFilters) {
  const params: Record<string, string | number> = {};
  if (filters.search) params.search = filters.search;
  if (filters.status) params.status = filters.status;
  if (filters.priority) params.priority = filters.priority;
  if (filters.categoryId) params.categoryId = filters.categoryId;
  if (filters.assignedToId) params.assignedToId = filters.assignedToId;
  params.page = filters.page ?? 1;
  params.pageSize = filters.pageSize ?? 10;
  return params;
}

export function useTickets(filters: TicketFilters) {
  return useQuery({
    queryKey: ["tickets", filters],
    queryFn: async () => {
      const { data } = await api.get<Paged<Ticket>>("/tickets", {
        params: buildParams(filters),
      });
      return data;
    },
    placeholderData: (prev) => prev,
  });
}

export function useTicket(id: string) {
  return useQuery({
    queryKey: ["ticket", id],
    queryFn: async () => {
      const { data } = await api.get<Ticket>(`/tickets/${id}`);
      return data;
    },
    enabled: !!id,
  });
}

export function useCreateTicket() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (input: CreateTicketInput) => {
      const { data } = await api.post<Ticket>("/tickets", input);
      return data;
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: ["tickets"] }),
  });
}

export function useUpdateTicket(id: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (input: UpdateTicketInput) => {
      const { data } = await api.put<Ticket>(`/tickets/${id}`, input);
      return data;
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["ticket", id] });
      qc.invalidateQueries({ queryKey: ["tickets"] });
    },
  });
}

export function useDeleteTicket() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (id: string) => {
      await api.delete(`/tickets/${id}`);
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: ["tickets"] }),
  });
}

export function useChangeStatus(id: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (status: TicketStatus) => {
      await api.post(`/tickets/${id}/status`, { status });
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["ticket", id] });
      qc.invalidateQueries({ queryKey: ["ticket-activity", id] });
      qc.invalidateQueries({ queryKey: ["tickets"] });
    },
  });
}

export function useAssignTicket(id: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (assigneeId: string) => {
      await api.post(`/tickets/${id}/assign`, { assigneeId });
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["ticket", id] });
      qc.invalidateQueries({ queryKey: ["ticket-activity", id] });
      qc.invalidateQueries({ queryKey: ["tickets"] });
    },
  });
}

export function useComments(ticketId: string) {
  return useQuery({
    queryKey: ["ticket-comments", ticketId],
    queryFn: async () => {
      const { data } = await api.get<Comment[]>(
        `/tickets/${ticketId}/comments`
      );
      return data;
    },
    enabled: !!ticketId,
  });
}

export function useAddComment(ticketId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (input: { content: string; isInternal: boolean }) => {
      const { data } = await api.post<Comment>(
        `/tickets/${ticketId}/comments`,
        input
      );
      return data;
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["ticket-comments", ticketId] });
      qc.invalidateQueries({ queryKey: ["ticket-activity", ticketId] });
    },
  });
}

export function useActivity(ticketId: string) {
  return useQuery({
    queryKey: ["ticket-activity", ticketId],
    queryFn: async () => {
      const { data } = await api.get<ActivityEntry[]>(
        `/tickets/${ticketId}/activity`
      );
      return data;
    },
    enabled: !!ticketId,
  });
}

export function useAttachments(ticketId: string) {
  return useQuery({
    queryKey: ["ticket-attachments", ticketId],
    queryFn: async () => {
      const { data } = await api.get<Attachment[]>(
        `/tickets/${ticketId}/attachments`
      );
      return data;
    },
    enabled: !!ticketId,
  });
}

export function useUploadAttachment(ticketId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (file: File) => {
      const form = new FormData();
      form.append("file", file);
      const { data } = await api.post<Attachment>(
        `/tickets/${ticketId}/attachments`,
        form,
        { headers: { "Content-Type": "multipart/form-data" } }
      );
      return data;
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["ticket-attachments", ticketId] });
      qc.invalidateQueries({ queryKey: ["ticket-activity", ticketId] });
    },
  });
}
