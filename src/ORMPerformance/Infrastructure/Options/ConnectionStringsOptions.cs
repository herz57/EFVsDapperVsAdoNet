using System;
using System.Collections.Generic;
using System.Text;

namespace ORMPerformance.Infrastructure.Options
{
    public class ConnectionStringsOptions
    {
        public const string ConnectionStrings = "ConnectionStrings";

        public string AppConnection { get; set; }
    }
}
