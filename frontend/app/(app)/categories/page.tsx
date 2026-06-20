"use client";

import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Loader2, Plus, Tags } from "lucide-react";
import { toast } from "sonner";
import { useRouter } from "next/navigation";
import { useCategories, useCreateCategory } from "@/hooks/use-meta";
import { apiError } from "@/lib/api";
import { useAuth } from "@/lib/auth";
import { PageHeader } from "@/components/page-header";
import { EmptyState } from "@/components/empty-state";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Skeleton } from "@/components/ui/skeleton";
import { Textarea } from "@/components/ui/textarea";

const schema = z.object({
  name: z.string().min(2, "Name is required"),
  description: z.string().optional(),
});

type FormValues = z.infer<typeof schema>;

export default function CategoriesPage() {
  const router = useRouter();
  const { ready, isManagerOrAdmin } = useAuth();
  const { data, isLoading } = useCategories();
  const createCategory = useCreateCategory();
  const [open, setOpen] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({ resolver: zodResolver(schema) });

  useEffect(() => {
    if (ready && !isManagerOrAdmin) router.replace("/dashboard");
  }, [ready, isManagerOrAdmin, router]);

  async function onSubmit(values: FormValues) {
    try {
      await createCategory.mutateAsync(values);
      toast.success("Category created");
      reset();
      setOpen(false);
    } catch (err) {
      toast.error(apiError(err, "Could not create category"));
    }
  }

  if (ready && !isManagerOrAdmin) return null;

  return (
    <>
      <PageHeader
        title="Categories"
        description="Organize tickets into categories for triage and reporting."
      >
        <Dialog open={open} onOpenChange={setOpen}>
          <DialogTrigger asChild>
            <Button>
              <Plus className="h-4 w-4" />
              New category
            </Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>New category</DialogTitle>
              <DialogDescription>
                Categories help route and report on tickets.
              </DialogDescription>
            </DialogHeader>
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="name">Name</Label>
                <Input id="name" placeholder="e.g. Hardware" {...register("name")} />
                {errors.name && (
                  <p className="text-xs text-destructive">
                    {errors.name.message}
                  </p>
                )}
              </div>
              <div className="space-y-2">
                <Label htmlFor="description">Description</Label>
                <Textarea
                  id="description"
                  rows={3}
                  placeholder="Optional"
                  {...register("description")}
                />
              </div>
              <DialogFooter>
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => setOpen(false)}
                >
                  Cancel
                </Button>
                <Button type="submit" disabled={isSubmitting}>
                  {isSubmitting && <Loader2 className="h-4 w-4 animate-spin" />}
                  Create
                </Button>
              </DialogFooter>
            </form>
          </DialogContent>
        </Dialog>
      </PageHeader>

      {isLoading ? (
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {Array.from({ length: 6 }).map((_, i) => (
            <Skeleton key={i} className="h-28" />
          ))}
        </div>
      ) : !data || data.length === 0 ? (
        <Card>
          <CardContent className="p-0">
            <EmptyState
              icon={Tags}
              title="No categories yet"
              description="Create your first category to start organizing tickets."
            />
          </CardContent>
        </Card>
      ) : (
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {data.map((c) => (
            <Card key={c.id}>
              <CardContent className="space-y-1.5 p-5">
                <div className="flex items-center gap-2">
                  <div className="flex h-8 w-8 items-center justify-center rounded-md bg-muted">
                    <Tags className="h-4 w-4 text-muted-foreground" />
                  </div>
                  <h3 className="font-medium">{c.name}</h3>
                </div>
                <p className="text-sm text-muted-foreground">
                  {c.description || "No description"}
                </p>
              </CardContent>
            </Card>
          ))}
        </div>
      )}
    </>
  );
}
