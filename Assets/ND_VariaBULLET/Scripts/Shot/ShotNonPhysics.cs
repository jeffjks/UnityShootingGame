#region Script Synopsis
    //The base class for all non-physics type shots that are not re-poolable.
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class ShotNonPhysics : ShotBaseColorizable
    {
        private Vector2 move;

        public override void Update()
        {
            base.Update();
            movement();
        }

        private void movement()
        {
            move.x = scaledSpeed * Trajectory.x / Application.targetFrameRate * Time.timeScale;
            move.y = scaledSpeed * Trajectory.y / Application.targetFrameRate * Time.timeScale;

            transform.position += new Vector3(move.x, move.y, 0);
        }
    }
}