using TrainingTracker.Presentation;

namespace TrainingTracker.App;

/// <summary>
/// The page that displays the training plan calendar.
/// </summary>
public partial class TrainingPlanPage : ContentPage
{
    private readonly TrainingPlanViewModel _viewModel;
    private readonly IPlanFilePicker _filePicker;

    public TrainingPlanPage(
        TrainingPlanViewModel viewModel, IPlanFilePicker filePicker)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _filePicker = filePicker;
        BindingContext = viewModel;
    }

    /// <summary>
    /// Lets the runner pick a training plan JSON file and load it in place
    /// of the current one.
    /// </summary>
    private async void OnLoadPlanClicked(object? sender, EventArgs e)
    {
        try
        {
            var path = await _filePicker.PickAsync().ConfigureAwait(true);
            if (path is null)
            {
                return; // The runner cancelled the picker.
            }

            _viewModel.LoadPlan(path);
        }
#pragma warning disable CA1031 // Surface any failure to the runner at this UI boundary.
        catch (Exception ex)
#pragma warning restore CA1031
        {
            await DisplayAlertAsync("Couldn't load plan", ex.Message, "OK")
                .ConfigureAwait(true);
        }
    }

    /// <summary>
    /// Toggles the tapped day's run between completed and uncompleted, so a
    /// check-off made by mistake can be undone with another tap.
    /// </summary>
    private void OnDayTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is not DayViewModel { IsRestDay: false } day)
        {
            return;
        }

        if (day.IsCompleted)
        {
            _viewModel.MarkUncompleted(day.Date);
        }
        else
        {
            _viewModel.MarkCompleted(day.Date);
        }
    }
}
