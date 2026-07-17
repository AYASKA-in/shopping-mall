namespace ShoppingMall.Core.Enums;

public enum UserRole
{
    SuperAdmin,
    Corporate,
    RegionalManager,
    StoreManager,
    Cashier,
    InventoryClerk,
    Viewer
}

public enum TransactionStatus
{
    Draft,
    Active,
    Tendering,
    Completed,
    Voided,
    Suspended,
    Refunded,
    PartiallyRefunded,
    Disputed
}

public enum PaymentStatus
{
    Pending,
    Authorized,
    Captured,
    Settled,
    Failed,
    Cancelled,
    Refunded,
    PartiallyRefunded,
    Disputed
}

public enum PaymentMethod
{
    Cash,
    Card,
    UPI,
    Wallet,
    GiftCard,
    StoreCredit,
    Cheque
}

public enum TaxType
{
    CGST,
    SGST,
    IGST,
    Cess,
    None
}

public enum SupplyType
{
    IntraState,
    InterState
}

public enum StoreStatus
{
    Active,
    Inactive,
    UnderMaintenance,
    Closed
}

public enum TerminalMode
{
    Server,
    Client
}
