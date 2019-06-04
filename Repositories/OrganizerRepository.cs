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

        public Tournament AddTournament(string email, string name, int numberOfParticipants,
                                        int sportId, int eventFormatId, int gameTime, int breakTime,
                                        TimeSpan gameDayStart, TimeSpan gameDayEnd, DateTime startDate, DateTime endDate)
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

            if (endDate < startDate)
                throw new InvalidDaysException();
            if (gameDayEnd.TotalMinutes < gameDayStart.TotalMinutes)
                throw new InvalidGameDayTimeException();

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
                                    .First();

            var organizer = GetOrganizer(email);

            tournament.Schedule = new Schedule()
            {
                TournamentId = tournament.Id,
                TournamentSchedule = new DateRange()
                {
                    Start = startDate,
                    End = endDate
                },
                GameDayStart = gameDayStart,
                GameDayEnd = gameDayEnd,
                BreakTime = breakTime,
                GameTime = gameTime,
            };

            context.UserTournament.Add(new UserTournament()
            {
                UserId = organizer.Id,
                TournamentId = tournament.Id
            });
            context.SaveChanges();

            return tournament;
        }

        public bool AutoGenerateTournamentSchedule(int tournamentId)
        {
            if (!context.Tournaments.Where(t => t.Id == tournamentId).Any())
                throw new NoTournamentException();

            Tournament tournament = context
                                        .Tournaments
                                        .Include(t => t.EventFormat)
                                        .Include(t => t.Schedule)
                                        .Where(t => t.Id == tournamentId)
                                        .FirstOrDefault();

            int numberOfGames = GetNumberOfGames(tournamentId);
            var endDate = tournament.Schedule.TournamentSchedule.End;
            var startDate = tournament.Schedule.TournamentSchedule.Start;
            var gameTime = tournament.Schedule.GameTime;
            var breakTime = tournament.Schedule.BreakTime;
            var gameDayEnd = tournament.Schedule.GameDayEnd;
            var gameDayStart = tournament.Schedule.GameDayStart;

            int numberOfGameDays = ((TimeSpan)(endDate - startDate)).Days + 1;
            int numberOfGamesPerDay = (numberOfGames - 1) / numberOfGameDays + 1;
            int estimatedGameDayTime = numberOfGamesPerDay * gameTime + (numberOfGamesPerDay - 1) * breakTime;
            int realGameDayTime = (int)(gameDayEnd.TotalMinutes - gameDayStart.TotalMinutes);

            if (estimatedGameDayTime > realGameDayTime)
                throw new NotEnoughTimeException();


            List<Game> games = ScheduleGames(tournament, numberOfGames,
                                                numberOfGameDays, numberOfGamesPerDay,
                                                estimatedGameDayTime, realGameDayTime,
                                                gameTime, breakTime,
                                                gameDayStart, gameDayEnd,
                                                startDate, endDate);
            tournament.Schedule.Games = games;
            context.SaveChanges();
            return true;
        }

        public Schedule GetTournamentSchedule(int tournamentId)
        {
            int scheduleId = context.Schedules.Where(t => t.TournamentId == tournamentId).FirstOrDefault().Id;
            return context.Schedules
                .Include(s => s.Games)
                .ThenInclude(g => g.FirstParticipant)
                .Include(s => s.Games)
                .ThenInclude(g => g.SecondParticipant)
                .Include(s => s.Games)
                .ThenInclude(g => g.Winner)
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
                case "Group stage":
                    int groupSize = numberOfParticipants / 4;
                    return (groupSize * (groupSize - 1) / 2) * 4;
                default:
                    return 0;
            }
        }
        public List<Game> ScheduleGames(
            Tournament tournament, int numberOfGames,
            int numberOfGameDays, int numberOfGamesPerDay,
            int estimatedGameDayTime, int realGameDayTime,
            int gameTime, int breakTime,
            TimeSpan gameDayStart, TimeSpan gameDayEnd,
            DateTime startDate, DateTime endDate)
        {
            List<Game> games = new List<Game>();
            int numberOfParticipants = tournament.NumberOfParticipants;
            List<User> users = GetStaticUsers(numberOfParticipants);
            var tbdParticipant = context.Users.Where(u => u.Email == "TBD" && u.Password == "TBD" && u.Role == "participant").First();
            Random r = new Random();
            int currentGame = 0;
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

                    games = games.OrderBy(x => r.Next()).ToList();
                    for (int i = 0; i < numberOfGameDays; ++i)
                    {
                        DateTime dt = startDate.AddDays(i).AddMinutes(gameDayStart.TotalMinutes);
                        for (int j = 0; j < numberOfGamesPerDay; ++j)
                        {
                            if (currentGame < games.Count)
                            {
                                games[currentGame].GameSchedule = new DateRange();
                                games[currentGame].Winner = tbdParticipant;
                                games[currentGame].Score = new Score();
                                games[currentGame].GameSchedule.Start = dt.AddMinutes(j * gameTime).AddMinutes(j * breakTime);
                                games[currentGame].GameSchedule.End = dt.AddMinutes((j + 1) * gameTime).AddMinutes(j * breakTime);
                                currentGame++;
                            }
                        }
                    }

                    return games;
                case "Single elimination":
                    if (!isPowerOfTwo(numberOfParticipants))
                    {
                        throw new InvalidNumberOfParticipantsException();
                    }

                    for (int i = 0; i < numberOfParticipants; ++i)
                    {
                        context.UserTournament.Add(new UserTournament()
                        {
                            Tournament = tournament,
                            User = users[i],
                            Joined = false
                        });
                    }

                    var numberOfRounds = Math.Round(Math.Log(numberOfParticipants) / Math.Log(2));

                    // First round
                    for (int i = 0; i < numberOfParticipants / 2; ++i)
                    {
                        games.Add(new Game()
                        {
                            FirstParticipant = users[i],
                            SecondParticipant = users[numberOfParticipants - i - 1]
                        });
                    }

                    // All next rounds
                    for (int i = 2; i <= numberOfRounds; ++i)
                    {
                        int numberOfGamesPerRound = Convert.ToInt32(numberOfParticipants / (Math.Pow(2, i)));
                        for (int j = 0; j < numberOfGamesPerRound; ++j)
                        {
                            games.Add(new Game()
                            {
                                FirstParticipant = tbdParticipant,
                                SecondParticipant = tbdParticipant
                            });
                        }
                    }

                    for (int i = 0; i < numberOfGameDays; ++i)
                    {
                        DateTime dt = startDate.AddDays(i).AddMinutes(gameDayStart.TotalMinutes);
                        for (int j = 0; j < numberOfGamesPerDay; ++j)
                        {
                            if (currentGame < games.Count)
                            {
                                games[currentGame].GameSchedule = new DateRange();
                                games[currentGame].Winner = tbdParticipant;
                                games[currentGame].Score = new Score();
                                games[currentGame].GameSchedule.Start = dt.AddMinutes(j * gameTime).AddMinutes(j * breakTime);
                                games[currentGame].GameSchedule.End = dt.AddMinutes((j + 1) * gameTime).AddMinutes(j * breakTime);
                                currentGame++;
                            }
                        }
                    }
                    return games;
                case "Group stage":
                    var numberOfGroups = 4;
                    if (numberOfParticipants % numberOfGroups != 0)
                    {
                        throw new InvalidNumberOfParticipantsException();
                    }

                    var participantsPerGroup = numberOfParticipants / numberOfGroups;

                    var groupGames = new List<List<Game>>();

                    for (int i = 0; i < numberOfGroups; ++i)
                    {
                        groupGames.Add(new List<Game>());
                    }

                    var numberOfGamesPerGroup = participantsPerGroup * (participantsPerGroup - 1) / 2;


                    for (int j = 0; j < numberOfParticipants; ++j)
                    {
                        context.UserTournament.Add(new UserTournament()
                        {
                            Tournament = tournament,
                            User = users[j],
                            Joined = false
                        });
                    }

                    for (int i = 0; i < numberOfGroups; ++i)
                    {
                        for (int j = i * participantsPerGroup; j < (i + 1) * participantsPerGroup; ++j)
                        {
                            for (int k = j + 1; k < (i + 1) * participantsPerGroup; ++k)
                            {
                                groupGames[i].Add(new Game()
                                {
                                    FirstParticipant = users[j],
                                    SecondParticipant = users[k],
                                    GroupNumber = i + 1
                                });
                            }
                        }
                    }

                    for (int i = 0; i < numberOfGameDays; ++i)
                    {
                        DateTime dt = startDate.AddDays(i).AddMinutes(gameDayStart.TotalMinutes);
                        for (int j = 0; j < numberOfGamesPerDay; ++j)
                        {
                            if (currentGame < numberOfGames)
                            {
                                var game = groupGames[currentGame % 4][(currentGame - currentGame % 4) / 4];
                                game.GameSchedule = new DateRange();
                                game.Winner = tbdParticipant;
                                game.Score = new Score();
                                game.GameSchedule.Start = dt.AddMinutes(j * gameTime).AddMinutes(j * breakTime);
                                game.GameSchedule.End = dt.AddMinutes((j + 1) * gameTime).AddMinutes(j * breakTime);
                                currentGame++;
                            }
                        }
                    }

                    for (int i = 0; i < numberOfGamesPerGroup; ++i)
                    {
                        for (int j = 0; j < numberOfGroups; ++j)
                        {
                            games.Add(groupGames[j][i]);
                        }
                    }

                    return games;
                default:
                    throw new InvalidOperationException();
            }
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

        public List<EventFormat> GetEventFormats()
        {
            return context.EventFormats.ToList();
        }

        public List<Sport> GetSports()
        {
            return context.Sports.ToList();
        }

        public List<Tournament> GetOngoingTournaments(string email)
        {
            var organizer = GetOrganizer(email);
            var organizedTournamentsIds = context.UserTournament.Where(ut => ut.UserId == organizer.Id).Select(ut => ut.TournamentId).ToList();
            List<Tournament> organizedTournamentsList = new List<Tournament>();
            var now = DateTime.Now;
            foreach (var tournamentId in organizedTournamentsIds)
            {
                var tournament = context.Tournaments
                                    .Include(t => t.Schedule)
                                    .Include(t => t.EventFormat)
                                    .Include(t => t.Sport)
                                    .Where(t => t.Id == tournamentId)
                                    .First();
                if (tournament.Schedule.TournamentSchedule.End > now
                    && tournament.Schedule.TournamentSchedule.Start < now)
                {
                    organizedTournamentsList.Add(tournament);
                }
            }
            return organizedTournamentsList;
        }

        public List<Tournament> GetFinishedTournaments(string email)
        {
            var organizer = GetOrganizer(email);
            var organizedTournamentsIds = context.UserTournament.Where(ut => ut.UserId == organizer.Id).Select(ut => ut.TournamentId).ToList();
            List<Tournament> organizedTournamentsList = new List<Tournament>();
            var now = DateTime.Now;
            foreach (var tournamentId in organizedTournamentsIds)
            {
                var tournament = context.Tournaments
                                    .Include(t => t.Schedule)
                                    .Include(t => t.EventFormat)
                                    .Include(t => t.Sport)
                                    .Where(t => t.Id == tournamentId)
                                    .First();
                if (tournament.Schedule.TournamentSchedule.End < now)
                {
                    organizedTournamentsList.Add(tournament);
                }
            }
            return organizedTournamentsList;
        }

        public List<Tournament> GetIncomingTournaments(string email)
        {
            var organizer = GetOrganizer(email);
            var organizedTournamentsIds = context.UserTournament.Where(ut => ut.UserId == organizer.Id).Select(ut => ut.TournamentId).ToList();
            List<Tournament> organizedTournamentsList = new List<Tournament>();
            var now = DateTime.Now;
            foreach (var tournamentId in organizedTournamentsIds)
            {
                var tournament = context.Tournaments
                                    .Include(t => t.Schedule)
                                    .Include(t => t.EventFormat)
                                    .Include(t => t.Sport)
                                    .Where(t => t.Id == tournamentId)
                                    .First();
                if (tournament.Schedule.TournamentSchedule.Start > now)
                {
                    organizedTournamentsList.Add(tournament);
                }
            }
            return organizedTournamentsList;
        }

        private User GetOrganizer(string email)
        {
            return context.Users.Where(u => u.Email == email && u.Role == "organizer").FirstOrDefault();
        }
        public bool AddScore(int tournamentId, int gameId, int winnerId, int firstParticipantScore, int secondParticipantScore)
        {
            if (!context.Games.Where(g => g.Id == gameId).Any())
                return false;
            var winner = context.Users.Where(u => u.Id == winnerId).First();
            var game = context.Games.Include(g => g.Winner).Where(g => g.Id == gameId).First();
            game.Score.FirstParticipantScore = firstParticipantScore;
            game.Score.SecondParticipantScore = secondParticipantScore;
            game.Winner = winner;
            context.SaveChanges();

            var tournament = context.Tournaments.Where(t => t.Id == tournamentId).First();

            var singleElimination = context.EventFormats.Where(ef => ef.Name == "Single elimination").First();

            if (tournament.EventFormatId == singleElimination.Id)
            {
                var schedule = context
                                    .Schedules
                                    .Where(s => s.TournamentId == tournamentId)
                                    .First();

                var tournamentGames = context
                                            .Games
                                            .Where(g => g.ScheduleId == schedule.Id)
                                            .Include(g => g.GameSchedule)
                                            .OrderBy(g => g.GameSchedule.Start)
                                            .ToList();

                var gameIndex = tournamentGames.FindIndex(g => g.Id == gameId);

                var numberOfGames = tournamentGames.Count;
                var numberOfRounds = (int)Math.Round(Math.Log(numberOfGames + 1, 2));

                var currentIndex = 0;

                var nextRoundGameInRoundIndex = -1;
                var flag = false;
                var nextRoundGameIndex = 0;
                for (int i = numberOfRounds - 1; i >= 0; i--)
                {
                    for (int j = 0; j < Math.Pow(2, i); j++)
                    {
                        if (nextRoundGameInRoundIndex == j)
                        {
                            nextRoundGameIndex = currentIndex;
                            flag = true;
                            break;
                        }
                        if (currentIndex == gameIndex)
                        {
                            nextRoundGameInRoundIndex = j / 2;
                        }

                        currentIndex++;
                    }
                    if (flag) break;
                }

                var nextRoundGame = context.Games.Where(g => g.Id == tournamentGames[nextRoundGameIndex].Id).First();
                if (gameIndex % 2 == 0)
                {
                    nextRoundGame.FirstParticipant = winner;
                }
                else
                {
                    nextRoundGame.SecondParticipant = winner;
                }

                context.SaveChanges();

            }

            return true;
        }

        public bool DeleteTournament(int tournamentId)
        {
            if (!context.Tournaments.Where(t => t.Id == tournamentId).Any()) return false;
            Tournament tournament = context.Tournaments.Where(t => t.Id == tournamentId).First();
            context.Remove(tournament);
            context.SaveChanges();
            return true;
        }

        static bool isPowerOfTwo(int n)
        {
            if (n == 0)
                return false;

            return (int)(Math.Ceiling((Math.Log(n) /
                                       Math.Log(2)))) ==
                   (int)(Math.Floor(((Math.Log(n) /
                                      Math.Log(2)))));
        }
    }
}