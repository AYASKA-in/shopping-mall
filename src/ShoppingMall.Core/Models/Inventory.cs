namespace ShoppingMall.Core.Models;

public class StockLedger
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public Guid ProductId { get; set; }
    public MovementType MovementType { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }
    public decimal Quantity { get; set; }
    public decimal QuantityBefore { get; set; }
    public decimal QuantityAfter { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public Guid? BatchId { get; set; }
    public string? LotNumber { get; set; }
    public string? Notes { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Store Store { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

public class CurrentStock
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public Guid ProductId { get; set; }
    public decimal OnHand { get; set; }
    public decimal Reserved { get; set; }
    public decimal Available { get; set; }
    public decimal OnOrder { get; set; }
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    public uint RowVersion { get; set; }

    public Store Store { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

public class GoodsReceipt
{
    public Guid Id { get; set; }
    public string GRNNumber { get; set; } = string.Empty;
    public Guid StoreId { get; set; }
    public Guid? POId { get; set; }
    public Guid SupplierId { get; set; }
    public string? DeliveryChallanNo { get; set; }
    public string? VehicleNo { get; set; }
    public GRNStatus Status { get; set; } = GRNStatus.Draft;
    public DateTime ReceiptDate { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public PurchaseOrder? PurchaseOrder { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public ICollection<GRNLine> Lines { get; set; } = new List<GRNLine>();
}

public class GRNLine
{
    public Guid Id { get; set; }
    public Guid GRNId { get; set; }
    public Guid ProductId { get; set; }
    public decimal ReceivedQty { get; set; }
    public decimal AcceptedQty { get; set; }
    public decimal RejectedQty { get; set; }
    public decimal UnitPrice { get; set; }
    public string? BatchNo { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public DateOnly? MfgDate { get; set; }
    public InspectionResult InspectionStatus { get; set; } = InspectionResult.Pending;
    public string? RejectionReason { get; set; }

    public GoodsReceipt GoodsReceipt { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

public class StockAdjustment
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public string AdjustmentNumber { get; set; } = string.Empty;
    public AdjustmentType Type { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid CreatedByUserId { get; set; }
    public bool IsPosted { get; set; }

    public ICollection<StockAdjustmentLine> Lines { get; set; } = new List<StockAdjustmentLine>();
}

public class StockAdjustmentLine
{
    public Guid Id { get; set; }
    public Guid AdjustmentId { get; set; }
    public Guid ProductId { get; set; }
    public decimal QuantityChange { get; set; }
    public decimal UnitCost { get; set; }
    public string? Reason { get; set; }

    public StockAdjustment Adjustment { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

public class InterStoreTransfer
{
    public Guid Id { get; set; }
    public string TransferNumber { get; set; } = string.Empty;
    public Guid FromStoreId { get; set; }
    public Guid ToStoreId { get; set; }
    public string Status { get; set; } = "Draft";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ShippedAt { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string? Notes { get; set; }

    public ICollection<InterStoreTransferLine> Lines { get; set; } = new List<InterStoreTransferLine>();
}

public class InterStoreTransferLine
{
    public Guid Id { get; set; }
    public Guid TransferId { get; set; }
    public Guid ProductId { get; set; }
    public decimal RequestedQty { get; set; }
    public decimal ShippedQty { get; set; }
    public decimal ReceivedQty { get; set; }

    public InterStoreTransfer Transfer { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

public class ReorderRule
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public Guid ProductId { get; set; }
    public decimal ReorderLevel { get; set; }
    public decimal ReorderQty { get; set; }
    public decimal MaxStock { get; set; }
    public decimal SafetyStock { get; set; }
    public Guid? PreferredSupplierId { get; set; }
    public bool IsActive { get; set; } = true;
}
