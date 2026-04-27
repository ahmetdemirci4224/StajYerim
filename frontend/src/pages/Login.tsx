import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import api from '../services/api';

const Login = () => {
  const [apiError, setApiError] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();

  // Zod ile Login validasyon şeması
  const schema = z.object({
    email: z.string()
      .min(1, 'E-posta adresi zorunludur.')
      .email('Lütfen geçerli bir e-posta adresi girin.'),
    password: z.string()
      .min(1, 'Şifre zorunludur.')
  });

  type FormData = z.infer<typeof schema>;

  const { register, handleSubmit, formState: { errors } } = useForm<FormData>({
    resolver: zodResolver(schema),
    mode: 'onChange' // Anlık validasyon
  });

  const onSubmit = async (data: FormData) => {
    setIsLoading(true);
    setApiError('');
    try {
      const response = await api.post('/auth/login', { 
        email: data.email, 
        password: data.password 
      });
      
      if (response.data.isSuccess) {
        localStorage.setItem('token', response.data.data.token);
        
        const role = data.email.includes('.edu') ? 'Student' : 'Company';
        localStorage.setItem('role', role);

        if (role === 'Student') {
          navigate('/student-dashboard');
        } else {
          navigate('/company-dashboard');
        }
      } else {
        setApiError(response.data.message);
      }
    } catch (err: any) {
      setApiError(err.response?.data?.message || 'Giriş işlemi başarısız.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="flex justify-center items-center min-h-[80vh]">
      <div className="bg-white p-8 rounded-2xl shadow-xl border border-gray-100 w-full max-w-md transition-all duration-300">
        <div className="text-center mb-8">
          <h2 className="text-3xl font-bold text-gray-900 mb-2">Hoş Geldiniz</h2>
          <p className="text-gray-500 text-sm">StajYerim hesabınıza giriş yapın</p>
        </div>
        
        {apiError && (
          <div className="bg-red-50 text-red-600 p-3 rounded-lg mb-6 text-sm border border-red-100 text-center animate-fade-in">
            {apiError}
          </div>
        )}
        
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
          {/* Email Alanı */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Email Adresi</label>
            <input 
              type="text" 
              {...register('email')}
              placeholder="ornek@universite.edu.tr" 
              className={`w-full px-4 py-2 border rounded-lg outline-none transition-all focus:ring-2 ${
                errors.email 
                  ? 'border-red-300 focus:ring-red-200 focus:border-red-500 bg-red-50/30' 
                  : 'border-gray-300 focus:ring-blue-200 focus:border-blue-500'
              }`}
            />
            {errors.email && (
              <p className="mt-1.5 text-sm text-red-500 flex items-center gap-1 animate-fade-in">
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
                {errors.email.message}
              </p>
            )}
          </div>

          {/* Şifre Alanı */}
          <div>
            <div className="flex justify-between items-center mb-1">
              <label className="block text-sm font-medium text-gray-700">Şifre</label>
              <a href="#" className="text-xs text-blue-600 hover:text-blue-800 font-medium">Şifremi Unuttum</a>
            </div>
            <input 
              type="password" 
              {...register('password')}
              placeholder="••••••••" 
              className={`w-full px-4 py-2 border rounded-lg outline-none transition-all focus:ring-2 ${
                errors.password 
                  ? 'border-red-300 focus:ring-red-200 focus:border-red-500 bg-red-50/30' 
                  : 'border-gray-300 focus:ring-blue-200 focus:border-blue-500'
              }`}
            />
            {errors.password && (
              <p className="mt-1.5 text-sm text-red-500 flex items-start gap-1 animate-fade-in">
                <svg className="w-4 h-4 mt-0.5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
                <span>{errors.password.message}</span>
              </p>
            )}
          </div>

          <button 
            type="submit" 
            disabled={isLoading}
            className="w-full mt-2 bg-blue-600 hover:bg-blue-700 text-white font-medium py-2.5 rounded-lg transition-colors shadow-sm disabled:opacity-70 flex justify-center items-center"
          >
            {isLoading ? (
              <svg className="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle><path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path></svg>
            ) : "Giriş Yap"}
          </button>
        </form>
        <p className="mt-6 text-center text-sm text-gray-600">
          Hesabınız yok mu? <Link to="/register" className="text-blue-600 hover:text-blue-800 font-medium">Hemen Kayıt Olun</Link>
        </p>
      </div>
    </div>
  );
};

export default Login;
