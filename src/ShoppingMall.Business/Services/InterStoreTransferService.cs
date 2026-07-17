using ShoppingMall.Core.Interfaces;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Business.Services;

public class InterStoreTransferService
{
    private readonly IRepository<InterStoreTransfer> _transferRepo;
    private readonly IRepository<InterStoreTransferLine> _lineRepo;
    private readonly InventoryService _inventory;

    public InterStoreTransferService(
        IRepository<InterStoreTransfer> transferRepo,
        IRepository<InterStoreTransferLine> lineRepo,
        InventoryService inventory)
    {
        _transferRepo = transferRepo;
        _lineRepo = lineRepo;
        _inventory = inventory;
    }

    public async Task<InterStoreTransfer> CreateAsync(Guid fromStoreId, Guid toStoreId, Guid createdByUserId,
        List<CreateTransferLine> lines, string? notes = null)
    {
        var date = DateTime.UtcNow;
        var number = $"TFR-{fromStoreId:N[..4]}-{date:yyyyMMdd}-{Guid.NewGuid():N[..4]}";

        var transfer = new InterStoreTransfer
        {
            Id = Guid.NewGuid(),
            TransferNumber = number,
            FromStoreId = fromStoreId,
            ToStoreId = toStoreId,
            Status = "Draft",
            Notes = notes,
            CreatedByUserId = createdByUserId,
            CreatedAt = date
        };

        transfer.Lines = lines.Select(l => new InterStoreTransferLine
        {
            Id = Guid.NewGuid(),
            TransferId = transfer.Id,
            ProductId = l.ProductId,
            RequestedQty = l.Quantity,
            ShippedQty = 0,
            ReceivedQty = 0
        }).ToList();

        await _transferRepo.AddAsync(transfer);
        return transfer;
    }

    public async Task<InterStoreTransfer?> ShipAsync(Guid transferId, List<ShipLineUpdate>? shipments = null)
    {
        var transfer = await _transferRepo.GetByIdAsync(transferId);
        if (transfer == null) return null;

        if (transfer.Status != "Draft" && transfer.Status != "Shipped")
            throw new InvalidOperationException($"Cannot ship transfer with status '{transfer.Status}'");

        if (transfer.Lines == null || transfer.Lines.Count == 0)
        {
            var lines = await _lineRepo.FindAsync(l => l.TransferId == transferId);
            transfer.Lines = lines.ToList();
        }

        foreach (var line in transfer.Lines)
        {
            var shipped = shipments?.FirstOrDefault(s => s.ProductId == line.ProductId);
            var shipQty = shipped?.Quantity ?? line.RequestedQty;
            if (shipQty <= 0) continue;

            await _inventory.DeductStockAsync(transfer.FromStoreId, line.ProductId, shipQty,
                "TRANSFER_OUT", transfer.Id);
            line.ShippedQty = shipQty;
        }

        transfer.Status = "Shipped";
        transfer.ShippedAt = DateTime.UtcNow;
        await _transferRepo.UpdateAsync(transfer);
        return transfer;
    }

    public async Task<InterStoreTransfer?> ReceiveAsync(Guid transferId, List<ShipLineUpdate>? receipts = null)
    {
        var transfer = await _transferRepo.GetByIdAsync(transferId);
        if (transfer == null) return null;

        if (transfer.Status != "Shipped")
            throw new InvalidOperationException($"Cannot receive transfer with status '{transfer.Status}'");

        if (transfer.Lines == null || transfer.Lines.Count == 0)
        {
            var lines = await _lineRepo.FindAsync(l => l.TransferId == transferId);
            transfer.Lines = lines.ToList();
        }

        foreach (var line in transfer.Lines)
        {
            var received = receipts?.FirstOrDefault(s => s.ProductId == line.ProductId);
            var receiveQty = received?.Quantity ?? line.ShippedQty;
            if (receiveQty <= 0) continue;

            await _inventory.AddStockAsync(transfer.ToStoreId, line.ProductId, receiveQty,
                "TRANSFER_IN", transfer.Id);
            line.ReceivedQty = receiveQty;
        }

        transfer.Status = "Received";
        transfer.ReceivedAt = DateTime.UtcNow;
        await _transferRepo.UpdateAsync(transfer);
        return transfer;
    }

    public async Task<InterStoreTransfer?> CancelAsync(Guid transferId)
    {
        var transfer = await _transferRepo.GetByIdAsync(transferId);
        if (transfer == null) return null;

        if (transfer.Status == "Received" || transfer.Status == "Cancelled")
            throw new InvalidOperationException($"Cannot cancel transfer with status '{transfer.Status}'");

        if (transfer.Status == "Shipped")
        {
            if (transfer.Lines == null || transfer.Lines.Count == 0)
            {
                var lines = await _lineRepo.FindAsync(l => l.TransferId == transferId);
                transfer.Lines = lines.ToList();
            }

            foreach (var line in transfer.Lines)
            {
                if (line.ShippedQty > 0)
                {
                    await _inventory.AddStockAsync(transfer.FromStoreId, line.ProductId, line.ShippedQty,
                        "TRANSFER_REVERSE", transfer.Id);
                }
            }
        }

        transfer.Status = "Cancelled";
        await _transferRepo.UpdateAsync(transfer);
        return transfer;
    }

    public async Task<IEnumerable<InterStoreTransfer>> GetByStoreAsync(Guid storeId)
    {
        return await _transferRepo.FindAsync(t =>
            t.FromStoreId == storeId || t.ToStoreId == storeId);
    }
}

public record CreateTransferLine(Guid ProductId, decimal Quantity);
public record ShipLineUpdate(Guid ProductId, decimal Quantity);
