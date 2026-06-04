using NightStalker.GamePlay.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NightStalker.Actors.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _sprintMultiplier = 3f;

        // Compared against Vector2.sqrMagnitude.
        // 0.0001f means actual magnitude deadzone is 0.01f.
        [SerializeField] private float _inputDeadzoneSqr = 0.0001f;

        private PlayerMovement _movement;
        private PlayerAnimator _animator;
        private PlayerInteractor _interactor;
        private PlayerStatus _status;

        private InputAction _moveAction;
        private InputAction _sprintAction;

        private bool _isControlLocked;
        private bool _isInteracting;

        private ActorDirection _direction;
        private PlayerMoveState _moveState;
        private InteractionControlMode _interactionControlMode;

        public ActorDirection Direction => _direction;
        public PlayerMoveState MoveState => _moveState;
        public InteractionControlMode InteractionControlMode => _interactionControlMode;

        public void Init(PlayerMovement movement, PlayerAnimator animator, PlayerInteractor interactor, PlayerStatus status, InputAction moveAction, InputAction sprintAction)
        {
            _movement = movement;
            _animator = animator;
            _interactor = interactor;
            _status = status;

            _moveAction = moveAction;
            _sprintAction = sprintAction;

            _direction = ActorDirection.Down;
            _moveState = PlayerMoveState.Idle;

            SetMoveState(_moveState, _direction);
        }

        private void Update()
        {
            if (_isControlLocked)
            {
                StopMovementInput();
                return;
            }

            Vector2 moveInput = ReadMoveInput(out bool hasMoveInput);

            if (_isInteracting)
            {
                if (!hasMoveInput)
                    return;

                // Contract:
                // Movement-cancellable interactions are expected to release control immediately.
                // If cancel can keep control locked or play a cancel animation later,
                // this method should return whether movement may continue this frame.
                RequestInteractionCancelByMovement();
            }

            ApplyMoveInput(moveInput, hasMoveInput);
        }

        public void SetInteractionControlMode(InteractionControlMode mode)
        {
            _interactionControlMode = mode;

            switch (mode)
            {
                case InteractionControlMode.None:
                    _isControlLocked = false;
                    _isInteracting = false;
                    break;

                case InteractionControlMode.Cancellable:
                    _isControlLocked = false;
                    _isInteracting = true;
                    StopMovement();
                    break;

                case InteractionControlMode.InteractionOnly:
                case InteractionControlMode.ControlLocked:
                    _isControlLocked = true;
                    _isInteracting = true;
                    StopMovement();
                    break;

                default:
                    Debug.LogWarning($"Unhandled interaction control mode: {mode}");
                    _interactionControlMode = InteractionControlMode.None;
                    _isControlLocked = false;
                    _isInteracting = false;
                    StopMovement();
                    break;
            }
        }

        private Vector2 ReadMoveInput(out bool hasMoveInput)
        {
            Vector2 moveInput = _moveAction.ReadValue<Vector2>();

            float sqrMag = moveInput.sqrMagnitude;
            if (sqrMag > 1f)
            {
                moveInput.Normalize();
                sqrMag = 1f;
            }

            hasMoveInput = sqrMag > _inputDeadzoneSqr;
            return moveInput;
        }

        private void ApplyMoveInput(Vector2 moveInput, bool hasMoveInput)
        {
            ActorDirection nextDirection = hasMoveInput
                ? GetDirection(moveInput)
                : _direction;

            PlayerMoveState nextState = GetMoveState(hasMoveInput);

            SetMoveState(nextState, nextDirection);

            _movement.SetMoveInput(moveInput);
            _movement.SetSpeedMultiplier(
                nextState == PlayerMoveState.Sprint ? _sprintMultiplier : 1f);
        }

        private PlayerMoveState GetMoveState(bool hasMoveInput)
        {
            if (!hasMoveInput)
                return PlayerMoveState.Idle;

            if (_sprintAction.IsPressed() && _status.CanSprint)
                return PlayerMoveState.Sprint;

            return PlayerMoveState.Walk;
        }

        private ActorDirection GetDirection(Vector2 input)
        {
            const float threshold = 0.0001f;

            if (input.sqrMagnitude <= threshold)
                return _direction;

            bool hasHorizontal = Mathf.Abs(input.x) > threshold;
            bool hasVertical = Mathf.Abs(input.y) > threshold;

            if (hasHorizontal && hasVertical)
            {
                ActorDirection horizontalDirection =
                    input.x > 0f ? ActorDirection.Right : ActorDirection.Left;

                ActorDirection verticalDirection =
                    input.y > 0f ? ActorDirection.Up : ActorDirection.Down;

                if (_direction == horizontalDirection ||
                    _direction == verticalDirection)
                {
                    return _direction;
                }

                return horizontalDirection;
            }

            if (hasHorizontal)
                return input.x > 0f ? ActorDirection.Right : ActorDirection.Left;

            return input.y > 0f ? ActorDirection.Up : ActorDirection.Down;
        }

        private void SetMoveState(PlayerMoveState nextState, ActorDirection nextDirection)
        {
            if (_moveState == nextState && _direction == nextDirection)
                return;

            _moveState = nextState;
            _direction = nextDirection;

            PlayerAnimationState animationState = _moveState.ToAnimationState();

            if (animationState == PlayerAnimationState.None)
                return;

            _animator.Play(animationState, _direction);
        }

        private void RequestInteractionCancelByMovement()
        {
            _interactor.Interact();
        }

        private void StopMovement()
        {
            _moveState = PlayerMoveState.Idle;
            _movement.SetMoveInput(Vector2.zero);
            _movement.SetSpeedMultiplier(1f);
        }

        private void StopMovementInput()
        {
            _movement.SetMoveInput(Vector2.zero);
        }

    }
}

