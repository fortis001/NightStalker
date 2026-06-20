using System;
using LSH.Core;
using NightStalker.Actors.Player;
using NightStalker.Core;
using NightStalker.GamePlay.InGame;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NightStalker.Actors
{
    public class PlayerActor : MonoBehaviour
    {
        [Header("Player Components")]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private PlayerAnimator _playerAnimator;
        [SerializeField] private PlayerInteractor _playerInteractor;
        [SerializeField] private PlayerStatus _playerStatus;
        [Header("Unity Components")]
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private Animator _unityAnimator;
        [SerializeField] private SpriteRenderer _spriteRenderer;


        public bool IsHidden { get; private set; }

        public event Action<PlayerAnimationState> OnAnimationFinished;
        public event Action OnEnteredCabinet;
        public event Action OnExitCabinet;

        public void Init(InGameTimeManager timeManager)
        {
            InputAction moveAction = InputManager.Instance.GetAction(InputActionName.Move);
            InputAction sprintAction = InputManager.Instance.GetAction(InputActionName.Sprint);
            InputAction interactAction = InputManager.Instance.GetAction(InputActionName.Interact);

            _playerAnimator.Init(_unityAnimator, timeManager);
            _playerAnimator.OnAnimationFinished -= HandleAnimationFinished;
            _playerAnimator.OnAnimationFinished += HandleAnimationFinished;

            _playerMovement.Init(_rigidbody, timeManager);
            _playerInteractor.Init(this, _playerController, _playerAnimator, _playerMovement, interactAction);
            _playerController.Init(_playerMovement, _playerAnimator, _playerInteractor, _playerStatus, moveAction, sprintAction);
            _playerStatus.Init(timeManager, _playerController);
        }

        public void TakeDamage(int damage, Vector2 stalkerPosition)
        {

        }

        public void EnterCabinet()
        {
            _collider.enabled = false;
            _spriteRenderer.enabled = false;
            IsHidden = true;
            OnEnteredCabinet?.Invoke();
        }

        public void ExitCabinet()
        {
            _collider.enabled = true;
            _spriteRenderer.enabled = true;
            IsHidden = false;
            OnExitCabinet?.Invoke();
        }

        private void HandleAnimationFinished(PlayerAnimationState animationState)
        {
            OnAnimationFinished?.Invoke(animationState);
        }

        private void OnDestroy()
        {
            _playerAnimator.OnAnimationFinished -= HandleAnimationFinished;
        }
    }
}

