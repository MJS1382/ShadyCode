using Microsoft.EntityFrameworkCore;
using PersonalBlog.Data;
using PersonalBlog.Services.Redis;

namespace PersonalBlog.BackgroundServices;

public sealed class CacheToDbService(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private const int INTERVAL_MINUTS = 5;
    private const int TAKE_AMOUNT = 20;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //every n minuts update post views in database with view count from hyperloglog store
        while(!stoppingToken.IsCancellationRequested)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var hyperLogLogStore = scope.ServiceProvider.GetRequiredService<IHyperLogLogStore>();

            int count = await dbContext.Posts.CountAsync(cancellationToken: stoppingToken);
            for (int i = 0; i < count; i+=TAKE_AMOUNT)
            {
                var posts = await dbContext.Posts
                    .OrderBy(p => p.Id)
                    .Skip(i)
                    .Take(TAKE_AMOUNT)
                    .ToListAsync(cancellationToken: stoppingToken);

                foreach (var post in posts)
                {
                    string key = $"Post:{post.Id}:views";
                    long newViewCount = await hyperLogLogStore.GetUniqueItemsCountAsync(key);

                    if(newViewCount > post.ViewCount)
                        post.ViewCount = newViewCount;
                }

                await dbContext.SaveChangesAsync(stoppingToken);
                dbContext.ChangeTracker.Clear();
                await Task.Delay(1000, stoppingToken);
            }

            //I eat too much. Im getting fat. I have to go on a diet.
            await Task.Delay(TimeSpan.FromMinutes(INTERVAL_MINUTS), stoppingToken);
        }
    }
}