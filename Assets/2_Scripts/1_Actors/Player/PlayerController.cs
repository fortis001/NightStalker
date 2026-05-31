using LSH.Core;
using NightStalker.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NightStalker.Actors.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float _sprintMultiplier = 3f;

        private PlayerMovement _movement;
        private PlayerAnimator _animator;
        private PlayerInteractor _interactor;
        private PlayerStatus _status;

        private InputAction _moveAction;

        private InputAction _interactAction;
        private InputAction _sprintAction;

        private bool _isMovable = true;
        private bool _isRepairing = false;
        private ActorDirection _direction;
        private PlayerMoveState _moveState;

        public ActorDirection Direction => _direction;
        public PlayerMoveState MoveState => _moveState;

        public void Init(PlayerMovement movement, PlayerAnimator animator, PlayerInteractor interactor, PlayerStatus status)
        {
            _movement = movement;
            _animator = animator;
            _interactor = interactor;
            _status = status;

            _moveAction = InputManager.Instance.GetAction(InputActionName.Move);
            _interactAction = InputManager.Instance.GetAction(InputActionName.Interact);
            _sprintAction = InputManager.Instance.GetAction(InputActionName.Sprint);

            _interactAction.performed -= HandleInteractKeyPressed;
            _interactAction.performed += HandleInteractKeyPressed;

            _direction = ActorDirection.Down;
            _moveState = PlayerMoveState.Idle;
            SetMoveState(_moveState, _direction);
        }

        private void Update()
        {
            if (!_isMovable)
            {
                _movement.SetMoveInput(Vector2.zero);
                return;
            }

            Vector2 moveInput = _moveAction.ReadValue<Vector2>();

            if (_isRepairing)
            {
                if (moveInput.sqrMagnitude > 0.0001f)
                {
                    _interactor.StopRepair();
                }
                else
                {
                    _movement.SetMoveInput(Vector2.zero);
                    return;
                }
            }

            if (moveInput.sqrMagnitude > 1f)
                moveInput.Normalize();

            ActorDirection nextDirection = _direction;

            if (moveInput.sqrMagnitude > 0.0001f)
                nextDirection = GetDirection(moveInput);

            PlayerMoveState nextState;

            if (moveInput.sqrMagnitude <= 0.0001f)
                nextState = PlayerMoveState.Idle;
            else if (_sprintAction.IsPressed() && _status.CanSprint)
                nextState = PlayerMoveState.Sprint;
            else
                nextState = PlayerMoveState.Walk;

            SetMoveState(nextState, nextDirection);

            _movement.SetMoveInput(moveInput);
            _movement.SetSpeedMultiplier(nextState == PlayerMoveState.Sprint ? _sprintMultiplier : 1f);
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

        public void SetRepairing(bool state)
        {
            _isRepairing = state;
        }

        public void SetMovable(bool state)
        {
            _isMovable = state;
        }

        private void HandleInteractKeyPressed(InputAction.CallbackContext context)
        {

        }

        private void OnDestroy()
        {
            if (InputManager.Instance == null) return;
            _interactAction.performed -= HandleInteractKeyPressed;
        }
    }
}

