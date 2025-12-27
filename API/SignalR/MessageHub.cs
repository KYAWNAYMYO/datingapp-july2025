using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

[Authorize]
public class MessageHub (IUnitOfWork uow, IHubContext<PresenceHub> presenceHub) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext?.Request.Query["userId"].ToString()
            ?? throw new HubException("Other user not found.");
        var groupName = GetGroupName(GetUserId(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await AddToGroup(groupName);

        var messages = await uow.MessageRepository.GetMessageThread(GetUserId(), otherUser);

        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        var sender = await uow.MemberRepository.GetMemberByIdAsync(GetUserId());
        var recipient = await uow.MemberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);

        if (recipient == null || sender == null || sender.Id == createMessageDto.RecipientId)
        {
            throw new HubException("Connect send message.");
        }

        var message = new Message
        {
            SenderId = sender.Id,
            RecipientId = recipient.Id,
            Content = createMessageDto.Content
        };

        var groupName = GetGroupName(sender.Id, recipient.Id);        
        var group = uow.MessageRepository.GetMessageGroup(groupName);
        // TODO to fix a class does not contain a definition for property and no accessible extension method property accepting a first argument of type class could be found
        var userInGroup = group != null;  //&& group.Connections.Any(x => x.UserId == message.RecipientId);

        if (group != null)
        {
            //var connections = group.Connections.Any(x => x.UserId == message.RecipientId);
            message.DateRead = DateTime.UtcNow;
        }

        uow.MessageRepository.AddMessage(message);

        if (await uow.Complete())
        {
            //var group = GetGroupName(sender.Id, recipient.Id);
            await Clients.Group(groupName).SendAsync("NewMessage", message.ToDto());
            var connections = await PresenceTracker.GetConnectionsForUser(recipient.Id);
            if (connections != null && connections.Count > 0 && !userInGroup)
            {
                await presenceHub.Clients.Clients(connections)
                    .SendAsync("NewMessageReceived", message.ToDto());
            }
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await uow.MessageRepository.RemoveConnection(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task<bool> AddToGroup(string groupName)
    {
        var group = await uow.MessageRepository.GetMessageGroup(groupName);
        var connection = new Connection(Context.ConnectionId, GetUserId());

        if (group == null)
        {
            group = new Group(groupName);
            uow.MessageRepository.AddGroup(group);
        }

        group.Connections.Add(connection);

        return await uow.Complete();
    }

    private string GetGroupName(string? caller, string? other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }

    private string GetUserId()
    {
        return Context.User?.GetMemberId()
            ?? throw new HubException("Cannot get member id.");
    }
}
