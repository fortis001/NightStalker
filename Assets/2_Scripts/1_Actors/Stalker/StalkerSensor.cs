using System;
using LSH.Utils;
using UnityEngine;

namespace NightStalker.Actors.Stalker
{
    public class StalkerSensor : MonoBehaviour
    {
        [SerializeField] TriggerRelay2D _sight;
        [SerializeField] TriggerRelay2D _attackRange;

        public event Action<PlayerActor> OnPlayerInSight;
        public event Action<PlayerActor> OnPlayerInAttackRange;

        public void Init()
        {
            _sight.OnTriggerEntered -= HandleTriggerSight;
            _sight.OnTriggerEntered += HandleTriggerSight;

            _attackRange.OnTriggerEntered -= HandleTriggerAttackRange;
            _attackRange.OnTriggerEntered += HandleTriggerAttackRange;
        }

        public void SetDirection(ActorDirection direction)
        {
            _sight.transform.localRotation = direction switch
            {
                ActorDirection.Up => Quaternion.Euler(0f, 0f, 180f),
                ActorDirection.Right => Quaternion.Euler(0f, 0f, 90f),
                ActorDirection.Down => Quaternion.Euler(0f, 0f, 0f),
                ActorDirection.Left => Quaternion.Euler(0f, 0f, 270f),
                _ => _sight.transform.localRotation
            };
        }

        private void HandleTriggerSight(Collider2D collider)
        {
            PlayerActor player = collider.GetComponentInParent<PlayerActor>();

            if (player == null)
                return;

            OnPlayerInSight?.Invoke(player);
        }

        private void HandleTriggerAttackRange(Collider2D collider)
        {
            PlayerActor player = collider.GetComponentInParent<PlayerActor>();

            if (player == null)
                return;

            OnPlayerInAttackRange?.Invoke(player);
        }

        private void OnDestroy()
        {
            _sight.OnTriggerEntered -= HandleTriggerSight;
            _attackRange.OnTriggerEntered -= HandleTriggerAttackRange;
        }
    }
}

