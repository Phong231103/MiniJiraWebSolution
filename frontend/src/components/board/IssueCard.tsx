import { Draggable } from '@hello-pangea/dnd';
import type { Issue } from '../../types';
import { Bookmark, CheckSquare, AlertCircle, ArrowUp, ArrowDown, ArrowRight, Minus } from 'lucide-react';

const TypeIcon = ({ type }: { type: string }) => {
  switch (type) {
    case 'Story': return <Bookmark size={16} className="text-[#36b37e] fill-[#36b37e]" />;
    case 'Task': return <CheckSquare size={16} className="text-[#4c9aff]" />;
    case 'Bug': return <AlertCircle size={16} className="text-[#ff5630] fill-[#ff5630]" />;
    case 'Epic': return <div className="w-4 h-4 bg-[#8777d9] rounded flex items-center justify-center"><ArrowRight size={12} className="text-white"/></div>;
    default: return <Minus size={16} />
  }
}

const PriorityIcon = ({ priority }: { priority: string }) => {
  switch (priority) {
    case 'Highest': return <ArrowUp size={16} className="text-[#ff5630]" />;
    case 'High': return <ArrowUp size={16} className="text-[#ffab00]" />;
    case 'Medium': return <ArrowRight size={16} className="text-[#ffab00]" />;
    case 'Low': return <ArrowDown size={16} className="text-[#0065ff]" />;
    case 'Lowest': return <ArrowDown size={16} className="text-[#0065ff]" />;
    default: return <Minus size={16} />
  }
}

// H6: Fix avatar display — use assigneeName to show initials even without avatar URL
function getInitials(name: string): string {
  return name
    .split(' ')
    .map(part => part.charAt(0).toUpperCase())
    .slice(0, 2)
    .join('');
}

export function IssueCard({ issue, index }: { issue: Issue, index: number }) {
  return (
    <Draggable draggableId={issue.id} index={index}>
      {(provided, snapshot) => (
        <div
          ref={provided.innerRef}
          {...provided.draggableProps}
          {...provided.dragHandleProps}
          className={`bg-white p-4 rounded-xl shadow-sm border border-slate-200 mb-3 cursor-grab active:cursor-grabbing hover:shadow-md hover:border-slate-300 transition-all duration-200 group
            ${snapshot.isDragging ? 'shadow-2xl shadow-indigo-500/20 ring-2 ring-indigo-500 rotate-3 z-50 scale-105' : ''}
          `}
          style={provided.draggableProps.style}
        >
          <p className="text-slate-700 text-sm font-medium mb-4 leading-relaxed group-hover:text-indigo-600 transition-colors">{issue.summary}</p>
          
          <div className="flex items-center justify-between mt-auto">
            <div className="flex items-center gap-2">
               <TypeIcon type={issue.type} />
               <PriorityIcon priority={issue.priority} />
               <span className="text-xs text-[#5e6c84] font-medium ml-1">{issue.key}</span>
            </div>
            {issue.assigneeAvatar ? (
               <img src={issue.assigneeAvatar} alt={issue.assigneeName} className="w-6 h-6 rounded-full shadow-sm" />
            ) : issue.assigneeName ? (
               <div className="w-6 h-6 rounded-full bg-gradient-to-tr from-[#0052cc] to-[#4c9aff] text-white flex items-center justify-center text-[10px] shadow-sm font-bold">
                 {getInitials(issue.assigneeName)}
               </div>
            ) : (
               <div className="w-6 h-6 rounded-full bg-gray-200 border border-gray-300 flex items-center justify-center text-[10px] text-gray-400 shadow-sm">
                 <Minus size={12} />
               </div>
            )}
          </div>
        </div>
      )}
    </Draggable>
  );
}
