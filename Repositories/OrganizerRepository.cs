using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using System.Security.Claims;
using STOApi.Models;
using STOApi.Interfaces;
using STOApi.Entities;
using Microsoft.EntityFrameworkCore;
using STOApi.Errors;

namespace STOApi.Repositories
{
    public class OrganizerRepository : IOrganizerRepository
    {
        private STOContext context;

        public OrganizerRepository(STOContext context)
        {
            this.context = context;
        }

        public bool AddRepresentativesToTournament(int tournamentId, List<string> representativesEmails)
        {
            if (!context.Tournaments.Where(t => t.Id == tournamentId).Any())
            {
                return false;
            }

            foreach (var email in representativesEmails)
            {
                if (context.Users.Where(u => u.Email == email && u.Role == "representative").Any())
                {
                    User representative = context
                                            .Users
                                            .Where(u => u.Email == email
                                                        && u.Role == "representative")
                                            .First();
                    var tournament = context.Tournaments.Where(t => t.Id == tournamentId).First();
                    tournament.UserTournaments.Add(new UserTournament()
                    {
                        User = representative,
                        Tournament = tournament,
                        Joined = false
                    });
                }
                else
                {
                    return false;
                }
            }
            context.SaveChanges();
            return true;
        }

        public Tournament AddTournament(string name, int numberOfParticipants, int sportId, int eventFormatId)
        {
            if (!context.Sports.Where(s => s.Id == sportId).Any()
                || !context.EventFormats.Where(ef => ef.Id == eventFormatId).Any()
                || name.Equals(String.Empty))
                return new Tournament();
            if (context
                    .Tournaments
                    .Where(t => t.Name == name
                                && t.SportId == sportId
                                && t.EventFormatId == eventFormatId).Any())
                return new Tournament();

            context.Tournaments.Add(new Tournament()
            {
                Name = name,
                NumberOfParticipants = numberOfParticipants,
                SportId = sportId,
                EventFormatId = eventFormatId
            });
            context.SaveChanges();

            Tournament tournament = context
                                    .Tournaments
                                    .Where(t => t.Name == name
                                                && t.SportId == sportId
                                                && t.EventFormatId == eventFormatId)
                                    .FirstOrDefault();
            return tournament;
        }

        public bool AutoGenerateTournamentSchedule(int tournamentId, int gameTime, int breakTime,
            int gameDayStart, int gameDayEnd, DateTime startDate, DateTime endDate)
        {
            if (!context.Tournaments.Where(t => t.Id == tournamentId).Any())
                throw new NoTournamentException();

            if (endDate < startDate)
                throw new InvalidDaysException();
            if (gameDayEnd < gameDayStart)
                throw new InvalidGameDayTimeException();

            Tournament tournament = context
                                        .Tournaments
                                        .Include(t => t.EventFormat)
                                        .Where(t => t.Id == tournamentId)
                                        .FirstOrDefault();

            int numberOfGames = GetNumberOfGames(tournamentId);

            int numberOfGameDays = ((TimeSpan)(endDate - startDate)).Days + 1;
            int numberOfGamesPerDay = (numberOfGames - 1) / numberOfGameDays + 1;
            int estimatedGameDayTime = numberOfGamesPerDay * gameTime + (numberOfGamesPerDay - 1) * breakTime;
            int realGameDayTime = gameDayEnd - gameDayStart;

            if (estimatedGameDayTime > realGameDayTime)
                throw new NotEnoughTimeException();


            List<Game> games = ScheduleGames(tournament, numberOfGames,
                                                numberOfGameDays, numberOfGamesPerDay,
                                                estimatedGameDayTime, realGameDayTime,
                                                gameTime, breakTime,
                                                gameDayStart, gameDayEnd,
                                                startDate, endDate);
            tournament.Schedule = new Schedule()
            {
                TournamentId = tournamentId,
                TournamentSchedule = new DateRange()
                {
                    Start = startDate,
                    End = endDate
                },
                GameDayStart = gameDayStart,
                GameDayEnd = gameDayEnd,
                Games = games,
                BreakTime = breakTime,
                GameTime = gameTime,
            };
            context.SaveChanges();
            return true;
        }

        public Schedule GetTournamentSchedule(int tournamentId)
        {
            int scheduleId = context.Schedules.Where(t => t.Id == tournamentId).FirstOrDefault().Id;
            return context.Schedules
                .Include(s => s.Games)
                .ThenInclude(g => g.FirstParticipant)
                .Include(s => s.Games)
                .ThenInclude(g => g.SecondParticipant)
                .Where(s => s.Id == scheduleId).FirstOrDefault();
        }
        public List<User> GetTournamentRepresentatives(int tournamentId)
        {
            if (!context.Tournaments.Where(t => t.Id == tournamentId).Any()) return new List<User>();
            return context
                    .UserTournament
                    .Include(ut => ut.User)
                    .Where(ut => ut.TournamentId == tournamentId
                                    && ut.User.Role == "representative")
                    .Select(ut => ut.User)
                    .ToList();
        }

        public List<User> GetTournamentParticipants(int tournamentId)
        {
            if (!context.Tournaments.Where(t => t.Id == tournamentId).Any()) return new List<User>();
            return context
                    .UserTournament
                    .Include(ut => ut.User)
                    .Where(ut => ut.TournamentId == tournamentId
                                    && ut.User.Role == "participant")
                    .Select(ut => ut.User)
                    .ToList();
        }
        private int GetNumberOfGames(int tournamentId)
        {
            Tournament tournament = context
                                        .Tournaments
                                        .Include(t => t.EventFormat)
                                        .Where(t => t.Id == tournamentId)
                                        .FirstOrDefault();

            EventFormat eventFormat = tournament.EventFormat;
            int numberOfParticipants = tournament.NumberOfParticipants;

            switch (eventFormat.Name)
            {
                case "Round-robin":
                    return numberOfParticipants * (numberOfParticipants - 1) / 2;
                case "Single elimination":
                    return numberOfParticipants - 1;
                case "Double elimination":
                    return 2 * (numberOfParticipants - 1);
                case "Group stage":
                    int groupSize = numberOfParticipants / 4;
                    return (groupSize * (groupSize - 1) / 2) * 4;
                default:
                    return 0;
            }
        }
        private List<Game> ScheduleGames(
            Tournament tournament, int numberOfGames,
            int numberOfGameDays, int numberOfGamesPerDay,
            int estimatedGameDayTime, int realGameDayTime,
            int gameTime, int breakTime,
            int gameDayStart, int gameDayEnd,
            DateTime startDate, DateTime endDate)
        {
            List<Game> games = new List<Game>();
            int numberOfParticipants = tournament.NumberOfParticipants;
            List<User> users = GetStaticUsers(numberOfParticipants);

            switch (tournament.EventFormat.Name)
            {
                case "Round-robin":
                    for (int i = 0; i < numberOfParticipants; ++i)
                    {
                        context.UserTournament.Add(new UserTournament()
                        {
                            Tournament = tournament,
                            User = users[i],
                            Joined = false
                        });
                        for (int j = i + 1; j < numberOfParticipants; ++j)
                        {
                            games.Add(new Game()
                            {
                                FirstParticipant = users[i],
                                SecondParticipant = users[j]
                            });
                        }
                    }
                    Random r = new Random();
                    games = games.OrderBy(x => r.Next()).ToList();
                    int currentGame = 0;
                    for (int i = 0; i < numberOfGameDays; ++i)
                    {
                        DateTime dt = startDate.AddDays(i).AddMinutes(gameDayStart);
                        for (int j = 0; j < numberOfGamesPerDay; ++j)
                        {
                            if (currentGame < games.Count)
                            {
                                games[currentGame].GameSchedule = new DateRange();
                                games[currentGame].Winner = new User();
                                games[currentGame].Score = new Score();
                                games[currentGame].GameSchedule.Start = dt.AddMinutes(j * gameTime).AddMinutes(j * breakTime);
                                games[currentGame].GameSchedule.End = dt.AddMinutes((j + 1) * gameTime).AddMinutes(j * breakTime);
                                currentGame++;
                            }
                        }
                    }
                    return games;
                case "Single elimination":
                    break;
                case "Double elimination":
                    break;
                case "Group stage":
                    break;
                default:
                    break;
            }
            return new List<Game>();
        }
        private List<User> GetStaticUsers(int numberOfParticipants)
        {
            List<User> staticUsers = new List<User>();
            List<User> allStaticUsers = context.Users.Where(u => u.Email.Contains("Participant") && u.Password.Contains("Participant")).ToList();

            for (int i = 0; i < numberOfParticipants; ++i)
            {
                staticUsers.Add(allStaticUsers
                                    .Where(u => u.Email == $"Participant{i}" && u.Password == $"Participant{i}")
                                    .First());
            }
            return staticUsers;
        }
    }
}