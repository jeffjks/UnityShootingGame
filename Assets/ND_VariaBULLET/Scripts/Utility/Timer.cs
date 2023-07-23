#region Script Synopsis
    //An object that stores a typical counter as a float and increments/resets via available methods.
    //Examples: most places a counter is used (and particularly if it needs to be framerate independent)
#endregion

using UnityEngine;
using System.Collections;

namespace ND_VariaBULLET
{
    public struct Timer
    {
        public bool Flag;
        private int counter;
        public int GetCounter { get { return counter; } }
        private readonly int reset;

        public Timer(int startsAt)
        {
            Flag = false;
            counter = reset = startsAt;
        }

        public void Run(int limit)
        {
            if (counter < limit)
            {
                counter += 1;
                Flag = false;
            }
            else
            {
                counter = reset;
                Flag = true;
            }
        }

        public IEnumerator RunForFrames(int limit, System.Action<int> action)
        {
            while (counter < limit)
            {
                action(counter);

                counter += 1;
                Flag = false;

                yield return null;
            }

            counter = reset;
            Flag = true;
        }

        public IEnumerator WaitForFrames(int limit)
        {
            while (counter < limit)
            {
                counter += 1;
                yield return null;
            }

            counter = reset;
        }

        public void RunOnce(int limit)
        {
            if (Flag)
                return;

            counter += 1;
            Flag = !(counter < limit);
        }

        public void Reset()
        {
            counter = reset;
            Flag = false;
        }

        public void ForceFlag(int limit)
        {
            counter = limit;
        }
    }
}