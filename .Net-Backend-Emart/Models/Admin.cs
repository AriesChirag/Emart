using System;
using System.Collections.Generic;

namespace Emart_DotNet.Models;

public partial class Admin
{
    public int AdminId { get; set; }

    public ulong Active { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Role { get; set; }

    public string? Username { get; set; }
}
