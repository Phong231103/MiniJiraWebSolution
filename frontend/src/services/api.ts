import axios from 'axios';
import type { Issue, CreateIssueRequest, UpdateIssueStatusRequest } from '../types';

const api = axios.create({
  baseURL: 'http://localhost:5252/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

export const issueApi = {
  getSprintBoard: async (sprintId: string): Promise<Issue[]> => {
    const response = await api.get<Issue[]>(`/issues/sprint/${sprintId}/board`);
    return response.data;
  },

  getProjectBacklog: async (projectId: string): Promise<Issue[]> => {
    const response = await api.get<Issue[]>(`/issues/project/${projectId}/backlog`);
    return response.data;
  },

  createIssue: async (data: CreateIssueRequest): Promise<string> => {
    const response = await api.post<string>('/issues', data);
    return response.data;
  },

  updateIssueStatus: async (data: UpdateIssueStatusRequest): Promise<void> => {
    await api.patch(`/issues/${data.issueId}/status`, data);
  },
};

export default api;
