using UnityEngine;

namespace NightStalker.Rendering
{
    public class YSortRenderer : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Transform _sortPoint;
        [SerializeField] private int _offset;
        [SerializeField] private bool _sortOnlyOnce = true;

        private void Reset()
        {
            _renderer = GetComponent<Renderer>();
            _sortPoint = transform;
        }

        private void Awake()
        {
            if (_renderer == null)
                _renderer = GetComponent<Renderer>();

            if (_sortPoint == null)
                _sortPoint = transform;
        }

        private void Start()
        {
            UpdateSortingOrder();
        }

        private void LateUpdate()
        {
            if (_sortOnlyOnce)
                return;

            UpdateSortingOrder();
        }

        private void UpdateSortingOrder()
        {
            if (_renderer == null || _sortPoint == null)
                return;

            _renderer.sortingOrder = -(int)(_sortPoint.position.y * 100f) + _offset;
        }
    }
}