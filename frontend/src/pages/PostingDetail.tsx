import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import api from '../services/api';

const PostingDetail = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [posting, setPosting] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const [applying, setApplying] = useState(false);

  useEffect(() => {
    const fetchDetail = async () => {
      try {
        const response = await api.get(`/jobposting/${id}`);
        if (response.data.isSuccess) {
          setPosting(response.data.data);
        }
      } catch (err) {
        console.error('İlan detayı çekilirken hata:', err);
      } finally {
        setLoading(false);
      }
    };
    fetchDetail();
  }, [id]);

  const handleApply = async () => {
    setApplying(true);
    try {
      const response = await api.post(`/jobposting/apply/${id}`);
      alert(`Başarılı: ${response.data.message}`);
      setPosting((prev: any) => ({ ...prev, hasApplied: true }));
    } catch (err: any) {
      alert(`Hata: ${err.response?.data?.message || 'Başvuru yapılamadı. Öğrenci hesabı ile giriş yaptığınızdan emin olun.'}`);
    } finally {
      setApplying(false);
    }
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center py-20">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (!posting) {
    return (
      <div className="text-center py-20">
        <h2 className="text-2xl font-bold text-gray-800">İlan Bulunamadı</h2>
        <button onClick={() => navigate('/postings')} className="mt-4 text-blue-600 hover:underline">Listeye Dön</button>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto space-y-6">
      <button onClick={() => navigate('/postings')} className="text-gray-500 hover:text-blue-600 flex items-center gap-2 mb-4 transition-colors">
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"></path></svg>
        İlanlara Dön
      </button>

      <div className="bg-white rounded-2xl p-8 md:p-12 shadow-sm border border-gray-100">
        <div className="flex flex-col md:flex-row md:items-start justify-between gap-6 mb-8">
          <div className="flex items-center gap-6">
             <div className="w-20 h-20 bg-blue-50 text-blue-600 rounded-2xl flex items-center justify-center font-bold text-3xl uppercase shadow-inner">
               {(posting.company || 'C').charAt(0)}
             </div>
             <div>
               <h1 className="text-3xl font-bold text-gray-900 mb-2">{posting.title}</h1>
               <div className="flex flex-wrap items-center gap-3 text-gray-600">
                 <span className="font-medium text-blue-600 flex items-center gap-1">
                   <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"></path></svg>
                   {posting.company}
                 </span>
                 {posting.location && (
                   <>
                     <span className="text-gray-300">•</span>
                     <span className="flex items-center gap-1">
                       <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.243-4.243a8 8 0 1111.314 0z"></path><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"></path></svg>
                       {posting.location}
                     </span>
                   </>
                 )}
                 <span className="text-gray-300">•</span>
                 <span className={`px-2.5 py-0.5 rounded-full text-xs font-semibold ${
                   posting.workModel === 'Remote' ? 'bg-green-100 text-green-700' :
                   posting.workModel === 'On-Site' ? 'bg-red-100 text-red-700' :
                   'bg-blue-100 text-blue-700'
                 }`}>
                   {posting.workModel || 'Belirtilmemiş'}
                 </span>
               </div>
             </div>
          </div>

          <button 
            onClick={handleApply}
            disabled={applying || posting.hasApplied}
            className={`w-full md:w-auto font-medium py-3 px-8 rounded-xl transition-all shadow-md flex justify-center items-center gap-2 ${
              posting.hasApplied 
              ? 'bg-gray-200 text-gray-500 cursor-not-allowed' 
              : 'bg-blue-600 hover:bg-blue-700 text-white hover:shadow-lg disabled:opacity-70 disabled:cursor-not-allowed'
            }`}
          >
            {applying ? (
              <svg className="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle><path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path></svg>
            ) : posting.hasApplied ? (
              <>
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M5 13l4 4L19 7"></path></svg>
                Başvuru Yapıldı
              </>
            ) : "Hemen Başvur"}
          </button>
        </div>

        <div className="border-t border-gray-100 pt-8 mt-4 space-y-8">
          <div>
            <h3 className="text-xl font-bold text-gray-900 mb-4">İlan Detayı</h3>
            <div className="prose prose-blue max-w-none text-gray-600 leading-relaxed whitespace-pre-wrap">
              {posting.description}
            </div>
          </div>

          {posting.skills && posting.skills.length > 0 && (
            <div>
              <h3 className="text-xl font-bold text-gray-900 mb-4">Aranan Yetenekler</h3>
              <div className="flex flex-wrap gap-2">
                {posting.skills.map((skill: string, index: number) => (
                  <span key={index} className="bg-indigo-50 text-indigo-700 px-4 py-2 rounded-lg font-medium border border-indigo-100">
                    {skill}
                  </span>
                ))}
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default PostingDetail;
