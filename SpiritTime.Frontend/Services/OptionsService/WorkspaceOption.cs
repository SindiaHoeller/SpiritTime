using System.Collections.Generic;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Frontend.Services.OptionsService
{
    public class WorkspaceOption
    {
        public List<WorkspaceDto> WorkspaceList { get; set; }
        public int CurrentWorkspaceId { get; set; }
        public bool Success { get; set; }
        public string ErrorMsg { get; set; }
    }
}