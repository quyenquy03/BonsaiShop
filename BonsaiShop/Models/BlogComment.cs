using System;
using System.Collections.Generic;

namespace BonsaiShop.Models;

public partial class BlogComment
{
    public long CommentId { get; set; }

    public long? UserId { get; set; }

    public long? BlogId { get; set; }

    public string? Detail { get; set; }

    public long? ParrentId { get; set; }

    public int? IsActive { get; set; }

    public int? Levels { get; set; }

    public virtual Blog? Blog { get; set; }

    public virtual AllCode? IsActiveNavigation { get; set; }

    public virtual User? User { get; set; }
}
