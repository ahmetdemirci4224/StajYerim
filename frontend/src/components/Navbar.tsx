import { Link, useNavigate } from 'react-router-dom';
import { useState, useEffect, useRef } from 'react';
import api from '../services/api';

const Navbar = () => {
  const navigate = useNavigate();
  const token = localStorage.getItem('token');
  const role = localStorage.getItem('role');

  const [notifications, setNotifications] = useState<any[]>([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const [showDropdown, setShowDropdown] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (token) {
      fetchNotifications();
    }
  }, [token]);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setShowDropdown(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const fetchNotifications = async () => {
    try {
      const resCount = await api.get('/notification/unread-count');
      if (resCount.data.isSuccess) {
        setUnreadCount(resCount.data.data);
      }
      const resNotif = await api.get('/notification');
      if (resNotif.data.isSuccess) {
        setNotifications(resNotif.data.data);
      }
    } catch (err) {
      console.error("Bildirimler çekilemedi:", err);
    }
  };

  const handleMarkAsRead = async (id: number) => {
    try {
      await api.put(`/notification/${id}/read`);
      fetchNotifications();
    } catch (err) {
      console.error(err);
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    navigate('/login');
  };

  return (
    <nav className="bg-white/70 backdrop-blur-md border-b border-white/20 sticky top-0 z-50 shadow-sm">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16 items-center">
          <div className="flex-shrink-0 flex items-center">
            <Link to="/" className="text-2xl font-bold text-blue-600 hover:text-blue-700 transition-colors">
              StajYerim
            </Link>
          </div>
          <div className="flex items-center space-x-4 sm:space-x-8">
            {token ? (
              <>
                <Link to="/postings" className="text-gray-600 hover:text-blue-600 font-medium transition-colors hidden sm:block">
                  İlanlar
                </Link>
                <Link 
                  to={role === 'Student' ? '/student-dashboard' : '/company-dashboard'} 
                  className="text-gray-600 hover:text-blue-600 font-medium transition-colors hidden sm:block"
                >
                  Profil
                </Link>

                {/* Bildirim Dropdown */}
                <div className="relative" ref={dropdownRef}>
                  <button 
                    onClick={() => setShowDropdown(!showDropdown)} 
                    className="relative p-2 text-gray-500 hover:text-blue-600 transition-colors focus:outline-none"
                  >
                    <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9"></path></svg>
                    {unreadCount > 0 && (
                      <span className="absolute top-1 right-1 flex items-center justify-center h-4 w-4 rounded-full bg-red-500 text-[10px] font-bold text-white shadow-sm ring-2 ring-white">
                        {unreadCount}
                      </span>
                    )}
                  </button>

                  {showDropdown && (
                    <div className="absolute right-0 mt-2 w-80 bg-white rounded-xl shadow-lg py-2 border border-gray-100 z-50 animate-fade-in origin-top-right">
                      <div className="px-4 py-2 border-b border-gray-100 flex justify-between items-center">
                        <h3 className="font-bold text-gray-900">Bildirimler</h3>
                        <span className="text-xs text-gray-500">{notifications.length} Toplam</span>
                      </div>
                      <div className="max-h-96 overflow-y-auto">
                        {notifications.length > 0 ? (
                          notifications.map((notif) => (
                            <div 
                              key={notif.id} 
                              onClick={() => !notif.isRead && handleMarkAsRead(notif.id)}
                              className={`px-4 py-3 border-b border-gray-50 hover:bg-gray-50 cursor-pointer transition-colors ${!notif.isRead ? 'bg-blue-50/30' : ''}`}
                            >
                              <div className="flex justify-between items-start mb-1">
                                <h4 className={`text-sm ${!notif.isRead ? 'font-bold text-gray-900' : 'font-medium text-gray-700'}`}>
                                  {notif.title}
                                </h4>
                                {!notif.isRead && <span className="w-2 h-2 rounded-full bg-blue-600 mt-1.5 flex-shrink-0"></span>}
                              </div>
                              <p className={`text-xs ${!notif.isRead ? 'text-gray-700' : 'text-gray-500'} line-clamp-2`}>{notif.message}</p>
                              <p className="text-[10px] text-gray-400 mt-1">{new Date(notif.createdAt).toLocaleString('tr-TR')}</p>
                            </div>
                          ))
                        ) : (
                          <div className="px-4 py-8 text-center text-sm text-gray-500">
                            Okunmamış veya yeni bildiriminiz yok.
                          </div>
                        )}
                      </div>
                    </div>
                  )}
                </div>

                <button 
                  onClick={handleLogout} 
                  className="text-red-500 hover:text-red-700 font-medium transition-colors cursor-pointer hidden sm:block"
                >
                  Çıkış Yap
                </button>

                {/* Mobile Menu Icon (Placeholder for styling) */}
                <button className="sm:hidden text-gray-600">
                  <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M4 6h16M4 12h16M4 18h16"></path></svg>
                </button>
              </>
            ) : (
              <>
                <Link to="/login" className="text-gray-600 hover:text-blue-600 font-medium transition-colors">
                  Giriş Yap
                </Link>
                <Link 
                  to="/register" 
                  className="bg-blue-600 text-white px-4 py-2 rounded-lg font-medium hover:bg-blue-700 transition-colors shadow-sm"
                >
                  Kayıt Ol
                </Link>
              </>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;
