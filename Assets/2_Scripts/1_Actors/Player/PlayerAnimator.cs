using System;
using System.Collections;
using NightStalker.GamePlay.InGame;
using UnityEngine;

namespace NightStalker.Actors.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        private Animator _animator;
        private InGameTimeManager _timeManager;

        private string _currentStateName;
        private PlayerAnimationState _currentState;
        private ActorDirection _currentDirection;

        private Coroutine _animationFinishRoutine;

        public event Action<PlayerAnimationState> OnAnimationFinished;

        public void Init(Animator animator, InGameTimeManager timeManager)
        {
            _animator = animator;
            _timeManager = timeManager;
        }

        public void Play(PlayerAnimationState state, ActorDirection direction)
        {
            if (state == PlayerAnimationState.None)
                return;

            string stateName = $"{state}_{direction}";

            if (_currentStateName == stateName)
                return;

            _currentStateName = stateName;
            _currentState = state;
            _currentDirection = direction;

            Debug.Log(stateName);
            _animator.Play(stateName);
        }

        public void PlayAndNotifyFinished(PlayerAnimationState state, ActorDirection direction)
        {
            if (state == PlayerAnimationState.None)
                return;

            Play(state, direction);

            if (_animationFinishRoutine != null)
            {
                StopCoroutine(_animationFinishRoutine);
                _animationFinishRoutine = null;
            }

            _animationFinishRoutine = StartCoroutine(
                WaitAnimationFinishedRoutine(state, direction));
        }

        private IEnumerator WaitAnimationFinishedRoutine(PlayerAnimationState animationState, ActorDirection direction)
        {
            if (_animator == null || _timeManager == null)
                yield break;

            string expectedStateName = $"{animationState}_{direction}";

            yield return null;

            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

            if (!stateInfo.IsName(expectedStateName))
            {
                _animationFinishRoutine = null;
                yield break;
            }

            float duration = stateInfo.length;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                if (_timeManager == null)
                    yield break;

                elapsedTime += _timeManager.GameDeltaTime;
                yield return null;
            }

            _animationFinishRoutine = null;
            OnAnimationFinished?.Invoke(animationState);
        }

    }
}

