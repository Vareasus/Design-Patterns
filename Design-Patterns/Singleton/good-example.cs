// Singleton Design Pattern - Creational Category
// Source: salihcantekin/youtube_DesignPatterns_Builder

/* *** Pros ***
    - Ensure single instance (can be use for memory caching)
    - Globally Access
    - Created only when requested
*/

/* *** Cons ***
    - Difficult to UnitTest (mocking)
    - In multithread word, threads cannot create its own instance
*/

public class CountryProvider
{
    private static CountryProvider instance = null;
    public static CountryProvider Instance => instance ?? (instance = new CountryProvider());

    private List<Country> Countries { get; set; }

    private CountryProvider()
    {
        // Retrieving data from db (simulating delay)
        Task.Delay(2000).GetAwaiter().GetResult();

        Countries = new List<Country>()
        {
            new Country(){ Name = "Türkiye" },
            new Country(){ Name = "Birleşik Krallık" }
        };
    }

    public int CountryCount => Countries.Count;

    public async Task<List<Country>> GetCountries() => Countries;
}

public class Country
{
    public string Name { get; set; }
}

// === Usage ===
// Console.WriteLine(DateTime.Now.ToLongTimeString());
// var countries = await CountryProvider.Instance.GetCountries();
// var countries1 = await CountryProvider.Instance.GetCountries(); // Same instance!
// Console.WriteLine(CountryProvider.Instance.CountryCount);
// Console.WriteLine(DateTime.Now.ToLongTimeString());
//
// İlk çağrıda oluşur (2 sn bekler), ikinci çağrıda aynı instance gelir → 0 sn.
