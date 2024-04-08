using Microsoft.AspNetCore.Components.Authorization;

namespace Chinook.Areas
{
    public interface ICustomAuthenticationService
    {
        Task<AuthenticationState> GetAuthenticationStateAsync();
    }

    public class CustomAuthenticationProvider(AuthenticationStateProvider authenticationStateProvider): ICustomAuthenticationService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider = authenticationStateProvider;

        public async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return await _authenticationStateProvider.GetAuthenticationStateAsync();
        }
    }
}
