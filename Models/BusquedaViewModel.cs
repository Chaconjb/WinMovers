using System.Collections.Generic;

namespace WinMovers.Models.ViewModels
{
    public class BusquedaViewModel
    {
        public string Criterio { get; set; } = string.Empty;

        public List<Exportacion> Exportaciones { get; set; } = new List<Exportacion>();

        public List<Importacion> Importaciones { get; set; } = new List<Importacion>();

        public bool HayResultados =>
            Importaciones.Count > 0 || Exportaciones.Count > 0;
    }
}