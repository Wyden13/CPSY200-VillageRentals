using Microsoft.Extensions.Logging;
using VillageRentalManagementSystem.Services;

namespace VillageRentalManagementSystem
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<CustomerService>();
            builder.Services.AddSingleton<CategoryService>();
            builder.Services.AddSingleton<EquipmentService>();
            builder.Services.AddSingleton<RentalService>();
            builder.Services.AddSingleton<ReportService>();

            return builder.Build();
        }
    }
}
