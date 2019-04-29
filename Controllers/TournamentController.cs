using Microsoft.AspNetCore.Mvc;
using STOApi.Interfaces;
using Microsoft.AspNetCore.Cors;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace STOApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class TournamentController : Controller
    {
        public ITournamentRepository tournamentRepository { set; get; }

        public TournamentController(ITournamentRepository tournamentRepository)
        {
            this.tournamentRepository = tournamentRepository;
        }

        [HttpPost]
        [Authorize(Roles = "organizer", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public JsonResult AddTournament(string name, int sportId, int eventFormatId)
        {
            int tournamentId = tournamentRepository.AddTournament(name, sportId, eventFormatId);
            JsonResult jr = Json(tournamentId);
            if (tournamentId == 0)
            {
                jr.StatusCode = 401;
            }
            else
            {
                jr.StatusCode = 200;
            }
            return jr;
        }

    }
}