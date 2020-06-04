using System;
using Microsoft.Extensions.Options;

namespace SpiritTime.Frontend.Infrastructure.Config.WriteOptions
{
    public interface IWritableOptions<out T> : IOptions<T> where T : class, new()
    {
        void Update(Action<T> applyChanges);
    }
}