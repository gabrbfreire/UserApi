using FluentResults;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserApi.Data.Dtos;
using UserApi.Services;

namespace UserApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CreateUserController : ControllerBase
    {
        private CreateUserService _createUserService;

        public CreateUserController(CreateUserService createUserService)
        {
            _createUserService = createUserService;
        }

        [HttpPost]
        public IActionResult CreateUser(CreateUserDto createUserDto)
        {
            Result result = _createUserService.CreateUser(createUserDto);
            if (result.IsFailed) return BadRequest(result.Errors);
            return Ok();
        }

        [HttpGet("/activate/{Id}/{activationCode}")]
        public IActionResult ActivateUserAccount([FromRoute] ActivateUserAccountDto activateUserAccountDto)
        {
            Result result = _createUserService.ActivateUserAccount(activateUserAccountDto);
            if (result.IsFailed) return BadRequest(result.Errors);
            return Ok();
        }
    }
}
