using NightStalker.GamePlay.InGame;
using UnityEngine;

namespace NightStalker.Actors.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _walkSpeed = 1.5f;

        private Rigidbody2D _rigidbody;
        private InGameTimeManager _timeManager;

        private Vector2 _moveInput;
        private float _speedMultiplier = 1f;

        public void Init(Rigidbody2D rigidbody, InGameTimeManager timeManager)
        {
            _rigidbody = rigidbody;
            _timeManager = timeManager;
        }

        public void SetMoveInput(Vector2 moveInput)
        {
            _moveInput = moveInput.sqrMagnitude > 1f
                ? moveInput.normalized
                : moveInput;
        }

        public void SetSpeedMultiplier(float multipiler)
        {
            _speedMultiplier = multipiler;
        }

        public void SnapTo(Vector2 snapPosition)
        {
            _rigidbody.position = snapPosition;
        }

        private void FixedUpdate()
        {
            if (_timeManager == null)
                return;

            if (_moveInput.sqrMagnitude <= 0.0001f)
                return;

            float speed = _walkSpeed * _speedMultiplier;

            _rigidbody.MovePosition(
                _rigidbody.position +
                _moveInput * speed * _timeManager.GameFixedDeltaTime
            );
        }
    }
}

