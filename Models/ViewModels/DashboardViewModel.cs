namespace WinMovers.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalOrdenes { get; set; }
        public int TotalVisitas { get; set; }
        public int TotalExportaciones { get; set; }
        public int TotalImportaciones { get; set; }
        public List<OrdenTrabajo> OrdenesRecientes { get; set; } = new();
    }
}
