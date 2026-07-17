namespace ShoppingMall.Core.Models;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public Guid TerminalId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? CustomerId { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public TransactionStatus Status { get; set; } = TransactionStatus.Active;
    public decimal SubTotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal RoundingAmount { get; set; }
    public string? Notes { get; set; }
    public Guid? IdempotencyKey { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public uint RowVersion { get; set; }

    public Store Store { get; set; } = null!;
    public Terminal Terminal { get; set; } = null!;
    public User? User { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<TransactionLine> Lines { get; set; } = new List<TransactionLine>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<TaxBreakdown> TaxBreakdowns { get; set; } = new List<TaxBreakdown>();
}

public class TransactionLine
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public Guid ProductId { get; set; }
    public int LineNumber { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? SKU { get; set; }
    public string? Barcode { get; set; }
    public decimal Quantity { get; set; }
    public Guid UOMId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Mrp { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal? DiscountPercent { get; set; }
    public string? DiscountReason { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetAmount { get; set; }
    public bool IsWeighable { get; set; }
    public decimal? WeightKg { get; set; }
    public Guid? ParentLineId { get; set; }

    public Transaction Transaction { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

public class Payment
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public decimal? TenderedAmount { get; set; }
    public decimal? ChangeAmount { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? GatewayResponse { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public Guid? IdempotencyKey { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SettledAt { get; set; }

    public Transaction Transaction { get; set; } = null!;
}

public class TaxBreakdown
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public Guid? TransactionLineId { get; set; }
    public TaxType TaxType { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }

    public Transaction Transaction { get; set; } = null!;
    public TransactionLine? TransactionLine { get; set; }
}

public class SuspendedTransaction
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public Guid TerminalId { get; set; }
    public Guid? UserId { get; set; }
    public string BasketData { get; set; } = string.Empty;
    public decimal BasketTotal { get; set; }
    public int ItemCount { get; set; }
    public DateTime SuspendedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RecalledAt { get; set; }
    public bool IsRecalled { get; set; }
}

public class VoidLog
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public Guid? VoidedByUserId { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? ReasonCode { get; set; }
    public DateTime VoidedAt { get; set; } = DateTime.UtcNow;

    public Transaction Transaction { get; set; } = null!;
}

public class Refund
{
    public Guid Id { get; set; }
    public Guid OriginalTransactionId { get; set; }
    public Guid? RefundTransactionId { get; set; }
    public decimal RefundAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public PaymentMethod RefundMethod { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Transaction OriginalTransaction { get; set; } = null!;
}

public class CashMovement
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public Guid TerminalId { get; set; }
    public Guid UserId { get; set; }
    public string MovementType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class CashDeclaration
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public Guid TerminalId { get; set; }
    public Guid UserId { get; set; }
    public Guid? SessionId { get; set; }
    public string DeclarationType { get; set; } = "START";
    public int Count2000 { get; set; }
    public int Count500 { get; set; }
    public int Count200 { get; set; }
    public int Count100 { get; set; }
    public int Count50 { get; set; }
    public int Count20 { get; set; }
    public int Count10 { get; set; }
    public int CountCoins { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
