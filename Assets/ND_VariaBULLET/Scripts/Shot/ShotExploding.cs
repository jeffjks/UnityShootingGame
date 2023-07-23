#region Script Synopsis
    //A shot that has it's own controller/emitters. It triggers the child controller when the timer is reached and destroys the parent shot.
    //Learn more about this custom shot type at: https://neondagger.com/variabullet2d-scripting-guide/#exploding-bullets
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class ShotExploding : ShotNonPhysics, IRePoolable
    {
        [Header("Explode Settings")]

        [Range(1, 20)]
        [Tooltip("Sets a timer in frames after which the shot explodes.")]
        public int ExplodeTimer = 1;

        [Range(1, 10)]
        [Tooltip("Sets the rate at which the shot slows before it explodes. [higher = abrupt; lower = gradual].")]
        public int SlowdownRate = 1;

        public override void Start()
        {
            base.Start();
            
            rend = GetComponent<SpriteRenderer>();
        }

        public override void Update()
        {
            base.Update();

            OnEventTimerDo(
                shot => {
                    shot.ShotSpeed -= SlowdownRate * shot.ShotSpeed / 2 * scale;

                    if (shot.ShotSpeed < 5 & shot.ShotSpeed > 2)
                    {
                        rend.enabled = false; //makes invisible immediately before destroying
                    }
                    else if (shot.ShotSpeed < 2)
                        RePoolOrDestroy();

                }, ExplodeTimer * 5
            );
        }

        public override void RePool(IPooler poolingScript)
        {
            rend.enabled = true;
            base.RePool(poolingScript);
        }
    }
}