using System.Threading.Tasks;

namespace kafi.Contracts
{
    public interface IActivationService
    {
        Task ActivateAsync(object activationArgs);
    }
}
