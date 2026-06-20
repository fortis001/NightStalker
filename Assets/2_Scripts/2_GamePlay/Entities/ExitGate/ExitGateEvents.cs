

namespace NightStalker.GamePlay.Entities
{
    public readonly struct ExitGateFocusedEvent
    {
        public ExitGate ExitGate { get; }
        public float CurrentProgress { get; }
        public float MaxProgress { get; }

        public ExitGateFocusedEvent(
            ExitGate exitGate,
            float currentProgress,
            float maxProgress)
        {
            ExitGate = exitGate;
            CurrentProgress = currentProgress;
            MaxProgress = maxProgress;
        }
    }

    public readonly struct ExitGateUnfocusedEvent
    {
        public ExitGate ExitGate { get; }

        public ExitGateUnfocusedEvent(ExitGate exitGate)
        {
            ExitGate = exitGate;
        }
    }

    public readonly struct ExitGateOpenProgressChangedEvent
    {
        public ExitGate ExitGate { get; }
        public float CurrentProgress { get; }
        public float MaxProgress { get; }

        public float NormalizedProgress =>
            MaxProgress <= 0f ? 0f : CurrentProgress / MaxProgress;

        public ExitGateOpenProgressChangedEvent(
            ExitGate exitGate,
            float currentProgress,
            float maxProgress)
        {
            ExitGate = exitGate;
            CurrentProgress = currentProgress;
            MaxProgress = maxProgress;
        }
    }

    public readonly struct ExitGateOpenedEvent
    {
        public ExitGate ExitGate { get; }

        public ExitGateOpenedEvent(ExitGate exitGate)
        {
            ExitGate = exitGate;
        }
    }
}

