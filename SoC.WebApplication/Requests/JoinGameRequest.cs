﻿using System;

namespace SoC.WebApplication.Requests
{
    public class JoinGameRequest : RequestBase
    {
        public Guid GameId { get; set; }
    }
}
