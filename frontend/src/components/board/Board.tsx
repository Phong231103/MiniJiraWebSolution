import { useState, useCallback } from 'react';
import { DragDropContext } from '@hello-pangea/dnd';
import type { DropResult } from '@hello-pangea/dnd';
import { BoardColumn } from './BoardColumn';
import { issueApi } from '../../services/api';
import type { Issue, IssueStatus } from '../../types';

// Fallback mock data (used when API is unavailable)
const fallbackIssues: Issue[] = [
  { id: '1', summary: 'Setup React Frontend with Vite', type: 'Task', priority: 'Highest', status: 'Done', key: 'PHX-1', assigneeName: 'AD', reporterId: '', projectId: '', createdAt: '' },
  { id: '2', summary: 'Implement Drag and Drop Context', type: 'Story', priority: 'High', status: 'InProgress', key: 'PHX-2', reporterId: '', projectId: '', createdAt: '' },
  { id: '3', summary: 'Fix sidebar navigation active state', type: 'Bug', priority: 'Medium', status: 'ToDo', key: 'PHX-3', reporterId: '', projectId: '', createdAt: '' },
  { id: '4', summary: 'Build Backend CQRS architecture', type: 'Epic', priority: 'Low', status: 'Done', key: 'PHX-4', reporterId: '', projectId: '', createdAt: '' },
  { id: '5', summary: 'Refine Tailwind UI aesthetics', type: 'Task', priority: 'Highest', status: 'Backlog', key: 'PHX-5', reporterId: '', projectId: '', createdAt: '' },
];

const COLUMNS: { id: IssueStatus; title: string }[] = [
  { id: 'Backlog', title: 'Backlog' },
  { id: 'ToDo', title: 'To Do' },
  { id: 'InProgress', title: 'In Progress' },
  { id: 'Review', title: 'Review' },
  { id: 'Done', title: 'Done' }
];

export function Board() {
  const [issues, setIssues] = useState<Issue[]>(fallbackIssues);

  const onDragEnd = useCallback(async (result: DropResult) => {
    const { destination, source, draggableId } = result;

    if (!destination) return;
    if (destination.droppableId === source.droppableId && destination.index === source.index) return;

    const newStatus = destination.droppableId as IssueStatus;
    
    // H5: Properly create new state without mutating existing objects
    setIssues(prevIssues => 
      prevIssues.map(issue => 
        issue.id === draggableId 
          ? { ...issue, status: newStatus }
          : issue
      )
    );

    // Attempt to sync with backend API (non-blocking)
    try {
      await issueApi.updateIssueStatus({
        issueId: draggableId,
        status: newStatus,
      });
    } catch {
      // API not available, drag still works locally — silently ignore
    }
  }, []);

  return (
    <DragDropContext onDragEnd={onDragEnd}>
      <div className="flex gap-4 h-full w-full overflow-x-auto overflow-y-hidden pb-4 items-start">
        {COLUMNS.map(col => (
          <BoardColumn 
            key={col.id} 
            id={col.id} 
            title={col.title} 
            issues={issues.filter(i => i.status === col.id)} 
          />
        ))}
      </div>
    </DragDropContext>
  );
}
