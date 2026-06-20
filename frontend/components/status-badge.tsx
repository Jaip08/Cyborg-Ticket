import { STATUS_BADGE, STATUS_LABELS } from "@/lib/constants";
import type { TicketStatus } from "@/lib/types";
import { cn } from "@/lib/utils";

export function StatusBadge({
  status,
  className,
}: {
  status: TicketStatus;
  className?: string;
}) {
  return (
    <span
      className={cn(
        "inline-flex items-center rounded-full border px-2.5 py-0.5 text-xs font-medium",
        STATUS_BADGE[status],
        className
      )}
    >
      {STATUS_LABELS[status]}
    </span>
  );
}
