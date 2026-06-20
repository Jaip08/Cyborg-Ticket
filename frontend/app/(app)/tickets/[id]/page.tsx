"use client";

import { useState } from "react";
import Link from "next/link";
import { useParams, useRouter } from "next/navigation";
import {
  ArrowLeft,
  CalendarClock,
  CalendarDays,
  FolderOpen,
  Pencil,
  Trash2,
  User as UserIcon,
} from "lucide-react";
import { toast } from "sonner";
import { useDeleteTicket, useTicket } from "@/hooks/use-tickets";
import { apiError } from "@/lib/api";
import { useAuth } from "@/lib/auth";
import { formatDate, initials } from "@/lib/utils";
import { StatusBadge } from "@/components/status-badge";
import { PriorityBadge } from "@/components/priority-badge";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { StatusControl } from "@/components/ticket/status-control";
import { AssignControl } from "@/components/ticket/assign-control";
import { CommentsSection } from "@/components/ticket/comments-section";
import { ActivityTimeline } from "@/components/ticket/activity-timeline";
import { AttachmentsPanel } from "@/components/ticket/attachments-panel";
import { EditTicketDialog } from "@/components/ticket/edit-ticket-dialog";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import { Skeleton } from "@/components/ui/skeleton";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";

function MetaRow({
  icon: Icon,
  label,
  children,
}: {
  icon: typeof UserIcon;
  label: string;
  children: React.ReactNode;
}) {
  return (
    <div className="flex items-start gap-3">
      <Icon className="mt-0.5 h-4 w-4 shrink-0 text-muted-foreground" />
      <div className="min-w-0 flex-1">
        <p className="text-xs text-muted-foreground">{label}</p>
        <div className="text-sm font-medium">{children}</div>
      </div>
    </div>
  );
}

export default function TicketDetailPage() {
  const params = useParams<{ id: string }>();
  const router = useRouter();
  const { user, isManagerOrAdmin } = useAuth();
  const { data: ticket, isLoading, isError } = useTicket(params.id);
  const deleteTicket = useDeleteTicket();
  const [editOpen, setEditOpen] = useState(false);
  const [confirmDelete, setConfirmDelete] = useState(false);

  if (isLoading) {
    return (
      <div className="space-y-6">
        <Skeleton className="h-9 w-40" />
        <div className="grid gap-6 lg:grid-cols-3">
          <div className="space-y-4 lg:col-span-2">
            <Skeleton className="h-32 w-full" />
            <Skeleton className="h-64 w-full" />
          </div>
          <Skeleton className="h-80 w-full" />
        </div>
      </div>
    );
  }

  if (isError || !ticket) {
    return (
      <Card>
        <CardContent className="flex flex-col items-center gap-3 py-16 text-center">
          <p className="font-medium">Ticket not found</p>
          <p className="text-sm text-muted-foreground">
            It may have been deleted or you don&apos;t have access.
          </p>
          <Button asChild variant="outline">
            <Link href="/tickets">Back to tickets</Link>
          </Button>
        </CardContent>
      </Card>
    );
  }

  const isOwner = user?.id === ticket.createdBy.id;
  const canEdit = isManagerOrAdmin || isOwner;
  const canDelete = isManagerOrAdmin || isOwner;

  async function handleDelete() {
    try {
      await deleteTicket.mutateAsync(ticket!.id);
      toast.success("Ticket deleted");
      router.replace("/tickets");
    } catch (err) {
      toast.error(apiError(err, "Could not delete ticket"));
      setConfirmDelete(false);
    }
  }

  return (
    <>
      <div className="flex flex-wrap items-center justify-between gap-3">
        <Button variant="ghost" size="sm" asChild className="-ml-2">
          <Link href="/tickets">
            <ArrowLeft className="h-4 w-4" />
            Tickets
          </Link>
        </Button>
        <div className="flex items-center gap-2">
          <StatusControl ticketId={ticket.id} status={ticket.status} />
          {canEdit && (
            <Button variant="outline" size="sm" onClick={() => setEditOpen(true)}>
              <Pencil className="h-4 w-4" />
              Edit
            </Button>
          )}
          {canDelete && (
            <Button
              variant="outline"
              size="sm"
              className="text-destructive hover:text-destructive"
              onClick={() => setConfirmDelete(true)}
            >
              <Trash2 className="h-4 w-4" />
              Delete
            </Button>
          )}
        </div>
      </div>

      <div className="grid gap-6 lg:grid-cols-3">
        <div className="space-y-6 lg:col-span-2">
          <Card>
            <CardHeader className="gap-3">
              <div className="flex flex-wrap items-center gap-2">
                <span className="font-mono text-sm text-muted-foreground">
                  {ticket.ticketNumber}
                </span>
                <StatusBadge status={ticket.status} />
                <PriorityBadge priority={ticket.priority} />
              </div>
              <CardTitle className="text-xl leading-snug">
                {ticket.title}
              </CardTitle>
            </CardHeader>
            <CardContent>
              <p className="whitespace-pre-wrap text-sm leading-relaxed text-foreground/90">
                {ticket.description}
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardContent className="pt-6">
              <Tabs defaultValue="comments">
                <TabsList>
                  <TabsTrigger value="comments">Comments</TabsTrigger>
                  <TabsTrigger value="activity">Activity</TabsTrigger>
                </TabsList>
                <TabsContent value="comments">
                  <CommentsSection ticket={ticket} />
                </TabsContent>
                <TabsContent value="activity">
                  <ActivityTimeline ticketId={ticket.id} />
                </TabsContent>
              </Tabs>
            </CardContent>
          </Card>
        </div>

        <div className="space-y-6">
          <Card>
            <CardHeader>
              <CardTitle className="text-base">Details</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="space-y-1.5">
                <p className="text-xs text-muted-foreground">Assignee</p>
                {isManagerOrAdmin ? (
                  <AssignControl
                    ticketId={ticket.id}
                    assignedTo={ticket.assignedTo}
                  />
                ) : (
                  <div className="flex items-center gap-2">
                    {ticket.assignedTo ? (
                      <>
                        <Avatar className="h-7 w-7">
                          <AvatarFallback className="text-[10px]">
                            {initials(ticket.assignedTo.fullName)}
                          </AvatarFallback>
                        </Avatar>
                        <span className="text-sm font-medium">
                          {ticket.assignedTo.fullName}
                        </span>
                      </>
                    ) : (
                      <span className="text-sm italic text-muted-foreground">
                        Unassigned
                      </span>
                    )}
                  </div>
                )}
              </div>

              <Separator />

              <MetaRow icon={FolderOpen} label="Category">
                {ticket.category?.name ?? "—"}
              </MetaRow>
              <MetaRow icon={UserIcon} label="Reported by">
                {ticket.createdBy.fullName}
              </MetaRow>
              <MetaRow icon={CalendarDays} label="Created">
                {formatDate(ticket.createdAt)}
              </MetaRow>
              <MetaRow icon={CalendarClock} label="Due date">
                {formatDate(ticket.dueDate)}
              </MetaRow>
              {ticket.resolvedAt && (
                <MetaRow icon={CalendarDays} label="Resolved">
                  {formatDate(ticket.resolvedAt)}
                </MetaRow>
              )}
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle className="text-base">Attachments</CardTitle>
            </CardHeader>
            <CardContent>
              <AttachmentsPanel ticketId={ticket.id} />
            </CardContent>
          </Card>
        </div>
      </div>

      {canEdit && (
        <EditTicketDialog
          ticket={ticket}
          open={editOpen}
          onOpenChange={setEditOpen}
        />
      )}
      <ConfirmDialog
        open={confirmDelete}
        onOpenChange={setConfirmDelete}
        title="Delete this ticket?"
        description={`${ticket.ticketNumber} will be permanently removed. This can't be undone.`}
        confirmLabel="Delete ticket"
        destructive
        loading={deleteTicket.isPending}
        onConfirm={handleDelete}
      />
    </>
  );
}
