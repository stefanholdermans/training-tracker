using Foundation;
using UIKit;
using UniformTypeIdentifiers;

namespace TrainingTracker.App;

/// <summary>
/// Picks a file by driving UIDocumentPickerViewController directly. MAUI's
/// FilePicker.PickAsync returns null on this Mac Catalyst setup, so we present
/// the document picker ourselves.
/// </summary>
/// <remarks>
/// asCopy: false returns the runner's original file, not a container copy, so
/// completed runs can be written back to it. The original lives outside the
/// sandbox, so we hold its security-scoped resource open for the lifetime of
/// the picker (a singleton) — both loading the plan and saving completions then
/// reach it through ordinary file access.
/// </remarks>
public sealed class MacCatalystPlanFilePicker : IPlanFilePicker
{
    private NSUrl? _scopedUrl;

    public async Task<string?> PickAsync()
    {
        var presenter = TopViewController();
        if (presenter is null)
        {
            return null;
        }

        var completion = new TaskCompletionSource<NSUrl?>();

        // public.data matches any file, so the runner's JSON is selectable.
        UTType[] contentTypes = [UTTypes.Data];
        using var picker =
            new UIDocumentPickerViewController(contentTypes, asCopy: false)
            {
                AllowsMultipleSelection = false
            };

        picker.DidPickDocumentAtUrls += (_, args) =>
            completion.TrySetResult(args.Urls?.FirstOrDefault());
        picker.WasCancelled += (_, _) => completion.TrySetResult(null);

        await presenter.PresentViewControllerAsync(picker, animated: true)
            .ConfigureAwait(true);

        NSUrl? url = await completion.Task.ConfigureAwait(true);
        return url is null ? null : RetainAccess(url);
    }

    // Hold the chosen file's security-scoped resource open so later reads and
    // writes are permitted, releasing any previously held one.
    private string? RetainAccess(NSUrl url)
    {
        if (url.StartAccessingSecurityScopedResource())
        {
            _scopedUrl?.StopAccessingSecurityScopedResource();
            _scopedUrl = url;
        }

        return url.Path;
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
