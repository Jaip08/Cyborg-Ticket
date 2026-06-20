"use client";

import { useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { ArrowLeft, Loader2 } from "lucide-react";
import { toast } from "sonner";
import { useCreateTicket } from "@/hooks/use-tickets";
import { useCategories, useUsers } from "@/hooks/use-meta";
import { apiError } from "@/lib/api";
import { useAuth } from "@/lib/auth";
import { PRIORITY_OPTIONS } from "@/lib/constants";
import type { CreateTicketInput, TicketPriority } from "@/lib/types";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

const UNASSIGNED = "unassigned";

const schema = z.object({
  title: z.string().min(4, "Give it a clear title").max(160),
  description: z.string().min(10, "Add a bit more detail"),
  priority: z.enum(["Low", "Medium", "High", "Critical"]),
  categoryId: z.string().min(1, "Pick a category"),
  assignedToId: z.string().optional(),
  dueDate: z.string().optional(),
});

type FormValues = z.infer<typeof schema>;

export default function NewTicketPage() {
  const router = useRouter();
  const { isManagerOrAdmin } = useAuth();
  const { data: categories } = useCategories();
  const { data: users } = useUsers(isManagerOrAdmin);
  const createTicket = useCreateTicket();
  const [submitting, setSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: { priority: "Medium", assignedToId: UNASSIGNED },
  });

  async function onSubmit(values: FormValues) {
    setSubmitting(true);
    const payload: CreateTicketInput = {
      title: values.title,
      description: values.description,
      priority: values.priority as TicketPriority,
      categoryId: values.categoryId,
      assignedToId:
        values.assignedToId && values.assignedToId !== UNASSIGNED
          ? values.assignedToId
          : undefined,
      dueDate: values.dueDate || undefined,
    };
    try {
      const created = await createTicket.mutateAsync(payload);
      toast.success(`Ticket ${created.ticketNumber} created`);
      router.replace(`/tickets/${created.id}`);
    } catch (err) {
      toast.error(apiError(err, "Could not create ticket"));
      setSubmitting(false);
    }
  }

  return (
    <div className="mx-auto w-full max-w-3xl space-y-6">
      <div className="flex items-center gap-3">
        <Button variant="ghost" size="icon" asChild>
          <Link href="/tickets" aria-label="Back to tickets">
            <ArrowLeft className="h-4 w-4" />
          </Link>
        </Button>
        <div>
          <h1 className="text-2xl font-semibold tracking-tight">New ticket</h1>
          <p className="text-sm text-muted-foreground">
            Log a new request or issue.
          </p>
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Ticket details</CardTitle>
          <CardDescription>
            Fields marked with details help your team triage faster.
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
            <div className="space-y-2">
              <Label htmlFor="title">Title</Label>
              <Input
                id="title"
                placeholder="Short summary of the issue"
                {...register("title")}
              />
              {errors.title && (
                <p className="text-xs text-destructive">
                  {errors.title.message}
                </p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="description">Description</Label>
              <Textarea
                id="description"
                rows={6}
                placeholder="What happened? Include steps, expected vs actual, anything relevant."
                {...register("description")}
              />
              {errors.description && (
                <p className="text-xs text-destructive">
                  {errors.description.message}
                </p>
              )}
            </div>

            <div className="grid gap-5 sm:grid-cols-2">
              <div className="space-y-2">
                <Label>Priority</Label>
                <Controller
                  control={control}
                  name="priority"
                  render={({ field }) => (
                    <Select value={field.value} onValueChange={field.onChange}>
                      <SelectTrigger>
                        <SelectValue placeholder="Select priority" />
                      </SelectTrigger>
                      <SelectContent>
                        {PRIORITY_OPTIONS.map((o) => (
                          <SelectItem key={o.value} value={o.value}>
                            {o.label}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  )}
                />
              </div>

              <div className="space-y-2">
                <Label>Category</Label>
                <Controller
                  control={control}
                  name="categoryId"
                  render={({ field }) => (
                    <Select value={field.value} onValueChange={field.onChange}>
                      <SelectTrigger>
                        <SelectValue placeholder="Select category" />
                      </SelectTrigger>
                      <SelectContent>
                        {categories?.length ? (
                          categories.map((c) => (
                            <SelectItem key={c.id} value={c.id}>
                              {c.name}
                            </SelectItem>
                          ))
                        ) : (
                          <div className="px-2 py-1.5 text-sm text-muted-foreground">
                            No categories
                          </div>
                        )}
                      </SelectContent>
                    </Select>
                  )}
                />
                {errors.categoryId && (
                  <p className="text-xs text-destructive">
                    {errors.categoryId.message}
                  </p>
                )}
              </div>

              {isManagerOrAdmin && (
                <div className="space-y-2">
                  <Label>Assignee</Label>
                  <Controller
                    control={control}
                    name="assignedToId"
                    render={({ field }) => (
                      <Select
                        value={field.value}
                        onValueChange={field.onChange}
                      >
                        <SelectTrigger>
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
                    )}
                  />
                </div>
              )}

              <div className="space-y-2">
                <Label htmlFor="dueDate">Due date</Label>
                <Input id="dueDate" type="date" {...register("dueDate")} />
              </div>
            </div>

            <div className="flex justify-end gap-2 pt-2">
              <Button type="button" variant="outline" asChild>
                <Link href="/tickets">Cancel</Link>
              </Button>
              <Button type="submit" disabled={submitting}>
                {submitting && <Loader2 className="h-4 w-4 animate-spin" />}
                Create ticket
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
