using ResourceService.Application.Common.Models;

namespace ResourceService.Application.Common.Interfaces;

public interface IUserContext
{
    Guid GetCurrentUserId();
    UserRole GetUserRole();
}