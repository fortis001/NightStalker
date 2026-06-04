using System;
using NightStalker.Actors;
using NightStalker.Core;
using NightStalker.GamePlay.InGame;
using UnityEngine;

namespace NightStalker.GamePlay.Entities
{
    public class PowerGenerator : MonoBehaviour, IInteractable
    {
        [Header("Interact Local Points")]
        [SerializeField] private Vector2 _rightInteractPoint = new Vector2(-0.765f, 0.12f);
        [SerializeField] private Vector2 _leftInteractPoint = new Vector2(0.72f, 0.02f);
        [SerializeField] private Vector2 _upInteractPoint = new Vector2(-0.15f, -0.38f);
        [SerializeField] private Vector2 _downInteractPoint = new Vector2(-0.02f, 0.6f);

        [Header("Repair Settings")]
        [SerializeField] private float _repairProgressPerSecond = 0.02f;
        [SerializeField] private float _maxRepairProgress = 1f;

        [Header("FocusAnimation")]
        [SerializeField] private InteractableFocusView _animation;

        private InGameTimeManager _timeManager;

        private float _repairProgress;
        private bool _isFocused;
        private bool _isInteracting;
        private bool _isRepaired;

        public Vector2 WorldPosition => transform.position;

        public event Action OnInteractionCompleted;

        public void Init(InGameTimeManager timeManager)
        {
            _timeManager = timeManager;

            _repairProgress = 0f;
            _isFocused = false;
            _isInteracting = false;
            _isRepaired = false;
        }

        private void Update()
        {
            if (!_isInteracting)
                return;

            if (_isRepaired)
                return;

            float previousProgress = _repairProgress;

            _repairProgress += _repairProgressPerSecond * _timeManager.GameDeltaTime;
            _repairProgress = Mathf.Clamp(_repairProgress, 0f, _maxRepairProgress);

            if (!Mathf.Approximately(previousProgress, _repairProgress))
            {
                GameEventBus.Publish(new PowerGeneratorRepairProgressChangedEvent(
                this,
                _repairProgress,
                _maxRepairProgress));
            }

            if (_repairProgress >= _maxRepairProgress)
            {
                CompleteRepair();
            }
        }

        public bool TryInteraction(PlayerActor player, ActorDirection direction, out InteractionContext context)
        {
            if (_isRepaired)
            {
                context = default;
                return false;
            }

            if(_isInteracting)
            {
                context = CancelRepair();
                return true;
            }

            context = StartRepair(direction);
            return true;
        }

        private InteractionContext StartRepair(ActorDirection direction)
        {
            Vector2 snapPosition = GetInteractWorldPosition(direction);
            _isInteracting = true;


            InteractionContext context = new InteractionContext(
                snapPosition,
                InteractionFlow.Ongoing,
                direction,
                InteractionControlMode.Cancellable,
                PlayerAnimationState.Crouch,
                ActorDirection.None,
                InteractionControlMode.None,
                PlayerAnimationState.Idle);


            _animation.Stop();
            return context;
        }

        private InteractionContext CancelRepair()
        {
            InteractionContext context = new InteractionContext(null, InteractionFlow.Instant, ActorDirection.None, InteractionControlMode.None, PlayerAnimationState.Idle);

            _isInteracting = false;
            _animation.Play();

            return context;
        }

        private void CompleteRepair()
        {
            if (_isRepaired)
                return;

            _isRepaired = true;
            _isInteracting = false;
            _isFocused = false;
            _repairProgress = _maxRepairProgress;

            OnInteractionCompleted?.Invoke();
            GameEventBus.Publish(new PowerGeneratorRepairedEvent(this));
        }

        private Vector2 GetInteractWorldPosition(ActorDirection playerFacingDirection)
        {
            Vector2 localPoint = playerFacingDirection switch
            {
                ActorDirection.Right => _rightInteractPoint,
                ActorDirection.Left => _leftInteractPoint,
                ActorDirection.Up => _upInteractPoint,
                ActorDirection.Down => _downInteractPoint,
                _ => Vector2.zero
            };

            return transform.TransformPoint(localPoint);
        }

        public void Focus()
        {
            if (_isRepaired || _isFocused) return;

            _isFocused = true;
            _animation.Play();

            GameEventBus.Publish(new PowerGeneratorFocusedEvent(
                this,
                _repairProgress,
                _maxRepairProgress));
        }

        public void Unfocus()
        {
            if (!_isFocused) return;

            _isFocused = false;
            _animation.Stop();

            GameEventBus.Publish(new PowerGeneratorUnfocusedEvent(this));
        }

        
    }
}

