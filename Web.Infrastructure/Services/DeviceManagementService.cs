//using Microsoft.EntityFrameworkCore;
//using Web.Application.Common.Interfaces;
//using Web.Domain.Entities;

//namespace Web.Infrastructure.Services
//{
//    public class DeviceManagementService : IDeviceManagementService
//    {
//        private readonly IApplicationDbContext _context;

//        public DeviceManagementService(IApplicationDbContext applicationDbContext)
//        {
//            _context = applicationDbContext;
//        }

//        public async Task LoginOnNewDevice(Guid userId, Device newDevice, CancellationToken cancellationToken)
//        {
//            var activeDevices = await _context.Devices
//                .Where(d => d.UserId == userId && d.IsActive)
//                .OrderBy(d => d.LastLogin)
//                .ToListAsync();

//            if (activeDevices.Count >= 2)
//            {
//                var oldestDevice = activeDevices.First();

//                oldestDevice.Login();
//            }

//            //Thêm thiết bị mới vào
//            newDevice.Login(newDevice.RefreshToken, newDevice.RefreshTokenExpiryTime.GetValueOrDefault());

//            _context.Devices.Add(newDevice);

//            await _context.SaveChangesAsync(cancellationToken);
//        }

//        public async Task TrustDevice(Guid userId, string deviceFingerprint, CancellationToken cancellationToken)
//        {
//            var currentDevice = await _context.Devices.FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceFingerprint == deviceFingerprint);

//            if (currentDevice == null)
//            {
//                return;
//            }
//            // Lấy thiết bị ghi nhớ
//            var trustedDevices = await _context.Devices
//                .Where(d => d.UserId == userId && d.IsTrusted)
//                .OrderBy(d => d.CreatedDate)
//                .ToListAsync();

//            // bỏ ghi nhớ thiết bị cũ nhất
//            if (trustedDevices.Count >= 2 && !currentDevice.IsTrusted)
//            {
//                var oldestTrustedDevice = trustedDevices.First();
//                oldestTrustedDevice.IsTrusted = false;
//                oldestTrustedDevice.RefreshToken = null;
//            }

//            // 4. Cập nhật trạng thái ghi nhớ cho thiết bị hiện tại
//            currentDevice.IsTrusted = true;

//            await _context.SaveChangesAsync(cancellationToken);
//        }
//    }
//}
