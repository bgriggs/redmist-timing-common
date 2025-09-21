using RedMist.TimingCommon.Models;

namespace RedMist.TimingCommon.Extensions;

public static class CarPositionExtensions
{
    /// <summary>
    /// Creates a deep copy of a CarPosition using the auto-generated CarPositionMapper.
    /// If the mapper is not available, falls back to manual copying.
    /// </summary>
    /// <param name="original">The CarPosition to copy</param>
    /// <returns>A deep copy of the CarPosition</returns>
    public static CarPosition DeepCopy(this CarPosition original)
    {
        if (original == null) return new CarPosition();
        var copy = new CarPosition();
        var p = Models.Mappers.CarPositionMapper.CreatePatch(copy, original);
        Models.Mappers.CarPositionMapper.ApplyPatch(p, copy);
        return copy;
    }

    /// <summary>
    /// Creates deep copies of a collection of CarPositions.
    /// </summary>
    public static IEnumerable<CarPosition> DeepCopy(this IEnumerable<CarPosition> carPositions)
    {
        ArgumentNullException.ThrowIfNull(carPositions);
        return carPositions.Select(cp => cp.DeepCopy());
    }
}
