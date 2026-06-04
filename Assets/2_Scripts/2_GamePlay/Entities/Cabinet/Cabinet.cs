using System;
using NightStalker.Actors;
using UnityEngine;


namespace NightStalker.GamePlay.Entities
{
    public class Cabinet : MonoBehaviour, IInteractable
    {
        [Header("InteractLocalPoint")]
        [SerializeField] Vector2 _interactPoint = new Vector2(0f, 0f);

        [Header("FocusAnimation")]
        [SerializeField] private InteractableFocusView _animation;


        private PlayerActor _occupiedActor;

        private bool _isFocused;
        private bool _isOccupied;

        public Vector2 WorldPosition => transform.position;

        public event Action OnInteractionCompleted;

        public void Init()
        {
            _occupiedActor = null;
            _isFocused = false;
            _isOccupied = false;
        }

        public bool TryInteraction(PlayerActor player, ActorDirection direction, out InteractionContext context)
        {

            if (_isOccupied)
            {
                context = ExitCabinet();
                return true;
            }

            context = EnterCabinet(player);
            return true;
        }

        private InteractionContext EnterCabinet(PlayerActor player)
        {
            Vector2 interactPosition = transform.TransformPoint(_interactPoint);

            InteractionContext context = new InteractionContext(
                interactPosition,
                InteractionFlow.Transition,
                ActorDirection.Up,
                InteractionControlMode.ControlLocked,
                PlayerAnimationState.Interact,
                ActorDirection.Down,
                InteractionControlMode.InteractionOnly,
                PlayerAnimationState.Idle);

            _occupiedActor = player;
            _occupiedActor.OnAnimationFinished -= HandleAnimationFinished;
            _occupiedActor.OnAnimationFinished += HandleAnimationFinished;

            return context;
        }

        private InteractionContext ExitCabinet()
        {
            Vector2 interactPosition = transform.TransformPoint(_interactPoint);

            InteractionContext context = new InteractionContext(
                interactPosition,
                InteractionFlow.Transition,
                ActorDirection.Down,
                InteractionControlMode.ControlLocked,
                PlayerAnimationState.Interact,
                ActorDirection.Down,
                InteractionControlMode.None,
                PlayerAnimationState.Idle);

            _occupiedActor.OnAnimationFinished -= HandleAnimationFinished;
            _occupiedActor.OnAnimationFinished += HandleAnimationFinished;

            return context;
        }


        public void Focus()
        {
            if (_isFocused) return;

            _isFocused = true;

            _animation.Play();
        }

        public void Unfocus()
        {
            if (!_isFocused) return;

            _isFocused = false;

            _animation.Stop();
        }

        private void HandleAnimationFinished(PlayerAnimationState state)
        {
            if (state != PlayerAnimationState.Interact)
                return;

            if (_occupiedActor == null)
                return;

            PlayerActor actor = _occupiedActor;
            actor.OnAnimationFinished -= HandleAnimationFinished;

            if (_isOccupied)
            {
                actor.ExitCabinet();
                _isOccupied = false;
                _occupiedActor = null;
            }
            else
            {
                actor.EnterCabinet();
                _isOccupied = true;
            }
        }

        private void OnDestroy()
        {
            if (_occupiedActor != null)
                _occupiedActor.OnAnimationFinished -= HandleAnimationFinished;
        }
    }
}

