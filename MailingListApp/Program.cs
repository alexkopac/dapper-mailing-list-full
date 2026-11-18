using System.Text;
using MailingListApp.Models;
using MailingListApp.Repositories;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

const string connectionString =
    @"Server=DESKTOP-A5B95S7;Database=MailingListDb;Trusted_Connection=True;TrustServerCertificate=True;";

var repo = new MailingListRepository(connectionString);

while (true)
{
    Console.WriteLine("\n================ СПИСОК РОЗСИЛКИ (DAPPER) ================");
    Console.WriteLine("--- Частина 1: CRUD та базові вибірки ---");
    Console.WriteLine(" 1. Додати покупця            2. Додати країну");
    Console.WriteLine(" 3. Додати місто              4. Додати розділ");
    Console.WriteLine(" 5. Додати акційний товар     6. Підписати покупця на розділ");
    Console.WriteLine(" 7. Оновити покупця           8. Оновити країну");
    Console.WriteLine(" 9. Оновити місто            10. Оновити розділ");
    Console.WriteLine("11. Оновити акційний товар");
    Console.WriteLine("12. Видалити покупця         13. Видалити країну");
    Console.WriteLine("14. Видалити місто           15. Видалити розділ");
    Console.WriteLine("16. Видалити акційний товар");
    Console.WriteLine("17. Міста певної країни      18. Розділи певного покупця");
    Console.WriteLine("19. Акційні товари певного розділу");
    Console.WriteLine("--- Частина 2 (Завдання 3): Товари по розділах ---");
    Console.WriteLine("20. Топ-3 розділів за кількістю акційних товарів");
    Console.WriteLine("21. Розділ з найбільшою кількістю акційних товарів");
    Console.WriteLine("22. Топ-3 розділів з найменшою кількістю акційних товарів");
    Console.WriteLine("23. Розділ з найменшою кількістю акційних товарів");
    Console.WriteLine("--- Частина 2 (Завдання 4): Терміни акцій та середній вік ---");
    Console.WriteLine("24. Акційні товари, до кінця яких залишилось 3 дні");
    Console.WriteLine("25. Акційні товари, у яких закінчився термін дії");
    Console.WriteLine("26. Перенести прострочені товари до таблиці «Архів акцій»");
    Console.WriteLine("27. Переглянути «Архів акцій»");
    Console.WriteLine("28. Середній вік покупців по кожному розділу");
    Console.WriteLine("29. Середній вік покупця по кожному місту");
    Console.WriteLine("30. Середній вік покупця по кожній країні");
    Console.WriteLine("--- Частина 2 (Завдання 5): Статистика за статтю ---");
    Console.WriteLine("31. Найпопулярніший розділ для кожної статі");
    Console.WriteLine("32. Топ-3 розділи для кожної статі");
    Console.WriteLine("33. Кількість покупців кожної статі");
    Console.WriteLine("34. Кількість покупців кожної статі з кожної країни");
    Console.WriteLine(" 0. Вихід");
    Console.Write("Оберіть дію: ");

    var choice = Console.ReadLine();
    Console.WriteLine();

    try
    {
        switch (choice)
        {
            case "1":
                PrintCities(repo);
                Console.Write("ПІБ покупця: ");
                var fullName = Console.ReadLine() ?? "";
                Console.Write("Дата народження (рррр-мм-дд): ");
                var birthDate = DateTime.Parse(Console.ReadLine() ?? "2000-01-01");
                Console.Write("Стать: ");
                var gender = Console.ReadLine() ?? "";
                Console.Write("Email: ");
                var email = Console.ReadLine() ?? "";
                Console.Write("Id міста: ");
                var cityId = int.Parse(Console.ReadLine() ?? "1");

                var newCustId = repo.InsertCustomer(new Customer
                {
                    FullName = fullName,
                    BirthDate = birthDate,
                    Gender = gender,
                    Email = email,
                    CityId = cityId
                });
                Console.WriteLine($"Покупця додано з Id = {newCustId}");
                break;

            case "2":
                Console.Write("Назва країни: ");
                var countryId = repo.InsertCountry(new Country { Name = Console.ReadLine() ?? "" });
                Console.WriteLine($"Країну додано з Id = {countryId}");
                break;

            case "3":
                PrintCountries(repo);
                Console.Write("Назва міста: ");
                var cityName = Console.ReadLine() ?? "";
                Console.Write("Id країни: ");
                var cId = int.Parse(Console.ReadLine() ?? "1");
                var newCityId = repo.InsertCity(new City { Name = cityName, CountryId = cId });
                Console.WriteLine($"Місто додано з Id = {newCityId}");
                break;

            case "4":
                Console.Write("Назва розділу: ");
                var secId = repo.InsertSection(new Section { Name = Console.ReadLine() ?? "" });
                Console.WriteLine($"Розділ додано з Id = {secId}");
                break;

            case "5":
                PrintSections(repo);
                Console.Write("Назва акційного товару: ");
                var itemName = Console.ReadLine() ?? "";
                Console.Write("Відсоток знижки: ");
                var discount = decimal.Parse(Console.ReadLine() ?? "10");
                Console.Write("Дата початку (рррр-мм-дд): ");
                var start = DateTime.Parse(Console.ReadLine() ?? DateTime.Today.ToString("yyyy-MM-dd"));
                Console.Write("Дата завершення (рррр-мм-дд): ");
                var end = DateTime.Parse(Console.ReadLine() ?? DateTime.Today.AddDays(14).ToString("yyyy-MM-dd"));
                Console.Write("Id розділу: ");
                var itemSecId = int.Parse(Console.ReadLine() ?? "1");

                var newItemId = repo.InsertPromotionalItem(new PromotionalItem
                {
                    Name = itemName,
                    DiscountPercent = discount,
                    StartDate = start,
                    EndDate = end,
                    SectionId = itemSecId
                });
                Console.WriteLine($"Акційний товар додано з Id = {newItemId}");
                break;

            case "6":
                PrintCustomers(repo);
                PrintSections(repo);
                Console.Write("Id покупця: ");
                var subCustId = int.Parse(Console.ReadLine() ?? "1");
                Console.Write("Id розділу: ");
                var subSecId = int.Parse(Console.ReadLine() ?? "1");
                repo.SubscribeCustomerToSection(subCustId, subSecId);
                Console.WriteLine("Підписку оформлено.");
                break;

            case "7":
                PrintCustomers(repo);
                Console.Write("Id покупця для оновлення: ");
                var updCustId = int.Parse(Console.ReadLine() ?? "0");
                PrintCities(repo);
                Console.Write("Нове ПІБ: ");
                var updName = Console.ReadLine() ?? "";
                Console.Write("Нова дата народження (рррр-мм-дд): ");
                var updBirth = DateTime.Parse(Console.ReadLine() ?? "2000-01-01");
                Console.Write("Нова стать: ");
                var updGender = Console.ReadLine() ?? "";
                Console.Write("Новий Email: ");
                var updEmail = Console.ReadLine() ?? "";
                Console.Write("Новий Id міста: ");
                var updCityId = int.Parse(Console.ReadLine() ?? "1");

                Console.WriteLine(repo.UpdateCustomer(new Customer
                {
                    Id = updCustId,
                    FullName = updName,
                    BirthDate = updBirth,
                    Gender = updGender,
                    Email = updEmail,
                    CityId = updCityId
                }) ? "Дані покупця оновлено." : "Покупця не знайдено.");
                break;

            case "8":
                PrintCountries(repo);
                Console.Write("Id країни для оновлення: ");
                var updCountryId = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("Нова назва країни: ");
                var updCountryName = Console.ReadLine() ?? "";
                Console.WriteLine(repo.UpdateCountry(new Country { Id = updCountryId, Name = updCountryName })
                    ? "Країну оновлено." : "Країну не знайдено.");
                break;

            case "9":
                PrintCities(repo);
                PrintCountries(repo);
                Console.Write("Id міста для оновлення: ");
                var updCityKey = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("Нова назва міста: ");
                var updCityName = Console.ReadLine() ?? "";
                Console.Write("Новий Id країни: ");
                var updCityCountryId = int.Parse(Console.ReadLine() ?? "1");
                Console.WriteLine(repo.UpdateCity(new City { Id = updCityKey, Name = updCityName, CountryId = updCityCountryId })
                    ? "Місто оновлено." : "Місто не знайдено.");
                break;

            case "10":
                PrintSections(repo);
                Console.Write("Id розділу для оновлення: ");
                var updSecId = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("Нова назва розділу: ");
                var updSecName = Console.ReadLine() ?? "";
                Console.WriteLine(repo.UpdateSection(new Section { Id = updSecId, Name = updSecName })
                    ? "Розділ оновлено." : "Розділ не знайдено.");
                break;

            case "11":
                PrintItems(repo);
                PrintSections(repo);
                Console.Write("Id товару для оновлення: ");
                var updItemId = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("Нова назва товару: ");
                var updItemName = Console.ReadLine() ?? "";
                Console.Write("Новий відсоток знижки: ");
                var updDiscount = decimal.Parse(Console.ReadLine() ?? "10");
                Console.Write("Нова дата початку (рррр-мм-дд): ");
                var updStart = DateTime.Parse(Console.ReadLine() ?? DateTime.Today.ToString("yyyy-MM-dd"));
                Console.Write("Нова дата завершення (рррр-мм-дд): ");
                var updEnd = DateTime.Parse(Console.ReadLine() ?? DateTime.Today.AddDays(14).ToString("yyyy-MM-dd"));
                Console.Write("Новий Id розділу: ");
                var updItemSecId = int.Parse(Console.ReadLine() ?? "1");

                Console.WriteLine(repo.UpdatePromotionalItem(new PromotionalItem
                {
                    Id = updItemId,
                    Name = updItemName,
                    DiscountPercent = updDiscount,
                    StartDate = updStart,
                    EndDate = updEnd,
                    SectionId = updItemSecId
                }) ? "Товар оновлено." : "Товар не знайдено.");
                break;

            case "12":
                PrintCustomers(repo);
                Console.Write("Id покупця для видалення: ");
                Console.WriteLine(repo.DeleteCustomer(int.Parse(Console.ReadLine() ?? "0"))
                    ? "Покупця видалено." : "Покупця не знайдено.");
                break;

            case "13":
                PrintCountries(repo);
                Console.Write("Id країни для видалення: ");
                Console.WriteLine(repo.DeleteCountry(int.Parse(Console.ReadLine() ?? "0"))
                    ? "Країну видалено." : "Країну не знайдено.");
                break;

            case "14":
                PrintCities(repo);
                Console.Write("Id міста для видалення: ");
                Console.WriteLine(repo.DeleteCity(int.Parse(Console.ReadLine() ?? "0"))
                    ? "Місто видалено." : "Місто не знайдено.");
                break;

            case "15":
                PrintSections(repo);
                Console.Write("Id розділу для видалення: ");
                Console.WriteLine(repo.DeleteSection(int.Parse(Console.ReadLine() ?? "0"))
                    ? "Розділ видалено." : "Розділ не знайдено.");
                break;

            case "16":
                PrintItems(repo);
                Console.Write("Id акційного товару для видалення: ");
                Console.WriteLine(repo.DeletePromotionalItem(int.Parse(Console.ReadLine() ?? "0"))
                    ? "Товар видалено." : "Товар не знайдено.");
                break;

            case "17":
                PrintCountries(repo);
                Console.Write("Введіть Id країни: ");
                var filterCountryId = int.Parse(Console.ReadLine() ?? "0");
                Console.WriteLine("Міста обраної країни:");
                foreach (var city in repo.GetCitiesByCountry(filterCountryId))
                    Console.WriteLine($"  [{city.Id}] {city.Name}");
                break;

            case "18":
                PrintCustomers(repo);
                Console.Write("Введіть Id покупця: ");
                var filterCustomerId = int.Parse(Console.ReadLine() ?? "0");
                Console.WriteLine("Розділи обраного покупця:");
                foreach (var sec in repo.GetSectionsByCustomer(filterCustomerId))
                    Console.WriteLine($"  [{sec.Id}] {sec.Name}");
                break;

            case "19":
                PrintSections(repo);
                Console.Write("Введіть Id розділу: ");
                var filterSectionId = int.Parse(Console.ReadLine() ?? "0");
                Console.WriteLine("Акційні товари обраного розділу:");
                foreach (var item in repo.GetPromotionalItemsBySection(filterSectionId))
                    Console.WriteLine($"  [{item.Id}] {item.Name} — знижка {item.DiscountPercent}% (до {item.EndDate:yyyy-MM-dd})");
                break;

            case "20":
                Console.WriteLine("Топ-3 розділів за кількістю акційних товарів:");
                foreach (var stat in repo.GetTop3SectionsByPromoCount())
                    Console.WriteLine($"  [{stat.SectionId}] {stat.SectionName} — {stat.PromoItemsCount} шт.");
                break;

            case "21":
                var maxSec = repo.GetMostPromoItemsSection();
                if (maxSec != null)
                    Console.WriteLine($"Найбільше акційних товарів: [{maxSec.SectionId}] {maxSec.SectionName} — {maxSec.PromoItemsCount} шт.");
                break;

            case "22":
                Console.WriteLine("Топ-3 розділів з найменшою кількістю акційних товарів:");
                foreach (var stat in repo.GetBottom3SectionsByPromoCount())
                    Console.WriteLine($"  [{stat.SectionId}] {stat.SectionName} — {stat.PromoItemsCount} шт.");
                break;

            case "23":
                var minSec = repo.GetLeastPromoItemsSection();
                if (minSec != null)
                    Console.WriteLine($"Найменше акційних товарів: [{minSec.SectionId}] {minSec.SectionName} — {minSec.PromoItemsCount} шт.");
                break;

            case "24":
                Console.WriteLine("Акційні товари, до кінця яких залишилось 3 дні:");
                var endingSoon = repo.GetPromotionsEndingIn3Days().ToList();
                if (endingSoon.Count == 0) Console.WriteLine("  Немає товарів з терміном закінчення через 3 дні.");
                foreach (var item in endingSoon)
                    Console.WriteLine($"  [{item.Id}] {item.Name} — знижка {item.DiscountPercent}% (до {item.EndDate:yyyy-MM-dd})");
                break;

            case "25":
                Console.WriteLine("Акційні товари з вичерпаним терміном дії:");
                var expired = repo.GetExpiredPromotions().ToList();
                if (expired.Count == 0) Console.WriteLine("  Прострочених товарів немає.");
                foreach (var item in expired)
                    Console.WriteLine($"  [{item.Id}] {item.Name} — закінчилась {item.EndDate:yyyy-MM-dd}");
                break;

            case "26":
                var moved = repo.ArchiveExpiredPromotions();
                Console.WriteLine($"Перенесено товарів до таблиці «Архів акцій»: {moved}");
                break;

            case "27":
                Console.WriteLine("Вміст таблиці «Архів акцій»:");
                var archived = repo.GetArchivedPromotions().ToList();
                if (archived.Count == 0) Console.WriteLine("  Архів порожній.");
                foreach (var a in archived)
                    Console.WriteLine($"  [Архів #{a.Id} | Оригінал #{a.OriginalItemId}] {a.Name} (-{a.DiscountPercent}%) | Завершена: {a.EndDate:yyyy-MM-dd} | Архівовано: {a.ArchivedAt:yyyy-MM-dd HH:mm}");
                break;

            case "28":
                Console.WriteLine("Середній вік покупців по кожному розділу:");
                foreach (var stat in repo.GetAverageAgeBySection())
                    Console.WriteLine($"  {stat.EntityName}: {stat.AverageAge} р.");
                break;

            case "29":
                Console.WriteLine("Середній вік покупця по кожному місту:");
                foreach (var stat in repo.GetAverageAgeByCity())
                    Console.WriteLine($"  {stat.EntityName}: {stat.AverageAge} р.");
                break;

            case "30":
                Console.WriteLine("Середній вік покупця по кожній країні:");
                foreach (var stat in repo.GetAverageAgeByCountry())
                    Console.WriteLine($"  {stat.EntityName}: {stat.AverageAge} р.");
                break;

            case "31":
                Console.WriteLine("Найпопулярніший розділ для кожної статі:");
                foreach (var stat in repo.GetTopSectionByGender())
                    Console.WriteLine($"  Стать: {stat.Gender} -> [{stat.SectionId}] {stat.SectionName} (підписників: {stat.SubscribersCount})");
                break;

            case "32":
                Console.WriteLine("Топ-3 розділи для кожної статі:");
                foreach (var stat in repo.GetTop3SectionsByGender())
                    Console.WriteLine($"  Стать: {stat.Gender} -> [{stat.SectionId}] {stat.SectionName} (підписників: {stat.SubscribersCount})");
                break;

            case "33":
                Console.WriteLine("Кількість покупців кожної статі:");
                foreach (var stat in repo.GetCustomersCountByGender())
                    Console.WriteLine($"  {stat.Gender}: {stat.CustomersCount} осіб");
                break;

            case "34":
                Console.WriteLine("Кількість покупців кожної статі з кожної країни:");
                foreach (var stat in repo.GetCustomersCountByGenderAndCountry())
                    Console.WriteLine($"  {stat.CountryName} | {stat.Gender}: {stat.CustomersCount} осіб");
                break;

            case "0":
                return;

            default:
                Console.WriteLine("Невідома команда.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Помилка: {ex.Message}");
    }
}

static void PrintCountries(MailingListRepository repo)
{
    Console.WriteLine("Країни: " + string.Join(", ", repo.GetAllCountries().Select(c => $"[{c.Id}] {c.Name}")));
}

static void PrintCities(MailingListRepository repo)
{
    Console.WriteLine("Міста: " + string.Join(", ", repo.GetAllCities().Select(c => $"[{c.Id}] {c.Name} (CountryId={c.CountryId})")));
}

static void PrintCustomers(MailingListRepository repo)
{
    Console.WriteLine("Покупці: " + string.Join(", ", repo.GetAllCustomers().Select(c => $"[{c.Id}] {c.FullName} ({c.Email})")));
}

static void PrintSections(MailingListRepository repo)
{
    Console.WriteLine("Розділи: " + string.Join(", ", repo.GetAllSections().Select(s => $"[{s.Id}] {s.Name}")));
}

static void PrintItems(MailingListRepository repo)
{
    Console.WriteLine("Акційні товари: " + string.Join(", ", repo.GetAllPromotionalItems().Select(i => $"[{i.Id}] {i.Name} (-{i.DiscountPercent}%)")));
}
