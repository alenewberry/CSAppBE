﻿namespace CSAppBE.Web.Helpers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Entities;
    using Microsoft.AspNetCore.Identity;
    using Models;    

    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();

        Task<IdentityResult> UpdateUserAsync(User user);

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        Task<SignInResult> ValidatePasswordAsync(User user, string password);

        User GetUserWithCertificateByEmail(string email);
    }
}