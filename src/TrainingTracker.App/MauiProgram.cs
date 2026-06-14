using Microsoft.Extensions.Logging;
using TrainingTracker.Application;
using TrainingTracker.Infrastructure;
using TrainingTracker.Presentation;

namespace TrainingTracker.App;

public static class MauiProgram
{
    /// <summary>
    /// Builds and returns the configured MAUI application.
    /// </summary>
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // The app starts with a clean slate: no plan is seeded. The repository
        // reads and writes the runner's own file, located through IPlanLocation,
        // which is empty until they pick a plan and persists across restarts.
        builder.Services.AddSingleton<ITrainingPlanRepository>(
            sp => new JsonTrainingPlanRepository(
                sp.GetRequiredService<IPlanLocation>()));
        builder.Services.AddSingleton<IClock, SystemClock>();
        builder.Services
            .AddSingleton<IGetTrainingPlanQuery, GetTrainingPlanQuery>();
        builder.Services
            .AddSingleton<ILoadTrainingPlanCommand, LoadTrainingPlanCommand>();
        builder.Services
            .AddSingleton<IMarkSessionCompletedCommand,
                MarkSessionCompletedCommand>();
        builder.Services
            .AddSingleton<IMarkSessionUncompletedCommand,
                MarkSessionUncompletedCommand>();
#if MACCATALYST
        // One object both picks the runner's file and remembers it across
        // launches, so it serves as picker and location alike.
        builder.Services.AddSingleton<MacCatalystPlanFile>();
        builder.Services.AddSingleton<IPlanFilePicker>(
            sp => sp.GetRequiredService<MacCatalystPlanFile>());
        builder.Services.AddSingleton<IPlanLocation>(
            sp => sp.GetRequiredService<MacCatalystPlanFile>());
#endif
        builder.Services.AddSingleton<TrainingPlanViewModel>();
        builder.Services.AddSingleton<TrainingPlanPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
