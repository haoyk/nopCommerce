namespace Nop.Core.Domain.Shipping
{
    /// <summary>
    /// ����״̬
    /// Represents the shipping status enumeration
    /// </summary>
    public enum ShippingStatus
    {
        /// <summary>
        /// ���跢��
        /// </summary>
        ShippingNotRequired = 10,
        /// <summary>
        /// δ����
        /// </summary>
        NotYetShipped = 20,
        /// <summary>
        /// ���ַ���
        /// </summary>
        PartiallyShipped = 25,
        /// <summary>
        /// �ѷ���
        /// </summary>
        Shipped = 30,
        /// <summary>
        /// �ѽ���
        /// </summary>
        Delivered = 40,
    }
}
