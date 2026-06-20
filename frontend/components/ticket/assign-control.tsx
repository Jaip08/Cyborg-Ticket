"use client";

import { toast } from "sonner";
import { useAssignTicket } from "@/hooks/use-tickets";
import { useUsers } from "@/hooks/use-meta";
import { apiError } from "@/lib/api";
import type { UserRef } from "@/lib/types";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

const UNASSIGNED = "unassigned";

export function AssignControl({
  ticketId,
  assignedTo,
}: {
  ticketId: string;
  assignedTo: UserRef | null;
}) {
  const { data: users } = useUsers();
  const assign = useAssignTicket(ticketId);

  async function handle(value: string) {
    const next = value === UNASSIGNED ? "" : value;
    try {
      await assign.mutateAsync(next);
      toast.success(next ? "Ticket assigned" : "Ticket unassigned");
    } catch (err) {
      toast.error(apiError(err, "Could not assign ticket"));
    }
  }

  return (
    <Select
      value={assignedTo?.id ?? UNASSIGNED}
      onValueChange={handle}
      disabled={assign.isPending}
    >
      <SelectTrigger className="h-9 w-full">
        <SelectValue placeholder="Unassigned" />
      </SelectTrigger>
      <SelectContent>
        <SelectItem value={UNASSIGNED}>Unassigned</SelectItem>
        {users?.map((u) => (
          <SelectItem key={u.id} value={u.id}>
            {u.fullName}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  );
}
