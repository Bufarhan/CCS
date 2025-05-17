using MyOlxScraper.Dto;
using System.Reflection;
using System.Text.Json;

namespace MyOlxScraper.Data
{
    public static class JsonStorage
    {
        private static readonly string FilePath = "db/products.json";
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        public static List<ProductDto> Load()
        {
            if (!File.Exists(FilePath))
                return new List<ProductDto>();
            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<ProductDto>>(json) ?? new List<ProductDto>();
        }

        public static void Save(List<ProductDto> ProductDtos)
        {
            var json = JsonSerializer.Serialize(ProductDtos, JsonOptions);
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            File.WriteAllText(FilePath, json);
        }
    }

   
}
