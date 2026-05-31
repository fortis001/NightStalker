using UnityEngine;

namespace NightStalker.Actors.Player
{
    public class PlayerInteractor : MonoBehaviour
    {
        private PlayerController _controller;
        private PlayerAnimator _animator;

        public void Init(PlayerController controller, PlayerAnimator animator)
        {
            _controller = controller;
            _animator = animator;
        }

        public void StartRepair()
        {
            _controller.SetMovable(false);
            _controller.SetRepairing(true);

            _animator.Play(PlayerAnimationState.Crouch, _controller.Direction);
        }

        public void StopRepair()
        {
            _controller.SetRepairing(false);
            _controller.SetMovable(true);
        }
    }
}

