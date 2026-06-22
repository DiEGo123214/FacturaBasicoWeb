namespace POS.Application.DTOs;

public class DuplicarFacturaDto
{
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public List<DuplicarFacturaItemDto> Items { get; set; } = new();
    public bool TodosDisponibles { get; set; }
}

public class DuplicarFacturaItemDto
{
    public int ProductoId { get; set; }
    public string ProductoCodigo { get; set; } = string.Empty;
    public string ProductoNombre { get; set; } = string.Empty;
    public int CantidadSolicitada { get; set; }
    public int StockActual { get; set; }
    public decimal PrecioUnitario { get; set; }
    public bool TieneStock { get; set; }
}
