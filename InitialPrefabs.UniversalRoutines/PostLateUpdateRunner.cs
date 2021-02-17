using System.Collections;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace InitialPrefabs.UniversalRoutines {

    // TODO: Add all the static methods here.
    public static class PostLateUpdateRunner {

        internal static ContextRunner ContextRunner = new ContextRunner(10);
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Setup() {
            var loop = PlayerLoop.GetCurrentPlayerLoop();

            for (int i = 0; i < loop.subSystemList.Length; i++) {
                if (loop.subSystemList[i].type == typeof(PostLateUpdate)) {
                    loop.subSystemList[i].updateDelegate += PostLateUpdate;
                }
            }

            PlayerLoop.SetPlayerLoop(loop);
        }

        internal static void PostLateUpdate() {
            ContextRunner.Run();
        }

        public static void Push(IEnumerator enumerator, int id) {
            ContextRunner.PushRoutine(enumerator, id);
        }
    }
}
