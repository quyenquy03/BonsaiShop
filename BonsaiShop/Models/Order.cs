using System;
using System.Collections.Generic;

namespace BonsaiShop.Models;

public partial class Order
{
    public long OrderId { get; set; }

    public string? Code { get; set; }

    public long? UserId { get; set; }

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public decimal? TotalAmount { get; set; }

    public decimal? TotalPayment { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? OrderStatus { get; set; }

    public long? FeeShipId { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User? User { get; set; }
}
