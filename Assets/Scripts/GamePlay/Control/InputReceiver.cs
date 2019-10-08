using System;
using UnityEngine;

namespace GamePlay.Control
{
    internal class InputReceiver
    {
        private IControllable controllable;

        public InputReceiver(IControllable controllable)
        {
            this.controllable = controllable;
        }
        
        public void Move(Vector2 direction)
        {
            controllable.Move(direction.normalized);
        }

        public void Jump()
        {
            controllable.Jump();
        }
    }
}