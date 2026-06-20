using System.Collections.Generic;
using NightStalker.Actors.Stalker;
using NightStalker.GamePlay.Entities;
using NightStalker.GamePlay.InGame;
using UnityEngine;

namespace NightStalker.Actors
{
    public class StalkerActor : MonoBehaviour
    {
        [Header("Stalker Components")]
        [SerializeField] StalkerAI _stalkerAI;
        [SerializeField] StalkerMovement _stalkerMovement;
        [SerializeField] StalkerAnimator _stalkerAnimator;
        [SerializeField] StalkerSensor _stalkerSensor;
        [Header("Unity Components")]
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private Animator _unityAnimator;

        private PlayerActor _player;

        public void Init(PlayerActor player, InGameTimeManager timeManager, List<PowerGenerator> remainGenerator)
        {
            _player = player;

            _stalkerSensor.Init();
            _stalkerAnimator.Init(_unityAnimator, timeManager);
            _stalkerMovement.Init(_rigidbody, timeManager);
            _stalkerAI.Init(_stalkerMovement, _stalkerSensor, _stalkerAnimator, remainGenerator, _player);

            _player.OnEnteredCabinet -= HandleEnteredCabinet;
            _player.OnEnteredCabinet += HandleEnteredCabinet;
        }
        private void HandleEnteredCabinet()
        {
            _stalkerAI.StopChasingTarget();
        }
        private void OnDestroy()
        {
            if(_player != null )
            {
                _player.OnEnteredCabinet += HandleEnteredCabinet;
            }
        }
    }
}

