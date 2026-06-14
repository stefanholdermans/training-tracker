using Foundation;
using TrainingTracker.Application;
using UIKit;
using UniformTypeIdentifiers;

namespace TrainingTracker.App;

/// <summary>
/// Gives access to the runner's plan file on Mac Catalyst. It both picks the
/// file (presenting the document picker) and remembers it across launches: the
/// file lives outside the sandbox, so access is granted through a
/// security-scoped bookmark, created when the runner picks the file and
/// resolved — re-acquiring access — on the next launch.
/// </summary>
/// <remarks>
/// MAUI's FilePicker.PickAsync returns null on this Mac Catalyst setup, so we
/// present the document picker ourselves with asCopy: false to obtain the
/// runner's original file rather than a container copy.
/// </remarks>
public sealed class MacCatalystPlanFile : IPlanFilePicker, IPlanLocation
{
    private static string BookmarkPath => Path.Combine(
        FileSystem.Current.AppDataDirectory, "plan-bookmark");

    private NSUrl? _scopedUrl;
    private string? _filePath;
    private bool _resolved;

    public string? FilePath
    {
        get
        {
            if (!_resolved)
            {
                _filePath = ResolveBookmark();
                _resolved = true;
            }

            return _filePath;
        }
    }

    public void Remember(string filePath)
    {
        // Picking already started access and saved the bookmark; adopt the
        // chosen file as the current one for this session.
        _filePath = filePath;
        _resolved = true;
    }

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
        return url is null ? null : Adopt(url);
    }

    // Hold the chosen file's security-scoped resource open, save a bookmark so
    // it can be reopened after a restart, and adopt it as the current file.
    private string? Adopt(NSUrl url)
    {
        if (!url.StartAccessingSecurityScopedResource())
        {
            return url.Path;
        }

        _scopedUrl?.StopAccessingSecurityScopedResource();
        _scopedUrl = url;
        SaveBookmark(url);
        return url.Path;
    }

    // Resolve the saved bookmark from a previous launch and re-acquire access.
    private string? ResolveBookmark()
    {
        if (!File.Exists(BookmarkPath))
        {
            return null;
        }

        NSData data = NSData.FromArray(File.ReadAllBytes(BookmarkPath));
        NSUrl? url = NSUrl.FromBookmarkData(
            data,
            NSUrlBookmarkResolutionOptions.WithSecurityScope,
            relativeToUrl: null,
            isStale: out bool isStale,
            error: out NSError? error);

        if (error is not null || url is null
            || !url.StartAccessingSecurityScopedResource())
        {
            return null;
        }

        _scopedUrl = url;
        if (isStale)
        {
            // The bookmark still resolved but should be refreshed for next time.
            SaveBookmark(url);
        }

        return url.Path;
    }

    private static void SaveBookmark(NSUrl url)
    {
        NSData? bookmark = url.CreateBookmarkData(
            NSUrlBookmarkCreationOptions.WithSecurityScope,
            resourceValueForKeys: null,
            relativeUrl: null,
            error: out NSError? error);

        if (error is null && bookmark is not null)
        {
            File.WriteAllBytes(BookmarkPath, bookmark.ToArray());
        }
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
