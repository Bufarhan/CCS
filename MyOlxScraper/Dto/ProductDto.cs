namespace MyOlxScraper.Dto
{
    public record ProductDto(
    long Id,
    string Title,
    decimal Price,
    string Currency,
    DateTime DatePosted,
    string Location,
    string Url,
    double Lat,
    double Lon,
    string ImageUrl,
    string Description,
    DateTime ScrapedAt
);

}
