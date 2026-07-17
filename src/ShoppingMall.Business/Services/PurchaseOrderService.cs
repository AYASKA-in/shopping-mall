using ShoppingMall.Core.Enums;
using ShoppingMall.Core.Interfaces;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Business.Services;

public class PurchaseOrderService
{
    private readonly IRepository<PurchaseOrder> _poRepo;
    private readonly IRepository<POLine> _poLineRepo;

    public PurchaseOrderService(
        IRepository<PurchaseOrder> poRepo,
        IRepository<POLine> poLineRepo)
    {
        _poRepo = poRepo;
        _poLineRepo = poLineRepo;
    }

    public async Task<PurchaseOrder> CreateAsync(PurchaseOrder po)
    {
        po.Id = Guid.NewGuid();
        po.Status = POStatus.Draft;
        po.CreatedAt = DateTime.UtcNow;
        return await _poRepo.AddAsync(po);
    }

    public async Task<PurchaseOrder?> SubmitAsync(Guid poId)
    {
        var po = await _poRepo.GetByIdAsync(poId);
        if (po == null) return null;

        if (po.Status != POStatus.Draft && po.Status != POStatus.PendingApproval)
            throw new InvalidOperationException($"Cannot submit PO in status {po.Status}");

        po.Status = POStatus.Sent;
        po.UpdatedAt = DateTime.UtcNow;
        await _poRepo.UpdateAsync(po);
        return po;
    }

    public async Task<PurchaseOrder?> CloseAsync(Guid poId)
    {
        var po = await _poRepo.GetByIdAsync(poId);
        if (po == null) return null;

        if (po.Status == POStatus.Closed || po.Status == POStatus.Cancelled)
            throw new InvalidOperationException($"PO already {po.Status}");

        po.Status = POStatus.Closed;
        po.UpdatedAt = DateTime.UtcNow;
        await _poRepo.UpdateAsync(po);
        return po;
    }

    public async Task<PurchaseOrder?> MarkPartiallyReceivedAsync(Guid poId)
    {
        var po = await _poRepo.GetByIdAsync(poId);
        if (po == null) return null;

        if (po.Status != POStatus.Sent && po.Status != POStatus.Confirmed && po.Status != POStatus.PartiallyReceived)
            throw new InvalidOperationException($"Cannot receive PO in status {po.Status}");

        po.Status = POStatus.PartiallyReceived;
        po.UpdatedAt = DateTime.UtcNow;
        await _poRepo.UpdateAsync(po);
        return po;
    }

    public async Task<PurchaseOrder?> MarkFullyReceivedAsync(Guid poId)
    {
        var po = await _poRepo.GetByIdAsync(poId);
        if (po == null) return null;

        if (po.Status != POStatus.Sent && po.Status != POStatus.PartiallyReceived)
            throw new InvalidOperationException($"Cannot mark PO fully received in status {po.Status}");

        po.Status = POStatus.FullyReceived;
        po.UpdatedAt = DateTime.UtcNow;
        await _poRepo.UpdateAsync(po);
        return po;
    }
}
