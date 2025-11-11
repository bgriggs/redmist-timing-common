using RedMist.TimingCommon.Models;
using Riok.Mapperly.Abstractions;

namespace RedMist.TimingCommon.Extensions;

/// <summary>
/// Provides extension methods for creating deep copies of CarPosition instances and collections.
/// </summary>
/// <remarks>These extension methods use an auto-generated CarPositionMapper to perform deep cloning. If the
/// mapper is unavailable, the methods fall back to manual copying. The class is static and intended for use with
/// CarPosition objects to facilitate safe duplication without shared references.</remarks>
public static class CarPositionExtensions
{
    private static readonly CarPositionMapperExt mapper = new();

    /// <summary>
    /// Creates a deep copy of a CarPosition using the auto-generated CarPositionMapper.
    /// If the mapper is not available, falls back to manual copying.
    /// </summary>
    /// <param name="original">The CarPosition to copy</param>
    /// <returns>A deep copy of the CarPosition</returns>
    public static CarPosition DeepCopy(this CarPosition original)
    {
        return mapper.CloneCarPosition(original);
    }

    /// <summary>
    /// Creates deep copies of a collection of CarPositions.
    /// </summary>
    public static IEnumerable<CarPosition> DeepCopy(this IEnumerable<CarPosition> carPositions)
    {
        return mapper.CloneCarPositions(carPositions);
    }
}

/// <summary>
/// Mapper for CarPosition objects using Mapperly code generation
/// </summary>
[Mapper(UseDeepCloning = true)]
public partial class CarPositionMapperExt
{
    /// <summary>
    /// Creates deep copies of a list of CarPosition objects
    /// </summary>
    public partial IEnumerable<CarPosition> CloneCarPositions(IEnumerable<CarPosition> source);

    /// <summary>
    /// Creates a deep copy of a CarPosition object
    /// </summary>
    public partial CarPosition CloneCarPosition(CarPosition source);
}
