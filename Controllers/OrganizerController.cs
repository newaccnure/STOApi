using Microsoft.AspNetCore.Mvc;
using STOApi.Interfaces;
using Microsoft.AspNetCore.Cors;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using STOApi.Entities;

namespace STOApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class OrganizerController : Controller
    {
        public IOrganizerRepository organizerRepository { set; get; }

        public OrganizerController(IOrganizerRepository organizerRepository)
        {
            this.organizerRepository = organizerRepository;
        }

        [HttpPost]
        [Authorize(Roles = "organizer, admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public JsonResult AddTournament(string name, int sportId, int eventFormatId)
        {
            Tournament tournament = organizerRepository.AddTournament(name, sportId, eventFormatId);
            JsonResult jr = Json(tournament);
            if (tournament.Id == 0)
            {
                jr.StatusCode = 400;
            }
            else
            {
                jr.StatusCode = 200;
            }
            return jr;
        }

    }
}