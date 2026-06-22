namespace POS.Application.DTOs;

public class VerificarParidadDto
{
    public int FacturaId { get; set; }
    public string NumeroFactura { get; set; } = string.Empty;
    public int CantidadItems { get; set; }
    public decimal TotalFactura { get; set; }
    public bool ItemsEsPar { get; set; }
    public bool TotalEsPar { get; set; }
    public bool PuedeCopiarse { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public List<VerificarParidadItemDto> Items { get; set; } = new();
}

public class VerificarParidadItemDto
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public string ProductoCodigo { get; set; } = string.Empty;
    public string ProductoNombre { get; set; } = string.Empty;
    public int StockActual { get; set; }
}
