"use client";

import { Activity } from "lucide-react";
import { useActivity } from "@/hooks/use-tickets";
import { relativeTime } from "@/lib/utils";
import { Skeleton } from "@/components/ui/skeleton";
import { EmptyState } from "@/components/empty-state";

export function ActivityTimeline({ ticketId }: { ticketId: string }) {
  const { data, isLoading } = useActivity(ticketId);

  if (isLoading) {
    return (
      <div className="space-y-4">
        {Array.from({ length: 4 }).map((_, i) => (
          <div key={i} className="flex gap-3">
            <Skeleton className="h-2.5 w-2.5 rounded-full" />
            <Skeleton className="h-4 flex-1" />
          </div>
        ))}
      </div>
    );
  }

  if (!data || data.length === 0) {
    return <EmptyState icon={Activity} title="No activity recorded" />;
  }

  return (
    <ol className="relative space-y-5 border-l pl-6">
      {data.map((entry) => (
        <li key={entry.id} className="relative">
          <span className="absolute -left-[1.6rem] top-1 h-2.5 w-2.5 rounded-full border-2 border-background bg-primary" />
          <p className="text-sm">
            <span className="font-medium">{entry.user.fullName}</span>{" "}
            <span className="text-muted-foreground">{entry.description}</span>
          </p>
          <p className="text-xs text-muted-foreground">
            {relativeTime(entry.createdAt)}
          </p>
        </li>
      ))}
    </ol>
  );
}
