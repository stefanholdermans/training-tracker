using TrainingTracker.Presentation;

namespace TrainingTracker.App;

/// <summary>
/// The page that displays the training plan calendar.
/// </summary>
public partial class TrainingPlanPage : ContentPage
{
    public TrainingPlanPage(TrainingPlanViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
