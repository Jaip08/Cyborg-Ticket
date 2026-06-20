"use client";

import {
  Area,
  AreaChart,
  CartesianGrid,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";
import type { MonthlyPoint } from "@/lib/types";
import { ChartTooltip } from "./chart-tooltip";

function monthLabel(value: string) {
  const [y, m] = value.split("-").map(Number);
  return new Date(y, (m ?? 1) - 1).toLocaleDateString(undefined, {
    month: "short",
  });
}

export function MonthlyTrendChart({ data }: { data: MonthlyPoint[] }) {
  return (
    <ResponsiveContainer width="100%" height={280}>
      <AreaChart data={data} margin={{ top: 8, right: 8, left: -16, bottom: 0 }}>
        <defs>
          <linearGradient id="created" x1="0" y1="0" x2="0" y2="1">
            <stop offset="0%" stopColor="#6366f1" stopOpacity={0.35} />
            <stop offset="100%" stopColor="#6366f1" stopOpacity={0} />
          </linearGradient>
          <linearGradient id="resolved" x1="0" y1="0" x2="0" y2="1">
            <stop offset="0%" stopColor="#10b981" stopOpacity={0.35} />
            <stop offset="100%" stopColor="#10b981" stopOpacity={0} />
          </linearGradient>
        </defs>
        <CartesianGrid strokeDasharray="3 3" className="stroke-border" vertical={false} />
        <XAxis
          dataKey="month"
          tickFormatter={monthLabel}
          tickLine={false}
          axisLine={false}
          className="text-xs"
          stroke="currentColor"
        />
        <YAxis
          allowDecimals={false}
          tickLine={false}
          axisLine={false}
          width={36}
          className="text-xs"
          stroke="currentColor"
        />
        <Tooltip
          content={<ChartTooltip labelFormatter={monthLabel} />}
          cursor={{ stroke: "hsl(var(--border))" }}
        />
        <Area
          type="monotone"
          dataKey="created"
          name="Created"
          stroke="#6366f1"
          strokeWidth={2}
          fill="url(#created)"
        />
        <Area
          type="monotone"
          dataKey="resolved"
          name="Resolved"
          stroke="#10b981"
          strokeWidth={2}
          fill="url(#resolved)"
        />
      </AreaChart>
    </ResponsiveContainer>
  );
}
