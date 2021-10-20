﻿using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using IodemBot.Discords;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IodemBot.Discords.Contexts
{
    public class RequestCommandContext : RequestContext
    {
        public RequestCommandContext(SocketCommandContext context)
        {
            OriginalContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public SocketCommandContext OriginalContext { get; }

        public override DiscordSocketClient Client => OriginalContext.Client;
        public override SocketGuild Guild => OriginalContext.Guild;
        public override ISocketMessageChannel Channel => OriginalContext.Channel;
        public override SocketUser User => OriginalContext.User;
        public override SocketUserMessage Message => OriginalContext.Message;

        public async override Task<RestUserMessage> ReplyWithMessageAsync(EphemeralRule ephemeralRule, string message = null, bool isTTS = false, Embed[] embeds = null, Embed embed = null, 
            RequestOptions options = null, AllowedMentions allowedMentions = null, MessageReference messageReference = null, MessageComponent components = null, bool hasMentions = false)
        {
            await GetInitialAsync(true);

            if (embed == null && embeds != null && embeds.Any())
                embed = embeds.FirstOrDefault();

            return await Channel.SendMessageAsync(message, isTTS, embed, options, allowedMentions, messageReference, components);
        }

        public async override Task<RestUserMessage> ReplyWithFileAsync(EphemeralRule ephemeralRule, Stream stream, string filename, bool isSpoiler, string message = null, bool isTTS = false, Embed[] embeds = null, Embed embed = null, 
            RequestOptions options = null, AllowedMentions allowedMentions = null, MessageReference messageReference = null, MessageComponent components = null, bool hasMentions = false)
        {
            await GetInitialAsync(true);

            if (embed == null && embeds != null && embeds.Any())
                embed = embeds.FirstOrDefault();

            return await Channel.SendFileAsync(stream, filename, message, isTTS, embed, options, isSpoiler, allowedMentions, messageReference, components);
        }

        public override async Task UpdateReplyAsync(Action<MessageProperties> propBuilder, RequestOptions options = null)
        {
            await GetInitialAsync(true);
            await OriginalContext.Message?.ModifyAsync(propBuilder);
        }
    }
}