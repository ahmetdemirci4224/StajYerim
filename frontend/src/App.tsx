import type { ReactNode } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Navbar from './components/Navbar';
import Login from './pages/Login';
import Register from './pages/Register';
import VerifyEmail from './pages/VerifyEmail';
import StudentDashboard from './pages/StudentDashboard';
import CompanyDashboard from './pages/CompanyDashboard';
import PostingsList from './pages/PostingsList';
import PostingDetail from './pages/PostingDetail';

// Basit bir Route koruma bileşeni
const PrivateRoute = ({ children, allowedRole }: { children: ReactNode, allowedRole: 'Student' | 'Company' }) => {
  const token = localStorage.getItem('token');
  const role = localStorage.getItem('role');

  if (!token) return <Navigate to="/login" />;
  if (role !== allowedRole) return <Navigate to="/" />;

  return children;
};

function App() {
  return (
    <Router>
      <div className="min-h-screen bg-[radial-gradient(ellipse_at_top_right,_var(--tw-gradient-stops))] from-blue-50 via-slate-50 to-indigo-100 flex flex-col relative overflow-hidden">
        {/* Decorative background blur elements */}
        <div className="absolute top-0 -left-40 w-96 h-96 bg-purple-200 rounded-full mix-blend-multiply filter blur-3xl opacity-50 animate-blob pointer-events-none"></div>
        <div className="absolute top-0 -right-40 w-96 h-96 bg-cyan-200 rounded-full mix-blend-multiply filter blur-3xl opacity-50 animate-blob animation-delay-2000 pointer-events-none"></div>
        <div className="absolute -bottom-40 left-20 w-96 h-96 bg-pink-200 rounded-full mix-blend-multiply filter blur-3xl opacity-50 animate-blob animation-delay-4000 pointer-events-none"></div>
        
        <div className="relative z-10 flex flex-col min-h-screen">
          <Navbar />
          <main className="flex-1 w-full max-w-7xl mx-auto p-4 sm:p-6 lg:p-8">
            <Routes>
            <Route path="/" element={<Navigate to="/postings" />} />
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/verify-email" element={<VerifyEmail />} />
            <Route path="/postings" element={<PostingsList />} />
            <Route path="/postings/:id" element={<PostingDetail />} />
            
            <Route 
              path="/student-dashboard" 
              element={
                <PrivateRoute allowedRole="Student">
                  <StudentDashboard />
                </PrivateRoute>
              } 
            />
            
            <Route 
              path="/company-dashboard" 
              element={
                <PrivateRoute allowedRole="Company">
                  <CompanyDashboard />
                </PrivateRoute>
              } 
            />
          </Routes>
        </main>
        </div>
      </div>
    </Router>
  );
}

export default App;
