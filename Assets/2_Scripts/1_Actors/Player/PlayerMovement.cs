using NightStalker.GamePlay.InGame;
using UnityEngine;

namespace NightStalker.Actors.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private float _moveSpeed = 3f;

        private InGameTimeManager _timeManager;

        private Vector2 _moveInput;

        public void Init(InGameTimeManager timeManager)
        {
            _timeManager = timeManager;
        }

        public void SetMoveInput(Vector2 moveInput)
        {
            _moveInput = moveInput.sqrMagnitude > 1f
                ? moveInput.normalized
                : moveInput;
        }

        private void FixedUpdate()
        {
            if (_timeManager == null)
                return;

            if (_moveInput.sqrMagnitude <= 0.0001f)
                return;

            _rigidbody.MovePosition(
                _rigidbody.position +
                _moveInput * _moveSpeed * _timeManager.GameFixedDeltaTime
            );
        }
    }
}

