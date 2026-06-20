"use client";

import { useState } from "react";
import { Lock, Loader2, MessageSquare, Send } from "lucide-react";
import { toast } from "sonner";
import { useAddComment, useComments } from "@/hooks/use-tickets";
import { apiError } from "@/lib/api";
import { useAuth } from "@/lib/auth";
import type { Ticket } from "@/lib/types";
import { cn, initials, relativeTime } from "@/lib/utils";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { Skeleton } from "@/components/ui/skeleton";
import { EmptyState } from "@/components/empty-state";

export function CommentsSection({ ticket }: { ticket: Ticket }) {
  const { user, isManagerOrAdmin } = useAuth();
  const { data: comments, isLoading } = useComments(ticket.id);
  const addComment = useAddComment(ticket.id);
  const [content, setContent] = useState("");
  const [internal, setInternal] = useState(false);

  // Employees only see internal notes when they're directly involved.
  const involved =
    user?.id === ticket.createdBy.id || user?.id === ticket.assignedTo?.id;
  const canSeeInternal = isManagerOrAdmin || involved;

  const visible = (comments ?? []).filter(
    (c) => !c.isInternal || canSeeInternal
  );

  async function submit() {
    const trimmed = content.trim();
    if (!trimmed) return;
    try {
      await addComment.mutateAsync({ content: trimmed, isInternal: internal });
      setContent("");
      setInternal(false);
    } catch (err) {
      toast.error(apiError(err, "Could not post comment"));
    }
  }

  return (
    <div className="space-y-5">
      <div className="space-y-3">
        {isLoading ? (
          Array.from({ length: 2 }).map((_, i) => (
            <div key={i} className="flex gap-3">
              <Skeleton className="h-9 w-9 rounded-full" />
              <div className="flex-1 space-y-2">
                <Skeleton className="h-4 w-32" />
                <Skeleton className="h-12 w-full" />
              </div>
            </div>
          ))
        ) : visible.length === 0 ? (
          <EmptyState
            icon={MessageSquare}
            title="No comments yet"
            description="Start the conversation below."
          />
        ) : (
          visible.map((c) => (
            <div
              key={c.id}
              className={cn(
                "flex gap-3 rounded-lg p-3",
                c.isInternal &&
                  "border border-amber-200 bg-amber-50/60 dark:border-amber-900/60 dark:bg-amber-950/30"
              )}
            >
              <Avatar className="h-9 w-9">
                <AvatarFallback>{initials(c.author.fullName)}</AvatarFallback>
              </Avatar>
              <div className="min-w-0 flex-1">
                <div className="flex flex-wrap items-center gap-2">
                  <span className="text-sm font-medium">
                    {c.author.fullName}
                  </span>
                  <Badge variant="secondary" className="text-[10px]">
                    {c.author.role}
                  </Badge>
                  {c.isInternal && (
                    <span className="inline-flex items-center gap-1 text-xs font-medium text-amber-700 dark:text-amber-400">
                      <Lock className="h-3 w-3" />
                      Internal
                    </span>
                  )}
                  <span className="ml-auto text-xs text-muted-foreground">
                    {relativeTime(c.createdAt)}
                  </span>
                </div>
                <p className="mt-1 whitespace-pre-wrap text-sm text-foreground/90">
                  {c.content}
                </p>
              </div>
            </div>
          ))
        )}
      </div>

      <div className="space-y-3 rounded-lg border p-3">
        <Textarea
          value={content}
          onChange={(e) => setContent(e.target.value)}
          placeholder="Write a comment…"
          rows={3}
        />
        <div className="flex items-center justify-between">
          {isManagerOrAdmin ? (
            <label className="flex cursor-pointer items-center gap-2 text-sm text-muted-foreground">
              <input
                type="checkbox"
                checked={internal}
                onChange={(e) => setInternal(e.target.checked)}
                className="h-4 w-4 rounded border-input accent-amber-600"
              />
              <Lock className="h-3.5 w-3.5" />
              Internal note
            </label>
          ) : (
            <span />
          )}
          <Button
            size="sm"
            onClick={submit}
            disabled={addComment.isPending || !content.trim()}
          >
            {addComment.isPending ? (
              <Loader2 className="h-4 w-4 animate-spin" />
            ) : (
              <Send className="h-4 w-4" />
            )}
            Comment
          </Button>
        </div>
      </div>
    </div>
  );
}
