namespace Rocal.Core.Onboarding;

/// <summary>UI-independent state machine for the first-run tour.</summary>
public sealed class OnboardingFlow
{
    public OnboardingFlow(int stepCount)
    {
        if (stepCount < 1) throw new ArgumentOutOfRangeException(nameof(stepCount), "At least one step is required.");
        StepCount = stepCount;
    }

    public int StepCount { get; }
    public int CurrentIndex { get; private set; }
    public bool IsFinished { get; private set; }
    public bool IsFirst => CurrentIndex == 0;
    public bool IsLast => CurrentIndex == StepCount - 1;

    /// <summary>Advances one step; on the last step it finishes the tour instead.</summary>
    public void Next()
    {
        if (IsFinished) return;
        if (IsLast) IsFinished = true;
        else CurrentIndex++;
    }

    public void Back()
    {
        if (IsFinished || IsFirst) return;
        CurrentIndex--;
    }

    public void Skip() => IsFinished = true;

    public void Reset()
    {
        CurrentIndex = 0;
        IsFinished = false;
    }
}
