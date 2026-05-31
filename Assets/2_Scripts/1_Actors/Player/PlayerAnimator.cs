using System;
using UnityEngine;

namespace NightStalker.Actors.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        Animator _animator;

        private string _currentStateName;
        private PlayerAnimationState _currentState;
        private ActorDirection _currentDirection;

        public event Action<PlayerAnimationState> OnAnimationFinished;

        public void Init(Animator animator)
        {
            _animator = animator;
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

            _animator.Play(stateName);
        }

        // Animation Event에서 호출할 메서드
        public void NotifyAnimationFinished()
        {
            OnAnimationFinished?.Invoke(_currentState);
        }
    }
}

