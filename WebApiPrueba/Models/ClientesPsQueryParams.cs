namespace WebApiPrueba.Models
{
    public class ClientesPsQueryParams
    {
        private const int MaxPageSize = 50; // Límite máximo de elementos por página
        public int PageNumber { get; set; } = 1; // Página por defecto

        private int _pageSize = 5; // Tamaño de página por defecto
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string? SortField { get; set; } = "Id"; // Campo por defecto para ordenar
        public string? SortDirection { get; set; } = "asc"; // Dirección por defecto (ascendente)

        // Propiedades para los filtros
        public string? Nombres { get; set; }
        public string? NumId { get; set; }
        public bool? Estado { get; set; } // Nullable para permitir no filtrar por estado

    }
}
