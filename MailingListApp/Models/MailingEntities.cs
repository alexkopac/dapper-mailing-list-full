namespace MailingListApp.Models;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class City
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CountryId { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int CityId { get; set; }
}

public class Section
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class PromotionalItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal DiscountPercent { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int SectionId { get; set; }
}

public class ArchivedPromotionalItem
{
    public int Id { get; set; }
    public int OriginalItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal DiscountPercent { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int SectionId { get; set; }
    public DateTime ArchivedAt { get; set; }
}

public class SectionPromoStats
{
    public int SectionId { get; set; }
    public string SectionName { get; set; } = string.Empty;
    public int PromoItemsCount { get; set; }
}

public class AverageAgeStats
{
    public string EntityName { get; set; } = string.Empty;
    public double AverageAge { get; set; }
}

public class GenderSectionStats
{
    public string Gender { get; set; } = string.Empty;
    public int SectionId { get; set; }
    public string SectionName { get; set; } = string.Empty;
    public int SubscribersCount { get; set; }
}

public class GenderCountStats
{
    public string CountryName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public int CustomersCount { get; set; }
}