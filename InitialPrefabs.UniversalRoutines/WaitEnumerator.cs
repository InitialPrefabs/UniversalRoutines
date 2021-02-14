using System.Collections;
using UnityEngine;

namespace InitialPrefabs.UniversalRoutines {

    public struct WaitForNSeconds : IEnumerator {

        public object Current => null;

        float timeStampFuture;

        public WaitForNSeconds(float time) {
            timeStampFuture = Time.time + time;
        }

        public bool MoveNext() { 
            return Time.time < timeStampFuture;
        }

        public void Reset() { }
    }
}