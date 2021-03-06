﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Linq;
using App.Metrics.Data;

// ReSharper disable CheckNamespace

namespace App.Metrics.Infrastructure
// ReSharper restore CheckNamespace
{
    public static class EnvironmentInfoExtensions
    {
        public static IDictionary<string, string> ToEnvDictionary(this EnvironmentInfo environmentInfo)
        {
            return environmentInfo.Entries.ToDictionary(entry => entry.Name, entry => entry.Value);
        }

        public static MetricTags ToTags(this EnvironmentInfo environmentInfo)
        {
            return new MetricTags(new Dictionary<string, string>
            {
                { "version", environmentInfo.EntryAssemblyVersion },
                { "host", environmentInfo.MachineName },
                { "ip_adress", environmentInfo.IpAddress }
            });
        }
    }
}