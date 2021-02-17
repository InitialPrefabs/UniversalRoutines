using System.Collections;
using UnityEngine;

namespace InitialPrefabs.UniversalRoutines {

    // TODO: Add more WaitForIEnumerators such as RealTime
    /// <summary>
    /// Allows a wait for in game time.
    /// </summary>
    public struct WaitForNSeconds : IEnumerator {

        public object Current => null;

        float timeStampFuture;

        /// <summary>
        /// Constructs a new WaitForNSeconds instance with a future timestamp.
        /// </summary>
        /// <param name="wait">The amount of time in seconds to wait for.</param>
        public WaitForNSeconds(float wait) {
            timeStampFuture = Time.time + wait;
        }

        /// <summary>
        /// Polls Unity's Time class until the current Time has exceeded the future Time stamp.
        /// </summary>
        /// <returns>True, until Time.time exceed timeStampFuture</returns>
        public bool MoveNext() { 
            return Time.time < timeStampFuture;
        }

        public void Reset() { }
    }
}
