using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpiritTime.Frontend.Services.StaticDetails
{
    public static class Paths
    {
        private const string ServerPath = "https://localhost:5003";
        public const string RegisterPath = ServerPath + "/Account/Register";
        public const string LoginPath = ServerPath + "/Account/Login";

    }
}
