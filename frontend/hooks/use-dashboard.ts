"use client";

import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api";
import type {
  DashboardStats,
  EmployeePerformance,
  MonthlyPoint,
  PriorityBreakdown,
  StatusBreakdown,
} from "@/lib/types";

export function useDashboardStats() {
  return useQuery({
    queryKey: ["dashboard", "stats"],
    queryFn: async () => {
      const { data } = await api.get<DashboardStats>("/dashboard/stats");
      return data;
    },
  });
}

export function useMonthlyTrend() {
  return useQuery({
    queryKey: ["dashboard", "monthly"],
    queryFn: async () => {
      const { data } = await api.get<MonthlyPoint[]>("/dashboard/monthly");
      return data;
    },
  });
}

export function useStatusBreakdown() {
  return useQuery({
    queryKey: ["dashboard", "status-breakdown"],
    queryFn: async () => {
      const { data } = await api.get<StatusBreakdown[]>(
        "/dashboard/status-breakdown"
      );
      return data;
    },
  });
}

export function usePriorityBreakdown() {
  return useQuery({
    queryKey: ["dashboard", "priority-breakdown"],
    queryFn: async () => {
      const { data } = await api.get<PriorityBreakdown[]>(
        "/dashboard/priority-breakdown"
      );
      return data;
    },
  });
}

export function useEmployeePerformance() {
  return useQuery({
    queryKey: ["dashboard", "employee-performance"],
    queryFn: async () => {
      const { data } = await api.get<EmployeePerformance[]>(
        "/dashboard/employee-performance"
      );
      return data;
    },
  });
}
