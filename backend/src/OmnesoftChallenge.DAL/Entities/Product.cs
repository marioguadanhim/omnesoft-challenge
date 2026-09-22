namespace OmnesoftChallenge.DAL.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public DateTime InsertionDate { get; set; }
    public DateTime LastUpdateDate { get; set; }
}
