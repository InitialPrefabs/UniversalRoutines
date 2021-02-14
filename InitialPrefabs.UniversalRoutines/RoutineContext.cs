using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("InitialPrefabs.UniversalRoutines.Tests")]
namespace InitialPrefabs.UniversalRoutines {

    // TODO: Support StopRoutine(...)
    // TODO: Support StopAllRoutines(...)
    // TODO: Support Try/Catch
    public class RoutineContext {

        internal Stack<IEnumerator> Contexts;

        public RoutineContext() {
            Contexts = new Stack<IEnumerator>(5);
        }

        public RoutineContext(IEnumerator enumerator) : this() {
            Add(enumerator);
        }

        public void Run() {
            if (!IsEmpty()) {
                var top = Contexts.Peek();
                var moved = top.MoveNext();

                var inner = top.Current as IEnumerator;

                // We know that the IEnumerator yielded a new IEnumerator
                if (inner != null && moved) {
                    Contexts.Push(inner);
                }

                // If it did not move, then we hit the end.
                if (!moved) {
                    // Pop off the most recent coroutine from the stack.
                    Contexts.Pop();
                }
            }
        }

        public void Add(IEnumerator enumerator) {
            Contexts.Push(enumerator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty() {
            return Contexts.Count == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() {
            Contexts.Clear();
        }
    }
}
