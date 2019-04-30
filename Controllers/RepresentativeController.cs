using Microsoft.AspNetCore.Mvc;
using STOApi.Interfaces;
using Microsoft.AspNetCore.Cors;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using STOApi.Entities;
using System;

namespace STOApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class RepresentativeController : Controller
    {
        public IRepresentativeRepository representativeRepository { set; get; }

        public RepresentativeController(IRepresentativeRepository representativeRepository)
        {
            this.representativeRepository = representativeRepository;
        }

        [HttpPost]
        [Authorize(Roles = "representative, organizer, admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public JsonResult AddParticipantsToTournament(int tournamentId, List<string> participantsEmails)
        {
            bool added = representativeRepository.AddParticipantsToTournament(tournamentId, participantsEmails);
            JsonResult jr = Json(added);
            if (!added)
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