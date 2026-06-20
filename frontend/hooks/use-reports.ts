"use client";

import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api";
import type {
  EmployeePerformance,
  MonthlyPoint,
  PriorityBreakdown,
  StatusBreakdown,
} from "@/lib/types";

export function useReportByStatus() {
  return useQuery({
    queryKey: ["reports", "by-status"],
    queryFn: async () => {
      const { data } = await api.get<StatusBreakdown[]>("/reports/by-status");
      return data;
    },
  });
}

export function useReportByPriority() {
  return useQuery({
    queryKey: ["reports", "by-priority"],
    queryFn: async () => {
      const { data } = await api.get<PriorityBreakdown[]>(
        "/reports/by-priority"
      );
      return data;
    },
  });
}

export function useReportByEmployee() {
  return useQuery({
    queryKey: ["reports", "by-employee"],
    queryFn: async () => {
      const { data } = await api.get<EmployeePerformance[]>(
        "/reports/by-employee"
      );
      return data;
    },
  });
}

export function useReportMonthlyTrend() {
  return useQuery({
    queryKey: ["reports", "monthly-trend"],
    queryFn: async () => {
      const { data } = await api.get<MonthlyPoint[]>("/reports/monthly-trend");
      return data;
    },
  });
}

export async function downloadReport(format: "csv" | "excel") {
  const res = await api.get("/reports/export", {
    params: { report: "tickets", format },
    responseType: "blob",
  });
  const ext = format === "excel" ? "xlsx" : "csv";
  const url = URL.createObjectURL(res.data as Blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = `tickets-report.${ext}`;
  document.body.appendChild(link);
  link.click();
  link.remove();
  URL.revokeObjectURL(url);
}
