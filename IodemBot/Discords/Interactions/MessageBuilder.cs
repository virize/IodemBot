﻿using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Rest;
using Discord.Commands;
using System.IO;
using IodemBot.Discords.Contexts;

namespace IodemBot.Discords
{
    public enum MessageStyle
    {
        Embed,
        PlainText,
        None
    }

    public class MessageBuilder
    {
        public IUser UserData { get; set; }
        string BaseMessage { get; set; }
        public bool Success { get; protected set; }
        public string Title { get; set; }
        public Color Color { get; set; }
        public string ThumbnailUrl { get; set; }
        public MessageStyle Style { get; set; }
        public Func<IServiceProvider, RequestContext, RestUserMessage, Task<MessageBuilder>> GetDeferredMessage { get; set; }

        public string ImageUrl { get; set; }
        public Stream ImageStream { get; set; }
        public bool ImageIsSpoiler { get; set; } = false;
        public string ImageFileName { get; set; } = "attachment.png";

        public ComponentBuilder ComponentBuilder {  get; set; }

        public List<OutputFieldMessage> OutputFieldMessages { get; set; }

        public static MessageBuilder Blank(bool success) => new MessageBuilder(success);

        private MessageBuilder(bool success)
        {
            Success = success;
            Style = MessageStyle.None;
        }

        public MessageBuilder(IUser userData, string baseMessage, bool success, string title, Color? color = null, string thumbnailUrl = null, MessageStyle style = MessageStyle.Embed, params OutputFieldMessage[] outputFieldMessages)
        {
            UserData = userData;
            BaseMessage = baseMessage;
            Success = success;
            Title = title;
            Color = color ?? Color.Purple;
            ThumbnailUrl = thumbnailUrl;
            Style = style;
            OutputFieldMessages = outputFieldMessages?.ToList();
        }

        public MessageBuilder Modify(Action<MessageBuilder> action)
        {
            action?.Invoke(this);
            return this;
        }

        public MessageMetadata BuildOutput()
        {
            if (Style == MessageStyle.Embed)
            {
                var embed = new EmbedBuilder()
                {
                    Title = Title,
                    Description = BaseMessage,
                    Color = Color,
                    ThumbnailUrl = ThumbnailUrl
                };

                if (ImageUrl != null)
                    embed.ImageUrl = ImageUrl;

                string message = null;
                if (OutputFieldMessages != null && OutputFieldMessages.Any())
                {
                    embed.Fields = new List<EmbedFieldBuilder>();

                    var invalidFields = OutputFieldMessages.Where(om => om.Message != null && om.MessageType == OutputFieldMessageType.Invalid);
                    if (invalidFields.Any())
                    {
                        string invalidValue = string.Join(Environment.NewLine, invalidFields.Select(om => om.Message));
                        if (string.IsNullOrWhiteSpace(invalidValue))
                            invalidValue = "(missing)";

                        embed.Description = null;
                        embed.Fields.Add("I couldn't do that! Here's why:", invalidValue);
                    }

                    var modificationFields = OutputFieldMessages.Where(om => om.Message != null && om.MessageType == OutputFieldMessageType.Modification);
                    if (modificationFields.Any())
                    {
                        string modificationValue = string.Join(Environment.NewLine, modificationFields.Select(om => om.Message));
                        if (string.IsNullOrWhiteSpace(modificationValue))
                            modificationValue = "(missing)";

                        embed.Fields.Add("Changed Actions:", modificationValue);
                    }

                    var protectionFields = OutputFieldMessages.Where(om => om.Message != null && om.MessageType == OutputFieldMessageType.Protection || om.MessageType == OutputFieldMessageType.BreakingProtection);
                    if (protectionFields.Any())
                    {
                        string protectionValue = string.Join(Environment.NewLine, protectionFields.Select(om => om.Message));
                        if (string.IsNullOrWhiteSpace(protectionValue))
                            protectionValue = "(missing)";

                        embed.Fields.Add("Protections:", protectionValue);
                    }

                    var resultFields = OutputFieldMessages.Where(om => om.Message != null && om.MessageType == OutputFieldMessageType.Result);
                    if (resultFields.Any())
                    {
                        string resultValue = string.Join(Environment.NewLine, resultFields.Select(om => om.Message));
                        if (string.IsNullOrWhiteSpace(resultValue))
                            resultValue = "(missing)";

                        embed.Fields.Add("Results:", resultValue);
                    }

                    var bonusFields = OutputFieldMessages.Where(om => om.Message != null && om.MessageType == OutputFieldMessageType.Bonus);
                    if (bonusFields.Any())
                    {
                        string bonusValue = string.Join(Environment.NewLine, bonusFields.Select(om => om.Message));
                        if (string.IsNullOrWhiteSpace(bonusValue))
                            bonusValue = "(missing)";

                        embed.Fields.Add("Bonuses:", bonusValue);
                    }
                }

                var mentions = OutputFieldMessages.Select(u => u.AffectedUserData.Mention).ToList();
                if (UserData != null)
                    mentions.Insert(0, UserData.Mention);

                bool hasMentions = mentions.Any();

                message = string.Join(", ", mentions.Distinct());

                return new MessageMetadata(embed.Build(), ImageStream, ImageIsSpoiler, ImageFileName, message, Success, hasMentions, ComponentBuilder);
            }
            else if (Style == MessageStyle.PlainText)
            {
                return new MessageMetadata(null, ImageStream, ImageIsSpoiler, ImageFileName, BaseMessage, Success, false, ComponentBuilder);
            }
            else
            {
                return new MessageMetadata(null, ImageStream, ImageIsSpoiler, ImageFileName, null, Success, false, ComponentBuilder);
            }
        }
    }
}
