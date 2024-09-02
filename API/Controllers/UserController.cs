using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;
using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly ApiDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(ApiDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Kullanıcıları listelemek için (Yetki gerektirir)
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // Kullanıcı kayıt işlemi
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User newUser)
        {
            // Kullanıcı adının zaten mevcut olup olmadığını kontrol et
            if (await _context.Users.AnyAsync(u => u.Username == newUser.Username))
            {
                return BadRequest("Kullanıcı adı zaten mevcut. Lütfen başka bir kullanıcı adı seçin.");
            }

            if (newUser.Password.Length < 7)
            {
                return BadRequest("Şifre en az 7 karakter uzunluğunda olmalıdır.");
            }

            // Yeni kullanıcıyı ekle
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok("Kullanıcı başarıyla kaydedildi.");
        }

        // Tüm kullanıcıları listelemek için
        [HttpGet]
        [Route("GetAllUser")]
        public IActionResult GetUser()
        {
            string connectionString = _configuration.GetConnectionString("API");
            string query = "SELECT * FROM [Users]";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        List<User> usersList = new List<User>();

                        foreach (DataRow row in dt.Rows)
                        {
                            User user = new User
                            {
                                ID = Convert.ToInt32(row["ID"]),
                                FirstName= Convert.ToString(row["FirstName"]),
                                Surname = Convert.ToString(row["Surname"]),
                                Username = Convert.ToString(row["Username"]),
                                Password = Convert.ToString(row["Password"])
                            };
                            usersList.Add(user);
                        }

                        return Ok(usersList);
                    }
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"SQL Hatası: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Beklenmedik Hata: {ex.Message}");
            }
        }
    }
}
