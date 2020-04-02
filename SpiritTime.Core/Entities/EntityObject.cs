using System;
using System.ComponentModel.DataAnnotations;
using SpiritTime.Core.Contracts;

namespace SpiritTime.Core.Entities
{
    public class EntityObject :IEntityObject
    {
        [Key] public int Id { get; set; }

        [Timestamp] public byte[] Timestamp { get; set; }
    }
}
