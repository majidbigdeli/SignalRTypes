using Example.ViewModel;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Example.Hubs.Clients
{
    public interface IChatClient
    {
        [HubMethodName("hello")]
        Task Hello();

        [HubMethodName("start-work")]
        Task StartWorkAsync(StartWorkVm message);

        [HubMethodName("stop-work")]
        Task StopWorkAsync(StopWorkVm message);

    }
}
