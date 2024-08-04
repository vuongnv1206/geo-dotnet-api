namespace FSH.WebApi.Application.Payment;
public class SubcriptionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Duration { get; set; }
    public string Image { get; set; }
}
