using NightStalker.Actors.Player;
using NightStalker.GamePlay.InGame;
using UnityEngine;

namespace NightStalker.Actors
{
    public class PlayerActor : MonoBehaviour
    {
        [SerializeField] private PlayerController _controller;
        [SerializeField] private PlayerMovement _movement;
        [SerializeField] private PlayerAnimator _animator;
        [SerializeField] private PlayerInteractor _interactor;

        public void Init(InGameTimeManager timeManager)
        {
            _controller.Init();
            _movement.Init(timeManager);
        }
    }
}

