import { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import api from '../services/api';

const Register = () => {
  const [role, setRole] = useState<'Student' | 'Company'>('Student');
  const [apiError, setApiError] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();

  // Zod ile dinamik validasyon şeması
  const schema = z.object({
    name: z.string().min(3, 'En az 3 karakter girilmelidir.'),
    email: z.string()
      .min(1, 'E-posta adresi zorunludur.')
      .email('Lütfen geçerli bir e-posta adresi girin.')
      .refine(val => {
        if (role === 'Student') {
          return val.endsWith('.edu') || val.endsWith('.edu.tr');
        }
        return true;
      }, 'Öğrenciler sadece .edu veya .edu.tr uzantılı e-posta kullanabilir.'),
    password: z.string()
      .min(8, 'Şifre en az 8 karakter olmalıdır.')
      .regex(/[A-Z]/, 'Şifre en az 1 büyük harf içermelidir.')
      .regex(/[0-9]/, 'Şifre en az 1 rakam içermelidir.')
  });

  type FormData = z.infer<typeof schema>;

  const { register, handleSubmit, formState: { errors }, clearErrors } = useForm<FormData>({
    resolver: zodResolver(schema),
    mode: 'onChange' // Anlık validasyon için onChange modunu kullanıyoruz
  });

  // Rol değiştiğinde hataları temizle ve gerekirse email'i yeniden doğrula
  useEffect(() => {
    clearErrors();
  }, [role, clearErrors]);

  const onSubmit = async (data: FormData) => {
    setIsLoading(true);
    setApiError('');
    try {
      const endpoint = role === 'Student' ? '/auth/register/student' : '/auth/register/company';
      const payload = role === 'Student' 
        ? { fullName: data.name, email: data.email, password: data.password }
        : { companyName: data.name, email: data.email, password: data.password };

      const response = await api.post(endpoint, payload);
      
      if (response.data.isSuccess) {
        navigate(`/verify-email?email=${encodeURIComponent(data.email)}`);
      } else {
        setApiError(response.data.message);
      }
    } catch (err: any) {
        const errorData = err.response?.data;
        if(errorData?.errors && errorData.errors.length > 0) {
            setApiError(errorData.errors[0]);
        } else {
            setApiError(errorData?.message || 'Kayıt işlemi başarısız.');
        }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="flex justify-center items-center min-h-[80vh]">
      <div className="bg-white p-8 rounded-2xl shadow-xl border border-gray-100 w-full max-w-md transition-all duration-300">
        <div className="text-center mb-8">
          <h2 className="text-3xl font-bold text-gray-900 mb-2">Aramıza Katıl</h2>
          <p className="text-gray-500 text-sm">StajYerim'e hemen ücretsiz kayıt ol</p>
        </div>
        
        {apiError && (
          <div className="bg-red-50 text-red-600 p-3 rounded-lg mb-6 text-sm border border-red-100 text-center animate-fade-in">
            {apiError}
          </div>
        )}

        <div className="flex p-1 mb-6 bg-gray-100 rounded-lg">
          <button
            type="button"
            className={`flex-1 py-2 text-sm font-medium rounded-md transition-all ${role === 'Student' ? 'bg-white shadow text-blue-600' : 'text-gray-500 hover:text-gray-700'}`}
            onClick={() => setRole('Student')}
          >
            Öğrenci
          </button>
          <button
            type="button"
            className={`flex-1 py-2 text-sm font-medium rounded-md transition-all ${role === 'Company' ? 'bg-white shadow text-blue-600' : 'text-gray-500 hover:text-gray-700'}`}
            onClick={() => setRole('Company')}
          >
            Şirket
          </button>
        </div>
        
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
          {/* İsim Alanı */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              {role === 'Student' ? 'Ad Soyad' : 'Şirket Adı'}
            </label>
            <input 
              type="text" 
              {...register('name')}
              className={`w-full px-4 py-2 border rounded-lg outline-none transition-all focus:ring-2 ${
                errors.name 
                  ? 'border-red-300 focus:ring-red-200 focus:border-red-500 bg-red-50/30' 
                  : 'border-gray-300 focus:ring-blue-200 focus:border-blue-500'
              }`}
            />
            {errors.name && (
              <p className="mt-1.5 text-sm text-red-500 flex items-center gap-1 animate-fade-in">
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
                {errors.name.message}
              </p>
            )}
          </div>

          {/* Email Alanı */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Email Adresi {role === 'Student' && <span className="text-xs text-blue-500 font-normal ml-1">(.edu uzantılı)</span>}
            </label>
            <input 
              type="text" 
              {...register('email')}
              placeholder={role === 'Student' ? "ogrenci@universite.edu.tr" : "iletisim@sirket.com"}
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
            <label className="block text-sm font-medium text-gray-700 mb-1">Şifre</label>
            <input 
              type="password" 
              {...register('password')}
              placeholder="En az 8 karakter, 1 büyük harf, 1 rakam"
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
            className="w-full mt-4 bg-blue-600 hover:bg-blue-700 text-white font-medium py-2.5 rounded-lg transition-colors shadow-sm disabled:opacity-70 flex justify-center items-center"
          >
            {isLoading ? (
              <svg className="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle><path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path></svg>
            ) : "Kayıt Ol"}
          </button>
        </form>
        <p className="mt-6 text-center text-sm text-gray-600">
          Zaten hesabınız var mı? <Link to="/login" className="text-blue-600 hover:text-blue-800 font-medium">Giriş Yapın</Link>
        </p>
      </div>
    </div>
  );
};

export default Register;
