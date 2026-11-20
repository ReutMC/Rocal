using Rocal.Core.Onboarding;

namespace Rocal.Core.Tests;

public class OnboardingFlowTests
{
    [Fact]
    public void StartsAtFirstStep_NotFinished()
    {
        var flow = new OnboardingFlow(3);
        Assert.Equal(0, flow.CurrentIndex);
        Assert.True(flow.IsFirst);
        Assert.False(flow.IsLast);
        Assert.False(flow.IsFinished);
    }

    [Fact]
    public void Next_AdvancesThroughSteps_ThenFinishes()
    {
        var flow = new OnboardingFlow(3);
        flow.Next();
        flow.Next();
        Assert.Equal(2, flow.CurrentIndex);
        Assert.True(flow.IsLast);
        Assert.False(flow.IsFinished);

        flow.Next();
        Assert.True(flow.IsFinished);
        Assert.Equal(2, flow.CurrentIndex);
    }

    [Fact]
    public void Back_StopsAtFirstStep()
    {
        var flow = new OnboardingFlow(3);
        flow.Back();
        Assert.Equal(0, flow.CurrentIndex);

        flow.Next();
        flow.Back();
        Assert.Equal(0, flow.CurrentIndex);
    }

    [Fact]
    public void Skip_FinishesImmediately()
    {
        var flow = new OnboardingFlow(5);
        flow.Skip();
        Assert.True(flow.IsFinished);
    }

    [Fact]
    public void Reset_ReturnsToFirstStepAndClearsFinished()
    {
        var flow = new OnboardingFlow(2);
        flow.Next();
        flow.Next();
        Assert.True(flow.IsFinished);

        flow.Reset();
        Assert.Equal(0, flow.CurrentIndex);
        Assert.False(flow.IsFinished);
    }

    [Fact]
    public void ZeroSteps_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new OnboardingFlow(0));
    }
}
