using System.Globalization;

namespace TrainingTracker.App;

/// <summary>
/// Converts a relative intensity value (0.0–1.0) to a colour by linearly interpolating
/// between a cool blue (low intensity) and a warm amber (peak intensity).
/// Adapts to the current app theme. Returns <see cref="Colors.Transparent"/> for null input.
/// </summary>
public class RelativeIntensityToColorConverter : IValueConverter
{
    private static readonly Color LightLow  = Color.FromArgb("#4080C0");
    private static readonly Color LightPeak = Color.FromArgb("#E07820");
    private static readonly Color DarkLow   = Color.FromArgb("#2060A0");
    private static readonly Color DarkPeak  = Color.FromArgb("#C06010");

    /// <summary>
    /// Converts a relative intensity value to an interpolated colour.
    /// </summary>
    public object? Convert(
        object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not decimal intensity)
            return Colors.Transparent;

        bool isDark = Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark;
        Color low  = isDark ? DarkLow  : LightLow;
        Color peak = isDark ? DarkPeak : LightPeak;

        float t = (float)intensity;
        return new Color(
            low.Red   + (peak.Red   - low.Red)   * t,
            low.Green + (peak.Green - low.Green) * t,
            low.Blue  + (peak.Blue  - low.Blue)  * t);
    }

    /// <summary>
    /// Not supported; this converter is one-way only.
    /// </summary>
    public object? ConvertBack(
        object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
