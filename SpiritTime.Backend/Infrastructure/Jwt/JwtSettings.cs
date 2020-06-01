using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpiritTime.Backend.Infrastructure.Jwt
{
    /// <summary>
    /// JwtSettings
    /// </summary>
    public abstract class JwtSettings
    {
        /// <summary>
        /// SecurityKey
        /// </summary>
        public string SecurityKey { get; set; }
        /// <summary>
        /// ValidIssuer
        /// </summary>
        public string ValidIssuer { get; set; }
        /// <summary>
        /// ValidAudience
        /// </summary>
        public string ValidAudience { get; set; }
    }
}
