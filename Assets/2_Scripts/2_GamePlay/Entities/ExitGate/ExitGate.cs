using System;
using NightStalker.Actors;
using NightStalker.Core;
using NightStalker.GamePlay.InGame;
using UnityEngine;

namespace NightStalker.GamePlay.Entities
{
    public class ExitGate : MonoBehaviour, IInteractable
    {
        [Header("Interact Local Points")]
        [SerializeField] Vector2 _interactPoint = new Vector2(0f, -0.5f);

        [Header("Gate Opening Settings")]
        [SerializeField] private float _gateOpenProgressPerSecond = 0.02f;
        [SerializeField] private float _maxProgress = 1f;

        [Header("FocusAnimation")]
        [SerializeField] private InteractableFocusView _animation;

        private InGameTimeManager _timeManager;

        private float _gateOpenProgress;
        private bool _isFocused;
        private bool _isInteracting;
        private bool _isOpened;

        public Vector2 WorldPosition => transform.position;

        public event Action OnInteractionCompleted;

        public void Init(InGameTimeManager timeManager)
        {
            _timeManager = timeManager;
        }

        private void Update()
        {
            if (!_isInteracting)
                return;

            if (_isOpened)
                return;

            float previousProgress = _gateOpenProgress;

            _gateOpenProgress += _gateOpenProgressPerSecond * _timeManager.GameDeltaTime;
            _gateOpenProgress  = Mathf.Clamp(_gateOpenProgress, 0f, _maxProgress);

            if (!Mathf.Approximately(previousProgress, _gateOpenProgress))
            {
                GameEventBus.Publish(new ExitGateOpenProgressChangedEvent(
                this,
                _gateOpenProgress,
                _maxProgress));
            }

            if (_gateOpenProgress >= _maxProgress)
            {
                CompleteGateOpen();
            }

        }

        public bool TryInteraction(PlayerActor player, ActorDirection direction, out InteractionContext context)
        {
            if (_isOpened)
            {
                context = default;
                return false;
            }
            if (_isInteracting)
            {
                context = CancelGateOpen();
                return true;
            }

            context = StartGateOpen(direction);
            return true;
        }

        private InteractionContext StartGateOpen(ActorDirection direction)
        {
            Vector2 snapPosition = transform.TransformPoint(_interactPoint);
            _isInteracting = true;

            InteractionContext context = new InteractionContext(
                snapPosition,
                InteractionFlow.Ongoing,
                ActorDirection.Up,
                InteractionControlMode.Cancellable,
                PlayerAnimationState.Interact,
                ActorDirection.None,
                InteractionControlMode.None,
                PlayerAnimationState.Idle);


            _animation.Stop();
            return context;
        }

        private InteractionContext CancelGateOpen()
        {
            InteractionContext context = new InteractionContext(null, InteractionFlow.Instant, ActorDirection.None, InteractionControlMode.None, PlayerAnimationState.Idle);

            _isInteracting = false;
            _animation.Play();

            return context;
        }

        private void CompleteGateOpen()
        {
            if (_isOpened)
                return;

            _isOpened = true;
            _isInteracting = false;
            _isFocused = false;
            _gateOpenProgress = _maxProgress;

            OnInteractionCompleted?.Invoke();
            GameEventBus.Publish(new ExitGateOpenedEvent(this));
        }

        public void Focus()
        {
            if (_isOpened || _isFocused) return;

            _isFocused = true;
            _animation.Play();

            GameEventBus.Publish(new ExitGateFocusedEvent(
                this,
                _gateOpenProgress,
                _maxProgress));
        }

        public void Unfocus()
        {
            if (!_isFocused) return;

            _isFocused = false;
            _animation.Stop();

            GameEventBus.Publish(new ExitGateUnfocusedEvent(this));
        }
    }
}


