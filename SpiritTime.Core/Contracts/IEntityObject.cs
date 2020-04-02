using System;

namespace SpiritTime.Core.Contracts
{
    public interface IEntityObject
    {
        int Id { get; set; }
        byte[] Timestamp { get; set; }
    }
}
