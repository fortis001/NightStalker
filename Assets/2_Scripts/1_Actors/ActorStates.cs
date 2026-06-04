

namespace NightStalker.Actors
{
    public enum ActorDirection
    {
        None,
        Left,
        Right,
        Up,
        Down
    }

    public enum PlayerMoveState
    {
        None,
        Idle,
        Walk,
        Sprint,
    }

    public enum PlayerHealthState
    {
        None,
        Healthy,
        Injured,
        Dead,
    }

    public enum PlayerAnimationState
    {
        None,
        Idle,
        Walk,
        Sprint,
        TakeDamage,
        Interact,
        Crouch,
        Die,
    }

    public static class PlayerAnimationStateMapper
    {
        public static PlayerAnimationState ToAnimationState(this PlayerMoveState moveState)
        {
            return moveState switch
            {
                PlayerMoveState.Idle => PlayerAnimationState.Idle,
                PlayerMoveState.Walk => PlayerAnimationState.Walk,
                PlayerMoveState.Sprint => PlayerAnimationState.Sprint,
                _ => PlayerAnimationState.None
            };
        }
    }
}