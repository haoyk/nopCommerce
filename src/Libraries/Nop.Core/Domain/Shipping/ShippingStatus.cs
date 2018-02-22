namespace Nop.Core.Domain.Shipping
{
    /// <summary>
    /// 发货状态
    /// Represents the shipping status enumeration
    /// </summary>
    public enum ShippingStatus
    {
        /// <summary>
        /// 不需发货
        /// </summary>
        ShippingNotRequired = 10,
        /// <summary>
        /// 未发货
        /// </summary>
        NotYetShipped = 20,
        /// <summary>
        /// 部分发货
        /// </summary>
        PartiallyShipped = 25,
        /// <summary>
        /// 已发货
        /// </summary>
        Shipped = 30,
        /// <summary>
        /// 已交货
        /// </summary>
        Delivered = 40,
    }
}
