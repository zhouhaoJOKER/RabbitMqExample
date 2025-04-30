
using System.Text;
using WebApplication2.FileStorage;
using WebApplication2.FileStorageService;

namespace WebApplication2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddMemoryCache();
            builder.Services.AddTransient<Simulator>();
            builder.Services.AddSingleton<UserSessionContext>();

            builder.Services.AddSingleton<IFileStorageService, LocalFileService>();
            builder.Services.AddSingleton<IFileStorageService, MinioService>();
            builder.Services.AddSingleton<IFileStorageService, FTPService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
