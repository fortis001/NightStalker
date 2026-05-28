using LSH.Core;
using NightStalker.Actors;
using NightStalker.Core;
using UnityEngine;

namespace NightStalker.GamePlay.InGame
{
    public class InGameSceneManager : MonoBehaviour
    {
        [SerializeField] PlayerActor _player;
        [SerializeField] InGameTimeManager _timeManager;

        [Header("TestObj")]
        [SerializeField] InputManager _testInputManager;
        [SerializeField] TimeManager _testTimeManager;
        [SerializeField] BootstrapContext _testContext;

        void Start()
        {
            _testInputManager.Init();
            _testInputManager.SetActionMap(InputMapName.InGame);

            _testTimeManager.Init(_testContext);
            _player.Init(_timeManager);

        }


    }
}

