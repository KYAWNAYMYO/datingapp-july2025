using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class MembersController(AppDbContext context) : BaseApiController
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
        [Authorize]
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
