

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
    public enum StalkerState
    {
        None,
        Standing,
        Patrol,
        Stalking,
        Chasing,
        Attacking,
    }
    public enum StalkerAnimationState
    {
        None,
        Idle,
        Walk,
        Run,
        Attack,
    }

    public static class AnimationStateMapper
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

        public static StalkerAnimationState ToAnimationState(this StalkerState moveState)
        {
            return moveState switch
            {
                StalkerState.Standing => StalkerAnimationState.Idle,
                StalkerState.Patrol => StalkerAnimationState.Walk,
                StalkerState.Stalking => StalkerAnimationState.Walk,
                StalkerState.Chasing => StalkerAnimationState.Run,
                StalkerState.Attacking => StalkerAnimationState.Attack,
                _ => StalkerAnimationState.None
            };
        }
    }
}