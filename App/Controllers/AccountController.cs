using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using App.Models;

namespace App.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7103/");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            if (ModelState.IsValid)
            {
                var jsonContent = JsonConvert.SerializeObject(userLogin);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                Class c = new Class();
                c.MyProperty=userLogin.Username;
                c.MyProperty2 = userLogin.Password;
                try
                {
                    var response = await _httpClient.PostAsync("api/Auth/token", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(jsonResponse);

                       HttpContext.Session.SetString("JWToken", tokenResponse.Token);

                        return RedirectToAction("IndexDashboard", "Dashboard");
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError(string.Empty, $"API Hatası: {errorMessage}");
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    ModelState.AddModelError(string.Empty, $"HTTP İsteği Hatası: {httpEx.Message}");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Beklenmeyen bir hata oluştu: {ex.Message}");
                }
            }

            return View(userLogin);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegister userRegister)
        {
            if (ModelState.IsValid)
            {
                var jsonContent = JsonConvert.SerializeObject(userRegister);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                try
                {
                    var response = await _httpClient.PostAsync("api/User/register", content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Kayıttan hemen sonra otomatik olarak giriş yap ve token al
                        var loginResponse = await _httpClient.PostAsync("api/Auth/token", content);
                        if (loginResponse.IsSuccessStatusCode)
                        {
                            var jsonResponse = await loginResponse.Content.ReadAsStringAsync();
                            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(jsonResponse);
                            HttpContext.Session.SetString("JWToken", tokenResponse.Token);

                            return RedirectToAction("PostRegistration", "Dashboard");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Kayıttan sonra otomatik giriş başarısız oldu.");
                        }
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError(string.Empty, $"API Hatası: {errorMessage}");
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    ModelState.AddModelError(string.Empty, $"HTTP İsteği Hatası: {httpEx.Message}");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Beklenmeyen bir hata oluştu: {ex.Message}");
                }
            }

            return View(userRegister);
        }



        IActionResult Logout()
            {
                HttpContext.Session.Remove("JWToken");
                return RedirectToAction("Login", "Account");
            }
        }
    }

 