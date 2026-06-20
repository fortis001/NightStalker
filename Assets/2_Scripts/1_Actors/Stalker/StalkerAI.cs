using System.Collections.Generic;
using NightStalker.GamePlay.Entities;
using UnityEngine;

namespace NightStalker.Actors.Stalker
{
    
    public class StalkerAI : MonoBehaviour
    {
        private StalkerMovement _movement;
        private StalkerSensor _sensor;
        private StalkerAnimator _animator;

        private ActorDirection _direction;
        private StalkerState _state;

        private List<PowerGenerator> _remainGenerators;
        private PowerGenerator _patrolTargetGenerator;

        private PlayerActor _player;

        public void Init(StalkerMovement movement, StalkerSensor sensor, StalkerAnimator animator, List<PowerGenerator> remainGenerators, PlayerActor player)
        {
            _movement = movement;
            _sensor = sensor;
            _animator = animator;
            _remainGenerators = remainGenerators;
            _player = player;

            if(_remainGenerators.Count == 0)
            {
                Debug.Log("remain Generator List is Null");
            }

            _patrolTargetGenerator = _remainGenerators[0];

            _movement.OnArrivedTarget -= HandleArrivedTarget;
            _movement.OnArrivedTarget += HandleArrivedTarget;
            _movement.OnDirectionChanged -= HandleDirectionChanged;
            _movement.OnDirectionChanged += HandleDirectionChanged;

            _sensor.OnPlayerInSight -= HandlePlayerInSight;
            _sensor.OnPlayerInSight += HandlePlayerInSight;
            _sensor.OnPlayerInAttackRange -= HandlePlayerInAttackRange;
            _sensor.OnPlayerInAttackRange += HandlePlayerInAttackRange;

            _animator.OnAnimationFinished -= HandleAnimationFinished;
            _animator.OnAnimationFinished += HandleAnimationFinished;

            _direction = ActorDirection.Down;
            _state = StalkerState.Standing;

            ReturnToPatrol();
        }

        public void StopChasingTarget()
        {
            _movement.ClearTarget();
            ReturnToPatrol();
        }

        private void ReturnToPatrol()
        {
            TryGetRandomPatrolTarget(out PowerGenerator target);

            _patrolTargetGenerator = target;

            ChangeState(StalkerState.Patrol);
            _movement.SetTargetPosition(target.transform.position);
        }
        private bool TryGetRandomPatrolTarget(out PowerGenerator target)
        {
            target = null;

            int count = _remainGenerators.Count;

            if (count == 0)
                return false;

            int currentIndex = _remainGenerators.IndexOf(_patrolTargetGenerator);

            if (count == 1)
            {
                if (currentIndex == 0)
                    return false;

                target = _remainGenerators[0];
                return true;
            }

            int randomIndex;

            if (currentIndex < 0)
            {
                randomIndex = Random.Range(0, count);
            }
            else
            {
                randomIndex = Random.Range(0, count - 1);

                if (randomIndex >= currentIndex)
                    randomIndex++;
            }

            target = _remainGenerators[randomIndex];
            return true;
        }

        private void HandleArrivedTarget()
        {
            if (_state == StalkerState.Attacking) return;

            ReturnToPatrol();
        }

        private void HandleDirectionChanged(ActorDirection direction)
        {
            if (_state == StalkerState.Attacking) return;

            _direction = direction;

            RefreshAnimation();
        }

        private void HandlePlayerInSight(PlayerActor player)
        {
            if (_state == StalkerState.Chasing) return;

            if (_state == StalkerState.Attacking) return;

            if (_player.IsHidden) return;

            ChangeState(StalkerState.Chasing);

            _movement.SetTarget(player.transform);

            RefreshAnimation();
        }

        private void HandlePlayerInAttackRange(PlayerActor player)
        {
            if (_state == StalkerState.Attacking) return;

            if (_player.IsHidden) return;

            _movement.SetTarget(player.transform);

            ChangeState(StalkerState.Attacking);

            StartAttack();
        }

        private void HandleAnimationFinished(StalkerAnimationState animation)
        {
            if (animation != StalkerAnimationState.Attack) return;

            if (_state != StalkerState.Attacking) return;

            ChangeState(StalkerState.Chasing);
        }

        private void RefreshAnimation()
        {
            if (_state == StalkerState.Attacking) return;

            StalkerAnimationState animationState = _state.ToAnimationState();

            _animator.Play(animationState, _direction);
            _sensor.SetDirection(_direction);
        }

        private void StartAttack()
        {
            StalkerAnimationState animationState = _state.ToAnimationState();

            _sensor.SetDirection(_direction);
            _animator.PlayAndNotifyFinished(animationState, _direction);
        }

        private void ChangeState(StalkerState state)
        {
            switch (state)
            {
                case StalkerState.Attacking:
                    _state = StalkerState.Attacking;
                    _movement.Pause();
                    _movement.SetSpeedMultiplier(0f);
                    break;
                case StalkerState.Patrol:
                    _state = StalkerState.Patrol;
                    _movement.Resume();
                    _movement.SetSpeedMultiplier(1f);
                    break;
                case StalkerState.Stalking:
                    _state = StalkerState.Stalking;
                    _movement.Resume();
                    _movement.SetSpeedMultiplier(1.2f);
                    break;
                case StalkerState.Chasing:
                    _state = StalkerState.Chasing;
                    _movement.Resume();
                    _movement.SetSpeedMultiplier(1.5f);
                    break;
            }

            RefreshAnimation();
        }

        private void OnDestroy()
        {
            if(_movement != null)
            {
                _movement.OnArrivedTarget -= HandleArrivedTarget;
                _movement.OnDirectionChanged -= HandleDirectionChanged;
            }

            if(_sensor != null)
            {
                _sensor.OnPlayerInSight -= HandlePlayerInSight;
                _sensor.OnPlayerInAttackRange -= HandlePlayerInAttackRange;
            }
            if(_animator != null)
            {
                _animator.OnAnimationFinished -= HandleAnimationFinished;
            }
        }
    }
}

