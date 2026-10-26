using RedMist.TimingCommon.Models;
using RedMist.TimingCommon.Models.Mappers;
using System.Reflection;

namespace RedMist.TimingCommon.Tests.Models;

/// <summary>
/// Sector lists are published by comparing a car's list before and after a change, element by
/// element, so whether a change reaches clients rests entirely on the element types' equality.
/// </summary>
[TestClass]
public class SectorModelTests
{
    private static SectorTime Sector(int index, int timeMs) =>
        new() { Index = index, Lap = 4, TimeMs = timeMs, IsPersonalBest = true };

    [TestMethod]
    public void CreatePatch_SendsSectors_WhenOneFlagChangesInPlace()
    {
        var before = new CarPosition { Number = "7", Sectors = [Sector(1, 26_700), Sector(2, 48_900)] };
        var after = new CarPosition { Number = "7", Sectors = [Sector(1, 26_700), Sector(2, 48_900)] };
        after.Sectors[1].IsOverallBest = true;

        var patch = CarPositionMapper.CreatePatch(before, after);

        Assert.IsNotNull(patch.Sectors);
        Assert.IsTrue(patch.Sectors[1].IsOverallBest);
    }

    [TestMethod]
    public void CreatePatch_OmitsSectors_WhenEqualButDistinctLists()
    {
        var before = new CarPosition { Number = "7", Sectors = [Sector(1, 26_700)] };
        var after = new CarPosition { Number = "7", Sectors = [Sector(1, 26_700)] };

        var patch = CarPositionMapper.CreatePatch(before, after);

        Assert.IsNull(patch.Sectors);
    }

    [TestMethod]
    public void CreatePatch_SendsEmptySectors_SoTheListCanBeCleared()
    {
        var before = new CarPosition { Number = "7", Sectors = [Sector(1, 26_700)] };
        var after = new CarPosition { Number = "7" };

        var patch = CarPositionMapper.CreatePatch(before, after);

        Assert.IsNotNull(patch.Sectors);
        Assert.AreEqual(0, patch.Sectors.Count);
    }

    [TestMethod]
    public void SessionCreatePatch_SendsLayout_OnlyWhenItChanges()
    {
        static SessionState WithLayout(double? s2Length) => new()
        {
            SectorLayout =
            [
                new SectorInfo { Index = 1, Label = "S1", LengthMeters = 1210 },
                new SectorInfo { Index = 2, Label = "S2", LengthMeters = s2Length },
            ],
            SectorSource = SectorSource.X2Loops,
        };

        Assert.IsNull(SessionStateMapper.CreatePatch(WithLayout(1795), WithLayout(1795)).SectorLayout);
        Assert.IsNotNull(SessionStateMapper.CreatePatch(WithLayout(1795), WithLayout(1800)).SectorLayout);
    }

    /// <summary>
    /// A member added later but left out of Equals would never be published on its own: the list
    /// would compare equal and the patch would skip it.
    /// </summary>
    [TestMethod]
    public void SectorTime_Equals_CoversEveryProperty() =>
        AssertEqualsCoversEveryProperty(() => new SectorTime { Index = 1, Lap = 2, TimeMs = 3 });

    [TestMethod]
    public void SectorInfo_Equals_CoversEveryProperty() =>
        AssertEqualsCoversEveryProperty(() => new SectorInfo { Index = 1, Label = "S1", LengthMeters = 100 });

    [TestMethod]
    public void SectorInfo_Equals_IsReflexiveForNaNLength()
    {
        var info = new SectorInfo { Index = 1, Label = "S1", LengthMeters = double.NaN };

        Assert.AreEqual(info, info);
    }

    private static void AssertEqualsCoversEveryProperty<T>(Func<T> create) where T : class
    {
        Assert.AreEqual(create(), create());

        foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var changed = create();
            property.SetValue(changed, Different(property.PropertyType, property.GetValue(changed)));

            Assert.AreNotEqual(create(), changed, $"{typeof(T).Name}.Equals ignores {property.Name}");
        }
    }

    private static object Different(Type type, object? current) => (Nullable.GetUnderlyingType(type) ?? type) switch
    {
        var t when t == typeof(int) => (int)(current ?? 0) + 1,
        var t when t == typeof(double) => (double)(current ?? 0d) + 1,
        var t when t == typeof(bool) => !(bool)(current ?? false),
        var t when t == typeof(string) => (string?)current + "x",
        var t => throw new NotSupportedException($"Add a different value for {t.Name}"),
    };
}
