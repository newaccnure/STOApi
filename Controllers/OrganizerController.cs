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
        [HttpPost]
        public JsonResult AddRepresentativesToTournament(int tournamentId, List<string> representativesEmails){
            bool added = organizerRepository.AddRepresentativesToTournament(tournamentId, representativesEmails);
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
        [HttpPost]
        public JsonResult AutoGenerateTournamentSchedule(int tournamentId, int gameTime, int breakTime, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        public JsonResult GetTournamentRepresentatives(int tournamentId)
        {
            
            return Json(organizerRepository.GetTournamentRepresentatives(tournamentId));
        }
    }
}