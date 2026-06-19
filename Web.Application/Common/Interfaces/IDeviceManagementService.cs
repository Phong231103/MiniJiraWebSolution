using Web.Domain.Entities;

namespace Web.Application.Common.Interfaces
{
    public interface IDeviceManagementService
    {
        Task LoginOnNewDevice(Guid userId, Device newDevice, CancellationToken cancellationToken);

        Task TrustDevice(Guid userId, string deviceFingerprint, CancellationToken cancellationToken);
    }
}
