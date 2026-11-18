using MailingListApp.Models;
using MailingListApp.Repositories;

namespace MailingListApp.Tests;

public class MailingListRepositoryTests
{
    private const string ConnectionString =
        @"Server=DESKTOP-A5B95S7;Database=MailingListDb;Trusted_Connection=True;TrustServerCertificate=True;";

    [Fact]
    public void CountryCrud_InsertUpdateDelete_WorksCorrectlyInDb()
    {
        var repo = new MailingListRepository(ConnectionString);

        var insertedId = repo.InsertCountry(new Country { Name = "Тестова Країна" });
        Assert.True(insertedId > 0);

        var updated = repo.UpdateCountry(new Country { Id = insertedId, Name = "Оновлена Країна" });
        Assert.True(updated);

        var deleted = repo.DeleteCountry(insertedId);
        Assert.True(deleted);
    }

    [Fact]
    public void Task3_PromoCountStoredProcedures_ReturnValidResults()
    {
        var repo = new MailingListRepository(ConnectionString);

        var top3 = repo.GetTop3SectionsByPromoCount().ToList();
        var most = repo.GetMostPromoItemsSection();
        var least = repo.GetLeastPromoItemsSection();

        Assert.NotEmpty(top3);
        Assert.NotNull(most);
        Assert.NotNull(least);
        Assert.True(most.PromoItemsCount >= least.PromoItemsCount);
    }

    [Fact]
    public void Task4_PromotionsAndAverageAge_ReturnExpectedData()
    {
        var repo = new MailingListRepository(ConnectionString);

        var endingIn3Days = repo.GetPromotionsEndingIn3Days().ToList();
        var avgBySection = repo.GetAverageAgeBySection().ToList();
        var avgByCity = repo.GetAverageAgeByCity().ToList();
        var avgByCountry = repo.GetAverageAgeByCountry().ToList();

        Assert.NotEmpty(endingIn3Days);
        Assert.NotEmpty(avgBySection);
        Assert.NotEmpty(avgByCity);
        Assert.NotEmpty(avgByCountry);
    }

    [Fact]
    public void Task5_GenderStatistics_ReturnExpectedData()
    {
        var repo = new MailingListRepository(ConnectionString);

        var topByGender = repo.GetTopSectionByGender().ToList();
        var top3ByGender = repo.GetTop3SectionsByGender().ToList();
        var countByGender = repo.GetCustomersCountByGender().ToList();
        var countByGenderAndCountry = repo.GetCustomersCountByGenderAndCountry().ToList();

        Assert.NotEmpty(topByGender);
        Assert.NotEmpty(top3ByGender);
        Assert.NotEmpty(countByGender);
        Assert.NotEmpty(countByGenderAndCountry);
    }
}