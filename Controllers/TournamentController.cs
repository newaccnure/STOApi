using Microsoft.AspNetCore.Mvc;
using STOApi.Interfaces;
using Microsoft.AspNetCore.Cors;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

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
        public JsonResult AddTournament(){
            bool created = tournamentRepository.AddTournament();
            JsonResult jr = Json(created);
            if (!created)
            {
                jr.StatusCode = 401;
                return jr;
            }
            jr.StatusCode = 200;
            return jr;
        }

    }
}