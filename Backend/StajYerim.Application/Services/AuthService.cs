using StajYerim.Application.DTOs.Auth;
using StajYerim.Application.Interfaces;
using StajYerim.Core.Common;
using StajYerim.Core.Entities;
using StajYerim.Core.Enums;
using StajYerim.Core.Interfaces;

namespace StajYerim.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtProvider _jwtProvider;
        private readonly IEmailService _emailService;

        public AuthService(IGenericRepository<User> userRepository, IUnitOfWork unitOfWork, IJwtProvider jwtProvider, IEmailService emailService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _jwtProvider = jwtProvider;
            _emailService = emailService;
        }

        public async Task<Result<AuthResponseDto>> RegisterStudentAsync(RegisterStudentDto dto, CancellationToken cancellationToken = default)
        {
            if (!dto.Email.EndsWith(".edu") && !dto.Email.EndsWith(".edu.tr"))
            {
                return Result<AuthResponseDto>.Failure("Öğrenciler sadece .edu veya .edu.tr uzantılı mail adresleriyle kayıt olabilirler.");
            }

            var existingUsers = await _userRepository.FindAsync(u => u.Email == dto.Email);
            if (existingUsers.Any())
            {
                return Result<AuthResponseDto>.Failure("Bu email adresi zaten kullanımda.");
            }

            var verificationCode = new Random().Next(100000, 999999).ToString();

            var user = new User
            {
                Email = dto.Email,
                FullName = dto.FullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = Role.Student,
                IsEmailVerified = false,
                VerificationCode = verificationCode
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            var subject = "StajYerim E-Posta Doğrulama";
            var body = $"<p>Merhaba {user.FullName},</p><p>Kayıt işlemini tamamlamak için doğrulama kodunuz: <strong>{verificationCode}</strong></p>";
            await _emailService.SendEmailAsync(user.Email, subject, body);

            return Result<AuthResponseDto>.Success(new AuthResponseDto(), "Öğrenci kaydı başarıyla tamamlandı. Lütfen e-posta adresinize gönderilen kod ile hesabınızı doğrulayın.");
        }

        public async Task<Result<AuthResponseDto>> RegisterCompanyAsync(RegisterCompanyDto dto, CancellationToken cancellationToken = default)
        {
            var existingUsers = await _userRepository.FindAsync(u => u.Email == dto.Email);
            if (existingUsers.Any())
            {
                return Result<AuthResponseDto>.Failure("Bu email adresi zaten kullanımda.");
            }

            var verificationCode = new Random().Next(100000, 999999).ToString();

            var user = new User
            {
                Email = dto.Email,
                FullName = dto.CompanyName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = Role.Company,
                IsEmailVerified = false,
                VerificationCode = verificationCode
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            var subject = "StajYerim E-Posta Doğrulama";
            var body = $"<p>Merhaba {user.FullName},</p><p>Kayıt işlemini tamamlamak için doğrulama kodunuz: <strong>{verificationCode}</strong></p>";
            await _emailService.SendEmailAsync(user.Email, subject, body);

            return Result<AuthResponseDto>.Success(new AuthResponseDto(), "Şirket kaydı başarıyla tamamlandı. Lütfen e-posta adresinize gönderilen kod ile hesabınızı doğrulayın.");
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
        {
            var users = await _userRepository.FindAsync(u => u.Email == dto.Email);
            var user = users.FirstOrDefault();
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Result<AuthResponseDto>.Failure("Email veya şifre hatalı.");
            }

            if (!user.IsEmailVerified)
            {
                return Result<AuthResponseDto>.Failure("Lütfen giriş yapmadan önce e-posta adresinizi doğrulayın.");
            }

            var token = _jwtProvider.GenerateToken(user);

            var responseData = new AuthResponseDto { Token = token };
            return Result<AuthResponseDto>.Success(responseData, "Giriş başarılı.");
        }

        public async Task<Result<bool>> VerifyEmailAsync(VerifyEmailDto dto, CancellationToken cancellationToken = default)
        {
            var users = await _userRepository.FindAsync(u => u.Email == dto.Email);
            var user = users.FirstOrDefault();

            if (user == null)
            {
                return Result<bool>.Failure("Kullanıcı bulunamadı.");
            }

            if (user.IsEmailVerified)
            {
                return Result<bool>.Failure("Bu e-posta adresi zaten doğrulanmış.");
            }

            if (user.VerificationCode != dto.Code)
            {
                return Result<bool>.Failure("Geçersiz doğrulama kodu.");
            }

            user.IsEmailVerified = true;
            user.VerificationCode = null; // Kodu kullandıktan sonra temizle

            _userRepository.Update(user);
            await _unitOfWork.CompleteAsync();

            return Result<bool>.Success(true, "E-posta adresi başarıyla doğrulandı.");
        }
    }
}
