namespace MyOlxScraper.Services
{


    public class ScraperSchedulerService : BackgroundService
    {
        const int SCRAPING_PERIOD_MINUTES = 5;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(SCRAPING_PERIOD_MINUTES);

        public ScraperSchedulerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var scraper = scope.ServiceProvider.GetRequiredService<ProductService>();
                         await scraper.ScrapAsync();  
                        Console.WriteLine($"[ScraperScheduler] Run at: {DateTime.Now}");

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ScraperScheduler] Error: {ex.Message}");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
