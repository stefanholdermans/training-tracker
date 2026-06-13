using UIKit;
using UniformTypeIdentifiers;

namespace TrainingTracker.App;

/// <summary>
/// Picks a file by driving UIDocumentPickerViewController directly. MAUI's
/// FilePicker.PickAsync returns null on this Mac Catalyst setup, so we present
/// the document picker ourselves.
/// </summary>
/// <remarks>
/// asCopy: true makes the system place a readable copy of the chosen file in
/// the app container, so we can read it without security-scoped access.
/// </remarks>
public sealed class MacCatalystPlanFilePicker : IPlanFilePicker
{
    public async Task<string?> PickAsync()
    {
        var presenter = TopViewController();
        if (presenter is null)
        {
            return null;
        }

        var completion = new TaskCompletionSource<string?>();

        // public.data matches any file, so the runner's JSON is selectable.
        UTType[] contentTypes = [UTTypes.Data];
        using var picker =
            new UIDocumentPickerViewController(contentTypes, asCopy: true)
            {
                AllowsMultipleSelection = false
            };

        picker.DidPickDocumentAtUrls += (_, args) =>
            completion.TrySetResult(args.Urls?.FirstOrDefault()?.Path);
        picker.WasCancelled += (_, _) => completion.TrySetResult(null);

        await presenter.PresentViewControllerAsync(picker, animated: true)
            .ConfigureAwait(true);
        return await completion.Task.ConfigureAwait(true);
    }

    private static UIViewController? TopViewController()
    {
        var scenes = UIApplication.SharedApplication.ConnectedScenes.ToArray();
        var windowScene =
            scenes.OfType<UIWindowScene>().FirstOrDefault(
                s => s.ActivationState == UISceneActivationState.ForegroundActive)
            ?? scenes.OfType<UIWindowScene>().FirstOrDefault();

        var window =
            windowScene?.Windows.FirstOrDefault(w => w.IsKeyWindow)
            ?? windowScene?.Windows.FirstOrDefault();

        var controller = window?.RootViewController;
        while (controller?.PresentedViewController is not null)
        {
            controller = controller.PresentedViewController;
        }

        return controller;
    }
}
