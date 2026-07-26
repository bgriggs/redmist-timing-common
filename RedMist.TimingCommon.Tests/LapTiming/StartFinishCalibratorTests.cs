using RedMist.TimingCommon.LapTiming;

namespace RedMist.TimingCommon.Tests.LapTiming;

[TestClass]
public class StartFinishCalibratorTests
{
    private const double LapLength = 4000.0;

    [TestMethod]
    public void Estimate_ClusteredObservations_ReturnsConsensus()
    {
        var observations = new List<double> { 120, 135, 128, 140, 125 };

        var offset = StartFinishCalibrator.EstimateOffsetMeters(observations, LapLength);

        Assert.IsNotNull(offset);
        Assert.AreEqual(128, offset.Value, 15);
    }

    [TestMethod]
    public void Estimate_TooFewObservations_ReturnsNull()
    {
        var observations = new List<double> { 120, 130, 125 };

        Assert.IsNull(StartFinishCalibrator.EstimateOffsetMeters(observations, LapLength));
    }

    [TestMethod]
    public void Estimate_ObservationsStraddlingTheOrigin_StaysNearTheLine()
    {
        // Crossings recorded just either side of the path origin: a plain average would land on the
        // far side of the track.
        var observations = new List<double> { 3980, 3995, 10, 25, 4000 - 5 };

        var offset = StartFinishCalibrator.EstimateOffsetMeters(observations, LapLength);

        Assert.IsNotNull(offset);
        var toLine = TrackGeometry.CircularDistanceMeters(offset.Value, 0, LapLength);
        Assert.IsTrue(toLine < 40, $"Offset should sit within a few tens of meters of the origin; was {offset.Value:F0} m");
    }

    [TestMethod]
    public void Estimate_SingleLateOutlier_IsIgnored()
    {
        // One car's lap was reported a long way late; the rest agree.
        var observations = new List<double> { 120, 130, 125, 1800, 135 };

        var offset = StartFinishCalibrator.EstimateOffsetMeters(observations, LapLength);

        Assert.IsNotNull(offset);
        Assert.AreEqual(128, offset.Value, 20);
    }

    [TestMethod]
    public void Estimate_ScatteredObservations_ReturnsNull()
    {
        // No consensus: these could be anywhere on the track.
        var observations = new List<double> { 100, 900, 1800, 2600, 3400 };

        Assert.IsNull(StartFinishCalibrator.EstimateOffsetMeters(observations, LapLength));
    }

    [TestMethod]
    public void Estimate_ZeroLengthPath_ReturnsNull()
    {
        var observations = new List<double> { 1, 2, 3, 4, 5 };

        Assert.IsNull(StartFinishCalibrator.EstimateOffsetMeters(observations, 0));
    }

    [TestMethod]
    public void Estimate_EvenNumberOfObservations_UsesMidpointOfDeviations()
    {
        var observations = new List<double> { 120, 130, 125, 140, 128, 135 };

        var offset = StartFinishCalibrator.EstimateOffsetMeters(observations, LapLength);

        Assert.IsNotNull(offset);
        Assert.AreEqual(129, offset.Value, 15);
    }

    [TestMethod]
    public void Estimate_EvenNumberOfScatteredObservations_ReturnsNull()
    {
        var observations = new List<double> { 100, 900, 1800, 2600, 3400, 3800 };

        Assert.IsNull(StartFinishCalibrator.EstimateOffsetMeters(observations, LapLength));
    }

    [TestMethod]
    public void Estimate_NonFiniteObservation_IsDiscardedRatherThanPoisoningTheConsensus()
    {
        // A NaN makes every circular distance NaN, which would leave the first observation winning
        // by default and sail through the consensus check.
        var observations = new List<double> { 3000, 130, double.NaN, 125, 140, 128, 135 };

        var offset = StartFinishCalibrator.EstimateOffsetMeters(observations, LapLength);

        Assert.IsNotNull(offset);
        Assert.AreEqual(131, offset.Value, 20, "Should agree with the finite cluster, not the first entry");
    }

    [TestMethod]
    public void Estimate_NonFiniteObservationsLeaveTooFewToAgree_ReturnsNull()
    {
        var observations = new List<double> { 120, double.NaN, double.PositiveInfinity, 125, double.NaN };

        Assert.IsNull(StartFinishCalibrator.EstimateOffsetMeters(observations, LapLength));
    }

    [TestMethod]
    public void Estimate_EmptyObservationsWithNoMinimum_ReturnsNull()
    {
        Assert.IsNull(StartFinishCalibrator.EstimateOffsetMeters([], LapLength, minObservations: 0));
    }

    [TestMethod]
    public void Estimate_OffsetIsNormalisedIntoThePath()
    {
        var observations = new List<double> { 4100, 4110, 4105, 4115, 4108 };

        var offset = StartFinishCalibrator.EstimateOffsetMeters(observations, LapLength);

        Assert.IsNotNull(offset);
        Assert.IsTrue(offset.Value >= 0 && offset.Value < LapLength, $"Offset should wrap into [0, len); was {offset.Value}");
        Assert.AreEqual(108, offset.Value, 15);
    }
}