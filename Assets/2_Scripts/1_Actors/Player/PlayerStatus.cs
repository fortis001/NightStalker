using System;
using System.Collections;
using LSH.Core;
using NightStalker.GamePlay.InGame;
using UnityEngine;

namespace NightStalker.Actors.Player
{
    public class PlayerStatus : MonoBehaviour
    {
        [SerializeField] private float _maxSprintGauge = 100f;
        [SerializeField] private float _sprintReducePerSecond = 25;
        [SerializeField] private float _sprintRecoverPerSecond = 20f;
        [SerializeField] private float _exhaustRecoverRatio = 0.3f;
        [SerializeField] private float _exhaustRecoverDelay = 1.5f;

        private InGameTimeManager _timeManager;
        private PlayerController _controller;


        private bool _isExhausted;
        private bool _canRecoverSprint;
        public float SprintGauge { get; private set; }
        public bool CanSprint => !_isExhausted && SprintGauge > 0f;

        public event Action OnExhausted;
        public event Action OnSprintRecovered;
        public event Action<float, float> OnSprintGaugeChanged;

        private Coroutine _exhaustCoroutine;


        public void Init(InGameTimeManager timeManager, PlayerController controller)
        {
            _timeManager = timeManager;
            _controller = controller;

            SprintGauge = _maxSprintGauge;
            _isExhausted = false;
            _canRecoverSprint = true;
        }

        private void FixedUpdate()
        {
            if (_timeManager == null)
                return;

            float fixedDeltaTime = _timeManager.GameFixedDeltaTime;

            if(_controller.MoveState == PlayerMoveState.Sprint)
            {
                ReduceSprintGauge(fixedDeltaTime);
            }
            else
            {
                RecoverSprintGauge(fixedDeltaTime);
            }
        }

        private void ReduceSprintGauge(float deltaTime)
        {
            if (_isExhausted)
                return;

            float previous = SprintGauge;
            SetSprintGauge(SprintGauge - _sprintReducePerSecond * deltaTime);

            if (previous > 0f && SprintGauge <= 0f)
            {
                Exhaust();
                OnExhausted?.Invoke();
            }
        }

        private void RecoverSprintGauge(float deltaTime)
        {
            if (!_canRecoverSprint)
                return;

            SetSprintGauge(SprintGauge + _sprintRecoverPerSecond * deltaTime);

            if (_isExhausted &&
                SprintGauge >= _maxSprintGauge * _exhaustRecoverRatio)
            {
                _isExhausted = false;
                OnSprintRecovered?.Invoke();
            }
        }

        private void SetSprintGauge(float value)
        {
            float nextValue = Mathf.Clamp(value, 0f, _maxSprintGauge);

            if (Mathf.Approximately(SprintGauge, nextValue))
                return;

            SprintGauge = nextValue;
            OnSprintGaugeChanged?.Invoke(SprintGauge, _maxSprintGauge);
        }

        private void Exhaust()
        {
            if (_isExhausted)
                return;

            _isExhausted = true;
            _canRecoverSprint = false;

            if (_exhaustCoroutine != null)
                StopCoroutine(_exhaustCoroutine);

            _exhaustCoroutine = StartCoroutine(ExhaustRecoverDelayRoutine());
        }

        private IEnumerator ExhaustRecoverDelayRoutine()
        {
            float elapsedTime = 0f;

            while (elapsedTime < _exhaustRecoverDelay)
            {
                elapsedTime += _timeManager.GameDeltaTime;
                yield return null;
            }

            _canRecoverSprint = true;
            _exhaustCoroutine = null;
        }

    }
}

