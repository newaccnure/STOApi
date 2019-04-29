using Microsoft.AspNetCore.Mvc;
using STOApi.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using STOApi.Entities;

namespace STOApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    [Authorize(Roles = "admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AdminController : Controller
    {
        public IAdminRepository adminRepository { set; get; }

        public AdminController(IAdminRepository adminRepository)
        {
            this.adminRepository = adminRepository;
        }

        [HttpPost]
        public JsonResult AddSport(string name)
        {
            Sport sport  = adminRepository.AddSport(name);
            JsonResult jr = Json(sport);
            if (sport.Id == 0)
            {
                jr.StatusCode = 400;
            }
            else
            {
                jr.StatusCode = 200;
            }
            return jr;
        }
        [HttpPost]
        public JsonResult AddEventFormat(string name)
        {
            EventFormat eventFormat  = adminRepository.AddEventFormat(name);
            JsonResult jr = Json(eventFormat);
            if (eventFormat.Id == 0)
            {
                jr.StatusCode = 400;
            }
            else
            {
                jr.StatusCode = 200;
            }
            return jr;
        }
        [HttpDelete]
        public JsonResult DeleteSport(int sportId)
        {
            bool deleted  = adminRepository.DeleteSport(sportId);
            JsonResult jr = Json(deleted);
            if (!deleted)
            {
                jr.StatusCode = 400;
            }
            else
            {
                jr.StatusCode = 200;
            }
            return jr;
        }
        [HttpDelete]
        public JsonResult DeleteEventFormat(int eventFormatId)
        {
            bool deleted  = adminRepository.DeleteEventFormat(eventFormatId);
            JsonResult jr = Json(deleted);
            if (!deleted)
            {
                jr.StatusCode = 400;
            }
            else
            {
                jr.StatusCode = 200;
            }
            return jr;
        }
        [HttpDelete]
        public JsonResult DeleteTournament(int tournamentId)
        {
            bool deleted  = adminRepository.DeleteTournament(tournamentId);
            JsonResult jr = Json(deleted);
            if (!deleted)
            {
                jr.StatusCode = 400;
            }
            else
            {
                jr.StatusCode = 200;
            }
            return jr;
        }
        [HttpGet]
        public JsonResult GetUsers()
        {
            return Json(adminRepository.GetUsers());
        }
        [HttpGet]
        public JsonResult GetTournaments()
        {
            return Json(adminRepository.GetTournaments());
        }
        [HttpGet]
        public JsonResult GetSports()
        {
            return Json(adminRepository.GetSports());
        }
        [HttpGet]
        public JsonResult GetEventFormats()
        {
            return Json(adminRepository.GetEventFormats());
        }

    }
}