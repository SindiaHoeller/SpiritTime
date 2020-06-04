using System;
using System.Threading.Tasks;

namespace SpiritTime.Frontend.Services
{
    public class NotifierService
    {
        // Can be called from anywhere
        public async Task UpdateTime(string time)
        {
            if (Notify != null)
            {
                await Notify.Invoke(time);
            }
        }

        public event Func<string, Task> Notify;
    }
}