using LSH.Core;
using NightStalker.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NightStalker.Actors.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] PlayerMovement _movement;
        
        private InputAction _moveAction;

        private InputAction _interactAction;
        private InputAction _sprintAction;

        public void Init()
        {
            _moveAction = InputManager.Instance.GetAction(InputActionName.Move);
            _interactAction = InputManager.Instance.GetAction(InputActionName.Interact);
            _sprintAction = InputManager.Instance.GetAction(InputActionName.Sprint);

            _interactAction.performed -= HandleInteractKeyPressed;
            _interactAction.performed += HandleInteractKeyPressed;
            _sprintAction.performed -= HandleSprintKeyPressed;
            _sprintAction.performed += HandleSprintKeyPressed;
        }

        private void Update()
        {
            Vector2 moveInput = _moveAction.ReadValue<Vector2>();
            moveInput.Normalize();
            _movement.SetMoveInput(moveInput);
        }

        private void HandleInteractKeyPressed(InputAction.CallbackContext context)
        {

        }

        private void HandleSprintKeyPressed(InputAction.CallbackContext context)
        {

        }

        private void OnDestroy()
        {
            if (InputManager.Instance == null) return;
            _interactAction.performed -= HandleInteractKeyPressed;
            _sprintAction.performed -= HandleSprintKeyPressed;
        }
    }
}

