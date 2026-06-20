"use client";

import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Loader2 } from "lucide-react";
import { toast } from "sonner";
import { useUpdateTicket } from "@/hooks/use-tickets";
import { useCategories } from "@/hooks/use-meta";
import { apiError } from "@/lib/api";
import { PRIORITY_OPTIONS } from "@/lib/constants";
import type { Ticket, TicketPriority } from "@/lib/types";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
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

const schema = z.object({
  title: z.string().min(4, "Give it a clear title").max(160),
  description: z.string().min(10, "Add a bit more detail"),
  priority: z.enum(["Low", "Medium", "High", "Critical"]),
  categoryId: z.string().min(1, "Pick a category"),
  dueDate: z.string().optional(),
});

type FormValues = z.infer<typeof schema>;

function toDateInput(value: string | null) {
  return value ? value.slice(0, 10) : "";
}

export function EditTicketDialog({
  ticket,
  open,
  onOpenChange,
}: {
  ticket: Ticket;
  open: boolean;
  onOpenChange: (v: boolean) => void;
}) {
  const { data: categories } = useCategories();
  const update = useUpdateTicket(ticket.id);
  const [saving, setSaving] = useState(false);

  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      title: ticket.title,
      description: ticket.description,
      priority: ticket.priority,
      categoryId: ticket.category.id,
      dueDate: toDateInput(ticket.dueDate),
    },
  });

  async function onSubmit(values: FormValues) {
    setSaving(true);
    try {
      await update.mutateAsync({
        title: values.title,
        description: values.description,
        priority: values.priority as TicketPriority,
        categoryId: values.categoryId,
        dueDate: values.dueDate || undefined,
      });
      toast.success("Ticket updated");
      onOpenChange(false);
    } catch (err) {
      toast.error(apiError(err, "Could not save changes"));
    } finally {
      setSaving(false);
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>Edit ticket</DialogTitle>
          <DialogDescription>
            Update the details for {ticket.ticketNumber}.
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="edit-title">Title</Label>
            <Input id="edit-title" {...register("title")} />
            {errors.title && (
              <p className="text-xs text-destructive">{errors.title.message}</p>
            )}
          </div>
          <div className="space-y-2">
            <Label htmlFor="edit-description">Description</Label>
            <Textarea id="edit-description" rows={5} {...register("description")} />
            {errors.description && (
              <p className="text-xs text-destructive">
                {errors.description.message}
              </p>
            )}
          </div>
          <div className="grid gap-4 sm:grid-cols-2">
            <div className="space-y-2">
              <Label>Priority</Label>
              <Controller
                control={control}
                name="priority"
                render={({ field }) => (
                  <Select value={field.value} onValueChange={field.onChange}>
                    <SelectTrigger>
                      <SelectValue />
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
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      {categories?.map((c) => (
                        <SelectItem key={c.id} value={c.id}>
                          {c.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                )}
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="edit-due">Due date</Label>
              <Input id="edit-due" type="date" {...register("dueDate")} />
            </div>
          </div>
          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={() => onOpenChange(false)}
            >
              Cancel
            </Button>
            <Button type="submit" disabled={saving}>
              {saving && <Loader2 className="h-4 w-4 animate-spin" />}
              Save changes
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
