using System;
using System.Collections.Generic;

namespace BonsaiShop.Models;

public partial class AllCode
{
    public int Id { get; set; }

    public string? Type { get; set; }

    public string? KeyCode { get; set; }

    public string? Value { get; set; }

    public virtual ICollection<BlogComment> BlogComments { get; set; } = new List<BlogComment>();

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<User> UserIsActiveNavigations { get; set; } = new List<User>();

    public virtual ICollection<User> UserRoles { get; set; } = new List<User>();
}
