using AutoMapper;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UserApi.Data.Dtos;
using UserApi.Models;

namespace UserApi.Services
{
    public class CreateUserService
    {
        private IMapper _mapper;
        private UserManager<IdentityUser<int>> _userManager;
        private EmailService _emailService;

        public CreateUserService(IMapper mapper, UserManager<IdentityUser<int>> userManager, EmailService emailService)
        {
            _mapper = mapper;
            _userManager = userManager;
            _emailService = emailService;
        }

        public Result CreateUser([FromBody] CreateUserDto createUserDto)
        {
            User user = _mapper.Map<User>(createUserDto);
            IdentityUser<int> userIdentity = _mapper.Map<IdentityUser<int>>(user);
            Task<IdentityResult> resultIdentity = _userManager.CreateAsync(userIdentity, createUserDto.Password);

            if (resultIdentity.Result.Succeeded)
            {
                var activationCode = _userManager.GenerateEmailConfirmationTokenAsync(userIdentity).Result;
                var encodedActivationCode = HttpUtility.UrlEncode(activationCode);
                    
                _emailService.SendConfirmationEmail(userIdentity.Id, createUserDto.Email, encodedActivationCode);
                    
                return Result.Ok();
            }

            List<string> errorsDescription = new List<string>();
            resultIdentity.Result.Errors.ToList().ForEach(n => errorsDescription.Add(n.Description));
            return Result.Fail(string.Join(",", errorsDescription.ToArray()));
        }

        internal Result ActivateUserAccount(ActivateUserAccountDto activateUserAccountDto)
        {
            var identityUser = _userManager.Users.FirstOrDefault(user => user.Id == activateUserAccountDto.Id);
            var decodedActivationCode = HttpUtility.UrlDecode(activateUserAccountDto.ActivationCode);
            decodedActivationCode = decodedActivationCode.Replace(" ", "+");
            var identityResult = _userManager.ConfirmEmailAsync(identityUser, decodedActivationCode).Result;

            if (identityResult.Succeeded) return Result.Ok();
            return Result.Fail("Failed to activate user account.");
        }
    }
}
