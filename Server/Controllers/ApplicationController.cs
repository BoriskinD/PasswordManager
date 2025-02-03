using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Model;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public ApplicationController(ApplicationDbContext context) => this.context = context;

        [HttpGet]
        [ProducesResponseType(typeof(Application), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get()
        {
            var applications = await context.Application.ToListAsync();
            return applications.Any() == true ? Ok(applications) : NotFound();
        }

        //Добавление
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Application application)
        {
            await context.Application.AddAsync(application);
            await context.SaveChangesAsync();

            return Created(string.Empty, application.Id);
        }

        //Редактирование
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Application application) 
        {
            context.Entry(application).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id)
        {
            Application? applicationToDelete = await context.Application.FindAsync(id);
            if (applicationToDelete == null)
                 return NotFound();

            context.Application.Remove(applicationToDelete);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
