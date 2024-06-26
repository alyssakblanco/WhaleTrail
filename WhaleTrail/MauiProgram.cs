﻿using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.LifecycleEvents;
using WhaleTrail.Data;

namespace WhaleTrail
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseMauiMaps();

            builder.ConfigureLifecycleEvents(lifecycle =>
            {
#if ANDROID
            lifecycle.AddAndroid(android => android
                .OnCreate((activity, savedInstanceState) =>
                {
                    var window = activity.Window;
                    window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#193441")); 
                }));
#endif
            });

            // dependency injection
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "sightings.db");
            builder.Services.AddSingleton(s => ActivatorUtilities.CreateInstance<SightingsRepo>(s, dbPath));

            string dbPath2 = Path.Combine(FileSystem.AppDataDirectory, "whaleinfo.db");
            builder.Services.AddSingleton(s => ActivatorUtilities.CreateInstance<WhaleInfoRepo>(s, dbPath2));

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
