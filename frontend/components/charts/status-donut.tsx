"use client";

import { Cell, Pie, PieChart, ResponsiveContainer, Tooltip } from "recharts";
import { STATUS_CHART_COLORS, STATUS_LABELS } from "@/lib/constants";
import type { StatusBreakdown } from "@/lib/types";
import { ChartTooltip } from "./chart-tooltip";

export function StatusDonut({ data }: { data: StatusBreakdown[] }) {
  const chartData = data.map((d) => ({
    name: STATUS_LABELS[d.status],
    value: d.count,
    color: STATUS_CHART_COLORS[d.status],
  }));
  const total = chartData.reduce((sum, d) => sum + d.value, 0);

  return (
    <div className="flex flex-col items-center gap-4 sm:flex-row">
      <div className="relative h-[200px] w-[200px] shrink-0">
        <ResponsiveContainer width="100%" height="100%">
          <PieChart>
            <Pie
              data={chartData}
              dataKey="value"
              nameKey="name"
              innerRadius={62}
              outerRadius={92}
              paddingAngle={2}
              strokeWidth={0}
            >
              {chartData.map((entry) => (
                <Cell key={entry.name} fill={entry.color} />
              ))}
            </Pie>
            <Tooltip content={<ChartTooltip />} />
          </PieChart>
        </ResponsiveContainer>
        <div className="pointer-events-none absolute inset-0 flex flex-col items-center justify-center">
          <span className="text-2xl font-semibold tabular-nums">{total}</span>
          <span className="text-xs text-muted-foreground">tickets</span>
        </div>
      </div>
      <div className="grid w-full grid-cols-1 gap-2">
        {chartData.map((entry) => (
          <div key={entry.name} className="flex items-center gap-2 text-sm">
            <span
              className="h-2.5 w-2.5 rounded-full"
              style={{ backgroundColor: entry.color }}
            />
            <span className="text-muted-foreground">{entry.name}</span>
            <span className="ml-auto font-medium tabular-nums">
              {entry.value}
            </span>
          </div>
        ))}
      </div>
    </div>
  );
}
