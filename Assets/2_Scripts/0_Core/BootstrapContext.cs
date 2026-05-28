using System.Collections.Generic;
using LSH.Core;
using UnityEngine;

namespace NightStalker.Core
{
    public class BootstrapContext : MonoBehaviour, ICoreBootstrapContext
    {
        [SerializeField] private CoreSceneSettings _sceneSettings;

        public IEnumerable<TimeChannelReference> TimeChannels =>
                TimeChannelName.All;

        public CoreSceneSettings SceneSettings => _sceneSettings;
    }
}

