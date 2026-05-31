using NightStalker.Actors.Player;
using NightStalker.GamePlay.InGame;
using UnityEngine;

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
        [SerializeField] private Animator _unityAnimator;


        public void Init(InGameTimeManager timeManager)
        {
            _playerAnimator.Init(_unityAnimator);
            _playerMovement.Init(_rigidbody, timeManager);
            _playerInteractor.Init(_playerController, _playerAnimator);
            _playerStatus.Init(timeManager, _playerController);
            _playerController.Init(_playerMovement, _playerAnimator, _playerInteractor, _playerStatus);
        }

        public void Hit(Vector2 stalkerPosition)
        {

        }
    }
}

