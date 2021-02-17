using System.Collections;
using System.Collections.Generic;

namespace InitialPrefabs.UniversalRoutines {

    public class ContextRunner {

        internal List<RoutineContext> Active;
        internal List<RoutineContext> Free;

        public ContextRunner(int initialCapacity) {
            Active = new List<RoutineContext>(initialCapacity);
            Free = new List<RoutineContext>(initialCapacity);

            for (int i = 0; i < initialCapacity; i++) {
                Free.Add(new RoutineContext());
            }
        }

        public void PushRoutine(IEnumerator enumerator, int id) {
            if (Free.Count == 0) {
                Active.Add(new RoutineContext(enumerator, id));
            } else {
                var last = Free.Count - 1;
                var context = Free[last];
                Free.RemoveAt(last);

                context.Initialize(enumerator, id);
                Active.Add(context);
            }
        }

        public void Run() {
            // Run in reverse order, so we can clean up any finished contexts.
            for (int i = Active.Count - 1; i >= 0; i--) {
                var current = Active[i];
                current.Run();

                if (current.IsEmpty()) {
                    Free.Add(current);
                    Active.RemoveAt(i);
                }
            }
        }

        public void Stop(int id) {
            for (int i = Active.Count - 1; i >= 0; i--) {
                var current = Active[i];
                if (current.ID == id) {
                    current.Reset();

                    Active.RemoveAt(i);
                    Free.Add(current);
                }
            }
        }

        public void StopAll() {
            for (int i = Active.Count - 1; i >= 0; i--) {
                var current = Active[i];
                current.Reset();
                Free.Add(current);
            }

            Active.Clear();
        }
    }
}
