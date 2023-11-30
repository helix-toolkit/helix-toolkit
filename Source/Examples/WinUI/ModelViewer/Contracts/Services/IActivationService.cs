using System.Threading.Tasks;

namespace ModelViewer.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
