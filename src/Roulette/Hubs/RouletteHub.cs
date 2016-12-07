using System;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Roulette.Data;
using Roulette.Data.TransferObjects;
using Roulette.Helpers;
using Roulette.Models;

namespace Roulette.Hubs
{
    public class RouletteHub : Hub<IRouletteClient>
    {
        private readonly ApplicationDbContext _dbContext;

        public RouletteHub(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Connect(int id)
        {
            var @event = _dbContext.Events.FirstOrDefault(x => x.Id == id);
            if (@event == null)
                return;

            var groupName = @event.Id.ToString();
            Groups.Add(Context.ConnectionId, groupName);
            Clients.Group(groupName).OnConnect();
        }

        public void Join(int id)
        {
            var user = _dbContext.Users
                .FirstOrDefault(x => x.UserName == Context.User.Identity.Name);
            if (user == null)
                return;

            var @event = _dbContext.Events
                .FirstOrDefault(x => x.Id == id);
            if (@event == null)
                return;

            var exist = _dbContext.UserEvents
                .Any(x => x.Event.Id == @event.Id && x.User.UserName == Context.User.Identity.Name);
            if (exist)
                return;

            var userEvent = new UserEvent
            {
                User = user,
                Event = @event
            };

            var groupName = userEvent.Event.Id.ToString();
            Groups.Add(Context.ConnectionId, groupName);
            Clients.Group(groupName).OnJoin(
                new UserDto
                {
                    Id = userEvent.User.Id,
                    Email = userEvent.User.Email,
                    ImageUrl = GravatarImageTagHelper.ToGravatarUrl(userEvent.User.Email)
                });

            _dbContext.UserEvents.Add(userEvent);
            _dbContext.SaveChanges();
        }

        public void Leave(int id)
        {
            var userEvent = _dbContext.UserEvents
                .Include(x => x.Event)
                .FirstOrDefault(x => x.Id == id);
            if (userEvent == null)
                return;

            var groupName = userEvent.Event.Id.ToString();
            Groups.Remove(Context.ConnectionId, groupName);
            Clients.OthersInGroup(groupName).OnLeave(
                new UserDto
                {
                    Id = userEvent.User.Id,
                    Email = userEvent.User.Email,
                    ImageUrl = GravatarImageTagHelper.ToGravatarUrl(userEvent.User.Email)
                });

            _dbContext.UserEvents.Remove(userEvent);
            _dbContext.SaveChanges();
        }

        public void DrawLots(int id)
        {
            var @event = _dbContext.Events
                .Include(x => x.UserEvents)
                .ThenInclude(x => x.User)
                .Include(x => x.Winner)
                .FirstOrDefault(x => x.Id == id);
            if (@event == null)
                return;
            if (!@event.UserEvents.Any())
                return;

            var groupName = @event.Id.ToString();
            if (@event.Winner != null)
            {
                Clients.Group(groupName).OnDrawLots(
                    new UserDto
                    {
                        Id = @event.Winner.Id,
                        Email = @event.Winner.Email,
                        ImageUrl = GravatarImageTagHelper.ToGravatarUrl(@event.Winner.Email)
                    });
                return;
            }

            var random = new Random();
            var result = random.Next(@event.UserEvents.Count);
            var hit = @event.UserEvents.ToArray()[result];
            Clients.Group(groupName).OnDrawLots(
                new UserDto
                {
                    Id = hit.User.Id,
                    Email = hit.User.Email,
                    ImageUrl = GravatarImageTagHelper.ToGravatarUrl(hit.User.Email)
                });

            @event.Winner = hit.User;
            _dbContext.SaveChanges();
        }
    }

    public interface IRouletteClient
    {
        void OnConnect();
        void OnJoin(UserDto user);
        void OnLeave(UserDto user);
        void OnDrawLots(UserDto user);
    }
}
