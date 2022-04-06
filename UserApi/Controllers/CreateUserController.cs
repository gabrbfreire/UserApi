using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserApi.Data.Dtos;

namespace UserApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CreateUserController : ControllerBase
    {
        [HttpPost]
        public IActionResult CreateUser(CreateUserDto createUserDto)
        {
            return Ok();
        } 
    }
}
