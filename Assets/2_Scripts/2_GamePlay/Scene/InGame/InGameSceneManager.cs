using System.Collections.Generic;
using LSH.Core;
using NightStalker.Actors;
using NightStalker.Core;
using NightStalker.GamePlay.Entities;
using UnityEngine;

namespace NightStalker.GamePlay.InGame
{
    public class InGameSceneManager : MonoBehaviour
    {
        [SerializeField] PlayerActor _player;
        [SerializeField] InGameTimeManager _timeManager;
        [SerializeField] StalkerActor _stalker;

        [Header("TestObj")]
        [SerializeField] InputManager _testInputManager;
        [SerializeField] TimeManager _testTimeManager;
        [SerializeField] BootstrapContext _testContext;
        [SerializeField] List<PowerGenerator> _powerGenerators;
        [SerializeField] ExitGate _exitGate;
        [SerializeField] Cabinet _cabinet;

        void Start()
        {
            _testInputManager.Init();
            _testInputManager.SetActionMap(InputMapName.InGame);
            foreach(PowerGenerator generator in _powerGenerators)
            {
                generator.Init(_timeManager);
            }
            _cabinet.Init();

            _testTimeManager.Init(_testContext);
            _player.Init(_timeManager);
            _exitGate.Init(_timeManager);
            _stalker.Init(_player, _timeManager, _powerGenerators);
        }


    }
}

