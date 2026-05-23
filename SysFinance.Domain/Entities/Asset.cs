namespace SysFinance.Domain.Entities;

public class Asset
{
    public Guid? Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal EstimatedValue { get; set; }
    public string Type { get; set; } = string.Empty;

    public Guid? UserId { get; set; }
    public User? User { get; set; }
}
