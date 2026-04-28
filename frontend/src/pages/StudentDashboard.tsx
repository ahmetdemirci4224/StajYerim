import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import { jwtDecode } from 'jwt-decode';

const StudentDashboard = () => {
  const navigate = useNavigate();
  const [profile, setProfile] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const [isEditing, setIsEditing] = useState(false);
  const [isAddingSkill, setIsAddingSkill] = useState(false);
  const [userName, setUserName] = useState('Öğrenci');
  const [invitations, setInvitations] = useState<any[]>([]);
  const [recommendedPostings, setRecommendedPostings] = useState<any[]>([]);
  const [availableSkills, setAvailableSkills] = useState<any[]>([]);
  const [showSkillModal, setShowSkillModal] = useState(false);
  const [newSkill, setNewSkill] = useState({ skillId: '', proficiencyLevel: '1' });

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const decoded: any = jwtDecode(token);
        if (decoded.FullName) {
          setUserName(decoded.FullName);
        }
      } catch (err) {
        console.error('Token okunamadı', err);
      }
    }
    fetchProfile();
    fetchInvitations();
    fetchRecommendedPostings();
    fetchAvailableSkills();
  }, []);

  const fetchAvailableSkills = async () => {
    try {
      const response = await api.get('/skill');
      if (response.data.isSuccess) {
        setAvailableSkills(response.data.data);
      }
    } catch (error) {
      console.error('Yetenekler çekilemedi', error);
    }
  };

  const fetchProfile = async () => {
    try {
      const response = await api.get('/student/profile');
      if (response.data.isSuccess) {
        setProfile(response.data.data);
      }
    } catch (error: any) {
      console.log('Profil henüz oluşturulmamış veya API hatası:', error.response?.data?.message || error.message);
      setProfile(null);
    } finally {
      setLoading(false);
    }
  };

  const fetchInvitations = async () => {
    try {
      const response = await api.get('/invitation/student-invitations');
      if (response.data.isSuccess) {
        setInvitations(response.data.data);
      }
    } catch (error) {
      console.error('Davetler çekilemedi', error);
    }
  };

  const fetchRecommendedPostings = async () => {
    try {
      const response = await api.get('/jobposting/recommended');
      if (response.data.isSuccess) {
        setRecommendedPostings(response.data.data);
      }
    } catch (error) {
      console.error('Önerilen ilanlar çekilemedi', error);
    }
  };

  const handleRespondInvitation = async (invitationId: number, status: string) => {
    try {
      const response = await api.put(`/invitation/${invitationId}/respond`, { status });
      if (response.data.isSuccess) {
        alert(status === 'Accepted' ? 'Daveti kabul ettiniz!' : 'Daveti reddettiniz.');
        fetchInvitations();
      }
    } catch (err: any) {
      alert(`Hata: ${err.response?.data?.message || 'İşlem başarısız.'}`);
    }
  };

  const handleEditProfile = async () => {
    if (isEditing) {
      // Save
      try {
        const response = await api.post('/student/profile', profile);
        if (response.data.isSuccess) {
          alert('Profil güncellendi!');
          setIsEditing(false);
          fetchRecommendedPostings();
        }
      } catch (error) {
         alert('Güncelleme sırasında hata oluştu.');
      }
    } else {
      setIsEditing(true);
    }
  };

  const handleAddSkill = () => {
    setShowSkillModal(true);
  };

  const handleAddSkillSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newSkill.skillId) return;

    setIsAddingSkill(true);
    try {
      const response = await api.post('/student/skills', {
        skillId: Number(newSkill.skillId),
        proficiencyLevel: Number(newSkill.proficiencyLevel)
      });

      if (response.data.isSuccess) {
        alert('Yetenek başarıyla eklendi!');
        setShowSkillModal(false);
        setNewSkill({ skillId: '', proficiencyLevel: '1' });
        fetchProfile(); // Yetenek listesini güncellemek için
      }
    } catch (error: any) {
      alert(`Hata: ${error.response?.data?.message || 'Yetenek eklenemedi.'}`);
    } finally {
      setIsAddingSkill(false);
    }
  };

  const handleRemoveSkill = async (skillId: number) => {
    if (!window.confirm("Bu yeteneği silmek istediğinize emin misiniz?")) return;
    try {
      const response = await api.delete(`/student/skills/${skillId}`);
      if (response.data.isSuccess) {
        fetchProfile();
      }
    } catch (error: any) {
      alert(`Hata: ${error.response?.data?.message || 'Yetenek silinemedi.'}`);
    }
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (!profile && !isEditing) {
    return (
      <div className="text-center py-20 bg-white/50 backdrop-blur-sm rounded-3xl border border-white/40 shadow-xl max-w-2xl mx-auto mt-10">
        <div className="w-24 h-24 bg-blue-100 text-blue-600 rounded-full flex items-center justify-center mx-auto mb-6">
          <svg className="w-12 h-12" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"></path></svg>
        </div>
        <h2 className="text-3xl font-bold text-gray-900 mb-4">Profilin Henüz Hazır Değil</h2>
        <p className="text-gray-600 mb-8 max-w-md mx-auto text-lg">Staj ilanlarına başvurabilmek, şirketlerden davet almak ve sana özel eşleşmeleri görmek için profilini oluşturmalısın.</p>
        <button onClick={() => { setProfile({ university: '', department: '', gpa: '', bio: '', githubUrl: '', linkedinUrl: '', preferredLocation: '', preferredWorkModel: '', interestedRoles: '' }); setIsEditing(true); }} className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-3 px-8 rounded-xl transition-all shadow-lg hover:shadow-xl transform hover:-translate-y-1">
          Hemen Profilini Oluştur
        </button>
      </div>
    );
  }

  return (
    <div className="space-y-8 relative">
      <header className="bg-white/80 backdrop-blur-md p-8 rounded-2xl shadow-sm border border-white/40 flex flex-col md:flex-row md:items-center justify-between gap-6">
        <div className="flex items-center gap-5">
          <div className="w-16 h-16 bg-blue-100 text-blue-600 rounded-2xl flex items-center justify-center text-2xl font-bold shadow-inner uppercase">
            {userName.charAt(0)}
          </div>
          <div>
            <h1 className="text-2xl font-bold text-gray-900 mb-1">Hoş Geldin, {userName.split(' ')[0]}</h1>
            <p className="text-gray-500">Staj bulma yolculuğunda seninleyiz.</p>
          </div>
        </div>
        <button 
          onClick={handleEditProfile}
          className={`px-6 py-2.5 font-medium rounded-lg border transition-all ${
            isEditing 
            ? 'bg-blue-600 hover:bg-blue-700 text-white border-blue-600 shadow-md' 
            : 'bg-white hover:bg-gray-50 text-gray-700 border-gray-200'
          }`}
        >
          {isEditing ? 'Değişiklikleri Kaydet' : 'Profili Düzenle'}
        </button>
      </header>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <div className="lg:col-span-2 space-y-8">
          <div className="bg-white/80 backdrop-blur-md p-8 rounded-2xl shadow-sm border border-white/40 flex flex-col md:flex-row gap-8 relative overflow-hidden">
            {isEditing && <div className="absolute top-0 right-0 w-full h-1 bg-blue-500 animate-pulse"></div>}
            <div className="w-full md:w-1/3 flex flex-col items-center text-center space-y-4">
              <div className="w-32 h-32 bg-gray-100 rounded-full border-4 border-white shadow-lg overflow-hidden flex items-center justify-center">
                <svg className="w-16 h-16 text-gray-400" fill="currentColor" viewBox="0 0 20 20"><path fillRule="evenodd" d="M10 9a3 3 0 100-6 3 3 0 000 6zm-7 9a7 7 0 1114 0H3z" clipRule="evenodd"></path></svg>
              </div>
              <div>
                <h3 className="font-bold text-gray-900 text-lg">{userName}</h3>
                <p className="text-gray-500 text-sm">Stajyer Adayı</p>
              </div>
              {!isEditing && (
                <div className="w-full bg-blue-50 text-blue-700 text-sm font-medium py-2 rounded-lg border border-blue-100 mt-2">
                  Profil Doluluğu: %85
                </div>
              )}
            </div>

            <div className="space-y-4 flex-1">
              <h3 className="font-bold text-gray-800 border-b pb-2">Eğitim Bilgileri</h3>
              <div className={`p-4 rounded-xl border transition-colors ${isEditing ? 'bg-white border-blue-300 shadow-inner' : 'bg-gray-50/50 border-transparent'}`}>
                <p className="text-sm text-gray-500 font-medium mb-1">Üniversite</p>
                {isEditing ? (
                  <input type="text" value={profile?.university || ''} onChange={e => setProfile({...profile, university: e.target.value})} className="w-full bg-transparent border-b border-gray-300 focus:border-blue-500 outline-none py-1 font-semibold text-gray-900" />
                ) : (
                  <p className="text-gray-900 font-semibold">{profile?.university || 'Belirtilmemiş'}</p>
                )}
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div className={`p-4 rounded-xl border transition-colors ${isEditing ? 'bg-white border-blue-300 shadow-inner' : 'bg-gray-50/50 border-transparent'}`}>
                  <p className="text-sm text-gray-500 font-medium mb-1">Bölüm</p>
                  {isEditing ? (
                    <input type="text" value={profile?.department || ''} onChange={e => setProfile({...profile, department: e.target.value})} className="w-full bg-transparent border-b border-gray-300 focus:border-blue-500 outline-none py-1 font-semibold text-gray-900" />
                  ) : (
                    <p className="text-gray-900 font-semibold">{profile?.department || 'Belirtilmemiş'}</p>
                  )}
                </div>
                <div className={`p-4 rounded-xl border transition-colors ${isEditing ? 'bg-white border-blue-300 shadow-inner' : 'bg-gray-50/50 border-transparent'}`}>
                  <p className="text-sm text-gray-500 font-medium mb-1">Not Ortalaması (GPA)</p>
                  {isEditing ? (
                    <input type="number" step="0.01" value={profile?.gpa || ''} onChange={e => setProfile({...profile, gpa: e.target.value})} className="w-full bg-transparent border-b border-gray-300 focus:border-blue-500 outline-none py-1 font-semibold text-gray-900" />
                  ) : (
                    <p className="text-gray-900 font-semibold">{profile?.gpa || 'Belirtilmemiş'}</p>
                  )}
                </div>
              </div>
              <div className={`p-4 rounded-xl border transition-colors ${isEditing ? 'bg-white border-blue-300 shadow-inner' : 'bg-gray-50/50 border-transparent'}`}>
                <p className="text-sm text-gray-500 font-medium mb-1">Hakkımda (Bio)</p>
                {isEditing ? (
                  <textarea rows={2} value={profile?.bio || ''} onChange={e => setProfile({...profile, bio: e.target.value})} placeholder="Kendinden kısaca bahset..." className="w-full bg-transparent border-b border-gray-300 focus:border-blue-500 outline-none py-1 font-semibold text-gray-900 resize-none"></textarea>
                ) : (
                  <p className="text-gray-900 font-semibold text-sm">{profile?.bio || 'Henüz bir biyografi eklenmemiş.'}</p>
                )}
              </div>

              <h3 className="font-bold text-gray-800 border-b pb-2 pt-4">Staj Beklentileri (Eşleşme İçin)</h3>
              <div className="grid grid-cols-2 gap-4">
                <div className={`p-4 rounded-xl border transition-colors ${isEditing ? 'bg-white border-blue-300 shadow-inner' : 'bg-gray-50/50 border-transparent'}`}>
                  <p className="text-sm text-gray-500 font-medium mb-1">Tercih Edilen Şehir</p>
                  {isEditing ? (
                    <input type="text" value={profile?.preferredLocation || ''} onChange={e => setProfile({...profile, preferredLocation: e.target.value})} placeholder="Örn: İstanbul, Ankara, Fark Etmez" className="w-full bg-transparent border-b border-gray-300 focus:border-blue-500 outline-none py-1 font-semibold text-gray-900" />
                  ) : (
                    <p className="text-gray-900 font-semibold">{profile?.preferredLocation || 'Fark Etmez'}</p>
                  )}
                </div>
                <div className={`p-4 rounded-xl border transition-colors ${isEditing ? 'bg-white border-blue-300 shadow-inner' : 'bg-gray-50/50 border-transparent'}`}>
                  <p className="text-sm text-gray-500 font-medium mb-1">Çalışma Modeli</p>
                  {isEditing ? (
                    <select value={profile?.preferredWorkModel || ''} onChange={e => setProfile({...profile, preferredWorkModel: e.target.value})} className="w-full bg-transparent border-b border-gray-300 focus:border-blue-500 outline-none py-1 font-semibold text-gray-900">
                      <option value="">Fark Etmez</option>
                      <option value="Remote">Remote</option>
                      <option value="Hybrid">Hybrid</option>
                      <option value="On-Site">On-Site</option>
                    </select>
                  ) : (
                    <p className="text-gray-900 font-semibold">{profile?.preferredWorkModel || 'Fark Etmez'}</p>
                  )}
                </div>
              </div>
              <div className={`p-4 rounded-xl border transition-colors ${isEditing ? 'bg-white border-blue-300 shadow-inner' : 'bg-gray-50/50 border-transparent'}`}>
                <p className="text-sm text-gray-500 font-medium mb-1">İlgilenilen Roller</p>
                {isEditing ? (
                  <input type="text" value={profile?.interestedRoles || ''} onChange={e => setProfile({...profile, interestedRoles: e.target.value})} placeholder="Örn: Frontend, Backend, Mobil (Virgülle ayırın)" className="w-full bg-transparent border-b border-gray-300 focus:border-blue-500 outline-none py-1 font-semibold text-gray-900" />
                ) : (
                  <p className="text-gray-900 font-semibold">{profile?.interestedRoles || 'Belirtilmemiş'}</p>
                )}
              </div>

              <h3 className="font-bold text-gray-800 border-b pb-2 pt-4">Bağlantılar</h3>
              <div className="grid grid-cols-2 gap-4">
                <div className={`p-4 rounded-xl border transition-colors ${isEditing ? 'bg-white border-blue-300 shadow-inner' : 'bg-gray-50/50 border-transparent'}`}>
                  <p className="text-sm text-gray-500 font-medium mb-1 flex items-center gap-1"><svg className="w-4 h-4" fill="currentColor" viewBox="0 0 24 24"><path d="M12 0c-6.626 0-12 5.373-12 12 0 5.302 3.438 9.8 8.207 11.387.599.111.793-.261.793-.577v-2.234c-3.338.726-4.033-1.416-4.033-1.416-.546-1.387-1.333-1.756-1.333-1.756-1.089-.745.083-.729.083-.729 1.205.084 1.839 1.237 1.839 1.237 1.07 1.834 2.807 1.304 3.492.997.107-.775.418-1.305.762-1.604-2.665-.305-5.467-1.334-5.467-5.931 0-1.311.469-2.381 1.236-3.221-.124-.303-.535-1.524.117-3.176 0 0 1.008-.322 3.301 1.23.957-.266 1.983-.399 3.003-.404 1.02.005 2.047.138 3.006.404 2.291-1.552 3.297-1.23 3.297-1.23.653 1.653.242 2.874.118 3.176.77.84 1.235 1.911 1.235 3.221 0 4.609-2.807 5.624-5.479 5.921.43.372.823 1.102.823 2.222v3.293c0 .319.192.694.801.576 4.765-1.589 8.199-6.086 8.199-11.386 0-6.627-5.373-12-12-12z"/></svg> GitHub</p>
                  {isEditing ? (
                    <input type="text" value={profile?.githubUrl || ''} onChange={e => setProfile({...profile, githubUrl: e.target.value})} placeholder="Kullanıcı adın" className="w-full bg-transparent border-b border-gray-300 focus:border-blue-500 outline-none py-1 font-semibold text-gray-900" />
                  ) : (
                    <p className="text-gray-900 font-semibold truncate">{profile?.githubUrl || '-'}</p>
                  )}
                </div>
                <div className={`p-4 rounded-xl border transition-colors ${isEditing ? 'bg-white border-blue-300 shadow-inner' : 'bg-gray-50/50 border-transparent'}`}>
                  <p className="text-sm text-gray-500 font-medium mb-1 flex items-center gap-1"><svg className="w-4 h-4 text-blue-600" fill="currentColor" viewBox="0 0 24 24"><path d="M19 0h-14c-2.761 0-5 2.239-5 5v14c0 2.761 2.239 5 5 5h14c2.762 0 5-2.239 5-5v-14c0-2.761-2.238-5-5-5zm-11 19h-3v-11h3v11zm-1.5-12.268c-.966 0-1.75-.79-1.75-1.764s.784-1.764 1.75-1.764 1.75.79 1.75 1.764-.783 1.764-1.75 1.764zm13.5 12.268h-3v-5.604c0-3.368-4-3.113-4 0v5.604h-3v-11h3v1.765c1.396-2.586 7-2.777 7 2.476v6.759z"/></svg> LinkedIn</p>
                  {isEditing ? (
                    <input type="text" value={profile?.linkedinUrl || ''} onChange={e => setProfile({...profile, linkedinUrl: e.target.value})} placeholder="Kullanıcı adın" className="w-full bg-transparent border-b border-gray-300 focus:border-blue-500 outline-none py-1 font-semibold text-gray-900" />
                  ) : (
                    <p className="text-gray-900 font-semibold truncate">{profile?.linkedinUrl || '-'}</p>
                  )}
                </div>
              </div>
              <div className="p-4 bg-gray-50/50 rounded-xl border border-transparent flex items-center justify-between">
                <div>
                  <p className="text-sm text-gray-500 font-medium mb-1 flex items-center gap-1"><svg className="w-4 h-4 text-red-500" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M7 21h10a2 2 0 002-2V9.414a1 1 0 00-.293-.707l-5.414-5.414A1 1 0 0012.586 3H7a2 2 0 00-2 2v14a2 2 0 002 2z"></path></svg> Özgeçmiş (CV)</p>
                  <p className="text-gray-900 font-semibold text-sm">cv_guncel_2026.pdf</p>
                </div>
                <button className="text-blue-600 hover:text-blue-800 bg-blue-50 hover:bg-blue-100 px-3 py-1.5 rounded-lg text-sm font-medium transition-colors">
                  Güncelle
                </button>
              </div>
            </div>
          </div>

          {/* Sana Özel Öneriler (İkili Eşleşme) */}
          <div className="bg-gradient-to-r from-blue-600 to-indigo-700 p-8 rounded-2xl shadow-lg border border-transparent flex flex-col text-white relative overflow-hidden">
            <div className="absolute top-0 right-0 w-64 h-64 bg-white opacity-5 rounded-full blur-3xl -mr-20 -mt-20"></div>
            <div className="flex justify-between items-center mb-6 relative z-10">
              <h2 className="text-2xl font-bold flex items-center gap-2">
                <svg className="w-6 h-6 text-yellow-300" fill="currentColor" viewBox="0 0 20 20"><path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"></path></svg>
                Sana Özel Öneriler
              </h2>
              <span className="bg-white/20 backdrop-blur-sm text-white text-xs font-bold px-3 py-1 rounded-full">Yapay Zeka Eşleşmesi</span>
            </div>
            
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 relative z-10">
              {recommendedPostings.length > 0 ? (
                recommendedPostings.slice(0, 3).map(posting => (
                  <div key={posting.id} onClick={() => navigate(`/postings/${posting.id}`)} className="bg-white rounded-xl p-5 text-gray-900 shadow-sm hover:shadow-xl transition-all cursor-pointer transform hover:-translate-y-1 relative overflow-hidden group">
                    <div className="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-green-400 to-blue-500"></div>
                    <div className="flex justify-between items-start mb-3">
                      <h3 className="font-bold text-lg leading-tight group-hover:text-blue-600 transition-colors">{posting.title}</h3>
                      <div className="flex flex-col items-end">
                        <span className="text-xs font-bold text-green-600 bg-green-50 px-2 py-1 rounded-md border border-green-100 flex items-center gap-1">
                          %{Math.round(posting.matchScore)} Uyum
                        </span>
                      </div>
                    </div>
                    <p className="text-gray-500 text-sm mb-3">{posting.company}</p>
                    <div className="flex flex-wrap gap-1 mt-auto">
                      <span className="text-xs font-medium text-gray-600 bg-gray-100 px-2 py-1 rounded-md">{posting.workModel}</span>
                      <span className="text-xs font-medium text-gray-600 bg-gray-100 px-2 py-1 rounded-md truncate max-w-[100px]">{posting.location}</span>
                    </div>
                  </div>
                ))
              ) : (
                <div className="col-span-full text-center py-6 text-blue-100">
                  <p>Şu an profiline tam olarak uyan bir ilan bulamadık. Yeteneklerini ve beklentilerini güncelleyerek eşleşme şansını artırabilirsin.</p>
                </div>
              )}
            </div>
            {recommendedPostings.length > 3 && (
              <button onClick={() => navigate('/postings')} className="mt-6 text-sm text-center font-bold hover:text-yellow-300 transition-colors w-full">
                Tüm Uygun İlanları Gör &rarr;
              </button>
            )}
          </div>
        </div>

        {/* Davetler ve Yetenekler */}
        <div className="space-y-8">
          {/* Gelen Davetler */}
          <div className="bg-white/80 backdrop-blur-md p-8 rounded-2xl shadow-sm border border-white/40 flex flex-col">
            <div className="flex justify-between items-center mb-6">
              <h2 className="text-xl font-bold text-gray-800 flex items-center gap-2">
                <svg className="w-5 h-5 text-indigo-600" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"></path></svg>
                Gelen Davetler
              </h2>
              <span className="bg-indigo-100 text-indigo-700 text-xs font-bold px-3 py-1 rounded-full">{invitations.length} Davet</span>
            </div>
            <div className="flex-1 space-y-4 max-h-80 overflow-y-auto pr-2">
              {invitations.length > 0 ? (
                invitations.map(inv => (
                  <div key={inv.invitationId} className="bg-white border border-gray-200 rounded-xl p-4 shadow-sm hover:shadow-md transition-shadow">
                    <div className="flex justify-between items-start mb-2">
                      <h3 className="font-bold text-gray-900">{inv.companyName}</h3>
                      <span className={`px-2 py-0.5 rounded-full text-[10px] font-bold ${
                        inv.status === 'Accepted' ? 'bg-green-100 text-green-700' :
                        inv.status === 'Declined' ? 'bg-red-100 text-red-700' :
                        'bg-yellow-100 text-yellow-700'
                      }`}>
                        {inv.status === 'Accepted' ? 'Kabul Edildi' : inv.status === 'Declined' ? 'Reddedildi' : 'Bekliyor'}
                      </span>
                    </div>
                    <p className="text-sm font-medium text-gray-600 mb-2">{inv.postingTitle || 'Genel Başvuru Daveti'}</p>
                    <div className="text-sm text-gray-500 bg-gray-50 p-3 rounded-lg border border-gray-100 mb-3 italic">
                      "{inv.message}"
                    </div>
                    {inv.status === 'Pending' && (
                      <div className="flex gap-2">
                        <button 
                          onClick={() => handleRespondInvitation(inv.invitationId, 'Accepted')}
                          className="flex-1 bg-green-50 hover:bg-green-100 text-green-700 border border-green-200 font-medium py-1.5 rounded-lg text-sm transition-colors"
                        >
                          Kabul Et
                        </button>
                        <button 
                          onClick={() => handleRespondInvitation(inv.invitationId, 'Declined')}
                          className="flex-1 bg-red-50 hover:bg-red-100 text-red-700 border border-red-200 font-medium py-1.5 rounded-lg text-sm transition-colors"
                        >
                          Reddet
                        </button>
                      </div>
                    )}
                  </div>
                ))
              ) : (
                <div className="text-center py-8 text-gray-400">
                  <p>Henüz bir şirketten davet almadınız.</p>
                </div>
              )}
            </div>
          </div>

          <div className="bg-white/80 backdrop-blur-md p-8 rounded-2xl shadow-sm border border-white/40 flex flex-col">
            <div className="flex justify-between items-center mb-6">
              <h2 className="text-xl font-bold text-gray-800 flex items-center gap-2">
                <svg className="w-5 h-5 text-indigo-600" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13 10V3L4 14h7v7l9-11h-7z"></path></svg>
                Yeteneklerim
              </h2>
            </div>
          
          <div className="flex-1">
            {profile?.skills?.length > 0 ? (
              <div className="flex flex-wrap gap-3">
                {profile.skills.map((skill: any) => (
                  <div key={skill.skillId} className="bg-indigo-50/80 border border-indigo-100 text-indigo-700 px-4 py-2 rounded-xl flex items-center gap-2 animate-fade-in">
                    <span className="font-medium">{skill.skillName}</span>
                    <span className="bg-indigo-100 text-indigo-800 text-xs px-2 py-0.5 rounded-full">
                      {skill.proficiencyLevel}
                    </span>
                    <button onClick={() => handleRemoveSkill(skill.skillId)} className="ml-1 text-indigo-400 hover:text-red-500 transition-colors" title="Yeteneği Sil">
                      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12"></path></svg>
                    </button>
                  </div>
                ))}
              </div>
            ) : (
              <div className="flex flex-col items-center justify-center text-center text-gray-400 p-8 border-2 border-dashed border-gray-200 rounded-xl">
                <svg className="w-12 h-12 mb-3 text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5" d="M19.428 15.428a2 2 0 00-1.022-.547l-2.387-.477a6 6 0 00-3.86.517l-.318.158a6 6 0 01-3.86.517L6.05 15.21a2 2 0 00-1.806.547M8 4h8l-1 1v5.172a2 2 0 00.586 1.414l5 5c1.26 1.26.367 3.414-1.415 3.414H4.828c-1.782 0-2.674-2.154-1.414-3.414l5-5A2 2 0 009 10.172V5L8 4z"></path></svg>
                <p>Henüz yetenek eklemediniz.</p>
                <p className="text-sm mt-1">İlanlarda öne çıkmak için yeteneklerinizi ekleyin.</p>
              </div>
            )}
          </div>
          
          <button 
            onClick={handleAddSkill}
            disabled={isAddingSkill}
            className="mt-6 w-full py-2.5 bg-indigo-50/50 hover:bg-indigo-100 text-indigo-700 font-medium rounded-lg border border-indigo-200 transition-colors flex items-center justify-center gap-2 disabled:opacity-70"
          >
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path></svg>
            Yetenek Ekle
          </button>
        </div>
        </div>
      </div>

      {/* Add Skill Modal */}
      {showSkillModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm animate-fade-in p-4">
          <div className="bg-white rounded-2xl shadow-2xl w-full max-w-md overflow-hidden flex flex-col animate-slide-up">
            <div className="p-6 border-b border-gray-100 flex justify-between items-center bg-gray-50">
              <h2 className="text-xl font-bold text-gray-900">Yetenek Ekle</h2>
              <button onClick={() => setShowSkillModal(false)} className="text-gray-400 hover:text-red-500 transition-colors">
                <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12"></path></svg>
              </button>
            </div>
            
            <div className="p-6">
              <form onSubmit={handleAddSkillSubmit} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Yetenek Seçin</label>
                  <select 
                    required
                    value={newSkill.skillId}
                    onChange={e => setNewSkill({...newSkill, skillId: e.target.value})}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 outline-none"
                  >
                    <option value="">-- Yetenek Seç --</option>
                    {availableSkills.map(skill => (
                      <option key={skill.id} value={skill.id}>{skill.name}</option>
                    ))}
                  </select>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Yetkinlik Seviyesi (1-5)</label>
                  <select 
                    required
                    value={newSkill.proficiencyLevel}
                    onChange={e => setNewSkill({...newSkill, proficiencyLevel: e.target.value})}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 outline-none"
                  >
                    <option value="1">1 - Başlangıç</option>
                    <option value="2">2 - Temel</option>
                    <option value="3">3 - Orta</option>
                    <option value="4">4 - İleri</option>
                    <option value="5">5 - Uzman</option>
                  </select>
                </div>
                <button 
                  type="submit" 
                  disabled={isAddingSkill}
                  className="w-full bg-indigo-600 hover:bg-indigo-700 text-white font-medium py-3 rounded-xl transition-colors mt-6 flex justify-center items-center gap-2"
                >
                  {isAddingSkill ? (
                    <svg className="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle><path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path></svg>
                  ) : "Kaydet"}
                </button>
              </form>
            </div>
          </div>
        </div>
      )}

      {/* Chat Widget Removed */}
      </div>
      );
      };

      export default StudentDashboard;
