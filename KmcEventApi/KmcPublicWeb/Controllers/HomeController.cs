using KmcPublicWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Json;

namespace KmcPublicWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("KmcApi");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var events = await _httpClient.GetFromJsonAsync<List<EventViewModel>>("api/Events");
                var participants = await _httpClient.GetFromJsonAsync<List<ParticipantViewModel>>("api/Participants");
                ViewBag.Participants = participants ?? new List<ParticipantViewModel>();
                return View(events ?? new List<EventViewModel>());
            }
            catch (Exception)
            {
                ViewBag.Participants = new List<ParticipantViewModel>();
                return View(new List<EventViewModel>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(EventViewModel ev)
        {
            ev.CreatedBy = "Admin"; // Creator Logic එක සඳහා අනිවාර්යයි
            var response = await _httpClient.PostAsJsonAsync("api/Events", ev);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddParticipant(ParticipantViewModel pv)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Participants", pv);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEvent(EventViewModel ev)
        {
            // API එකේ logic එකට අනුව UserId එක query string එකක් ලෙස යැවිය යුතුය
            var response = await _httpClient.PutAsJsonAsync($"api/Events/{ev.Id}?userId=Admin", ev);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Events/{id}");
            return RedirectToAction("Index");
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        // 5. Remove Participant
        [HttpPost]
        public async Task<IActionResult> RemoveParticipant(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Participants/{id}");
            return RedirectToAction("Index");
        }
    }
}