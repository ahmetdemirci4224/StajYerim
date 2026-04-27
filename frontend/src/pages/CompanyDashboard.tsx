import { useEffect, useState, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import { jwtDecode } from 'jwt-decode';

const CompanyDashboard = () => {
  const navigate = useNavigate();
  const [invitations, setInvitations] = useState<any[]>([]);
  const [myPostings, setMyPostings] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [isSearching, setIsSearching] = useState(false);
  const [isCreating, setIsCreating] = useState(false);
  const [companyName, setCompanyName] = useState('Şirket');
  const [isEditingProfile, setIsEditingProfile] = useState(false);
  const [profile, setProfile] = useState<any>({
    industry: 'Yazılım ve Bilişim',
    websiteUrl: 'www.sirket.com',
    foundedYear: 2010,
    description: 'Yenilikçi teknolojiler geliştiriyoruz.'
  });

  const handleEditProfile = () => {
    setIsEditingProfile(!isEditingProfile);
  };

  // ... fetchInvitations, etc. (we'll place the new section above invitations)
  const [showModal, setShowModal] = useState(false);
  const [newPosting, setNewPosting] = useState({
    title: '',
    description: '',
    location: '',
    workModel: 'Remote'
  });
  
  const [selectedPostingId, setSelectedPostingId] = useState<number | null>(null);
  const [editingPostingId, setEditingPostingId] = useState<number | null>(null);

  const [showSearchModal, setShowSearchModal] = useState(false);
  const [studentResults, setStudentResults] = useState<any[]>([]);
  const [selectedStudentForInvite, setSelectedStudentForInvite] = useState<any>(null);
  const [inviteMessage, setInviteMessage] = useState('');
  const [invitePostingId, setInvitePostingId] = useState<number | ''>('');
  const [sendingInvite, setSendingInvite] = useState(false);

  // Chatbot States
  const [isChatOpen, setIsChatOpen] = useState(false);
  const [chatMessages, setChatMessages] = useState<{role: 'user'|'assistant', text: string, matches?: any[]}[]>([
    { role: 'assistant', text: 'Merhaba! Ben Kurumsal İşe Alım Asistanınız. Platformdaki yetenekli adayları bulmanıza yardımcı olabilirim.' }
  ]);
  const [currentMessage, setCurrentMessage] = useState('');
  const [isChatLoading, setIsChatLoading] = useState(false);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  useEffect(() => {
    if (isChatOpen) {
      scrollToBottom();
    }
  }, [chatMessages, isChatOpen]);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const decoded: any = jwtDecode(token);
        if (decoded.FullName) {
          setCompanyName(decoded.FullName);
        }
      } catch (err) {
        console.error('Token okunamadı', err);
      }
    }
    fetchInvitations();
    fetchMyPostings();
  }, []);

  const fetchInvitations = async () => {
    try {
      const response = await api.get('/invitation/company-invitations');
      if (response.data.isSuccess) {
        setInvitations(response.data.data);
      }
    } catch (error) {
      console.error('Veriler yüklenirken hata oluştu', error);
      // Fallback dummy data for visualization
      setInvitations([
          { invitationId: 1, studentName: 'Ahmet Yılmaz', postingTitle: 'Frontend Geliştirici Stajyeri', status: 'Pending', sentAt: '2026-04-09T10:00:00Z' },
          { invitationId: 2, studentName: 'Ayşe Demir', postingTitle: 'Backend .NET Stajyeri', status: 'Accepted', sentAt: '2026-04-08T14:30:00Z' },
          { invitationId: 3, studentName: 'Mehmet Kaya', postingTitle: 'Mobil Geliştirici', status: 'Declined', sentAt: '2026-04-07T09:15:00Z' }
      ]);
    } finally {
      setLoading(false);
    }
  };

  const fetchMyPostings = async () => {
    try {
      const response = await api.get('/jobposting/my-postings');
      if (response.data.isSuccess) {
        setMyPostings(response.data.data);
      }
    } catch (err) {
      console.error('İlanlar çekilemedi', err);
      // Fallback
      setMyPostings([
        { id: 991, title: 'Frontend Developer', status: 'Open', applicantCount: 3 },
        { id: 992, title: 'Backend Developer', status: 'Closed', applicantCount: 0 }
      ]);
    }
  };

  const handleViewApplicants = async (postingId: number) => {
    setSelectedPostingId(selectedPostingId === postingId ? null : postingId);
  };

  const handleAISearch = async () => {
    setIsSearching(true);
    try {
      const response = await api.get('/invitation/search-students');
      if (response.data.isSuccess) {
        setStudentResults(response.data.data);
        setShowSearchModal(true);
      }
    } catch (err) {
      console.error('Adaylar aranırken hata oluştu:', err);
      alert('Aday araması sırasında bir hata oluştu.');
    } finally {
      setIsSearching(false);
    }
  };

  const handleSendMessage = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!currentMessage.trim()) return;

    const userMsg = currentMessage.trim();
    setChatMessages(prev => [...prev, { role: 'user', text: userMsg }]);
    setCurrentMessage('');
    setIsChatLoading(true);

    try {
      const response = await api.post('/chatbot/advice', { userMessage: userMsg });
      if (response.data.isSuccess) {
        const { assistantReply, matches } = response.data.data;
        setChatMessages(prev => [...prev, { role: 'assistant', text: assistantReply, matches }]);
      } else {
        setChatMessages(prev => [...prev, { role: 'assistant', text: 'Üzgünüm, şu an size yardımcı olamıyorum.' }]);
      }
    } catch (error) {
      setChatMessages(prev => [...prev, { role: 'assistant', text: 'Bağlantı hatası oluştu. Lütfen daha sonra tekrar dene.' }]);
    } finally {
      setIsChatLoading(false);
    }
  };

  const handleSendInvite = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedStudentForInvite) return;
    
    setSendingInvite(true);
    try {
      const payload = {
        studentId: selectedStudentForInvite.studentId,
        postingId: invitePostingId === '' ? null : Number(invitePostingId),
        message: inviteMessage
      };
      
      const response = await api.post('/invitation/send', payload);
      if (response.data.isSuccess) {
        alert('Davet başarıyla gönderildi!');
        setSelectedStudentForInvite(null);
        setInviteMessage('');
        setInvitePostingId('');
        fetchInvitations(); // Refresh the list
      }
    } catch (err: any) {
      alert(`Hata: ${err.response?.data?.message || 'Davet gönderilemedi.'}`);
    } finally {
      setSendingInvite(false);
    }
  };

  const handleEditPosting = (posting: any) => {
    setEditingPostingId(posting.id);
    setNewPosting({
      title: posting.title,
      description: posting.description || '',
      location: posting.location || '',
      workModel: posting.workModel || 'Remote'
    });
    setShowModal(true);
  };

  const handleClosePosting = async (id: number) => {
    if (!window.confirm("Bu ilanı yayından kaldırmak istediğinize emin misiniz?")) return;
    
    try {
      const response = await api.put(`/jobposting/close/${id}`);
      if (response.data.isSuccess) {
        alert("İlan yayından kaldırıldı.");
        fetchMyPostings();
      }
    } catch (err) {
      alert("İlan kapatılırken bir hata oluştu.");
    }
  };

  const handleNewPostingSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsCreating(true);
    try {
      if (editingPostingId) {
        const response = await api.put(`/jobposting/update/${editingPostingId}`, newPosting);
        if(response.data.isSuccess) {
          alert('İlanınız başarıyla güncellendi!');
          setShowModal(false);
          setEditingPostingId(null);
          setNewPosting({ title: '', description: '', location: '', workModel: 'Remote' });
          fetchMyPostings();
        }
      } else {
        const response = await api.post('/jobposting/create', newPosting);
        if(response.data.isSuccess) {
          alert('İlanınız başarıyla yayına alındı!');
          setShowModal(false);
          setNewPosting({ title: '', description: '', location: '', workModel: 'Remote' });
          fetchMyPostings();
        }
      }
    } catch (err: any) {
      alert(`Hata: ${err.response?.data?.message || 'İşlem başarısız.'}`);
    } finally {
      setIsCreating(false);
    }
  };

  const openNewPostingModal = () => {
    setEditingPostingId(null);
    setNewPosting({ title: '', description: '', location: '', workModel: 'Remote' });
    setShowModal(true);
  };

  const getStatusBadge = (status: string) => {
    switch (status.toLowerCase()) {
      case 'pending':
        return <span className="bg-yellow-100 text-yellow-800 px-3 py-1 rounded-full text-xs font-medium border border-yellow-200">Bekliyor</span>;
      case 'accepted':
        return <span className="bg-green-100 text-green-800 px-3 py-1 rounded-full text-xs font-medium border border-green-200">Kabul Edildi</span>;
      case 'declined':
        return <span className="bg-red-100 text-red-800 px-3 py-1 rounded-full text-xs font-medium border border-red-200">Reddedildi</span>;
      default:
        return <span className="bg-gray-100 text-gray-800 px-3 py-1 rounded-full text-xs font-medium border border-gray-200">{status}</span>;
    }
  };

  return (
    <div className="space-y-8 relative">
      {/* Search / Invite Modal */}
      {showSearchModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm animate-fade-in p-4">
          <div className="bg-white rounded-2xl shadow-2xl w-full max-w-4xl max-h-[90vh] overflow-hidden flex flex-col animate-slide-up">
            <div className="p-6 border-b border-gray-100 flex justify-between items-center bg-gray-50">
              <h2 className="text-xl font-bold text-gray-900">Aday Havuzu (AI Eşleşmeleri)</h2>
              <button onClick={() => setShowSearchModal(false)} className="text-gray-400 hover:text-red-500 transition-colors">
                <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12"></path></svg>
              </button>
            </div>
            
            <div className="p-6 overflow-y-auto flex-1 space-y-4">
              {studentResults.length > 0 ? (
                studentResults.map(student => (
                  <div key={student.studentId} className="border border-gray-200 rounded-xl p-4 flex flex-col md:flex-row md:items-center justify-between gap-4 bg-white hover:shadow-md transition-shadow">
                    <div className="flex items-center gap-4">
                      <div className="w-12 h-12 rounded-full bg-blue-100 text-blue-700 font-bold flex items-center justify-center text-lg">
                        {student.fullName.charAt(0)}
                      </div>
                      <div>
                        <h3 className="font-bold text-gray-900">{student.fullName}</h3>
                        <p className="text-sm text-gray-500">{student.university} • {student.department}</p>
                        {student.gpa && <p className="text-xs text-gray-400 mt-1">GPA: {student.gpa}</p>}
                      </div>
                    </div>
                    
                    <div className="flex flex-col sm:flex-row gap-2 w-full md:w-auto">
                      <button 
                        onClick={() => setSelectedStudentForInvite(selectedStudentForInvite?.studentId === student.studentId ? null : student)}
                        className="bg-indigo-50 hover:bg-indigo-100 text-indigo-600 font-medium py-2 px-4 rounded-lg transition-colors border border-indigo-100 whitespace-nowrap text-sm"
                      >
                        {selectedStudentForInvite?.studentId === student.studentId ? 'İptal' : 'Davet Gönder'}
                      </button>
                    </div>

                    {selectedStudentForInvite?.studentId === student.studentId && (
                      <div className="w-full md:w-full mt-4 bg-gray-50 p-4 rounded-lg border border-gray-200 animate-slide-up md:col-span-full">
                        <form onSubmit={handleSendInvite} className="space-y-3">
                          <h4 className="font-bold text-gray-800 text-sm mb-2">{student.fullName}'a Davet Gönder</h4>
                          <div>
                            <label className="block text-xs font-medium text-gray-700 mb-1">İlgili İlan (Opsiyonel)</label>
                            <select 
                              value={invitePostingId} 
                              onChange={e => setInvitePostingId(e.target.value === '' ? '' : Number(e.target.value))}
                              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-1 focus:ring-indigo-500 outline-none text-sm"
                            >
                              <option value="">Genel Davet</option>
                              {myPostings.map(p => (
                                <option key={p.id} value={p.id}>{p.title}</option>
                              ))}
                            </select>
                          </div>
                          <div>
                            <label className="block text-xs font-medium text-gray-700 mb-1">Özel Mesaj</label>
                            <textarea 
                              required
                              value={inviteMessage}
                              onChange={e => setInviteMessage(e.target.value)}
                              placeholder="Merhaba, profilinizi inceledik ve takımımızda görmekten mutluluk duyarız..."
                              rows={3}
                              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-1 focus:ring-indigo-500 outline-none resize-none text-sm"
                            ></textarea>
                          </div>
                          <button 
                            type="submit" 
                            disabled={sendingInvite}
                            className="w-full bg-indigo-600 hover:bg-indigo-700 text-white font-medium py-2 rounded-md transition-colors text-sm flex justify-center items-center gap-2"
                          >
                            {sendingInvite ? (
                              <svg className="animate-spin h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle><path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path></svg>
                            ) : "Daveti İlet"}
                          </button>
                        </form>
                      </div>
                    )}
                  </div>
                ))
              ) : (
                <div className="text-center py-12 text-gray-500">
                  Şu anda uygun aday bulunamadı.
                </div>
              )}
            </div>
          </div>
        </div>
      )}

      {/* Modal Overlay */}
      {showModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm animate-fade-in p-4">
          <div className="bg-white rounded-2xl shadow-2xl w-full max-w-lg overflow-hidden animate-slide-up">
            <div className="p-6 border-b border-gray-100 flex justify-between items-center bg-gray-50">
              <h2 className="text-xl font-bold text-gray-900">Yeni Staj İlanı Oluştur</h2>
              <button onClick={() => setShowModal(false)} className="text-gray-400 hover:text-red-500 transition-colors">
                <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12"></path></svg>
              </button>
            </div>
            
            <form onSubmit={handleNewPostingSubmit} className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">İlan Başlığı</label>
                <input type="text" required value={newPosting.title} onChange={e => setNewPosting({...newPosting, title: e.target.value})} placeholder="Örn: Frontend Geliştirici Stajyeri" className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none" />
              </div>
              
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Açıklama</label>
                <textarea required rows={4} value={newPosting.description} onChange={e => setNewPosting({...newPosting, description: e.target.value})} placeholder="İlan detayları, beklenen yetenekler..." className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none resize-none"></textarea>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Çalışma Modeli</label>
                  <select value={newPosting.workModel} onChange={e => setNewPosting({...newPosting, workModel: e.target.value})} className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none bg-white">
                    <option value="Remote">Remote (Uzaktan)</option>
                    <option value="Hybrid">Hybrid (Karma)</option>
                    <option value="On-Site">On-Site (Ofiste)</option>
                  </select>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Lokasyon</label>
                  <input type="text" value={newPosting.location} onChange={e => setNewPosting({...newPosting, location: e.target.value})} placeholder="Örn: İstanbul" className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none" />
                </div>
              </div>

              <div className="pt-4 flex gap-3">
                <button type="button" onClick={() => setShowModal(false)} className="flex-1 py-2.5 border border-gray-300 text-gray-700 font-medium rounded-lg hover:bg-gray-50 transition-colors">İptal</button>
                <button type="submit" disabled={isCreating} className="flex-1 py-2.5 bg-blue-600 hover:bg-blue-700 text-white font-medium rounded-lg transition-colors flex justify-center items-center gap-2">
                  {isCreating ? (
                    <svg className="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle><path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path></svg>
                  ) : "İlanı Yayınla"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      <header className="bg-white/80 backdrop-blur-md p-8 rounded-2xl shadow-sm border border-white/40 flex flex-col md:flex-row md:items-center justify-between gap-6">
        <div className="flex items-center gap-5">
          <div className="w-16 h-16 bg-blue-100 text-blue-600 rounded-2xl flex items-center justify-center text-2xl font-bold shadow-inner uppercase">
            {companyName.charAt(0)}
          </div>
          <div>
            <h1 className="text-2xl font-bold text-gray-900 mb-1">Hoş Geldiniz, {companyName}</h1>
            <p className="text-gray-500">Aradığınız yeteneklere sahip adayları keşfedin ve davet edin.</p>
          </div>
        </div>
        
        <div className="flex gap-3">
          <button 
            onClick={handleAISearch}
            disabled={isSearching}
            className="bg-white border border-gray-300 hover:bg-gray-50 text-gray-700 font-medium py-2.5 px-5 rounded-lg transition-colors flex items-center gap-2 shadow-sm disabled:opacity-70"
          >
            {isSearching ? (
              <svg className="animate-spin h-5 w-5 text-gray-500" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle><path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path></svg>
            ) : (
              <svg className="w-5 h-5 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path></svg>
            )}
            Aday Ara (AI)
          </button>
          <button 
            onClick={openNewPostingModal}
            className="bg-blue-600 hover:bg-blue-700 text-white font-medium py-2.5 px-5 rounded-lg transition-colors flex items-center gap-2 shadow-sm"
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 4v16m8-8H4"></path></svg>
            Yeni İlan
          </button>
        </div>
      </header>

      {/* Şirket Profil Bölümü */}
      <div className="bg-white/80 backdrop-blur-md p-8 rounded-2xl shadow-sm border border-white/40 flex flex-col relative">
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-xl font-bold text-gray-800 flex items-center gap-2">
            <svg className="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"></path></svg>
            Şirket Profili
          </h2>
          {isEditingProfile && <span className="bg-blue-100 text-blue-700 text-xs px-2 py-1 rounded-md animate-pulse">Düzenleniyor</span>}
        </div>
        
        <div className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className={`p-4 rounded-xl border transition-colors ${isEditingProfile ? 'bg-white border-blue-300 shadow-inner' : 'bg-gray-50/50 border-transparent'}`}>
              <p className="text-sm text-gray-500 font-medium mb-1">Sektör</p>
              {isEditingProfile ? (
                <input type="text" value={profile.industry} onChange={e => setProfile({...profile, industry: e.target.value})} className="w-full bg-transparent border-b border-gray-300 focus:border-blue-500 outline-none py-1 font-semibold text-gray-900" />
              ) : (
                <p className="text-gray-900 font-semibold">{profile.industry || 'Belirtilmemiş'}</p>
              )}
            </div>
            <div className={`p-4 rounded-xl border transition-colors ${isEditingProfile ? 'bg-white border-blue-300 shadow-inner' : 'bg-gray-50/50 border-transparent'}`}>
              <p className="text-sm text-gray-500 font-medium mb-1">Kuruluş Yılı</p>
              {isEditingProfile ? (
                <input type="number" value={profile.foundedYear} onChange={e => setProfile({...profile, foundedYear: e.target.value})} className="w-full bg-transparent border-b border-gray-300 focus:border-blue-500 outline-none py-1 font-semibold text-gray-900" />
              ) : (
                <p className="text-gray-900 font-semibold">{profile.foundedYear || 'Belirtilmemiş'}</p>
              )}
            </div>
            <div className={`p-4 rounded-xl border transition-colors ${isEditingProfile ? 'bg-white border-blue-300 shadow-inner' : 'bg-gray-50/50 border-transparent'}`}>
              <p className="text-sm text-gray-500 font-medium mb-1 flex items-center gap-1">Web Sitesi</p>
              {isEditingProfile ? (
                <input type="text" value={profile.websiteUrl} onChange={e => setProfile({...profile, websiteUrl: e.target.value})} className="w-full bg-transparent border-b border-gray-300 focus:border-blue-500 outline-none py-1 font-semibold text-gray-900" />
              ) : (
                <a href={profile.websiteUrl?.startsWith('http') ? profile.websiteUrl : `https://${profile.websiteUrl}`} target="_blank" rel="noopener noreferrer" className="text-blue-600 hover:text-blue-800 font-semibold truncate flex items-center gap-1 hover:underline">
                  {profile.websiteUrl || '-'}
                  {profile.websiteUrl && <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14"></path></svg>}
                </a>
              )}
            </div>
          </div>
          
          <div className={`p-4 rounded-xl border transition-colors ${isEditingProfile ? 'bg-white border-blue-300 shadow-inner' : 'bg-gray-50/50 border-transparent'}`}>
            <p className="text-sm text-gray-500 font-medium mb-1">Şirket Hakkında</p>
            {isEditingProfile ? (
              <textarea rows={3} value={profile.description} onChange={e => setProfile({...profile, description: e.target.value})} placeholder="Şirketinizin vizyonundan, misyonundan ve çalışma kültüründen bahsedin..." className="w-full bg-transparent border-b border-gray-300 focus:border-blue-500 outline-none py-1 font-semibold text-gray-900 resize-none"></textarea>
            ) : (
              <p className="text-gray-900 font-semibold text-sm leading-relaxed">{profile.description || 'Henüz şirket açıklaması eklenmemiş.'}</p>
            )}
          </div>
        </div>
        
        <button 
          onClick={handleEditProfile}
          className={`mt-6 ml-auto px-6 py-2.5 font-medium rounded-lg border transition-all ${
            isEditingProfile 
            ? 'bg-blue-600 hover:bg-blue-700 text-white border-blue-600 shadow-md' 
            : 'bg-white hover:bg-gray-50 text-gray-700 border-gray-200'
          }`}
        >
          {isEditingProfile ? 'Değişiklikleri Kaydet' : 'Profili Düzenle'}
        </button>
      </div>

      {/* İlanlarım ve Gelen Başvurular Bölümü */}
      <div className="bg-white/90 backdrop-blur-md rounded-2xl shadow-sm border border-white/40 overflow-hidden">
        <div className="p-6 border-b border-gray-100 flex justify-between items-center bg-gray-50/30">
          <h2 className="text-lg font-bold text-gray-800 flex items-center gap-2">
            <svg className="w-5 h-5 text-indigo-600" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10"></path></svg>
            İlanlarım ve Gelen Başvurular
          </h2>
          <span className="bg-indigo-100 text-indigo-700 text-xs font-bold px-3 py-1 rounded-full">{myPostings.length} Aktif İlan</span>
        </div>
        
        <div className="overflow-x-auto p-2">
          {loading ? (
            <div className="flex justify-center items-center h-32">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-600"></div>
            </div>
          ) : myPostings.length > 0 ? (
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4 p-4">
              {myPostings.map((posting) => (
                <div key={posting.id} className="border border-gray-200 rounded-xl p-5 hover:shadow-md transition-shadow bg-white flex flex-col justify-between">
                  <div className="flex justify-between items-start mb-3">
                    <h3 className="font-bold text-gray-900 text-lg leading-tight">{posting.title}</h3>
                    <div className="flex items-center gap-2">
                      <span className={`px-2.5 py-1 rounded-full text-xs font-bold ${posting.status === 'Open' ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'}`}>
                        {posting.status === 'Open' ? 'Yayında' : 'Kapalı'}
                      </span>
                      <button onClick={() => handleEditPosting(posting)} className="text-gray-400 hover:text-blue-600 transition-colors" title="Düzenle">
                        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"></path></svg>
                      </button>
                      {posting.status === 'Open' && (
                        <button onClick={() => handleClosePosting(posting.id)} className="text-gray-400 hover:text-red-600 transition-colors" title="Yayından Kaldır">
                          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
                        </button>
                      )}
                    </div>
                  </div>
                  
                  <div className="flex justify-between items-center mt-4">
                    <div className="flex items-center gap-2">
                      <div className="w-8 h-8 rounded-full bg-blue-50 flex items-center justify-center text-blue-600 font-bold shadow-sm">
                        {posting.applicantCount !== undefined ? posting.applicantCount : '?'}
                      </div>
                      <span className="text-sm font-medium text-gray-600">Aday Başvurusu</span>
                    </div>
                    <button 
                      onClick={() => handleViewApplicants(posting.id)}
                      className="text-sm bg-indigo-50 hover:bg-indigo-600 text-indigo-600 hover:text-white font-semibold py-2 px-4 rounded-lg transition-colors border border-indigo-100 hover:border-indigo-600"
                    >
                      {selectedPostingId === posting.id ? 'Gizle' : 'Adayları Gör'}
                    </button>
                  </div>
                  
                  {/* Başvurular Listesi */}
                  {selectedPostingId === posting.id && (
                    <div className="mt-4 pt-4 border-t border-gray-100 animate-slide-up">
                      <h4 className="text-sm font-bold text-gray-700 mb-3">Gelen Başvurular</h4>
                      {posting.applicants && posting.applicants.length > 0 ? (
                        <div className="space-y-2">
                          {posting.applicants.map((applicant: any) => (
                            <div key={applicant.applicationId} className="bg-gray-50 rounded-lg p-3 flex justify-between items-center border border-gray-200">
                              <div className="flex items-center gap-3">
                                <div className="w-8 h-8 rounded-full bg-indigo-100 text-indigo-700 font-bold flex items-center justify-center text-sm">
                                  {applicant.studentName.charAt(0)}
                                </div>
                                <div>
                                  <p className="text-sm font-bold text-gray-900">{applicant.studentName}</p>
                                  <p className="text-xs text-gray-500">{new Date(applicant.appliedAt).toLocaleDateString('tr-TR')}</p>
                                </div>
                              </div>
                              <button 
                                onClick={() => alert(`Aday detayları: ${applicant.studentName}`)}
                                className="text-xs text-indigo-600 hover:underline font-medium"
                              >
                                İncele
                              </button>
                            </div>
                          ))}
                        </div>
                      ) : (
                        <p className="text-sm text-gray-500 italic">Henüz başvuru bulunmuyor.</p>
                      )}
                    </div>
                  )}
                </div>
              ))}
            </div>
          ) : (
            <div className="py-10 text-center text-gray-500">
              <div className="flex flex-col items-center">
                <svg className="w-12 h-12 text-gray-300 mb-3" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10"></path></svg>
                Henüz yayınlanmış bir staj ilanınız bulunmuyor.
              </div>
            </div>
          )}
        </div>
      </div>

      <div className="bg-white/90 backdrop-blur-md rounded-2xl shadow-sm border border-white/40 overflow-hidden">
        <div className="p-6 border-b border-gray-100 flex justify-between items-center bg-gray-50/30">
          <h2 className="text-lg font-bold text-gray-800">Gönderilen Davetler</h2>
          <span className="bg-blue-100 text-blue-700 text-xs font-bold px-3 py-1 rounded-full">{invitations.length} Davet</span>
        </div>
        
        <div className="overflow-x-auto">
          {loading ? (
            <div className="flex justify-center items-center h-32">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
            </div>
          ) : (
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="bg-gray-50/50 border-b border-gray-200">
                  <th className="px-6 py-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">Öğrenci</th>
                  <th className="px-6 py-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">İlan / Pozisyon</th>
                  <th className="px-6 py-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">Durum</th>
                  <th className="px-6 py-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">Tarih</th>
                  <th className="px-6 py-4 text-xs font-semibold text-gray-500 uppercase tracking-wider text-right">İşlem</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {invitations.length > 0 ? (
                  invitations.map(inv => (
                    <tr key={inv.invitationId} className="hover:bg-gray-50/80 transition-colors">
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="flex items-center">
                          <div className="h-8 w-8 rounded-full bg-blue-100 flex items-center justify-center text-blue-700 font-bold mr-3">
                            {inv.studentName.charAt(0)}
                          </div>
                          <div className="font-medium text-gray-900">{inv.studentName}</div>
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-gray-600">
                        {inv.postingTitle || 'Genel Davet'}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        {getStatusBadge(inv.status)}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-gray-500 text-sm">
                        {new Date(inv.sentAt).toLocaleDateString('tr-TR', { day: 'numeric', month: 'short', year: 'numeric' })}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                        <button 
                          onClick={() => alert(`Detaylar yükleniyor: ${inv.studentName}`)}
                          className="text-blue-600 hover:text-blue-900 hover:underline transition-all"
                        >
                          Detay
                        </button>
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan={5} className="px-6 py-10 text-center text-gray-500">
                      <div className="flex flex-col items-center">
                        <svg className="w-12 h-12 text-gray-300 mb-3" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4"></path></svg>
                        Henüz davet göndermediniz.
                      </div>
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          )}
        </div>
      </div>

      {/* Floating Chat Widget */}
      <div className="fixed bottom-6 right-6 z-50 flex flex-col items-end">
        {isChatOpen && (
          <div className="bg-white border border-gray-200 shadow-2xl rounded-2xl w-80 sm:w-96 mb-4 flex flex-col overflow-hidden animate-slide-up transition-all duration-300 origin-bottom-right" style={{ height: '500px' }}>
            <div className="bg-gradient-to-r from-blue-600 to-indigo-700 p-4 text-white flex justify-between items-center shadow-md z-10">
              <div className="flex items-center gap-3">
                <div className="bg-white/20 p-2 rounded-full backdrop-blur-sm">
                  <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9.75 17L9 20l-1 1h8l-1-1-.75-3M3 13h18M5 17h14a2 2 0 002-2V5a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"></path></svg>
                </div>
                <div>
                  <h3 className="font-bold text-sm">İşe Alım Asistanı</h3>
                  <p className="text-xs text-blue-100 flex items-center gap-1">
                    <span className="w-2 h-2 rounded-full bg-green-400 animate-pulse"></span> Çevrimiçi
                  </p>
                </div>
              </div>
              <button onClick={() => setIsChatOpen(false)} className="text-white/80 hover:text-white transition-colors p-1">
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12"></path></svg>
              </button>
            </div>
            
            <div className="flex-1 overflow-y-auto p-4 space-y-4 bg-gray-50/50">
              {chatMessages.map((msg, idx) => (
                <div key={idx} className={`flex ${msg.role === 'user' ? 'justify-end' : 'justify-start'}`}>
                  <div className={`max-w-[85%] p-3 rounded-2xl text-sm shadow-sm ${
                    msg.role === 'user' 
                      ? 'bg-blue-600 text-white rounded-tr-none' 
                      : 'bg-white border border-gray-100 text-gray-800 rounded-tl-none'
                  }`}>
                    {msg.text}
                    {msg.matches && msg.matches.length > 0 && (
                      <div className="mt-3 space-y-2 border-t border-gray-100 pt-2">
                        {msg.matches.map((match, mIdx) => (
                          <div 
                            key={mIdx} 
                            onClick={() => navigate(match.type === 'Posting' ? `/postings/${match.id}` : `/company-dashboard`)}
                            className="bg-blue-50 hover:bg-blue-100 p-2 rounded-lg cursor-pointer transition-colors border border-blue-100 flex items-center justify-between group"
                          >
                            <div>
                              <p className="font-bold text-blue-800 text-xs">{match.title}</p>
                              <p className="text-[10px] text-blue-600">{match.subtitle}</p>
                            </div>
                            <svg className="w-3 h-3 text-blue-400 group-hover:translate-x-1 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 5l7 7-7 7"></path></svg>
                          </div>
                        ))}
                      </div>
                    )}
                  </div>
                </div>
              ))}
              {isChatLoading && (
                <div className="flex justify-start">
                  <div className="max-w-[85%] p-4 rounded-2xl bg-white border border-gray-100 rounded-tl-none shadow-sm flex items-center gap-2">
                    <div className="w-2 h-2 bg-blue-400 rounded-full animate-bounce"></div>
                    <div className="w-2 h-2 bg-blue-400 rounded-full animate-bounce" style={{ animationDelay: '0.2s' }}></div>
                    <div className="w-2 h-2 bg-blue-400 rounded-full animate-bounce" style={{ animationDelay: '0.4s' }}></div>
                  </div>
                </div>
              )}
              <div ref={messagesEndRef} />
            </div>

            <div className="p-3 bg-white border-t border-gray-100">
              <form onSubmit={handleSendMessage} className="flex items-center gap-2 relative">
                <input 
                  type="text" 
                  value={currentMessage}
                  onChange={e => setCurrentMessage(e.target.value)}
                  placeholder="Aday ara veya soru sor..." 
                  className="flex-1 bg-gray-100 text-sm border-transparent focus:bg-white focus:border-blue-500 focus:ring-2 focus:ring-blue-200 rounded-full px-4 py-2.5 outline-none transition-all"
                  disabled={isChatLoading}
                />
                <button 
                  type="submit" 
                  disabled={isChatLoading || !currentMessage.trim()}
                  className="bg-blue-600 text-white p-2.5 rounded-full hover:bg-blue-700 transition-colors shadow-sm disabled:opacity-50 disabled:cursor-not-allowed flex-shrink-0"
                >
                  <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8"></path></svg>
                </button>
              </form>
            </div>
          </div>
        )}
        
        <button 
          onClick={() => setIsChatOpen(!isChatOpen)}
          className={`w-14 h-14 rounded-full flex items-center justify-center shadow-2xl transition-all duration-300 transform hover:scale-110 ${
            isChatOpen ? 'bg-gray-800 text-white rotate-90' : 'bg-gradient-to-r from-blue-600 to-indigo-600 text-white hover:shadow-blue-500/50'
          }`}
        >
          {isChatOpen ? (
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12"></path></svg>
          ) : (
            <svg className="w-7 h-7" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-5l-5 5v-5z"></path></svg>
          )}
        </button>
      </div>
    </div>
  );
};

export default CompanyDashboard;
