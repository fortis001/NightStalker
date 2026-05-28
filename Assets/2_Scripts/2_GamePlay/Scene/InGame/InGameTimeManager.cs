using LSH.Core;
using NightStalker.Core;
using UnityEngine;


namespace NightStalker.GamePlay.InGame
{
    public class InGameTimeManager : MonoBehaviour
    {
        public bool IsGamePaused => TimeManager.Instance.IsPaused(TimeChannelName.Game);
        public float GameDeltaTime => TimeManager.Instance.GetDeltaTime(TimeChannelName.Game);
        public float GameFixedDeltaTime => TimeManager.Instance.GetFixedDeltaTime(TimeChannelName.Game);
        public float GameTime => TimeManager.Instance.GetTime(TimeChannelName.Game);

        public void PauseGame() => TimeManager.Instance.Pause(TimeChannelName.Game);
        public void ResumeGame() => TimeManager.Instance.Resume(TimeChannelName.Game);
    }
}

