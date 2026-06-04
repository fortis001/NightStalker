using System.Collections.Generic;
using NightStalker.GamePlay.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NightStalker.Actors.Player
{
    public class PlayerInteractor : MonoBehaviour
    {
        #region Field
        [SerializeField] LSH.Utils.TriggerRelay2D _trigger;

        private PlayerActor _player;
        private PlayerController _controller;
        private PlayerAnimator _animator;
        private PlayerMovement _movement;

        private InputAction _interactAction;

        private readonly List<IInteractable> _interactables = new();
        private IInteractable _focusedInteractable;
        private IInteractable _currentInteractable;
        private InteractionContext _currentInteractionContext;

        private bool _isInteractable;
        private bool _isInteractionEndApplied;

        #endregion

        public void Init(PlayerActor player, PlayerController controller, PlayerAnimator animator, PlayerMovement movement, InputAction interactAction)
        {
            _player = player;
            _controller = controller;
            _animator = animator;
            _movement = movement;

            _animator.OnAnimationFinished -= HandleAnimationFinished;
            _animator.OnAnimationFinished += HandleAnimationFinished;

            _interactAction = interactAction;
            _interactAction.performed -= HandleInteractKeyPressed;
            _interactAction.performed += HandleInteractKeyPressed;

            _trigger.OnTriggerEntered -= HandleTriggerEntered;
            _trigger.OnTriggerEntered += HandleTriggerEntered;
            _trigger.OnTriggerExited -= HandleTriggerExited;
            _trigger.OnTriggerExited += HandleTriggerExited;

            _isInteractable = true;
        }

        private void Update()
        {
            RefreshFocusedInteractable();
        }

        private void HandleInteractKeyPressed(InputAction.CallbackContext context)
        {
            if (!_isInteractable) return;
            Interact();
        }

        private void HandleAnimationFinished(PlayerAnimationState animation)
        {
            if (_currentInteractable == null)
                return;

            InteractionContext context = _currentInteractionContext;

            if (context.InteractionFlow != InteractionFlow.Transition)
                return;

            if (animation != context.StartAnimation)
                return;

            ApplyInteractionEnd();
        }

        private void HandleTriggerEntered(Collider2D collider)
        {
            IInteractable interactable = collider.GetComponentInParent<IInteractable>();

            RegisterInteractable(interactable);
        }

        private void HandleTriggerExited(Collider2D collider)
        {
            IInteractable interactable = collider.GetComponentInParent<IInteractable>();

            UnregisterInteractable(interactable);
        }

        #region Interaction
        public void Interact()
        {
            if (_focusedInteractable == null)
                return;

            IInteractable target = _currentInteractable ?? _focusedInteractable;
            ActorDirection direction = GetInteractDirection(target.WorldPosition);

            if (!target.TryInteraction(_player, direction, out InteractionContext context))
                return;

            _currentInteractable = target;
            _currentInteractionContext = context;
            _isInteractionEndApplied = false;

            ApplyContextFlow(context.InteractionFlow);
            ApplyInteractionStart();

            if (context.InteractionFlow == InteractionFlow.Instant)
            {
                ClearCurrentInteraction();
            }
        }
        private ActorDirection GetInteractDirection(Vector2 targetPosition)
        {
            Vector2 myPosition = transform.position;
            Vector2 delta = targetPosition - myPosition;

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                return delta.x > 0f
                    ? ActorDirection.Right
                    : ActorDirection.Left;
            }

            return delta.y > 0f
                ? ActorDirection.Up
                : ActorDirection.Down;
        }
        private void ApplyContextFlow(InteractionFlow flow)
        {
            if (_currentInteractable == null)
                return;

            _currentInteractable.OnInteractionCompleted -= ApplyInteractionEnd;

            switch (flow)
            {
                case InteractionFlow.Instant:
                    break;

                case InteractionFlow.Ongoing:
                    _currentInteractable.OnInteractionCompleted += ApplyInteractionEnd;
                    break;

                case InteractionFlow.Transition:
                    break;

                default:
                    Debug.LogWarning($"Unhandled interaction flow: {flow}");
                    break;
            }
        }

        private void ApplyInteractionStart()
        {
            InteractionContext context = _currentInteractionContext;

            if (context.SnapPosition.HasValue)
            {
                _movement.SnapTo(context.SnapPosition.Value);
            }

            ApplyControlMode(context.StartMode);

            if (context.StartAnimation == PlayerAnimationState.None) return;

            ActorDirection direction = context.StartDirection == ActorDirection.None
            ? _controller.Direction
            : context.StartDirection;

            if(context.InteractionFlow == InteractionFlow.Transition)
            {
                _animator.PlayAndNotifyFinished(context.StartAnimation, direction);
                return;
            }

            _animator.Play(context.StartAnimation, direction);
        }

        private void ApplyInteractionEnd()
        {
            if (_currentInteractable == null)
                return;

            if (_isInteractionEndApplied)
                return;

            _isInteractionEndApplied = true;

            InteractionContext context = _currentInteractionContext;

            ApplyControlMode(context.EndMode);

            if (context.EndAnimation != PlayerAnimationState.None)
            {
                ActorDirection direction = context.EndDirection == ActorDirection.None
                    ? _controller.Direction
                    : context.EndDirection;

                _animator.Play(context.EndAnimation, direction);
            }

            ClearCurrentInteraction();
        }

        private void ApplyControlMode(InteractionControlMode mode)
        {
            _controller.SetInteractionControlMode(mode);

            switch (mode)
            {
                case InteractionControlMode.None:
                case InteractionControlMode.Cancellable:
                case InteractionControlMode.InteractionOnly:
                    _isInteractable = true;
                    break;

                case InteractionControlMode.ControlLocked:
                    _isInteractable = false;
                    break;

                default:
                    Debug.LogWarning($"Unhandled interaction control mode: {mode}");
                    _controller.SetInteractionControlMode(InteractionControlMode.None);
                    _isInteractable = true;
                    break;
            }
        }
        private void ClearCurrentInteraction()
        {
            if (_currentInteractable != null)
            {
                _currentInteractable.OnInteractionCompleted -= ApplyInteractionEnd;
            }

            _currentInteractable = null;
            _currentInteractionContext = default;
            _isInteractionEndApplied = false;

            RefreshFocusedInteractable();
        }
        #endregion

        #region Interaction Trigger
        private void RegisterInteractable(IInteractable interactable)
        {
            if (interactable == null)
                return;

            if (!_interactables.Contains(interactable))
                _interactables.Add(interactable);

            RefreshFocusedInteractable();
        }

        private void UnregisterInteractable(IInteractable interactable)
        {
            if (interactable == null)
                return;

            _interactables.Remove(interactable);

            if (_focusedInteractable == interactable)
                ChangeFocusedInteractable(null);

            if (_currentInteractable == interactable)
                _currentInteractable = null;

            RefreshFocusedInteractable();
        }

        private void RefreshFocusedInteractable()
        {
            if (_currentInteractable != null)
                return;

            IInteractable nearest = GetNearestInteractable();

            if (nearest == _focusedInteractable)
                return;

            ChangeFocusedInteractable(nearest);
        }

        private void ChangeFocusedInteractable(IInteractable next)
        {
            _focusedInteractable?.Unfocus();

            _focusedInteractable = next;

            _focusedInteractable?.Focus();
        }

        private IInteractable GetNearestInteractable()
        {
            IInteractable nearest = null;
            float nearestSqrDistance = float.MaxValue;

            foreach (IInteractable interactable in _interactables)
            {
                if (interactable is not MonoBehaviour monoBehaviour)
                    continue;

                float sqrDistance =
                    (monoBehaviour.transform.position - transform.position).sqrMagnitude;

                if (sqrDistance < nearestSqrDistance)
                {
                    nearestSqrDistance = sqrDistance;
                    nearest = interactable;
                }
            }

            return nearest;
        }

        #endregion

        private void OnDestroy()
        {
            _trigger.OnTriggerEntered -= HandleTriggerEntered;
            _trigger.OnTriggerExited -= HandleTriggerExited;
            _animator.OnAnimationFinished -= HandleAnimationFinished;
            _interactAction.performed -= HandleInteractKeyPressed;
        }
    }
}

