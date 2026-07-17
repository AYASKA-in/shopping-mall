namespace ShoppingMall.Core.Enums;

public enum POStatus
{
    Draft,
    PendingApproval,
    Approved,
    Sent,
    Acknowledged,
    Confirmed,
    PartiallyReceived,
    FullyReceived,
    Closed,
    Cancelled
}

public enum GRNStatus
{
    Draft,
    PendingQI,
    QIInProgress,
    Completed,
    Cancelled
}

public enum MatchStatus
{
    Pending,
    Matched,
    OnHold,
    Blocked
}

public enum PaymentTermStatus
{
    Unpaid,
    Paid,
    Partial,
    Overpaid
}

public enum VendorTier
{
    Platinum,
    Gold,
    Silver,
    Bronze,
    Blacklist
}

public enum DebitNoteStatus
{
    Draft,
    Approved,
    GoodsReturned,
    Closed,
    Cancelled,
    Rejected
}

public enum TransferStatus
{
    Draft,
    Shipped,
    Received,
    Cancelled
}
