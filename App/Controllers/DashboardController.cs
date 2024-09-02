using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using App.Models;

namespace App.Controllers
{
    public class DashboardController : Controller
    {
        private readonly HttpClient _httpClient;

        public DashboardController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7103/api/"); // API adresiniz
        }

        public IActionResult IndexDashboard()
        {
            return View();
        }

        public async Task<IActionResult> ListTokens()
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Token'ları listelemeniz gereken bir endpoint varsa buradan çağırabilirsiniz
            // Bu örnekte varsayımsal olarak bir endpoint kullanıyoruz
            var tokens = new List<string> { token }; // Örnek olarak sadece tek bir token listeleniyor

            return View(tokens);
        }
        public IActionResult PostRegistration()
        {
            return View(); // PostRegistration için bir View  
        }

        public async Task<IActionResult> ListUsers()
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.GetAsync("user");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var users = JsonConvert.DeserializeObject<List<User>>(jsonResponse);
                    return View(users);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"API Hatası: {errorMessage}");
                    return View("Error");
                }
            }
            catch (HttpRequestException httpEx)
            {
                ModelState.AddModelError(string.Empty, $"HTTP İsteği Hatası: {httpEx.Message}");
                return View("Error");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Beklenmeyen bir hata oluştu: {ex.Message}");
                return View("Error");
            }
        }
    }
}