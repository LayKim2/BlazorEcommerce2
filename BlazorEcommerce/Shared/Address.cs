using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorEcommerce.Shared;

public class Address
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FirstName { get; set; } = String.Empty;
    public string LastName { get; set; } = String.Empty;
    public string Street { get; set; } = String.Empty;
    public string City { get; set; } = String.Empty;
    public string State { get; set; } = String.Empty;
    public string Zip { get; set; } = String.Empty;
    public string Country { get; set; } = String.Empty;
}
