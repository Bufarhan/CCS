using MyOlxScraper.Data;
using MyOlxScraper.Dto;
using System.Text.Json;

namespace MyOlxScraper.Services
{
    public class ProductService
    {
        private List<ProductDto> _ProductDtos;

        public ProductService()
        {
            _ProductDtos = JsonStorage.Load();
        }

        public IReadOnlyList<ProductDto> GetAll() => _ProductDtos.OrderByDescending(l => l.DatePosted).ToList();

        public async Task<int> ScrapAsync()
        {
            var apiUrl = "https://www.olx.pt/api/v1/offers/?limit=40&offset=0&search%5Bdist%5D=15&location=coracaodejesus";
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; ScraperBot/1.0)");

            JsonElement json;

            try
            {
                json = await httpClient.GetFromJsonAsync<JsonElement>(apiUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to fetch or parse JSON: " + ex.Message);
                return 0;
            }
          int  scrapedItemsCount = 0;
            if (json.TryGetProperty("data", out JsonElement dataArray))
            {
                foreach (var item in dataArray.EnumerateArray())
                {
                    var id = item.GetProperty("id").GetInt64();
                    var title = item.GetProperty("title").GetString();
                    var created_time = item.GetProperty("created_time").GetString();
                    var location = item.GetProperty("location").GetProperty("city").GetProperty("name").GetString();
                    var lat = item.GetProperty("map").GetProperty("lat").GetDouble();
                    var lon = item.GetProperty("map").GetProperty("lon").GetDouble();
                    var description = item.GetProperty("description").GetString();
                    var url = item.GetProperty("url").GetString();
                    var imageUrl = item.TryGetProperty("photos", out var photos) && photos.GetArrayLength() > 0
                        ? photos[0].GetProperty("link").GetString().Replace(";s={width}x{height}", "")
                        : "";
                    decimal price = 0;
                    string currency = "";
                    if (item.TryGetProperty("params", out JsonElement paramsArray))
                    {
                        foreach (var param in paramsArray.EnumerateArray())
                        {
                            if (param.TryGetProperty("key", out JsonElement keyProp) &&
                                keyProp.GetString() == "price")
                            {
                                if (param.TryGetProperty("value", out JsonElement valueObj) &&
                                    valueObj.TryGetProperty("value", out JsonElement priceValue))
                                {
                                     price = priceValue.GetDecimal(); // or GetInt32()
                                    currency = valueObj.GetProperty("currency").GetString(); // or GetInt32()
                                    Console.WriteLine($"Price: {price}");
                                }
                            }
                        }
                    }


                    if (AddIfNew(new ProductDto
                            (
                              Id: id,
                               Title: title,
                               Price: price,
                               Currency: currency,
                                DatePosted: DateTime.Parse(created_time),
                                Location: location,
                                Url: url,
                                Lat: lat,
                                Lon: lon,
                                ImageUrl: imageUrl,
                               Description: description,
                               ScrapedAt: DateTime.Now

                            )))
                        scrapedItemsCount++;
                  
                    Console.WriteLine($"{title} | {url} | {description} | {url}");
                }
            }
            else
            {
                Console.WriteLine("No 'data' field found in the response.");
            }
            return scrapedItemsCount;

        }

        public bool AddIfNew(ProductDto ProductDto)
        {
            if (_ProductDtos.Any(l => l.Id.Equals(ProductDto.Id)))
                return false;

            _ProductDtos.Add(ProductDto);
            JsonStorage.Save(_ProductDtos);
            return true;
        }
    }
}
