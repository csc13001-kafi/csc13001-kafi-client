using System.Threading.Tasks;

namespace kafi.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
