using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string UserName { get; }
        bool IsInRole(string role);
        Task<bool> AuthorizeAsync(string policyName);
    }
}
