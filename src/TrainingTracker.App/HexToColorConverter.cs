using System.Globalization;

namespace TrainingTracker.App;

/// <summary>
/// Converts a hex color string (e.g. "#4CAF80") to a <see cref="Color"/>.
/// Returns <see cref="Colors.Transparent"/> for null or invalid input.
/// </summary>
public class HexToColorConverter : IValueConverter
{
    /// <summary>
    /// Converts a hex color string to a <see cref="Color"/>.
    /// </summary>
    public object? Convert(
        object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is string hex ? Color.FromArgb(hex) : Colors.Transparent;

    /// <summary>
    /// Not supported; this converter is one-way only.
    /// </summary>
    public object? ConvertBack(
        object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
