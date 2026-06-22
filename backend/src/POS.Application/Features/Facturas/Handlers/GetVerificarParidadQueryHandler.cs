using MediatR;
using System.Text.Json;
using POS.Application.DTOs;
using POS.Application.Features.Facturas.Queries;
using POS.Domain.Entities;
using POS.Domain.Interfaces;

namespace POS.Application.Features.Facturas.Handlers;

public class GetVerificarParidadQueryHandler : IRequestHandler<GetVerificarParidadQuery, VerificarParidadDto?>
{
    private readonly IUnitOfWork _uow;

    public GetVerificarParidadQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<VerificarParidadDto?> Handle(GetVerificarParidadQuery request, CancellationToken ct)
    {
        var factura = await _uow.Facturas.GetByIdAsync(request.Id);
        if (factura is null) return null;

        string clienteNombre = "CONSUMIDOR FINAL";
        if (factura.Cliente != null)
        {
            clienteNombre = $"{factura.Cliente.Nombre} {factura.Cliente.Apellido}".Trim();
        }
        else if (!string.IsNullOrWhiteSpace(factura.SnapshotJson))
        {
            try
            {
                var snapshot = JsonSerializer.Deserialize<FacturaSnapshotDto>(factura.SnapshotJson);
                if (snapshot != null)
                {
                    clienteNombre = $"{snapshot.ClienteNombre} {snapshot.ClienteApellido}".Trim();
                }
            }
            catch { }
        }

        var detalles = factura.Detalles.ToList();
        if (!detalles.Any() && !string.IsNullOrWhiteSpace(factura.SnapshotJson))
        {
            try
            {
                var snapshot = JsonSerializer.Deserialize<FacturaSnapshotDto>(factura.SnapshotJson);
                if (snapshot?.Detalles != null)
                {
                    detalles = snapshot.Detalles.Select(d => new FacturaDetalle
                    {
                        ProductoId = d.ProductoId,
                        ProductoCodigo = d.ProductoCodigo,
                        ProductoNombre = d.ProductoNombre,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Subtotal
                    }).ToList();
                }
            }
            catch { }
        }

        int cantidadItems = detalles.Count;
        decimal totalFactura = factura.Total;

        if (totalFactura == 0 && !string.IsNullOrWhiteSpace(factura.SnapshotJson))
        {
            try
            {
                var snapshot = JsonSerializer.Deserialize<FacturaSnapshotDto>(factura.SnapshotJson);
                if (snapshot != null)
                {
                    totalFactura = snapshot.Total;
                }
            }
            catch { }
        }

        bool itemsEsPar = cantidadItems % 2 == 0;
        int totalEntero = (int)Math.Truncate(totalFactura);
        bool totalEsPar = totalEntero % 2 == 0;
        bool puedeCopiarse = itemsEsPar && totalEsPar;

        var itemsDto = new List<VerificarParidadItemDto>();
        foreach (var detail in detalles)
        {
            var producto = await _uow.Productos.GetByIdAsync(detail.ProductoId);
            itemsDto.Add(new VerificarParidadItemDto
            {
                ProductoId = detail.ProductoId,
                Cantidad = detail.Cantidad,
                PrecioUnitario = detail.PrecioUnitario,
                ProductoCodigo = producto?.Codigo ?? detail.ProductoCodigo,
                ProductoNombre = producto?.Nombre ?? detail.ProductoNombre,
                StockActual = producto?.Stock ?? 0
            });
        }

        return new VerificarParidadDto
        {
            FacturaId = factura.Id,
            NumeroFactura = factura.NumeroFactura,
            CantidadItems = cantidadItems,
            TotalFactura = totalFactura,
            ItemsEsPar = itemsEsPar,
            TotalEsPar = totalEsPar,
            PuedeCopiarse = puedeCopiarse,
            ClienteId = factura.ClienteId,
            ClienteNombre = clienteNombre,
            Items = itemsDto
        };
    }
}
