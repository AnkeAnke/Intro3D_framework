using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    /// <summary>
    /// Player control + Map rotation
    /// </summary>
    class Control
    {
        public Quaternion WorldRotation { get { return worldRotation; } }
        private Quaternion worldRotation = Quaternion.Identity;

        public void OnUpdateFrame(float timeSinceLastFrame)
        {
            if(Keyboard.GetState().IsKeyDown(Key.Up))
                worldRotation *= Quaternion.FromAxisAngle(Vector3.UnitX, (float)timeSinceLastFrame);
            if(Keyboard.GetState().IsKeyDown(Key.Down))
                worldRotation *= Quaternion.FromAxisAngle(Vector3.UnitX, -(float)timeSinceLastFrame);
            if(Keyboard.GetState().IsKeyDown(Key.Left))
                worldRotation *= Quaternion.FromAxisAngle(Vector3.UnitZ, -(float)timeSinceLastFrame);
            if(Keyboard.GetState().IsKeyDown(Key.Right))
                worldRotation *= Quaternion.FromAxisAngle(Vector3.UnitZ, (float)timeSinceLastFrame);
        }
    }
}
