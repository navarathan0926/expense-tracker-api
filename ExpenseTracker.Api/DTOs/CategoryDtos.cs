namespace ExpenseTracker.Api.DTOs;

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
}

public class CategoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
