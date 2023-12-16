using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
    public interface iCommandDataClient
    {
        Task SendPlatformToCommand(PlatformReadDto plat);
    }
}
