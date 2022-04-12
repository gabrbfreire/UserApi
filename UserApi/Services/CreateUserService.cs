using AutoMapper;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserApi.Data.Dtos;
using UserApi.Models;

namespace UserApi.Services
{
    public class CreateUserService
    {
        private IMapper _mapper;
        private UserManager<IdentityUser<int>> _userManager;

        public CreateUserService(IMapper mapper, UserManager<IdentityUser<int>> userManager)
        {
            _mapper = mapper;
            _userManager = userManager;
        }

        public Result CreateUser([FromBody] CreateUserDto createUserDto)
        {
            //Checks if email already exists
            if ((_userManager.FindByEmailAsync(createUserDto.Email).Result) == null){
                User user = _mapper.Map<User>(createUserDto);
                IdentityUser<int> userIdentity = _mapper.Map<IdentityUser<int>>(user);
                Task<IdentityResult> resultIdentity = _userManager.CreateAsync(userIdentity, createUserDto.Password);

                if (resultIdentity.Result.Succeeded)
                {
                    var activationCode = _userManager.GenerateEmailConfirmationTokenAsync(userIdentity).Result;
                    return Result.Ok().WithSuccess(activationCode);
                }

                List<string> errorsDescription = new List<string>();
                resultIdentity.Result.Errors.ToList().ForEach(n => errorsDescription.Add(n.Description));
                return Result.Fail(string.Join(",", errorsDescription.ToArray()));
            }
            return Result.Fail("Email '" + createUserDto.Email + "' is already taken.");
        }

        internal Result ActivateUserAccount(ActivateUserAccountDto activateUserAccountDto)
        {
            var identityUser = _userManager.Users.FirstOrDefault(user => user.Id == activateUserAccountDto.Id);
            var identityResult = _userManager.ConfirmEmailAsync(identityUser, activateUserAccountDto.ActivationCode).Result;

            if (identityResult.Succeeded) return Result.Ok();
            return Result.Fail("Failed to activate user account.");
        }
    }
}
