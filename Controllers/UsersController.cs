using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using UserManagementAPI.Services;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        // GET: api/users
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _userService.GetAllUsers();
                return Ok(new
                {
                    success = true,
                    count = users.Count(),
                    data = users
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = "Failed to retrieve users",
                    message = ex.Message
                });
            }
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            try
            {
                var user = _userService.GetUserById(id);
                if (user == null)
                    return NotFound(new
                    {
                        success = false,
                        error = $"User with ID {id} not found"
                    });

                return Ok(new
                {
                    success = true,
                    data = user
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = "Failed to retrieve user",
                    message = ex.Message
                });
            }
        }

        // POST: api/users
        [HttpPost]
        public IActionResult CreateUser([FromBody] User user)
        {
            try
            {
                // Model validation (automatically handled by [ApiController] attribute)
                if (!ModelState.IsValid)
                    return BadRequest(new
                    {
                        success = false,
                        error = "Validation failed",
                        details = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });

                var createdUser = _userService.CreateUser(user);

                return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, new
                {
                    success = true,
                    message = "User created successfully",
                    data = createdUser
                });
            }
            catch (ArgumentException ex)
            {
                return Conflict(new
                {
                    success = false,
                    error = "Conflict",
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = "Failed to create user",
                    message = ex.Message
                });
            }
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new
                    {
                        success = false,
                        error = "Validation failed",
                        details = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });

                var updatedUser = _userService.UpdateUser(id, user);
                if (updatedUser == null)
                    return NotFound(new
                    {
                        success = false,
                        error = $"User with ID {id} not found"
                    });

                return Ok(new
                {
                    success = true,
                    message = "User updated successfully",
                    data = updatedUser
                });
            }
            catch (ArgumentException ex)
            {
                return Conflict(new
                {
                    success = false,
                    error = "Conflict",
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = "Failed to update user",
                    message = ex.Message
                });
            }
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var success = _userService.DeleteUser(id);
                if (!success)
                    return NotFound(new
                    {
                        success = false,
                        error = $"User with ID {id} not found"
                    });

                return Ok(new
                {
                    success = true,
                    message = $"User with ID {id} deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = "Failed to delete user",
                    message = ex.Message
                });
            }
        }
    }
}