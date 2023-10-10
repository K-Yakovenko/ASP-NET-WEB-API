using APITask.DBContext;
using APITask.Models;
using APITask.Views;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APITask.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        public UserController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sort = "nameasc",
            [FromQuery] int minAge = 0,
            [FromQuery] int maxAge = 150)
        {
            if (page <= 0)
                return BadRequest("Page number must be greater than 0.");

            if (pageSize <= 0)
                return BadRequest("Page size must be greater than 0.");

            IQueryable<User> query = _context.Users;

            query = query.Where(u => u.Age >= minAge && u.Age <= maxAge);

            if (!string.IsNullOrEmpty(sort))
            {
                if (sort.ToLower() == "nameasc")
                    query = query.OrderBy(u => u.Name);
                else if (sort.ToLower() == "namedesc")
                    query = query.OrderByDescending(u => u.Name);
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            if (page > totalPages)
                return BadRequest("Page number exceeds the total number of pages.");

            var queryResult = await query.Skip((page - 1) * pageSize).Take(pageSize).Include(u => u.Roles).ToListAsync();

            var users = queryResult.Select(item => new UsersView
            {
                Id = item.Id,
                Age = item.Age,
                Email = item.Email,
                Roles = item.Roles
            }).ToList();

            var response = new GetUsersResponseView
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                Page = page,
                PageSize = pageSize,
                Users = users
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound($"User with ID {id} not found.");

            return Ok(user);
        }

        [HttpPut("{id}/{roleId}")]
        public async Task<IActionResult> AddRoleToUser(int id, int roleId)
        {
            var user = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id);
            var role = await _context.Roles.FindAsync(roleId);

            if (user == null)
                return NotFound($"User with ID {id} not found.");

            if (role == null)
                return BadRequest($"Role with ID {roleId} not found.");

            if (user.Roles.Contains(role))
                return BadRequest($"Role \"{role.Name}\" already exists for user with ID {id}.");

            user.Roles.Add(role);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<User>> AddNewUser([FromBody] User newUser)
        {
            if (newUser == null)
                return BadRequest($"User {newUser} caanot be added.");

            if (string.IsNullOrEmpty(newUser.Name) || string.IsNullOrEmpty(newUser.Email) || newUser.Age <= 0)
                return BadRequest("Email, name and age are required. Age must be greater than 0.");

            var existingUserEmail = _context.Users.FirstOrDefault(u => u.Email == newUser.Email);
            if (existingUserEmail != null)
                return BadRequest("User with this email already exists.");

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new User {Age = newUser.Age, Email = newUser.Email, Name = newUser.Name }, newUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserInfo(int id, [FromBody] User updatedUser)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound($"User with ID {id} not found.");

            if (updatedUser == null)
                return BadRequest("Incorrect user data.");

            user.Name = updatedUser.Name ?? user.Name;
            user.Email = updatedUser.Email ?? user.Email;
            user.Age = updatedUser.Age > 0 ? updatedUser.Age : user.Age;

            if (updatedUser.Roles != null)
            {
                user.Roles.Clear();
                foreach (var role in updatedUser.Roles)
                {
                    var existingRole = await _context.Roles.FindAsync(role.Id);
                    if (existingRole != null)
                        user.Roles.Add(existingRole);
                }
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound($"User with ID {id} not found.");

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}