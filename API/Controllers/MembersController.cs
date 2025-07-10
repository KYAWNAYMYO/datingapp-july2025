using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")] // localhost:5001/api/members
    [ApiController]
    public class MembersController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMembers()
        {
            var members = await context.Users.ToListAsync();

            return members;
        }

        /*         public ActionResult<IReadOnlyList<AppUser>> GetMembers()
                                {
                                    var member = context.Users.ToList();

                                    return member;
                                } */

        [HttpGet("{id}")] // localhost:5001/api/members/bob-id
        public async Task<ActionResult<AppUser>> GetMember(string id)
        {
            var member = await context.Users.FindAsync(id);

            if (member == null) NotFound();

            return member;
        }
/*         public ActionResult<AppUser> GetMember(string id)
                {
                    var member = context.Users.Find(id);

                    if (member == null) return NotFound();

                    return member;
                } */
    }
}
