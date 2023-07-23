#region Script Synopsis
    //Stepped automator, which automates controller parameters in a series of stepped intervals.
    //Becomes attached to a controller via the attached spreadpattern (controller) script.
    //Learn more about automators at: https://neondagger.com/variabullet2d-in-depth-controller-guide/#automators
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class AutomateStepped : AutomateBase
    {
        [Tooltip("Sets time interval frame for moving from one step to the next. [higher number = longer delay].")]
        public int Interval;

        [Tooltip("Sets values as a series of steps for the control being modified.")]
        public float[] Steps;

        [Tooltip("Sets the amount of trigger increments before this automation procedure ends. [0 = infinite].")]
        public int TriggerPasses;
        private int triggersPassed = 0;

        [Tooltip("Prints out the amount of frames it took for the entire automation procedure to complete.")]
        public string FinishTimeDebug;

        private int index;
        private bool isFwd = true;

        protected override void Awake()
        {
            Interval = Mathf.Abs(Interval);
            TriggerPasses = System.Math.Abs(TriggerPasses);

            accumulator = Interval + 1;
            index = -1;

            base.Awake();
        }

        void Update()
        {
            delay.RunOnce(InitialDelay);
            if (!delay.Flag) return;

            if (Steps.Length == 0) { Utilities.Warn("No steps have been set for ", this, transform.parent.parent);  return; }

            accumulator += 4;
            accumulatorTotal += 1;
            controlLink[Destination]((method(0, Steps.Length - 1, Interval)));
        }

        protected override float SinglePass(float start, float end, int interval)
        {
            if (index == end)
            {
                this.enabled = false;
                getCompletionTime(ref FinishTimeDebug);
            }
                
                
            if (accumulator > interval)
            {
                index++;

                accumulator = 0;
                triggerCheck();
            } 

            return Steps[index];
        }

        protected override float Continuous(float start, float end, int interval)
        {
            if (accumulator > interval)
            {
                if (index < end)
                    index++;
                else
                    index = (int)start;
                    
                accumulator = 0;
                triggerCheck();
            }
         
            return Steps[index];
        }

        protected override float PingPong(float start, float end, int interval)
        {
            if (accumulator > interval)
            {
                accumulator = 0;

                if (Steps.Length > 1)
                {
                    if (isFwd)
                    {
                        if (index < end)
                            index++;
                        else
                            index = (int)end - 1; isFwd = false; accumulator = 0;
                    }
                    else
                    {
                        if (index > start)
                            index--;
                        else { index = (int)start + 1; isFwd = true; accumulator = 0; }
                    }
                }
                else
                {
                    index = 0;
                    triggerCheck();
                    return Steps[index];
                }

                triggerCheck();
            }

            return Steps[index];
        }

        protected override float Randomized(float start, float end, int interval)
        {
            if (accumulator > interval)
            {
                int rand = index;

                if (Steps.Length > 2)
                    while (index == rand)
                        rand = Random.Range((int)start, (int)end + 1);
                else if (Steps.Length > 1)
                    rand = Random.Range((int)start, (int)end + 1);
                else
                    rand = 0;

                index = rand;
                accumulator = 0;
                triggerCheck();
            }

            return Steps[index];
        }

        private void triggerCheck()
        {
            triggersPassed++;

            if (triggersPassed == TriggerPasses)
            {
                this.enabled = false;
                getCompletionTime(ref FinishTimeDebug);
            }
        }
    }
}