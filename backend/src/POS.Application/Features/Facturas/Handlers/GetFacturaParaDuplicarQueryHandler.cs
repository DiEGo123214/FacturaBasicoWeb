using MediatR;
using System.Text.Json;
using POS.Application.DTOs;
using POS.Application.Features.Facturas.Queries;
using POS.Domain.Entities;
using POS.Domain.Interfaces;

namespace POS.Application.Features.Facturas.Handlers;

public class GetFacturaParaDuplicarQueryHandler : IRequestHandler<GetFacturaParaDuplicarQuery, DuplicarFacturaDto?>
{
    private readonly IUnitOfWork _uow;

    public GetFacturaParaDuplicarQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<DuplicarFacturaDto?> Handle(GetFacturaParaDuplicarQuery request, CancellationToken ct)
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

        bool todosDisponibles = true;
        var itemsDto = new List<DuplicarFacturaItemDto>();

        foreach (var detail in detalles)
        {
            var producto = await _uow.Productos.GetByIdAsync(detail.ProductoId);
            int stockActual = producto?.Stock ?? 0;
            bool tieneStock = stockActual >= detail.Cantidad;
            if (!tieneStock)
            {
                todosDisponibles = false;
            }

            itemsDto.Add(new DuplicarFacturaItemDto
            {
                ProductoId = detail.ProductoId,
                ProductoCodigo = producto?.Codigo ?? detail.ProductoCodigo,
                ProductoNombre = producto?.Nombre ?? detail.ProductoNombre,
                CantidadSolicitada = detail.Cantidad,
                StockActual = stockActual,
                PrecioUnitario = producto?.Precio ?? detail.PrecioUnitario,
                TieneStock = tieneStock
            });
        }

        return new DuplicarFacturaDto
        {
            ClienteId = factura.ClienteId,
            ClienteNombre = clienteNombre,
            Items = itemsDto,
            TodosDisponibles = todosDisponibles
        };
    }
}
