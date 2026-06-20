"use client";

import type { TooltipProps } from "recharts";

interface Props extends TooltipProps<number, string> {
  labelFormatter?: (label: string) => string;
}

export function ChartTooltip({
  active,
  payload,
  label,
  labelFormatter,
}: Props) {
  if (!active || !payload?.length) return null;
  return (
    <div className="rounded-lg border bg-background p-3 text-sm shadow-md">
      {label !== undefined && (
        <p className="mb-1.5 font-medium">
          {labelFormatter ? labelFormatter(String(label)) : label}
        </p>
      )}
      <div className="space-y-1">
        {payload.map((item) => (
          <div key={item.name} className="flex items-center gap-2">
            <span
              className="h-2.5 w-2.5 rounded-full"
              style={{ backgroundColor: item.color }}
            />
            <span className="text-muted-foreground">{item.name}</span>
            <span className="ml-auto font-medium tabular-nums">
              {item.value}
            </span>
          </div>
        ))}
      </div>
    </div>
  );
}
