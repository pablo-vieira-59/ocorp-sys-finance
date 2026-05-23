namespace SysFinance.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User";

    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<Investment> Investments { get; set; } = new List<Investment>();
    public ICollection<Asset> Assets { get; set; } = new List<Asset>();
}
