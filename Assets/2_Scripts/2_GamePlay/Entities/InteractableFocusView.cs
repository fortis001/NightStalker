using UnityEngine;

namespace NightStalker.GamePlay.Entities
{
    public class InteractableFocusView : MonoBehaviour
    {
        [SerializeField] private Transform _target;

        [Header("Scale")]
        [SerializeField] private float _focusedScaleMultiplier = 1.08f;

        [Header("Shake")]
        [SerializeField] private float _shakeAngle = 3f;
        [SerializeField] private float _shakeSpeed = 3.5f;

        private Vector3 _originScale;
        private Quaternion _originRotation;

        private bool _isPlaying;
        private float _elapsedTime;

        private void Awake()
        {
            if (_target == null)
                _target = transform;

            _originScale = _target.localScale;
            _originRotation = _target.localRotation;
        }

        private void Update()
        {
            if (!_isPlaying)
                return;

            _elapsedTime += Time.deltaTime;

            float angle = Mathf.Sin(_elapsedTime * _shakeSpeed) * _shakeAngle;
            _target.localRotation = _originRotation * Quaternion.Euler(0f, 0f, angle);
        }

        public void Play()
        {
            if (_isPlaying)
                return;

            _isPlaying = true;
            _elapsedTime = 0f;

            _target.localScale = _originScale * _focusedScaleMultiplier;
        }

        public void Stop()
        {
            if (!_isPlaying)
                return;

            _isPlaying = false;
            _elapsedTime = 0f;

            _target.localScale = _originScale;
            _target.localRotation = _originRotation;
        }
    }
}
