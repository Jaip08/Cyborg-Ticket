"use client";

import { useState } from "react";
import Link from "next/link";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { ArrowLeft, Loader2 } from "lucide-react";
import { toast } from "sonner";
import { api, apiError } from "@/lib/api";
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

const emailSchema = z.object({ email: z.string().email("Enter a valid email") });
const resetSchema = z
  .object({
    token: z.string().min(1, "Paste the reset token"),
    newPassword: z.string().min(8, "At least 8 characters"),
    confirm: z.string(),
  })
  .refine((d) => d.newPassword === d.confirm, {
    message: "Passwords don't match",
    path: ["confirm"],
  });

type EmailValues = z.infer<typeof emailSchema>;
type ResetValues = z.infer<typeof resetSchema>;

export default function ForgotPasswordPage() {
  const [email, setEmail] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  const emailForm = useForm<EmailValues>({
    resolver: zodResolver(emailSchema),
  });
  const resetForm = useForm<ResetValues>({
    resolver: zodResolver(resetSchema),
  });

  async function requestReset(values: EmailValues) {
    setBusy(true);
    try {
      const { data } = await api.post<{ message: string; resetToken?: string }>(
        "/auth/forgot-password",
        values
      );
      setEmail(values.email);
      if (data.resetToken) {
        resetForm.setValue("token", data.resetToken);
        toast.success("Reset token issued (dev mode)");
      } else {
        toast.success(data.message ?? "Check your inbox for a reset link");
      }
    } catch (err) {
      toast.error(apiError(err));
    } finally {
      setBusy(false);
    }
  }

  async function submitReset(values: ResetValues) {
    if (!email) return;
    setBusy(true);
    try {
      await api.post("/auth/reset-password", {
        email,
        token: values.token,
        newPassword: values.newPassword,
      });
      toast.success("Password updated — you can sign in now");
      window.location.href = "/login";
    } catch (err) {
      toast.error(apiError(err, "Could not reset password"));
    } finally {
      setBusy(false);
    }
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle className="text-lg">
          {email ? "Set a new password" : "Reset password"}
        </CardTitle>
        <CardDescription>
          {email
            ? `We issued a reset token for ${email}.`
            : "Enter your email and we'll send a reset token."}
        </CardDescription>
      </CardHeader>
      <CardContent>
        {!email ? (
          <form
            onSubmit={emailForm.handleSubmit(requestReset)}
            className="space-y-4"
          >
            <div className="space-y-2">
              <Label htmlFor="email">Email</Label>
              <Input
                id="email"
                type="email"
                autoComplete="email"
                placeholder="you@company.com"
                {...emailForm.register("email")}
              />
              {emailForm.formState.errors.email && (
                <p className="text-xs text-destructive">
                  {emailForm.formState.errors.email.message}
                </p>
              )}
            </div>
            <Button type="submit" className="w-full" disabled={busy}>
              {busy && <Loader2 className="h-4 w-4 animate-spin" />}
              Send reset token
            </Button>
          </form>
        ) : (
          <form
            onSubmit={resetForm.handleSubmit(submitReset)}
            className="space-y-4"
          >
            <div className="space-y-2">
              <Label htmlFor="token">Reset token</Label>
              <Input id="token" {...resetForm.register("token")} />
              {resetForm.formState.errors.token && (
                <p className="text-xs text-destructive">
                  {resetForm.formState.errors.token.message}
                </p>
              )}
            </div>
            <div className="space-y-2">
              <Label htmlFor="newPassword">New password</Label>
              <Input
                id="newPassword"
                type="password"
                autoComplete="new-password"
                {...resetForm.register("newPassword")}
              />
              {resetForm.formState.errors.newPassword && (
                <p className="text-xs text-destructive">
                  {resetForm.formState.errors.newPassword.message}
                </p>
              )}
            </div>
            <div className="space-y-2">
              <Label htmlFor="confirm">Confirm password</Label>
              <Input
                id="confirm"
                type="password"
                autoComplete="new-password"
                {...resetForm.register("confirm")}
              />
              {resetForm.formState.errors.confirm && (
                <p className="text-xs text-destructive">
                  {resetForm.formState.errors.confirm.message}
                </p>
              )}
            </div>
            <Button type="submit" className="w-full" disabled={busy}>
              {busy && <Loader2 className="h-4 w-4 animate-spin" />}
              Update password
            </Button>
          </form>
        )}
        <Link
          href="/login"
          className="mt-4 flex items-center justify-center gap-1 text-sm text-muted-foreground hover:text-foreground"
        >
          <ArrowLeft className="h-3.5 w-3.5" />
          Back to sign in
        </Link>
      </CardContent>
    </Card>
  );
}
