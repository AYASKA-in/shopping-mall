namespace ShoppingMall.Core.Enums;

public enum MovementType
{
    Sale,
    PurchaseReceipt,
    TransferOut,
    TransferIn,
    Adjustment,
    Return,
    GRN,
    WriteOff,
    InitialStock
}

public enum StockStatus
{
    InStock,
    LowStock,
    OutOfStock,
    Discontinued,
    OnOrder
}

public enum AdjustmentType
{
    CycleCount,
    Damage,
    Expiry,
    Theft,
    Administrative,
    Promotion
}

public enum InspectionResult
{
    Pending,
    Pass,
    Fail,
    Conditional,
    Hold
}
