namespace ShoppingMall.Core.Enums;

public enum PromotionType
{
    PercentageOff,
    FixedAmountOff,
    BuyXGetY,
    Bundle,
    CartDiscount,
    FreeItem,
    TieredDiscount,
    ShippingDiscount
}

public enum DiscountApplication
{
    PerLine,
    OnCart,
    OnCheapest,
    OnMostExpensive,
    OnCategory
}

public enum PromotionStackability
{
    Stackable,
    Exclusive,
    BestOf
}

public enum CouponStatus
{
    Active,
    Used,
    Expired,
    Disabled
}
