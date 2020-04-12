using System;

namespace SpiritTime.Frontend.Partials.ToastModal
{
    public class ToastInstance
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public ToastSettings ToastSettings { get; set; }
        
    }
}