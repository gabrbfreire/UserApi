using FluentResults;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserApi.Data.Dtos;
using UserApi.Models;

namespace UserApi.Services
{
    public class LoginService
    {
        private SignInManager<IdentityUser<int>> _signInManager;
        private TokenService _tokenService;

        public LoginService(SignInManager<IdentityUser<int>> signInManager, TokenService tokenService)
        {
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public Result LoginUser(LoginDto request)
        {
            Task<SignInResult> resultIdentity = _signInManager.PasswordSignInAsync(request.UserName, request.Password, false, false);
            if (resultIdentity.Result.Succeeded)
            {
                Token token = _tokenService.CreateToken(_signInManager.UserManager.Users.FirstOrDefault(user => user.NormalizedUserName == request.UserName.ToUpper()));
                return Result.Ok().WithSuccess(token.Value);
            }
            // if email is not confirmed
            if (resultIdentity.Result.IsNotAllowed)
            {
                return Result.Fail("User cannot sign in without a confirmed email.");
            }
            return Result.Fail("Incorrect username or password.");
        }
    }
}
