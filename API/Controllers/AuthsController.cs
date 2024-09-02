#region grearaegraeggraerega
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace API.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class AuthController : Controller
//    {
//        [HttpPost("token")]
//        public IActionResult GenerateToken([FromBody] UserLogin userLogin)
//        {
//            if (userLogin.Username == "testuser" && userLogin.Password == "testpassword")
//            {
//                var tokenHandler = new JwtSecurityTokenHandler();
//                var key = Encoding.ASCII.GetBytes("secret_key");
//                var tokenDescriptor = new SecurityTokenDescriptor
//                {
//                    Subject = new ClaimsIdentity(new[]
//                    {
//                        new Claim(ClaimTypes.Name, userLogin.Username)
//                    }),
//                    Expires = DateTime.UtcNow.AddHours(1),
//                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//                };
//                var token = tokenHandler.CreateToken(tokenDescriptor);
//                var tokenString = tokenHandler.WriteToken(token);

//                return Ok(new { Token = tokenString });
//            }

//            return Unauthorized();
//        }
//    }

//    public class UserLogin
//    {
//        public string Username { get; set; } = string.Empty;
//        public string Password { get; set; } = string.Empty;
//    }
//}
#endregion
#region
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using API.Models; // Burada JwtSettings sınıfının bulunduğu namespace'i ekleyin

//namespace API.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class AuthController : Controller
//    {
//        private readonly JwtSettings _jwtSettings;

//        public AuthController(IOptions<JwtSettings> jwtSettings)
//        {
//            _jwtSettings = jwtSettings.Value;
//        }

//        [HttpPost("token")]
//        public IActionResult GenerateToken([FromBody] UserLogin userLogin)
//        {
//            if (userLogin.Username == User. && userLogin.Password == "testpassword")
//            {
//                var tokenHandler = new JwtSecurityTokenHandler();
//                var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
//                var tokenDescriptor = new SecurityTokenDescriptor
//                {
//                    Subject = new ClaimsIdentity(new[]
//                    {
//                        new Claim(ClaimTypes.Name, userLogin.Username)
//                    }),
//                    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
//                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//                };
//                var token = tokenHandler.CreateToken(tokenDescriptor);
//                var tokenString = tokenHandler.WriteToken(token);

//                return Ok(new { Token = tokenString });
//            }

//            return Unauthorized();
//        }
//    }

//    public class UserLogin
//    {
//        public string Username { get; set; } = string.Empty;
//        public string Password { get; set; } = string.Empty;
//    }
//}
#endregion

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Data; // DbContext sınıfını buradan ekleyin
using API.Models; // User modelini buradan ekleyin

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ApiDbContext _context; // DbContext sınıfı

        public AuthController(IOptions<JwtSettings> jwtSettings, ApiDbContext context)
        {
            _jwtSettings = jwtSettings.Value;
            _context = context;
        }

        [HttpPost("token")]
        public IActionResult GenerateToken([FromBody] UserLogin userLogin)
        {
            // Kullanıcı adı ve şifre ile veritabanında kullanıcıyı doğrula
            var user = _context.Users.SingleOrDefault(u => u.Username == userLogin.Username);

            if (user == null || !VerifyPassword(userLogin.Password, user.Password))
            {
                return Unauthorized();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            // Şifre doğrulama işlemi için uygun bir yöntem kullanın
            // Örneğin, BCrypt, PBKDF2, vs.
            // Bu örnekte sadece basit bir eşleşme kullanılmıştır.
            return password == hashedPassword; // Düz metin kontrolü; gerçek projelerde hash kontrolü yapmalısınız
        }
    }

    public class UserLogin
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

