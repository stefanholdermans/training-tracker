using TrainingTracker.Presentation;

namespace TrainingTracker.App;

public partial class TrainingPlanPage : ContentPage
{
    public TrainingPlanPage(TrainingPlanViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
