using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Model;
using System.Security.Claims;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public ApplicationController(ApplicationDbContext context) => this.context = context;

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(Application), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get()
        {
            //Класс User представляет авторизированного пользователя и заполняется middleware, содержит данные из JWT токена пользователя
            //Извлекам id пользователя из токена
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var applications = await context.Application.Where(app => app.UserId == int.Parse(userId)).ToListAsync();
            return applications.Any() == true ? Ok(applications) : NotFound();
        }

        //Добавление
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Application application)
        {
            await context.Application.AddAsync(application);
            await context.SaveChangesAsync();

            return Created(string.Empty, application.Id);
        }

        //Редактирование
        [Authorize]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Application application)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            Application? applicationToUpdate = await context.Application.FirstOrDefaultAsync(app => app.Id == application.Id && app.UserId == int.Parse(userId));
            if (applicationToUpdate == null)
            { 
                return NotFound();
            }

            applicationToUpdate.Title = application.Title;
            applicationToUpdate.UserLogin = application.UserLogin;
            applicationToUpdate.UserPassword = application.UserPassword;
            applicationToUpdate.ImagePath = application.ImagePath;

            await context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            Application? applicationToDelete = await context.Application.FirstOrDefaultAsync(app => app.Id == id && app.UserId == int.Parse(userId));
            if (applicationToDelete == null)
            {
                return NotFound();
            }    
                
            context.Application.Remove(applicationToDelete);
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] User user)
        {
            if (context.Users.Any(u => u.Login == user.Login))
            {
                return BadRequest();
            }

            string hashPassword = PasswordHelper.CreateHash(user.PasswordHash);
            User registeredUser = new User()
            {
                Login = user.Login,
                PasswordHash = hashPassword,
            };
            context.Users.Add(registeredUser);
            context.SaveChanges();

            return Ok();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] User user)
        { 
            User? loginedUser = context.Users.FirstOrDefault(u => u.Login == user.Login);
            if (loginedUser != null && PasswordHelper.VerifyPassword(user.PasswordHash, loginedUser.PasswordHash))
            {
                string token = TokenGenerator.GenerateJwtToken(loginedUser);
                //Возвращаем анонимный объект
                return Ok(new { UserId = loginedUser.Id, Token = token });
            }

            return Unauthorized();
        }
    }
}
