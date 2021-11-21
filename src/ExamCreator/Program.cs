using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ExamCreator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    public enum CacheKeys
    {
        Last5Posts
    }
}
