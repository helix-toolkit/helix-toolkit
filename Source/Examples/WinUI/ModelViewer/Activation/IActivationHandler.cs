using System.Threading.Tasks;

namespace ModelViewer.Activation;

public interface IActivationHandler
{
    bool CanHandle(object args);

    Task HandleAsync(object args);
}
