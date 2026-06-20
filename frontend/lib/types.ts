export type Role = "Admin" | "Manager" | "Employee";

export type TicketStatus =
  | "Open"
  | "InProgress"
  | "OnHold"
  | "Resolved"
  | "Closed";

export type TicketPriority = "Low" | "Medium" | "High" | "Critical";

export interface User {
  id: string;
  fullName: string;
  email: string;
  role: Role;
}

export interface ManagedUser extends User {
  isActive: boolean;
}

export interface AuthResponse {
  token: string;
  expiresAtUtc: string;
  user: User;
}

export interface Category {
  id: string;
  name: string;
  description?: string;
}

export interface UserRef {
  id: string;
  fullName: string;
}

export interface Ticket {
  id: string;
  ticketNumber: string;
  title: string;
  description: string;
  status: TicketStatus;
  priority: TicketPriority;
  dueDate: string | null;
  createdAt: string;
  resolvedAt: string | null;
  closedAt: string | null;
  category: Category;
  createdBy: UserRef;
  assignedTo: UserRef | null;
}

export interface Paged<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface CommentAuthor {
  id: string;
  fullName: string;
  role: Role;
}

export interface Comment {
  id: string;
  content: string;
  isInternal: boolean;
  author: CommentAuthor;
  createdAt: string;
}

export interface ActivityEntry {
  id: string;
  action: string;
  description: string;
  user: { fullName: string };
  createdAt: string;
}

export interface Attachment {
  id: string;
  fileName: string;
  contentType: string;
  fileSize: number;
  createdAt: string;
}

export interface DashboardStats {
  totalTickets: number;
  openTickets: number;
  inProgressTickets: number;
  resolvedTickets: number;
  closedTickets: number;
  highPriorityTickets: number;
  overdueTickets: number;
  unassignedTickets: number;
}

export interface MonthlyPoint {
  month: string;
  created: number;
  resolved: number;
}

export interface StatusBreakdown {
  status: TicketStatus;
  count: number;
}

export interface PriorityBreakdown {
  priority: TicketPriority;
  count: number;
}

export interface EmployeePerformance {
  userId: string;
  fullName: string;
  assigned: number;
  resolved: number;
  open: number;
}

export interface TicketFilters {
  search?: string;
  status?: TicketStatus | "";
  priority?: TicketPriority | "";
  categoryId?: string;
  assignedToId?: string;
  page?: number;
  pageSize?: number;
}

export interface CreateTicketInput {
  title: string;
  description: string;
  priority: TicketPriority;
  categoryId: string;
  assignedToId?: string;
  dueDate?: string;
}

export interface UpdateTicketInput {
  title: string;
  description: string;
  priority: TicketPriority;
  categoryId: string;
  dueDate?: string;
}
