using System.Threading.Tasks;

namespace IotHubCommander
{
    /// <summary>
    /// interface for all IotHub/ServiceBus related classes
    /// </summary>
    internal interface IHubModule
    {
        Task Execute();
    }
}
