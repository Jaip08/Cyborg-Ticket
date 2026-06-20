import type { TicketPriority, TicketStatus } from "./types";

export const STATUS_OPTIONS: { value: TicketStatus; label: string }[] = [
  { value: "Open", label: "Open" },
  { value: "InProgress", label: "In Progress" },
  { value: "OnHold", label: "On Hold" },
  { value: "Resolved", label: "Resolved" },
  { value: "Closed", label: "Closed" },
];

export const PRIORITY_OPTIONS: { value: TicketPriority; label: string }[] = [
  { value: "Low", label: "Low" },
  { value: "Medium", label: "Medium" },
  { value: "High", label: "High" },
  { value: "Critical", label: "Critical" },
];

export const STATUS_LABELS: Record<TicketStatus, string> = {
  Open: "Open",
  InProgress: "In Progress",
  OnHold: "On Hold",
  Resolved: "Resolved",
  Closed: "Closed",
};

// Tailwind classes for badges — kept literal so the JIT compiler picks them up.
export const STATUS_BADGE: Record<TicketStatus, string> = {
  Open: "border-blue-200 bg-blue-50 text-blue-700 dark:border-blue-900 dark:bg-blue-950 dark:text-blue-300",
  InProgress:
    "border-amber-200 bg-amber-50 text-amber-700 dark:border-amber-900 dark:bg-amber-950 dark:text-amber-300",
  OnHold:
    "border-slate-200 bg-slate-100 text-slate-600 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-300",
  Resolved:
    "border-emerald-200 bg-emerald-50 text-emerald-700 dark:border-emerald-900 dark:bg-emerald-950 dark:text-emerald-300",
  Closed:
    "border-zinc-200 bg-zinc-100 text-zinc-500 dark:border-zinc-700 dark:bg-zinc-800 dark:text-zinc-400",
};

export const PRIORITY_BADGE: Record<TicketPriority, string> = {
  Low: "border-slate-200 bg-slate-50 text-slate-600 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-300",
  Medium:
    "border-sky-200 bg-sky-50 text-sky-700 dark:border-sky-900 dark:bg-sky-950 dark:text-sky-300",
  High: "border-orange-200 bg-orange-50 text-orange-700 dark:border-orange-900 dark:bg-orange-950 dark:text-orange-300",
  Critical:
    "border-red-200 bg-red-50 text-red-700 dark:border-red-900 dark:bg-red-950 dark:text-red-300",
};

export const STATUS_CHART_COLORS: Record<TicketStatus, string> = {
  Open: "#3b82f6",
  InProgress: "#f59e0b",
  OnHold: "#94a3b8",
  Resolved: "#10b981",
  Closed: "#71717a",
};

export const PRIORITY_CHART_COLORS: Record<TicketPriority, string> = {
  Low: "#94a3b8",
  Medium: "#0ea5e9",
  High: "#f97316",
  Critical: "#ef4444",
};
