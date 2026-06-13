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
        // The app starts with a clean slate: no plan is seeded. The store
        // reads this path, which does not exist until the runner loads their
        // own plan — at which point Load writes it here and it persists.
        var trainingPlanPath = Path.Combine(
            FileSystem.Current.AppDataDirectory, "training-plan.json");

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<ITrainingPlanRepository>(
            _ => new JsonTrainingPlanRepository(trainingPlanPath));
        builder.Services
            .AddSingleton<IGetTrainingPlanQuery, GetTrainingPlanQuery>();
        builder.Services
            .AddSingleton<ILoadTrainingPlanCommand, LoadTrainingPlanCommand>();
#if MACCATALYST
        builder.Services.AddSingleton<IPlanFilePicker, MacCatalystPlanFilePicker>();
#endif
        builder.Services.AddSingleton<TrainingPlanViewModel>();
        builder.Services.AddSingleton<TrainingPlanPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
