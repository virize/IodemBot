﻿using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IodemBot.Discords
{
    public class MessageMetadata
    {
        public Embed Embed { get; set; }
        public Stream ImageStream { get; set; }
        public bool ImageIsSpoiler { get; set; }
        public string ImageFileName { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public bool HasMentions { get; set; }
        public MessageComponent Components { get; set; }

        public MessageMetadata(Embed embed, Stream imageStream, bool imageIsSpoiler, string imageFileName, string message, bool success, bool hasMentions, ComponentBuilder componentBuilder)
        {
            Embed = embed;
            ImageStream = imageStream;
            ImageIsSpoiler = imageIsSpoiler;
            ImageFileName = imageFileName;
            Message = message;
            Success = success;
            HasMentions = hasMentions;
            Components = componentBuilder?.Build();
        }

        internal object RandomElement()
        {
            throw new NotImplementedException();
        }
    }
}