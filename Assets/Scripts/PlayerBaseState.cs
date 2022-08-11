using UnityEngine;

namespace Assets.Scripts
{
    public abstract class PlayerBaseState
    {
        public abstract void EnterState(PlayerController_FSM player);
        public abstract void OnUpdate(PlayerController_FSM player);
        public abstract void OnFixedUpdate(PlayerController_FSM player);
        public abstract void OnCollisionEnter(Collision other, PlayerController_FSM player);
        public abstract void OnCollisionExit(Collision other, PlayerController_FSM player);
    }
}
