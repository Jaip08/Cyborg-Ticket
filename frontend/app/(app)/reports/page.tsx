"use client";

import { useState } from "react";
import { Download, FileSpreadsheet, Loader2 } from "lucide-react";
import { toast } from "sonner";
import {
  downloadReport,
  useReportByEmployee,
  useReportByPriority,
  useReportByStatus,
  useReportMonthlyTrend,
} from "@/hooks/use-reports";
import { apiError } from "@/lib/api";
import { PageHeader } from "@/components/page-header";
import { MonthlyTrendChart } from "@/components/charts/monthly-trend-chart";
import { StatusDonut } from "@/components/charts/status-donut";
import { PriorityBar } from "@/components/charts/priority-bar";
import { EmployeeBar } from "@/components/charts/employee-bar";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";

function Panel({
  title,
  description,
  loading,
  hasData,
  children,
  height = 280,
}: {
  title: string;
  description: string;
  loading: boolean;
  hasData: boolean;
  children: React.ReactNode;
  height?: number;
}) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>{title}</CardTitle>
        <CardDescription>{description}</CardDescription>
      </CardHeader>
      <CardContent>
        {loading ? (
          <Skeleton className="w-full" style={{ height }} />
        ) : !hasData ? (
          <div className="flex items-center justify-center text-sm text-muted-foreground" style={{ height }}>
            No data for this report yet.
          </div>
        ) : (
          children
        )}
      </CardContent>
    </Card>
  );
}

export default function ReportsPage() {
  const status = useReportByStatus();
  const priority = useReportByPriority();
  const employee = useReportByEmployee();
  const monthly = useReportMonthlyTrend();
  const [exporting, setExporting] = useState<"csv" | "excel" | null>(null);

  async function handleExport(format: "csv" | "excel") {
    setExporting(format);
    try {
      await downloadReport(format);
      toast.success(`Exported as ${format.toUpperCase()}`);
    } catch (err) {
      toast.error(apiError(err, "Export failed"));
    } finally {
      setExporting(null);
    }
  }

  return (
    <>
      <PageHeader
        title="Reports"
        description="Operational insights across the helpdesk."
      >
        <Button
          variant="outline"
          onClick={() => handleExport("csv")}
          disabled={exporting !== null}
        >
          {exporting === "csv" ? (
            <Loader2 className="h-4 w-4 animate-spin" />
          ) : (
            <Download className="h-4 w-4" />
          )}
          CSV
        </Button>
        <Button
          onClick={() => handleExport("excel")}
          disabled={exporting !== null}
        >
          {exporting === "excel" ? (
            <Loader2 className="h-4 w-4 animate-spin" />
          ) : (
            <FileSpreadsheet className="h-4 w-4" />
          )}
          Excel
        </Button>
      </PageHeader>

      <Panel
        title="Monthly trend"
        description="Tickets created vs resolved by month"
        loading={monthly.isLoading}
        hasData={!!monthly.data?.length}
      >
        {monthly.data && <MonthlyTrendChart data={monthly.data} />}
      </Panel>

      <div className="grid gap-6 lg:grid-cols-2">
        <Panel
          title="By status"
          description="Distribution across statuses"
          loading={status.isLoading}
          hasData={!!status.data?.length}
          height={200}
        >
          {status.data && <StatusDonut data={status.data} />}
        </Panel>
        <Panel
          title="By priority"
          description="Tickets per priority level"
          loading={priority.isLoading}
          hasData={!!priority.data?.length}
          height={240}
        >
          {priority.data && <PriorityBar data={priority.data} />}
        </Panel>
      </div>

      <Panel
        title="By employee"
        description="Open and resolved tickets per team member"
        loading={employee.isLoading}
        hasData={!!employee.data?.length}
        height={320}
      >
        {employee.data && <EmployeeBar data={employee.data} />}
      </Panel>
    </>
  );
}
