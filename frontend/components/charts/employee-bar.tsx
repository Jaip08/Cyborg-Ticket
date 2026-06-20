"use client";

import {
  Bar,
  BarChart,
  CartesianGrid,
  Legend,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";
import type { EmployeePerformance } from "@/lib/types";
import { ChartTooltip } from "./chart-tooltip";

export function EmployeeBar({ data }: { data: EmployeePerformance[] }) {
  const chartData = data.map((d) => ({
    name: d.fullName.split(" ")[0],
    Assigned: d.assigned,
    Resolved: d.resolved,
    Open: d.open,
  }));

  return (
    <ResponsiveContainer width="100%" height={320}>
      <BarChart data={chartData} margin={{ top: 8, right: 8, left: -16, bottom: 0 }}>
        <CartesianGrid strokeDasharray="3 3" className="stroke-border" vertical={false} />
        <XAxis
          dataKey="name"
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
          content={<ChartTooltip />}
          cursor={{ fill: "hsl(var(--muted))", opacity: 0.4 }}
        />
        <Legend wrapperStyle={{ fontSize: 12 }} />
        <Bar dataKey="Open" fill="#3b82f6" radius={[4, 4, 0, 0]} maxBarSize={28} />
        <Bar dataKey="Resolved" fill="#10b981" radius={[4, 4, 0, 0]} maxBarSize={28} />
      </BarChart>
    </ResponsiveContainer>
  );
}
