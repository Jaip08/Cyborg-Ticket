"use client";

import {
  Bar,
  BarChart,
  CartesianGrid,
  Cell,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";
import { PRIORITY_CHART_COLORS } from "@/lib/constants";
import type { PriorityBreakdown } from "@/lib/types";
import { ChartTooltip } from "./chart-tooltip";

export function PriorityBar({ data }: { data: PriorityBreakdown[] }) {
  const chartData = data.map((d) => ({
    name: d.priority,
    value: d.count,
    color: PRIORITY_CHART_COLORS[d.priority],
  }));

  return (
    <ResponsiveContainer width="100%" height={240}>
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
        <Bar dataKey="value" name="Tickets" radius={[6, 6, 0, 0]} maxBarSize={56}>
          {chartData.map((entry) => (
            <Cell key={entry.name} fill={entry.color} />
          ))}
        </Bar>
      </BarChart>
    </ResponsiveContainer>
  );
}
