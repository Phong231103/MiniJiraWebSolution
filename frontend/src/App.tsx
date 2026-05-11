import { useState } from 'react';
import { LayoutDashboard, CheckSquare, Users, Settings, Search, Bell, X, Plus } from 'lucide-react';
import { Board } from './components/board/Board';
import type { IssueType, IssuePriority } from './types';

function App() {
  const [showCreateModal, setShowCreateModal] = useState(false);

  return (
    <div className="flex h-screen w-full bg-gradient-to-br from-[#f8f9fa] via-[#f1f5f9] to-[#e2e8f0] overflow-hidden text-slate-800 font-sans">
      {/* Sidebar Navigation */}
      <aside className="w-16 md:w-64 bg-slate-900 text-slate-300 flex flex-col transition-all duration-300 shadow-2xl z-20 relative overflow-hidden">
        {/* Decorative background glow */}
        <div className="absolute top-0 left-0 w-full h-32 bg-indigo-500/20 blur-[50px] -z-10 rounded-full" />
        
        <div className="h-16 flex items-center justify-center md:justify-start md:px-6 font-bold text-xl tracking-wide border-b border-slate-800 text-white">
           <span className="hidden md:flex items-center gap-2">
             <div className="w-8 h-8 rounded-lg bg-indigo-500 flex items-center justify-center shadow-lg shadow-indigo-500/30 font-black">J</div>
             JiraClone
           </span>
           <span className="md:hidden">JC</span>
        </div>
        
        <nav className="flex-1 py-6 flex flex-col gap-1 px-3">
          <NavItem icon={<LayoutDashboard size={20} />} label="Board" active />
          <NavItem icon={<CheckSquare size={20} />} label="Issues" />
          <NavItem icon={<Users size={20} />} label="Members" />
        </nav>

        <div className="p-4 border-t border-slate-800 mt-auto">
          <NavItem icon={<Settings size={20} />} label="Project Settings" />
        </div>
      </aside>

      {/* Main Content Area */}
      <main className="flex-1 flex flex-col h-full relative overflow-hidden">
        {/* Top Header */}
        <header className="h-16 bg-white/60 backdrop-blur-xl border-b border-white/40 flex items-center justify-between px-8 z-10 sticky top-0 shadow-sm">
          <div className="flex items-center gap-4 w-[400px]">
             <div className="relative w-full group">
               <Search className="absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-400 group-focus-within:text-indigo-500 transition-colors" size={18} />
               <input 
                 type="text" 
                 placeholder="Search issues, epics..." 
                 className="w-full bg-slate-100/50 hover:bg-slate-100 focus:bg-white focus:ring-2 focus:ring-indigo-500/50 border border-transparent rounded-full py-2 pl-10 pr-4 text-sm transition-all outline-none shadow-inner"
               />
             </div>
          </div>
          <div className="flex items-center gap-5">
             <button className="relative text-slate-500 hover:text-slate-800 transition-colors">
               <Bell size={20} />
               <span className="absolute top-0 right-0 w-2 h-2 bg-rose-500 rounded-full border-2 border-white"></span>
             </button>
             <div className="w-9 h-9 border-2 border-white rounded-full bg-gradient-to-tr from-indigo-600 to-cyan-400 text-white flex items-center justify-center font-bold text-sm shadow-md cursor-pointer hover:shadow-lg hover:scale-105 transition-all">
               AD
             </div>
          </div>
        </header>

        {/* Board Content Workspace */}
        <div className="flex-1 p-8 overflow-hidden flex flex-col">
          <div className="mb-8 flex justify-between items-end">
            <div>
              <div className="flex items-center gap-2 text-sm text-slate-500 font-medium mb-2">
                <span className="bg-slate-200/50 px-2 py-0.5 rounded-md">Projects</span>
                <span>/</span>
                <span className="text-slate-700">Phoenix</span>
                <span>/</span>
                <span className="text-indigo-600 font-semibold bg-indigo-50 px-2 py-0.5 rounded-md">Active Sprint</span>
              </div>
              <h1 className="text-3xl font-bold tracking-tight text-slate-800">Sprint Board</h1>
            </div>
            
            <button 
              onClick={() => setShowCreateModal(true)}
              className="group bg-indigo-600 hover:bg-indigo-700 text-white px-5 py-2.5 rounded-xl font-semibold text-sm shadow-lg shadow-indigo-600/30 transition-all hover:-translate-y-0.5 active:scale-95 flex items-center gap-2"
            >
              <Plus size={18} className="transition-transform group-hover:rotate-90" />
              Create Issue
            </button>
          </div>
          
          {/* Board Area */}
          <div className="flex-1 overflow-hidden flex bg-white/40 rounded-xl p-2 pt-4">
             <Board />
          </div>
        </div>
      </main>

      {/* L2: Create Issue Modal */}
      {showCreateModal && (
        <CreateIssueModal onClose={() => setShowCreateModal(false)} />
      )}
    </div>
  );
}

function NavItem({ icon, label, active = false }: { icon: React.ReactNode, label: string, active?: boolean }) {
  return (
    <div className={`flex items-center gap-3 px-3 py-2 rounded-md cursor-pointer transition-colors ${active ? 'bg-white/20 text-white font-medium' : 'text-blue-100 hover:bg-white/10 hover:text-white'}`}>
       {icon}
       <span className="hidden md:block text-sm">{label}</span>
    </div>
  )
}

function CreateIssueModal({ onClose }: { onClose: () => void }) {
  const [summary, setSummary] = useState('');
  const [description, setDescription] = useState('');
  const [type, setType] = useState<IssueType>('Task');
  const [priority, setPriority] = useState<IssuePriority>('Medium');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // In a full implementation, this would call the API
    console.log('Create issue:', { summary, description, type, priority });
    onClose();
  };

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 backdrop-blur-sm">
      <div className="bg-white rounded-xl shadow-2xl w-full max-w-lg mx-4 overflow-hidden">
        <div className="flex items-center justify-between px-6 py-4 border-b border-[#dfe1e6]">
          <h2 className="text-lg font-semibold text-[#172b4d]">Create Issue</h2>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600 transition-colors">
            <X size={20} />
          </button>
        </div>
        <form onSubmit={handleSubmit} className="p-6 space-y-4">
          <div>
            <label className="block text-sm font-medium text-[#5e6c84] mb-1">Summary *</label>
            <input
              type="text"
              value={summary}
              onChange={(e) => setSummary(e.target.value)}
              required
              className="w-full border border-[#dfe1e6] rounded-md px-3 py-2 text-sm focus:ring-2 focus:ring-[#4c9aff] focus:border-transparent outline-none transition-all"
              placeholder="What needs to be done?"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-[#5e6c84] mb-1">Description</label>
            <textarea
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              rows={3}
              className="w-full border border-[#dfe1e6] rounded-md px-3 py-2 text-sm focus:ring-2 focus:ring-[#4c9aff] focus:border-transparent outline-none transition-all resize-none"
              placeholder="Add a description..."
            />
          </div>
          <div className="flex gap-4">
            <div className="flex-1">
              <label className="block text-sm font-medium text-[#5e6c84] mb-1">Type</label>
              <select
                value={type}
                onChange={(e) => setType(e.target.value as IssueType)}
                className="w-full border border-[#dfe1e6] rounded-md px-3 py-2 text-sm focus:ring-2 focus:ring-[#4c9aff] focus:border-transparent outline-none transition-all bg-white"
              >
                <option value="Task">Task</option>
                <option value="Story">Story</option>
                <option value="Bug">Bug</option>
                <option value="Epic">Epic</option>
              </select>
            </div>
            <div className="flex-1">
              <label className="block text-sm font-medium text-[#5e6c84] mb-1">Priority</label>
              <select
                value={priority}
                onChange={(e) => setPriority(e.target.value as IssuePriority)}
                className="w-full border border-[#dfe1e6] rounded-md px-3 py-2 text-sm focus:ring-2 focus:ring-[#4c9aff] focus:border-transparent outline-none transition-all bg-white"
              >
                <option value="Highest">Highest</option>
                <option value="High">High</option>
                <option value="Medium">Medium</option>
                <option value="Low">Low</option>
                <option value="Lowest">Lowest</option>
              </select>
            </div>
          </div>
          <div className="flex justify-end gap-3 pt-2">
            <button 
              type="button" 
              onClick={onClose}
              className="px-4 py-2 text-sm font-medium text-[#5e6c84] hover:bg-gray-100 rounded-md transition-colors"
            >
              Cancel
            </button>
            <button 
              type="submit"
              disabled={!summary.trim()}
              className="px-4 py-2 text-sm font-medium text-white bg-[#0052cc] hover:bg-[#0747a6] rounded-md transition-all active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Create
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default App;
