using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using MailingListApp.Models;

namespace MailingListApp.Repositories;

public class MailingListRepository
{
    private readonly string _connectionString;

    public MailingListRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private SqlConnection CreateConnection() => new SqlConnection(_connectionString);

    // =========================================================================
    // ЧАСТИНА 1 — ЗАВДАННЯ 1: Вставка (INSERT)
    // =========================================================================

    public int InsertCustomer(Customer customer)
    {
        const string sql = """
            INSERT INTO Customers (FullName, BirthDate, Gender, Email, CityId)
            OUTPUT INSERTED.Id
            VALUES (@FullName, @BirthDate, @Gender, @Email, @CityId);
            """;
        using var db = CreateConnection();
        return db.ExecuteScalar<int>(sql, customer);
    }

    public int InsertCountry(Country country)
    {
        const string sql = """
            INSERT INTO Countries (Name)
            OUTPUT INSERTED.Id
            VALUES (@Name);
            """;
        using var db = CreateConnection();
        return db.ExecuteScalar<int>(sql, country);
    }

    public int InsertCity(City city)
    {
        const string sql = """
            INSERT INTO Cities (Name, CountryId)
            OUTPUT INSERTED.Id
            VALUES (@Name, @CountryId);
            """;
        using var db = CreateConnection();
        return db.ExecuteScalar<int>(sql, city);
    }

    public int InsertSection(Section section)
    {
        const string sql = """
            INSERT INTO Sections (Name)
            OUTPUT INSERTED.Id
            VALUES (@Name);
            """;
        using var db = CreateConnection();
        return db.ExecuteScalar<int>(sql, section);
    }

    public int InsertPromotionalItem(PromotionalItem item)
    {
        const string sql = """
            INSERT INTO PromotionalItems (Name, DiscountPercent, StartDate, EndDate, SectionId)
            OUTPUT INSERTED.Id
            VALUES (@Name, @DiscountPercent, @StartDate, @EndDate, @SectionId);
            """;
        using var db = CreateConnection();
        return db.ExecuteScalar<int>(sql, item);
    }

    public void SubscribeCustomerToSection(int customerId, int sectionId)
    {
        const string sql = """
            IF NOT EXISTS (SELECT 1 FROM CustomerSections WHERE CustomerId = @CustomerId AND SectionId = @SectionId)
                INSERT INTO CustomerSections (CustomerId, SectionId) VALUES (@CustomerId, @SectionId);
            """;
        using var db = CreateConnection();
        db.Execute(sql, new { CustomerId = customerId, SectionId = sectionId });
    }

    // =========================================================================
    // ЧАСТИНА 1 — ЗАВДАННЯ 2: Оновлення (UPDATE)
    // =========================================================================

    public bool UpdateCustomer(Customer customer)
    {
        const string sql = """
            UPDATE Customers
            SET FullName = @FullName,
                BirthDate = @BirthDate,
                Gender = @Gender,
                Email = @Email,
                CityId = @CityId
            WHERE Id = @Id;
            """;
        using var db = CreateConnection();
        return db.Execute(sql, customer) > 0;
    }

    public bool UpdateCountry(Country country)
    {
        const string sql = "UPDATE Countries SET Name = @Name WHERE Id = @Id;";
        using var db = CreateConnection();
        return db.Execute(sql, country) > 0;
    }

    public bool UpdateCity(City city)
    {
        const string sql = "UPDATE Cities SET Name = @Name, CountryId = @CountryId WHERE Id = @Id;";
        using var db = CreateConnection();
        return db.Execute(sql, city) > 0;
    }

    public bool UpdateSection(Section section)
    {
        const string sql = "UPDATE Sections SET Name = @Name WHERE Id = @Id;";
        using var db = CreateConnection();
        return db.Execute(sql, section) > 0;
    }

    public bool UpdatePromotionalItem(PromotionalItem item)
    {
        const string sql = """
            UPDATE PromotionalItems
            SET Name = @Name,
                DiscountPercent = @DiscountPercent,
                StartDate = @StartDate,
                EndDate = @EndDate,
                SectionId = @SectionId
            WHERE Id = @Id;
            """;
        using var db = CreateConnection();
        return db.Execute(sql, item) > 0;
    }

    // =========================================================================
    // ЧАСТИНА 1 — ЗАВДАННЯ 3: Видалення (DELETE)
    // =========================================================================

    public bool DeleteCustomer(int id)
    {
        using var db = CreateConnection();
        return db.Execute("DELETE FROM Customers WHERE Id = @Id;", new { Id = id }) > 0;
    }

    public bool DeleteCountry(int id)
    {
        using var db = CreateConnection();
        return db.Execute("DELETE FROM Countries WHERE Id = @Id;", new { Id = id }) > 0;
    }

    public bool DeleteCity(int id)
    {
        using var db = CreateConnection();
        return db.Execute("DELETE FROM Cities WHERE Id = @Id;", new { Id = id }) > 0;
    }

    public bool DeleteSection(int id)
    {
        using var db = CreateConnection();
        return db.Execute("DELETE FROM Sections WHERE Id = @Id;", new { Id = id }) > 0;
    }

    public bool DeletePromotionalItem(int id)
    {
        using var db = CreateConnection();
        return db.Execute("DELETE FROM PromotionalItems WHERE Id = @Id;", new { Id = id }) > 0;
    }

    // =========================================================================
    // ЧАСТИНА 1 — ЗАВДАННЯ 4: Спеціалізовані вибірки
    // =========================================================================

    public IEnumerable<City> GetCitiesByCountry(int countryId)
    {
        const string sql = "SELECT Id, Name, CountryId FROM Cities WHERE CountryId = @CountryId;";
        using var db = CreateConnection();
        return db.Query<City>(sql, new { CountryId = countryId });
    }

    public IEnumerable<Section> GetSectionsByCustomer(int customerId)
    {
        const string sql = """
            SELECT s.Id, s.Name
            FROM Sections s
            INNER JOIN CustomerSections cs ON s.Id = cs.SectionId
            WHERE cs.CustomerId = @CustomerId;
            """;
        using var db = CreateConnection();
        return db.Query<Section>(sql, new { CustomerId = customerId });
    }

    public IEnumerable<PromotionalItem> GetPromotionalItemsBySection(int sectionId)
    {
        const string sql = """
            SELECT Id, Name, DiscountPercent, StartDate, EndDate, SectionId
            FROM PromotionalItems
            WHERE SectionId = @SectionId;
            """;
        using var db = CreateConnection();
        return db.Query<PromotionalItem>(sql, new { SectionId = sectionId });
    }

    // =========================================================================
    // ЧАСТИНА 2 — ЗАВДАННЯ 3: Збережені процедури (Акційні товари по розділах)
    // =========================================================================

    public IEnumerable<SectionPromoStats> GetTop3SectionsByPromoCount()
    {
        using var db = CreateConnection();
        return db.Query<SectionPromoStats>("sp_GetTop3SectionsByPromoCount", commandType: CommandType.StoredProcedure);
    }

    public SectionPromoStats? GetMostPromoItemsSection()
    {
        using var db = CreateConnection();
        return db.QueryFirstOrDefault<SectionPromoStats>("sp_GetTop1SectionByPromoCount", commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<SectionPromoStats> GetBottom3SectionsByPromoCount()
    {
        using var db = CreateConnection();
        return db.Query<SectionPromoStats>("sp_GetBottom3SectionsByPromoCount", commandType: CommandType.StoredProcedure);
    }

    public SectionPromoStats? GetLeastPromoItemsSection()
    {
        using var db = CreateConnection();
        return db.QueryFirstOrDefault<SectionPromoStats>("sp_GetBottom1SectionByPromoCount", commandType: CommandType.StoredProcedure);
    }

    // =========================================================================
    // ЧАСТИНА 2 — ЗАВДАННЯ 4: Терміни акцій, архівування та середній вік
    // =========================================================================

    public IEnumerable<PromotionalItem> GetPromotionsEndingIn3Days()
    {
        using var db = CreateConnection();
        return db.Query<PromotionalItem>("sp_GetPromotionsEndingIn3Days", commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<PromotionalItem> GetExpiredPromotions()
    {
        using var db = CreateConnection();
        return db.Query<PromotionalItem>("sp_GetExpiredPromotions", commandType: CommandType.StoredProcedure);
    }

    public int ArchiveExpiredPromotions()
    {
        using var db = CreateConnection();
        return db.ExecuteScalar<int>("sp_ArchiveExpiredPromotions", commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<ArchivedPromotionalItem> GetArchivedPromotions()
    {
        using var db = CreateConnection();
        return db.Query<ArchivedPromotionalItem>(
            "SELECT Id, OriginalItemId, Name, DiscountPercent, StartDate, EndDate, SectionId, ArchivedAt FROM PromotionsArchive;");
    }

    public IEnumerable<AverageAgeStats> GetAverageAgeBySection()
    {
        using var db = CreateConnection();
        return db.Query<AverageAgeStats>("sp_GetAverageAgeBySection", commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<AverageAgeStats> GetAverageAgeByCity()
    {
        using var db = CreateConnection();
        return db.Query<AverageAgeStats>("sp_GetAverageAgeByCity", commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<AverageAgeStats> GetAverageAgeByCountry()
    {
        using var db = CreateConnection();
        return db.Query<AverageAgeStats>("sp_GetAverageAgeByCountry", commandType: CommandType.StoredProcedure);
    }

    // =========================================================================
    // ЧАСТИНА 2 — ЗАВДАННЯ 5: Статистика за статтю та країнами
    // =========================================================================

    public IEnumerable<GenderSectionStats> GetTopSectionByGender()
    {
        using var db = CreateConnection();
        return db.Query<GenderSectionStats>("sp_GetTopSectionByGender", commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<GenderSectionStats> GetTop3SectionsByGender()
    {
        using var db = CreateConnection();
        return db.Query<GenderSectionStats>("sp_GetTop3SectionsByGender", commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<GenderCountStats> GetCustomersCountByGender()
    {
        using var db = CreateConnection();
        return db.Query<GenderCountStats>("sp_GetCustomersCountByGender", commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<GenderCountStats> GetCustomersCountByGenderAndCountry()
    {
        using var db = CreateConnection();
        return db.Query<GenderCountStats>("sp_GetCustomersCountByGenderAndCountry", commandType: CommandType.StoredProcedure);
    }

    // =========================================================================
    // Допоміжні методи перегляду списків
    // =========================================================================

    public IEnumerable<Country> GetAllCountries()
    {
        using var db = CreateConnection();
        return db.Query<Country>("SELECT Id, Name FROM Countries;");
    }

    public IEnumerable<City> GetAllCities()
    {
        using var db = CreateConnection();
        return db.Query<City>("SELECT Id, Name, CountryId FROM Cities;");
    }

    public IEnumerable<Customer> GetAllCustomers()
    {
        using var db = CreateConnection();
        return db.Query<Customer>("SELECT Id, FullName, BirthDate, Gender, Email, CityId FROM Customers;");
    }

    public IEnumerable<Section> GetAllSections()
    {
        using var db = CreateConnection();
        return db.Query<Section>("SELECT Id, Name FROM Sections;");
    }

    public IEnumerable<PromotionalItem> GetAllPromotionalItems()
    {
        using var db = CreateConnection();
        return db.Query<PromotionalItem>("SELECT Id, Name, DiscountPercent, StartDate, EndDate, SectionId FROM PromotionalItems;");
    }
}