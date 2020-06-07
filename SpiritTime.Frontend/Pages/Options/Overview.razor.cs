using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using SpiritTime.Frontend.Infrastructure.Config;
using SpiritTime.Frontend.Infrastructure.Config.WriteOptions;
using SpiritTime.Frontend.Infrastructure.ElectronConfig;
using SpiritTime.Frontend.Partials.ToastModal;
using SpiritTime.Frontend.Services.OptionsService;
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Frontend.Pages.Options
{
    public partial class Overview
    {
        [Inject] private IOptionService Service { get; set; }
        [Inject] private IToastService ToastService { get; set; }
        [Inject] private IWebHostEnvironment Env { get; set; }
        [Inject] private IOptions<AppSettings> Settings { get; set; }
        [Inject] private IWritableOptions<ShortcutsConfig> WritableConfig { get; set; }
        [Inject] private IWritableOptions<ElectronProxyConfig> WritableProxyConfig { get; set; }
        private ElectronProxyConfig ElectronProxyConfig { get; set; }
        private ShortcutsConfig ShortcutsConfig { get; set; }
        private bool ShowError { get; set; }
        private string ErrMsg { get; set; }
        private bool ShowWorkspaces { get; set; }
        public List<WorkspaceDto> WorkspaceList { get; set; }
        public int CurrentWorkspaceId { get; set; }
        public string CurrentWorkspaceStringId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ShortcutsConfig = WritableConfig.Value;
            ElectronProxyConfig = WritableProxyConfig.Value;
            var result = await Service.GetCurrentWorkspaceAndList();
            if (result.Success)
            {
                WorkspaceList = result.WorkspaceList;
                CurrentWorkspaceId = result.CurrentWorkspaceId;
                CurrentWorkspaceStringId = result.CurrentWorkspaceId.ToString();
                ShowWorkspaces = true;
            }
            else
            {
                ToastService.ShowError(result.ErrorMsg);
                //ErrMsg = result.ErrorMsg;
                ShowError = true;
            }
        }

        private void SetWorkspace()
        {
            Int32.TryParse(CurrentWorkspaceStringId, out int id);
            if (id > 0)
            {
                Service.SetWorkspace(id);
                ToastService.ShowSuccess(SuccessMsg.UpdatedOptions);
            }
            else
            {
                ToastService.ShowError(ErrorMsg.ChooseOption);
                ShowError = true;
            }
            StateHasChanged();
        }

        private void WriteChangesToAppsettings()
        {
            try
            {
                WritableConfig.Update(opt =>
                {
                    opt.CurrentTask = ShortcutsConfig.CurrentTask;
                    opt.NewTask     = ShortcutsConfig.NewTask;
                });
                ElectronConfiguration.SetGlobalKeyboardShortcuts(ShortcutsConfig.NewTask, ShortcutsConfig.CurrentTask, ElectronProxyConfig.GetProxy());
                ToastService.ShowSuccess(SuccessMsg.SuccessedUpdate);
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }
        }
        private async Task WriteProxyChangesToAppsettings()
        {
            try
            {
                WritableProxyConfig.Update(opt =>
                {
                    opt.PacScript = ElectronProxyConfig.PacScript;
                    opt.ProxyRules     = ElectronProxyConfig.ProxyRules;
                    opt.ProxyBypassRules     = ElectronProxyConfig.ProxyBypassRules;
                });
                var result = await ElectronConfiguration.SetProxy(ElectronProxyConfig.GetProxy(), Settings.Value.BackendBaseAddress);
                if (result == "DIRECT")
                {
                    ToastService.ShowSuccess(SuccessMsg.UpdatedProxy);
                }
                else if(!string.IsNullOrEmpty(result))
                {
                    ToastService.ShowInfo(result);
                }
                else
                {
                    ToastService.ShowError(ErrorMsg.ProxyError);
                }
                
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }
        }
        
        

        private void Opentray()
        {
            ElectronConfiguration.CreateTray(Env);
        }
    }
}