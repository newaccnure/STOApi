using Microsoft.AspNetCore.Mvc;
using STOApi.Interfaces;
using Microsoft.AspNetCore.Cors;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using STOApi.Entities;
using System;
using STOApi.Errors;

namespace STOApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    [Authorize(Roles = "organizer", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrganizerController : Controller
    {
        public IOrganizerRepository organizerRepository { set; get; }

        public OrganizerController(IOrganizerRepository organizerRepository)
        {
            this.organizerRepository = organizerRepository;
        }

        [HttpPost]
        public JsonResult AddTournament([FromBody]Tournament tournament)
        {
            Tournament savedTournament = new Tournament();
            JsonResult jr = Json("");
            try
            {
                savedTournament =
                organizerRepository.AddTournament(User.Identity.Name, tournament.Name,
                                                    tournament.NumberOfParticipants, tournament.Sport.Id, tournament.EventFormat.Id, tournament.Schedule.GameTime, tournament.Schedule.BreakTime,
                                                    tournament.Schedule.GameDayStart, tournament.Schedule.GameDayEnd, tournament.Schedule.TournamentSchedule.Start, tournament.Schedule.TournamentSchedule.End);
            }
            catch (InvalidDaysException ex)
            {
                jr.StatusCode = 400;
            }
            catch (InvalidGameDayTimeException ex)
            {
                jr.StatusCode = 400;
            }
            
            jr = Json(savedTournament);

            if (savedTournament.Id == 0)
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
        public JsonResult AddRepresentativesToTournament(int tournamentId, List<string> representativesEmails)
        {
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
        public JsonResult AutoGenerateTournamentSchedule(int tournamentId)
        {
            JsonResult jr = Json(null);
            try
            {
                jr = Json(organizerRepository.AutoGenerateTournamentSchedule(tournamentId));
            }
            catch (InvalidNumberOfParticipantsException)
            {
                jr.StatusCode = 400;
            }

            jr.StatusCode = 200;
            return jr;
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
        public JsonResult GetTournamentSchedule(int tournamentId)
        {
            return Json(organizerRepository.GetTournamentSchedule(tournamentId));
        }

        [HttpGet]
        public JsonResult GetSports()
        {
            return Json(organizerRepository.GetSports());
        }

        [HttpGet]
        public JsonResult GetEventFormats()
        {
            return Json(organizerRepository.GetEventFormats());
        }

        [HttpGet]
        public JsonResult GetOngoingTournaments()
        {
            return Json(organizerRepository.GetOngoingTournaments(User.Identity.Name));
        }

        [HttpGet]
        public JsonResult GetFinishedTournaments()
        {
            return Json(organizerRepository.GetFinishedTournaments(User.Identity.Name));
        }

        [HttpGet]
        public JsonResult GetIncomingTournaments()
        {
            return Json(organizerRepository.GetIncomingTournaments(User.Identity.Name));
        }

        [HttpGet]
        public JsonResult GetEmail()
        {
            return Json(User.Identity.Name);
        }

        [HttpGet]
        public JsonResult GetIncomingGames(){
            return Json(organizerRepository.GetIncomingGames(User.Identity.Name));
        }

        [HttpGet]
        public JsonResult GetFinishedGamesWithNoScore(){
            return Json(organizerRepository.GetFinishedGamesWithNoScore(User.Identity.Name));
        }
        
        [HttpPost]
        public JsonResult AddScore(int tournamentId, int gameId, int winnerId, int firstParticipantScore, int secondParticipantScore)
        {
            return Json(organizerRepository.AddScore(tournamentId, gameId, winnerId, firstParticipantScore, secondParticipantScore));
        }

        [HttpDelete]
        public JsonResult DeleteTournament(int tournamentId)
        {
            bool deleted = organizerRepository.DeleteTournament(tournamentId);
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
    }
}