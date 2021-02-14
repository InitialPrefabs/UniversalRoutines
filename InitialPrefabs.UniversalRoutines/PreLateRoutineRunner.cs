using System.Collections;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace InitialPrefabs.UniversalRoutines {

    public static class PreLateRoutineRunner {

        internal static ContextRunner ContextRunner = new ContextRunner(10);
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Setup() {
            var loop = PlayerLoop.GetCurrentPlayerLoop();

            for (int i = 0; i < loop.subSystemList.Length; i++) {
                if (loop.subSystemList[i].type == typeof(PreLateUpdate)) {
                    loop.subSystemList[i].updateDelegate += PreLateUpdate;
                }
            }

            PlayerLoop.SetPlayerLoop(loop);
        }

        internal static void PreLateUpdate() {
            // TODO: Add an update routine to iterate through all coroutines and finalize them.
            ContextRunner.Run();
        }

        public static void Push(IEnumerator enumerator) {
            ContextRunner.PushRoutine(enumerator);
        }
    }
}
