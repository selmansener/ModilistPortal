﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ModilistPortal.Infrastructure.Azure.Extensions.Configurations;
using ModilistPortal.Infrastructure.Shared.Configurations;

namespace ModilistPortal.Functions.EventHandlers.Configurations
{
    public class ConfigurationOptions
    {
        public DbConnectionOptions ModilistDbConnectionOptions { get; set; }

        public string AppStorage { get; set; }

        public EventGridClientOptions EventGridClientOptions { get; set; }
    }
}
