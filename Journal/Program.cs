using Swashbuckle.AspNetCore.Swagger;
using Journal.DAL;
using Journal.DAL.Interfaces;
using Journal.DAL.Repositories;
using Journal.Service.Implementations;
using Journal.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Journal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var connection = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection));

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IMTAccountRepository, MTAccountRepository>();
            builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
            builder.Services.AddScoped<IMTDataRepository, MTDataRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IDealService, DealService>();
            builder.Services.AddScoped<IMTAccountService, MTAccountService>();
            builder.Services.AddScoped<IDealRepository, DealRepository>();
            builder.Services.AddScoped<ICTraderAccountRepository, CTraderAccountRepository>();
            builder.Services.AddScoped<ICTraderApiRepository, CTraderApiRepository>();
            builder.Services.AddScoped<ICTraderAccountService, CTraderAccountService>();
            builder.Services.AddScoped<IDXTradeAccountService, DXTradeAccountService>();
            builder.Services.AddScoped<IDXTradeAccountRepository, DXTradeAccountRepository>();
            builder.Services.AddScoped<IDXTradeDataRepository, DXTradeDataRepository>();
            builder.Services.AddScoped<INoteRepository, NoteRepository>();
            builder.Services.AddScoped<INoteService, NoteService>();
            builder.Services.AddScoped<IDescriptionRepository, DescriptionRepository>();
            builder.Services.AddScoped<ITradeLockerAPIRepository, TradeLockerAPIRepository>();
            builder.Services.AddScoped<ITradeLockerAccountRepository, TradeLockerAccountRepository>();
            builder.Services.AddScoped<ITradeLockerAccountService, TradeLockerAccountService>();
            builder.Services.AddScoped<IMatchTradeAccountRepository, MatchTradeAccountRepository>();
            builder.Services.AddScoped<IMatchTradeApiRepository, MatchTradeApiRepository>();
            builder.Services.AddScoped<IMatchTradeAccountService, MatchTradeAccountService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
