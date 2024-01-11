using System;
using System.Collections.Generic;

namespace BonsaiShop.Models;

public partial class User
{
    public long UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string? Password { get; set; }

    public string FullName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime Birthday { get; set; }

    public string? Avatar { get; set; }

    public int? RoleId { get; set; }

    public DateTime? LastLogin { get; set; }

    public bool? IsDeleted { get; set; }

    public int? IsBlocked { get; set; }

    public string? ConfirmCode { get; set; }

    public virtual ICollection<BlogComment> BlogComments { get; set; } = new List<BlogComment>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
