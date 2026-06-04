using System;
using NightStalker.Actors;
using UnityEngine;


namespace NightStalker.GamePlay.Entities
{
    public enum InteractionControlMode
    {
        None,
        Cancellable,
        InteractionOnly,
        ControlLocked
    }
    public enum InteractionFlow
    {
        None,
        Instant,
        Ongoing,
        Transition,
    }

    public readonly struct InteractionContext
    {
        private readonly Vector2? _snapPosition;
        private readonly InteractionFlow _interactionFlow;
        private readonly ActorDirection _startDirection;
        private readonly InteractionControlMode _startMode;
        private readonly PlayerAnimationState _startAnimation;
        private readonly ActorDirection _endDirection;
        private readonly InteractionControlMode _endMode;
        private readonly PlayerAnimationState _endAnimation;


        public Vector2? SnapPosition => _snapPosition;
        public InteractionFlow InteractionFlow => _interactionFlow;
        public ActorDirection StartDirection => _startDirection;
        public InteractionControlMode StartMode => _startMode;
        public PlayerAnimationState StartAnimation => _startAnimation;
        public ActorDirection EndDirection => _endDirection;
        public InteractionControlMode EndMode => _endMode;
        public PlayerAnimationState EndAnimation => _endAnimation;

        public InteractionContext(
        Vector2? snapPosition,
        InteractionFlow interactionFlow,
        ActorDirection startDirection,
        InteractionControlMode startMode,
        PlayerAnimationState startAnimation,
        ActorDirection endDirection = ActorDirection.None,
        InteractionControlMode endMode = InteractionControlMode.None,
        PlayerAnimationState endAnimation = PlayerAnimationState.None)
        {
            _snapPosition = snapPosition;
            _interactionFlow = interactionFlow;
            _startDirection = startDirection;
            _startMode = startMode;
            _startAnimation = startAnimation;
            _endDirection = endDirection;
            _endMode = endMode;
            _endAnimation = endAnimation;
        }
    }

    public interface IInteractable
    {
        Vector2 WorldPosition { get; }

        event Action OnInteractionCompleted;

        bool TryInteraction(PlayerActor player, ActorDirection direction, out InteractionContext context);

        void Focus();
        void Unfocus();
    }
}
