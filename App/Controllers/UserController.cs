using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using App.Models;

namespace App.Controllers
{
    public class UsersController : Controller
    {
        private readonly HttpClient _httpClient;

        public UsersController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7103/api/"); 
        }
        public async Task<IActionResult> UserView()
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
                    return View("Error"); // veya uygun bir hata görünümü
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

        /* public async Task<IActionResult> UserView()
         {
             var token = HttpContext.Session.GetString("JWToken");

             if (string.IsNullOrEmpty(token))
             {
                 return RedirectToAction("Login", "Account");
             }

             _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

             var response = await _httpClient.GetAsync("user");

             if (response.IsSuccessStatusCode)
             {
                 var jsonResponse = await response.Content.ReadAsStringAsync();
                 var users = JsonConvert.DeserializeObject<List<User>>(jsonResponse);
                 return View(users);
             }
             else
             {
                 return Unauthorized();
             }
         }*/
    }
}
