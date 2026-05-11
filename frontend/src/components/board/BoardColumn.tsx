import { Droppable } from '@hello-pangea/dnd';
import { IssueCard } from './IssueCard';
import type { Issue } from '../../types';

interface BoardColumnProps {
  id: string;
  title: string;
  issues: Issue[];
}

export function BoardColumn({ id, title, issues }: BoardColumnProps) {
  return (
    <div className="flex flex-col w-[320px] flex-shrink-0 bg-slate-100/60 backdrop-blur-sm border border-slate-200/50 rounded-2xl max-h-full shadow-sm overflow-hidden">
      <div className="p-4 pb-3 flex justify-between items-center bg-white/40 sticky top-0 z-10 border-b border-slate-200/50">
        <span className="text-xs font-bold text-slate-500 uppercase tracking-widest">{title}</span>
        <span className="bg-slate-200/80 text-slate-600 px-2.5 py-0.5 rounded-full text-[11px] font-bold shadow-sm">{issues.length}</span>
      </div>

      <Droppable droppableId={id}>
        {(provided, snapshot) => (
          <div
            ref={provided.innerRef}
            {...provided.droppableProps}
            className={`flex-1 p-2 overflow-y-auto min-h-[150px] transition-colors duration-200 rounded-b-lg ${snapshot.isDraggingOver ? 'bg-[#ebecf0]' : ''}`}
          >
            {issues.map((issue, index) => (
              <IssueCard key={issue.id} issue={issue} index={index} />
            ))}
            {provided.placeholder}
          </div>
        )}
      </Droppable>
    </div>
  );
}
