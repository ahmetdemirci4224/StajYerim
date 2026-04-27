import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

const PostingsList = () => {
  const navigate = useNavigate();
  const [searchQuery, setSearchQuery] = useState('');
  const [isSearching, setIsSearching] = useState(false);
  const [postingsData, setPostingsData] = useState<any[]>([]);
  const [recommendedData, setRecommendedData] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [viewMode, setViewMode] = useState<'all' | 'recommended'>('all');
  const [showFilters, setShowFilters] = useState(false);

  // Filters
  const [filterRole, setFilterRole] = useState('');
  const [filterLocation, setFilterLocation] = useState('');
  const [filterWorkModel, setFilterWorkModel] = useState('');

  const isLoggedInStudent = localStorage.getItem('token') && localStorage.getItem('role') === 'Student';

  useEffect(() => {
    fetchPostings();
    if (isLoggedInStudent) {
      fetchRecommendedPostings();
    }
  }, []);

  const fetchPostings = async () => {
    try {
      const response = await api.get('/jobposting/all');
      if (response.data.isSuccess) {
        setPostingsData(response.data.data);
      }
    } catch (err) {
      console.error('İlanlar çekilirken hata oluştu', err);
    } finally {
      if (!isLoggedInStudent) setLoading(false);
    }
  };

  const fetchRecommendedPostings = async () => {
    try {
      const response = await api.get('/jobposting/recommended');
      if (response.data.isSuccess) {
        setRecommendedData(response.data.data);
      }
    } catch (err) {
      console.error('Önerilen ilanlar çekilirken hata oluştu', err);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = async (e: React.FormEvent) => {
    e.preventDefault();
    if(!searchQuery.trim()) return;
    
    setIsSearching(true);
    try {
        const response = await api.post('/chatbot/parse', { userMessage: searchQuery });
        if(response.data.isSuccess) {
            const filters = response.data.data;
            alert(`NLP Asistanı anladı:\nLokasyon: ${filters.location}\nRol: ${filters.role}\nModel: ${filters.workModel}\nYetenekler: ${filters.skills.join(', ')}`);
        }
    } catch (error) {
        console.error(error);
        setTimeout(() => {
          setIsSearching(false);
        }, 1500);
    } finally {
        if (api.defaults.baseURL) {
          setIsSearching(false);
        }
    }
  };

  const handleViewDetail = (id: number) => {
    navigate(`/postings/${id}`);
  };

  const getTheme = (index: number) => {
    const themes = ['blue', 'purple', 'orange', 'green', 'pink'];
    return themes[index % themes.length];
  };

  return (
    <div className="space-y-12 pb-12">
      {/* Hero Section */}
      <div className="bg-gradient-to-r from-blue-600 to-indigo-700 rounded-3xl p-10 sm:p-16 text-center text-white shadow-xl relative overflow-hidden">
        {/* Decorative blobs */}
        <div className="absolute top-0 left-0 w-full h-full overflow-hidden opacity-20 pointer-events-none">
          <div className="absolute -top-24 -left-24 w-64 h-64 rounded-full bg-white blur-3xl"></div>
          <div className="absolute bottom-[-20%] right-[-10%] w-96 h-96 rounded-full bg-white blur-3xl"></div>
        </div>

        <div className="relative z-10 max-w-2xl mx-auto space-y-6">
          <h1 className="text-4xl sm:text-5xl font-extrabold tracking-tight">Sana En Uygun Stajı Bul</h1>
          <p className="text-lg text-blue-100 sm:text-xl">
            Yapay zeka asistanımıza ne aradığını doğal bir dille yaz, o senin için en iyi seçenekleri filtrelesin.
          </p>
          
          <form onSubmit={handleSearch} className="mt-8 relative max-w-xl mx-auto">
            <div className="relative flex items-center bg-white rounded-full shadow-lg overflow-hidden p-1 border-2 border-transparent focus-within:border-blue-300 transition-all">
              <input 
                type="text" 
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                placeholder='Örn: "Ankara’da React kullanan uzaktan staj arıyorum"'
                className="w-full px-6 py-3 text-gray-800 outline-none text-base"
                required
              />
              <button 
                type="submit" 
                disabled={isSearching}
                className="bg-blue-600 hover:bg-blue-700 text-white px-6 py-3 rounded-full font-medium transition-colors whitespace-nowrap flex items-center gap-2 disabled:opacity-80"
              >
                {isSearching ? (
                  <svg className="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle><path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path></svg>
                ) : (
                  <>AI İle Bul 🪄</>
                )}
              </button>
            </div>
          </form>
        </div>
      </div>

      <div className="space-y-6">
        <div className="flex flex-col gap-4 bg-white p-4 rounded-2xl shadow-sm border border-gray-100">
          <div className="flex justify-between items-center w-full">
            <button 
              onClick={() => setShowFilters(!showFilters)}
              className="flex items-center gap-2 text-gray-700 hover:text-blue-600 font-medium transition-colors bg-gray-50 hover:bg-gray-100 px-4 py-2.5 rounded-lg border border-gray-200"
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z"></path></svg>
              {showFilters ? 'Filtreleri Gizle' : 'Filtrele'}
            </button>
            
            {isLoggedInStudent && (
              <button 
                onClick={() => setViewMode(viewMode === 'all' ? 'recommended' : 'all')}
                className={`whitespace-nowrap px-5 py-2.5 rounded-lg font-bold text-sm transition-all flex items-center gap-2 ${
                  viewMode === 'recommended' 
                  ? 'bg-gradient-to-r from-yellow-400 to-yellow-500 text-yellow-900 shadow-md' 
                  : 'bg-indigo-50 text-indigo-600 hover:bg-indigo-100 border border-indigo-100'
                }`}
              >
                <svg className="w-4 h-4" fill="currentColor" viewBox="0 0 20 20"><path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"></path></svg>
                {viewMode === 'recommended' ? 'Tüm İlanları Göster' : 'Sana Özel İlanlar'}
              </button>
            )}
          </div>
          
          {showFilters && (
            <div className="flex flex-wrap items-center gap-3 w-full border-t border-gray-100 pt-4 animate-fade-in">
              <input 
                type="text" 
                placeholder="Rol (Örn: Frontend)" 
                value={filterRole}
                onChange={e => setFilterRole(e.target.value)}
                className="flex-1 min-w-[200px] px-4 py-2 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              <input 
                type="text" 
                placeholder="Şehir (Örn: İstanbul)" 
                value={filterLocation}
                onChange={e => setFilterLocation(e.target.value)}
                className="flex-1 min-w-[200px] px-4 py-2 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              <select 
                value={filterWorkModel}
                onChange={e => setFilterWorkModel(e.target.value)}
                className="flex-1 min-w-[200px] px-4 py-2 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="">Tüm Modeller</option>
                <option value="Remote">Remote</option>
                <option value="Hybrid">Hybrid</option>
                <option value="On-Site">On-Site</option>
              </select>
            </div>
          )}
        </div>

        <h2 className="text-2xl font-bold text-gray-900">
          {viewMode === 'recommended' ? 'Sana Özel Eşleşen İlanlar' : 'Öne Çıkan İlanlar'}
        </h2>
        
        {loading ? (
          <div className="flex justify-center items-center py-12">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
          </div>
        ) : (() => {
          let displayedPostings = viewMode === 'recommended' ? recommendedData : postingsData;
          
          if (viewMode === 'all') {
            if (filterRole) displayedPostings = displayedPostings.filter(p => p.title.toLowerCase().includes(filterRole.toLowerCase()) || (p.description && p.description.toLowerCase().includes(filterRole.toLowerCase())));
            if (filterLocation) displayedPostings = displayedPostings.filter(p => p.location && p.location.toLowerCase().includes(filterLocation.toLowerCase()));
            if (filterWorkModel) displayedPostings = displayedPostings.filter(p => p.workModel === filterWorkModel);
          }

          if (displayedPostings.length === 0) {
            return (
              <div className="bg-white rounded-2xl p-12 text-center border border-gray-200 shadow-sm text-gray-500">
                Şu an için aradığınız kriterlere uygun bir ilan bulunmuyor.
              </div>
            );
          }

          return (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {displayedPostings.map((posting, index) => {
                const theme = getTheme(index);
                return (
                <div 
                  key={posting.id} 
                  onClick={() => handleViewDetail(posting.id)}
                  className="bg-white/80 backdrop-blur-sm rounded-2xl p-6 border border-gray-200 shadow-sm hover:shadow-xl transition-all flex flex-col h-full group relative overflow-hidden cursor-pointer transform hover:-translate-y-1"
                >
                  {viewMode === 'recommended' && posting.matchScore !== undefined && (
                    <div className="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-green-400 to-blue-500"></div>
                  )}
                  {/* Badge & Icon Header */}
                  <div className="flex justify-between items-start mb-4">
                    <div className={`bg-${theme}-50 text-${theme}-600 w-12 h-12 rounded-xl flex items-center justify-center font-bold text-xl mb-4 group-hover:bg-${theme}-600 group-hover:text-white transition-colors`}>
                      {(posting.company || 'C').charAt(0)}
                    </div>
                    <div className="flex flex-col items-end gap-1">
                      {viewMode === 'recommended' && posting.matchScore !== undefined && (
                        <span className="text-[10px] font-bold text-green-600 bg-green-50 px-2 py-1 rounded-md border border-green-100 flex items-center gap-1">
                          %{Math.round(posting.matchScore)} Uyum
                        </span>
                      )}
                      <span className={`px-3 py-1 rounded-full font-semibold text-xs ${
                        posting.workModel === 'Remote' ? 'bg-green-100 text-green-700' :
                        posting.workModel === 'On-Site' ? 'bg-red-100 text-red-700' :
                        'bg-blue-100 text-blue-700'
                      }`}>
                        {posting.workModel || 'Belirtilmemiş'}
                      </span>
                    </div>
                  </div>
                  
                  <h3 className="text-lg font-bold text-gray-900 mb-1 group-hover:text-blue-600 transition-colors">{posting.title}</h3>
                  <p className="text-gray-500 text-sm mb-2">{posting.company} {posting.location ? `• ${posting.location}` : ''}</p>
                  <p className="text-gray-600 text-sm mb-4 line-clamp-2">{posting.description}</p>
                  
                  {/* Skills */}
                  <div className="flex flex-wrap gap-2 mb-6 mt-auto">
                    {posting.skills && posting.skills.map((skill: string, sIndex: number) => (
                      <span key={sIndex} className="bg-gray-100 text-gray-700 text-xs px-2.5 py-1 rounded-md font-medium border border-gray-200">
                        {skill}
                      </span>
                    ))}
                    {(!posting.skills || posting.skills.length === 0) && (
                      <span className="text-gray-400 text-xs italic">Özel yetenek belirtilmemiş</span>
                    )}
                  </div>
                  
                  {/* Action Button */}
                  <div className="w-full bg-blue-50 text-blue-600 font-medium py-2 rounded-lg flex items-center justify-center gap-2 group-hover:bg-blue-600 group-hover:text-white transition-colors">
                    İlanı İncele
                    <svg className="w-4 h-4 transform group-hover:translate-x-1 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M14 5l7 7m0 0l-7 7m7-7H3"></path></svg>
                  </div>
                </div>
              )})}
            </div>
          );
        })()}
      </div>
    </div>
  );
};

export default PostingsList;
