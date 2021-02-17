using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace InitialPrefabs.UniversalRoutines {

    // TODO: Support StopRoutine(...)
    // TODO: Support StopAllRoutines(...)
    // TODO: Support Try/Catch
    public class RoutineContext {

        internal Stack<IEnumerator> Contexts;
        internal int ID;

        public RoutineContext() {
            Contexts = new Stack<IEnumerator>(5);
            ID = 0;
        }

        public RoutineContext(IEnumerator enumerator, int id) {
            Contexts = new Stack<IEnumerator>(5);
            Initialize(enumerator, id);
        }

        public void Run() {
            if (!IsEmpty()) {
                var top = Contexts.Peek();

                try {
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
                } catch(Exception err) {
                    Reset();
                    Debug.LogError($"RoutineContext caught an error! Stopping the coroutine and cleaning up!\n" + 
                        $"{err.StackTrace}");
                }
            }
        }

        public void Initialize(IEnumerator enumerator, int id) {
            if (ID != 0 || enumerator == null) {
                throw new InvalidOperationException("Cannot reinitialize a RoutineContext that has been initialized!");
            }

            Contexts.Push(enumerator);
            ID = id;
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
            ID = 0;
        }
    }
}
