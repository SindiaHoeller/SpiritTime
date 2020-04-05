using System.Collections.Generic;
using SpiritTime.Core.Entities;

namespace SpiritTime.Shared.Models.WorkspaceModels
{
    public class WorkspaceListResult
    {
        public string Error { get; set; }
        public bool Successful { get; set; }
        public List<WorkspaceDto> Workspaces { get; set; }
    }
}