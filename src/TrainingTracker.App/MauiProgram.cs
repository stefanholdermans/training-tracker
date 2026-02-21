using Microsoft.Extensions.Logging;
using TrainingTracker.Application;
using TrainingTracker.Infrastructure;
using TrainingTracker.Presentation;

namespace TrainingTracker.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var trainingPlanPath = ExtractTrainingPlan();

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
        builder.Services.AddSingleton<IGetTrainingPlanQuery, GetTrainingPlanQuery>();
        builder.Services.AddSingleton<TrainingPlanViewModel>();
        builder.Services.AddSingleton<TrainingPlanPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    /// <summary>
    /// Copies the bundled training-plan.json asset to the app data directory
    /// on first launch and returns the file path for subsequent use.
    /// </summary>
    private static string ExtractTrainingPlan()
    {
        var filePath = Path.Combine(
            FileSystem.Current.AppDataDirectory,
            "training-plan.json");

        if (File.Exists(filePath))
            return filePath;

        using Stream source = FileSystem.Current
            .OpenAppPackageFileAsync("training-plan.json")
            .GetAwaiter()
            .GetResult();
        using FileStream destination = File.Create(filePath);
        source.CopyTo(destination);

        return filePath;
    }
}
