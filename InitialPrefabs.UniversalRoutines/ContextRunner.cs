using System.Collections;
using System.Collections.Generic;

namespace InitialPrefabs.UniversalRoutines {

    public class ContextRunner {

        List<RoutineContext> active;
        List<RoutineContext> free;

        public ContextRunner(int initialCapacity) {
            active = new List<RoutineContext>(initialCapacity);
            free = new List<RoutineContext>(initialCapacity);

            for (int i = 0; i < initialCapacity; i++) {
                free.Add(new RoutineContext());
            }
        }

        public void PushRoutine(IEnumerator enumerator) {
            if (free.Count == 0) {
                active.Add(new RoutineContext(enumerator));
            } else {
                var last = free.Count - 1;
                var context = free[last];
                free.RemoveAt(last);

                context.Add(enumerator);
                active.Add(context);
            }
        }

        public void Run() {
            // Run in reverse order, so we can clean up any finished contexts.
            for (int i = active.Count - 1; i >= 0; i--) {
                UnityEngine.Debug.Log($"Running: {i}");
                var current = active[i];
                current.Run();

                if (current.IsEmpty()) {
                    free.Add(current);
                    active.RemoveAt(i);
                }
            }
        }
    }
}
