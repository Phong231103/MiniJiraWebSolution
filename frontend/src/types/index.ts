export type IssueStatus = "Backlog" | "ToDo" | "InProgress" | "Review" | "Done";
export type IssuePriority = "Lowest" | "Low" | "Medium" | "High" | "Highest";
export type IssueType = "Epic" | "Story" | "Task" | "Bug";

export interface Issue {
  id: string;
  key: string;
  summary: string;
  description?: string;
  type: IssueType;
  priority: IssuePriority;
  status: IssueStatus;
  assigneeId?: string;
  assigneeName?: string;
  assigneeAvatar?: string;
  reporterId: string;
  projectId: string;
  sprintId?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateIssueRequest {
  summary: string;
  description?: string;
  type: IssueType;
  priority: IssuePriority;
  assigneeId?: string;
  reporterId: string;
  projectId: string;
  sprintId?: string;
}

export interface UpdateIssueStatusRequest {
  issueId: string;
  status: IssueStatus;
}
