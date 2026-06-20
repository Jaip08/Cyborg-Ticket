"use client";

import { useRef } from "react";
import { FileText, Loader2, Paperclip, Upload } from "lucide-react";
import { toast } from "sonner";
import { useAttachments, useUploadAttachment } from "@/hooks/use-tickets";
import { apiError } from "@/lib/api";
import { formatBytes, formatDate } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";

export function AttachmentsPanel({ ticketId }: { ticketId: string }) {
  const { data, isLoading } = useAttachments(ticketId);
  const upload = useUploadAttachment(ticketId);
  const inputRef = useRef<HTMLInputElement>(null);

  async function onPick(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (!file) return;
    try {
      await upload.mutateAsync(file);
      toast.success("File uploaded");
    } catch (err) {
      toast.error(apiError(err, "Upload failed"));
    } finally {
      if (inputRef.current) inputRef.current.value = "";
    }
  }

  return (
    <div className="space-y-3">
      <input
        ref={inputRef}
        type="file"
        className="hidden"
        onChange={onPick}
      />
      <Button
        variant="outline"
        size="sm"
        className="w-full"
        onClick={() => inputRef.current?.click()}
        disabled={upload.isPending}
      >
        {upload.isPending ? (
          <Loader2 className="h-4 w-4 animate-spin" />
        ) : (
          <Upload className="h-4 w-4" />
        )}
        Upload file
      </Button>

      {isLoading ? (
        <div className="space-y-2">
          {Array.from({ length: 2 }).map((_, i) => (
            <Skeleton key={i} className="h-12 w-full" />
          ))}
        </div>
      ) : !data || data.length === 0 ? (
        <div className="flex flex-col items-center gap-1.5 py-6 text-center text-sm text-muted-foreground">
          <Paperclip className="h-5 w-5" />
          No attachments
        </div>
      ) : (
        <ul className="space-y-2">
          {data.map((file) => (
            <li
              key={file.id}
              className="flex items-center gap-3 rounded-md border p-2.5"
            >
              <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-md bg-muted">
                <FileText className="h-4 w-4 text-muted-foreground" />
              </div>
              <div className="min-w-0 flex-1">
                <p className="truncate text-sm font-medium">{file.fileName}</p>
                <p className="text-xs text-muted-foreground">
                  {formatBytes(file.fileSize)} · {formatDate(file.createdAt)}
                </p>
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
