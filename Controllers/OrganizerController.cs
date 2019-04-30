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
        public JsonResult AddTournament(string name, int numberOfParticipants, int sportId, int eventFormatId)
        {
            Tournament tournament = organizerRepository.AddTournament(name, numberOfParticipants, sportId, eventFormatId);
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
        public JsonResult AutoGenerateTournamentSchedule(int tournamentId, int gameTime, int breakTime, 
            int gameDayStart, int gameDayEnd, DateTime startDate, DateTime endDate)
        {
            return Json(organizerRepository.AutoGenerateTournamentSchedule(tournamentId, gameTime, breakTime, 
                gameDayStart, gameDayEnd, startDate, endDate));
        }
        [HttpGet]
        public JsonResult GetTournamentRepresentatives(int tournamentId)
        {
            
            return Json(organizerRepository.GetTournamentRepresentatives(tournamentId));
        }
        [HttpGet]
        public JsonResult GetTournamentParticipants(int tournamentId)
        {
            
            return Json(organizerRepository.GetTournamentParticipants(tournamentId));
        }
        [HttpGet]
        public JsonResult GetTournamentSchedule(int tournamentId){
            return Json(organizerRepository.GetTournamentSchedule(tournamentId));
        }
    }
}