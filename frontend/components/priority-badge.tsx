import { PRIORITY_BADGE } from "@/lib/constants";
import type { TicketPriority } from "@/lib/types";
import { cn } from "@/lib/utils";

const dot: Record<TicketPriority, string> = {
  Low: "bg-slate-400",
  Medium: "bg-sky-500",
  High: "bg-orange-500",
  Critical: "bg-red-500",
};

export function PriorityBadge({
  priority,
  className,
}: {
  priority: TicketPriority;
  className?: string;
}) {
  return (
    <span
      className={cn(
        "inline-flex items-center gap-1.5 rounded-full border px-2.5 py-0.5 text-xs font-medium",
        PRIORITY_BADGE[priority],
        className
      )}
    >
      <span className={cn("h-1.5 w-1.5 rounded-full", dot[priority])} />
      {priority}
    </span>
  );
}
