using System;
using NightStalker.GamePlay.InGame;
using UnityEngine;

namespace NightStalker.Actors.Stalker
{
    public class StalkerMovement : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _stopDistance = 0.8f;

        private Rigidbody2D _rigidbody;
        private InGameTimeManager _timeManager;

        private Transform _targetTransform;
        private Vector2 _targetPosition;
        private bool _hasTarget;
        private bool _hasArrived;
        private bool _isPaused;
        private ActorDirection _direction = ActorDirection.Down;
        private float _speedMultiplier = 1f;

        public ActorDirection Direction => _direction;

        public event Action OnArrivedTarget;
        public event Action<ActorDirection> OnDirectionChanged;

        public void Init(Rigidbody2D rigidbody, InGameTimeManager timeManager)
        {
            _rigidbody = rigidbody;
            _timeManager = timeManager;

            _isPaused = false;
        }

        public void SetTarget(Transform target)
        {
            _targetTransform = target;
            _hasTarget = target != null;
            _hasArrived = false;
        }

        public void SetTargetPosition(Vector2 targetPosition)
        {
            _targetTransform = null;
            _targetPosition = targetPosition;
            _hasTarget = true;
            _hasArrived = false;
        }

        public void ClearTarget()
        {
            _targetTransform = null;
            _hasTarget = false;
            _hasArrived = false;
        }

        public void SetSpeedMultiplier(float value)
        {
            _speedMultiplier = value;
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }

        private void FixedUpdate()
        {
            if(_isPaused) return;

            if (!_hasTarget)
                return;

            Vector2 targetPosition = _targetTransform != null
                ? _targetTransform.position
                : _targetPosition;

            Vector2 currentPosition = _rigidbody.position;
            Vector2 toTarget = targetPosition - currentPosition;

            if (toTarget.sqrMagnitude <= _stopDistance)
            {
                ArriveTarget();
                return;
            }

            Vector2 direction = toTarget.normalized;

            UpdateDirection(direction);

            float speed = _moveSpeed * _speedMultiplier;

            _rigidbody.MovePosition(
                currentPosition +
                direction * speed * _timeManager.GameFixedDeltaTime);
        }

        private void UpdateDirection(Vector2 moveDirection)
        {
            if (moveDirection.sqrMagnitude <= 0.0001f)
                return;

            ActorDirection nextDirection = GetDirection(moveDirection);

            if (_direction == nextDirection)
                return;

            _direction = nextDirection;
            OnDirectionChanged?.Invoke(_direction);
        }

        private ActorDirection GetDirection(Vector2 direction)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                return direction.x > 0f
                    ? ActorDirection.Right
                    : ActorDirection.Left;

            return direction.y > 0f
                ? ActorDirection.Up
                : ActorDirection.Down;
        }

        private void ArriveTarget()
        {

            if (_hasArrived)
                return;

            _hasArrived = true;
            OnArrivedTarget?.Invoke();
        }
    }
}

