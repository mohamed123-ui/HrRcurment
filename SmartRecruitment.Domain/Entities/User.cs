using System;
using System.Collections.Generic;
namespace SmartRecruitment.Domain.Entities;


public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Hr> Hrs { get; set; } = new List<Hr>();
}
