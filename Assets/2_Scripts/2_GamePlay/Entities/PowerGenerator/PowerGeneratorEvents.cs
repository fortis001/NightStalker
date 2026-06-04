using UnityEngine;

namespace NightStalker.GamePlay.Entities
{
    public readonly struct PowerGeneratorFocusedEvent
    {
        public PowerGenerator Generator { get; }
        public float CurrentProgress { get; }
        public float MaxProgress { get; }

        public PowerGeneratorFocusedEvent(
            PowerGenerator generator,
            float currentProgress,
            float maxProgress)
        {
            Generator = generator;
            CurrentProgress = currentProgress;
            MaxProgress = maxProgress;
        }
    }
    public readonly struct PowerGeneratorUnfocusedEvent
    {
        public PowerGenerator Generator { get; }

        public PowerGeneratorUnfocusedEvent(PowerGenerator generator)
        {
            Generator = generator;
        }
    }
    public readonly struct PowerGeneratorRepairProgressChangedEvent
    {
        public PowerGenerator Generator { get; }
        public float CurrentProgress { get; }
        public float MaxProgress { get; }

        public float NormalizedProgress =>
            MaxProgress <= 0f ? 0f : CurrentProgress / MaxProgress;

        public PowerGeneratorRepairProgressChangedEvent(
            PowerGenerator generator,
            float currentProgress,
            float maxProgress)
        {
            Generator = generator;
            CurrentProgress = currentProgress;
            MaxProgress = maxProgress;
        }
    }
    public readonly struct PowerGeneratorRepairedEvent
    {
        public PowerGenerator Generator { get; }

        public PowerGeneratorRepairedEvent(PowerGenerator generator)
        {
            Generator = generator;
        }
    }
}
