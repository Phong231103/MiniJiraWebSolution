//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using Web.Application.Auth.Models;
//using Web.Application.Common.Interfaces;
//using Web.Domain.Primitives;

//namespace Web.Application.Auth.Commands
//{
//    public record VerifyEmailCommand(string Email) : IRequest<Result<string>>;

//    public class InitiateRegisterCommandHandler : IRequestHandler<VerifyEmailCommand, Result<string>>
//    {
//        private readonly IApplicationDbContext _context;
//        private readonly ICacheService _cacheService;
//        private readonly IEmailService _emailService;

//        public InitiateRegisterCommandHandler(IApplicationDbContext context, ICacheService cacheService, IEmailService emailService)
//        {
//            _context = context;
//            _cacheService = cacheService;
//            _emailService = emailService;
//        }

//        public async Task<Result<string>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
//        {
//            // 1. Kiểm tra xem Email đã tồn tại trong DB chính thức chưa
//            var emailExists = await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);

//            if (emailExists)
//            {
//                return Error.Conflict("User.EmailExists", "This email is already registered.");
//            }

//            // 2. Tạo OTP (6 số ngẫu nhiên)
//            var otp = new Random().Next(100000, 999999).ToString();

//            // 3. Tạo RegistrationId (Dùng Guid hoặc mã hóa Email)
//            var registrationId = Guid.NewGuid().ToString();

//            // 4. Lưu thông tin tạm vào Cache (Sống 5 phút)
//            var pendingData = new PendingRegistration
//            {
//                Email = request.Email,
//                Otp = otp
//            };

//            var cacheKey = $"reg_{registrationId}";

//            await _cacheService.SetAsync(cacheKey, pendingData, TimeSpan.FromMinutes(5), cancellationToken);

//            // 5. Gửi OTP qua Email
//            await _emailService.SendOtpAsync(request.Email, otp);

//            // 6. Trả về RegistrationId cho Client để bước sau dùng
//            return Result<string>.Success(registrationId);
//        }
//    }
//}
