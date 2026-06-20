"use client";

import { toast } from "sonner";
import { useChangeStatus } from "@/hooks/use-tickets";
import { apiError } from "@/lib/api";
import { STATUS_OPTIONS } from "@/lib/constants";
import type { TicketStatus } from "@/lib/types";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

export function StatusControl({
  ticketId,
  status,
}: {
  ticketId: string;
  status: TicketStatus;
}) {
  const changeStatus = useChangeStatus(ticketId);

  async function handle(next: string) {
    if (next === status) return;
    try {
      await changeStatus.mutateAsync(next as TicketStatus);
      toast.success("Status updated");
    } catch (err) {
      toast.error(apiError(err, "Could not update status"));
    }
  }

  return (
    <Select value={status} onValueChange={handle} disabled={changeStatus.isPending}>
      <SelectTrigger className="h-9 w-[150px]">
        <SelectValue />
      </SelectTrigger>
      <SelectContent>
        {STATUS_OPTIONS.map((o) => (
          <SelectItem key={o.value} value={o.value}>
            {o.label}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  );
}
