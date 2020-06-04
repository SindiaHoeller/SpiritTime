using System;

namespace SpiritTime.Frontend.Partials.ToastModal
{
    public interface IToastService
    {
        event Action<ToastLevel, string, string> OnShow;
        void ShowInfo(string message, string heading = "");
        void ShowSuccess(string message, string heading = "");
        void ShowWarning(string message, string heading = "");
        void ShowError(string message, string heading = "");
        void ShowToast(ToastLevel level, string message, string heading = "");
        
    }
}