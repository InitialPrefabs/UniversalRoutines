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
            ContextRunner.Run();
        }

        public static void Push(IEnumerator enumerator, int id) {
            ContextRunner.PushRoutine(enumerator, id);
        }

        public static void Stop(int id) {
            ContextRunner.Stop(id);
        }

        public static void StopAll() {
            ContextRunner.StopAll();
        }
    }
}
