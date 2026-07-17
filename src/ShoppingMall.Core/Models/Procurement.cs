namespace ShoppingMall.Core.Models;

public class PurchaseOrder
{
    public Guid Id { get; set; }
    public string PONumber { get; set; } = string.Empty;
    public Guid StoreId { get; set; }
    public Guid SupplierId { get; set; }
    public POStatus Status { get; set; } = POStatus.Draft;
    public DateOnly? ExpectedDeliveryDate { get; set; }
    public DateOnly? PromisedDeliveryDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public string? Notes { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Store Store { get; set; } = null!;
    public Supplier Supplier { get; set; } = null!;
    public ICollection<POLine> Lines { get; set; } = new List<POLine>();
    public ICollection<GoodsReceipt> GoodsReceipts { get; set; } = new List<GoodsReceipt>();
}

public class POLine
{
    public Guid Id { get; set; }
    public Guid POId { get; set; }
    public Guid ProductId { get; set; }
    public int LineNo { get; set; }
    public decimal OrderedQty { get; set; }
    public decimal ReceivedQty { get; set; }
    public decimal AcceptedQty { get; set; }
    public decimal RejectedQty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal NetAmount { get; set; }
    public DateOnly? RequiredDate { get; set; }
    public bool IsBackorder { get; set; }

    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

public class VendorInvoice
{
    public Guid Id { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public Guid POId { get; set; }
    public Guid SupplierId { get; set; }
    public DateOnly InvoiceDate { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetAmount { get; set; }
    public MatchStatus MatchStatus { get; set; } = MatchStatus.Pending;
    public PaymentTermStatus PaymentStatus { get; set; } = PaymentTermStatus.Unpaid;
    public DateOnly? DueDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public Supplier Supplier { get; set; } = null!;
}

public class DebitNote
{
    public Guid Id { get; set; }
    public string DebitNoteNo { get; set; } = string.Empty;
    public Guid GRNId { get; set; }
    public Guid POId { get; set; }
    public Guid SupplierId { get; set; }
    public string ReturnType { get; set; } = string.Empty;
    public DebitNoteStatus Status { get; set; } = DebitNoteStatus.Draft;
    public decimal TotalAmount { get; set; }
    public decimal TaxAdjustment { get; set; }
    public decimal NetAdjustment { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
