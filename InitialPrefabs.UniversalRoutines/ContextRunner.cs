using System.Collections;
using System.Collections.Generic;

namespace InitialPrefabs.UniversalRoutines {

    /// <summary>
    /// The ContextRunner stores all pending and running coroutines. This pools all RoutineContext to 
    /// avoid GC allocation.
    /// </summary>
    public class ContextRunner {

        internal List<RoutineContext> Active;
        internal List<RoutineContext> Free;

        /// <summary>
        /// Constructor to initialize the pool of RoutineContexts.
        /// </summary>
        /// <param name="initialCapacity">The number of RoutineContexts we want to initially store before allocating more memory.</param>
        public ContextRunner(int initialCapacity) {
            Active = new List<RoutineContext>(initialCapacity);
            Free = new List<RoutineContext>(initialCapacity);

            for (int i = 0; i < initialCapacity; i++) {
                Free.Add(new RoutineContext());
            }
        }

        /// <summary>
        /// Pushes a new IEnumerator with an id to the Active pool. If there is no free RoutineContext, 
        /// then the Active pool will allocate a new RoutineContext.
        /// </summary>
        /// <param name="enumerator"></param>
        /// <param name="id"></param>
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

        /// <summary>
        /// Runs all RoutineContexts. If the Active Pool is empty, the RoutineContext 
        /// is moved to the Free Pool and resetted.
        /// </summary>
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

        /// <summary>
        /// Stops a RoutineContext given an ID.
        /// </summary>
        /// <param name="id"></param>
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

        /// <summary>
        /// Flushes all Active RoutineContexts that are running and moves them to 
        /// the Free Pool.
        /// </summary>
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
