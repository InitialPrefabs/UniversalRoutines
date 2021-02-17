using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace InitialPrefabs.UniversalRoutines {

    // TODO: Test what happens you yield a null.

    /// <summary>
    /// A bucket which stores the IEnumerators which need to suspend execution.
    /// </summary>
    public class RoutineContext {

        internal Stack<IEnumerator> Contexts;
        internal int ID;

        /// <summary>
        /// Default constructor, does not iniitalize the bucket with an IEnumerator appended.
        /// </summary>
        public RoutineContext() {
            Contexts = new Stack<IEnumerator>(5);
            ID = 0;
        }

        /// <summary>
        /// Initializes the RoutineContext with a default IEnumerator and ID.
        /// </summary>
        /// <param name="enumerator"></param>
        /// <param name="id"></param>
        public RoutineContext(IEnumerator enumerator, int id) {
            Contexts = new Stack<IEnumerator>(5);
            Initialize(enumerator, id);
        }

        /// <summary>
        /// Updates the most recent IEnumerator in the bucket. When the IEnumerator is finished, 
        /// the RoutineContext will pop it off the stack. Similarly, this method attempts to queue 
        /// more IEnumerators if there is a chain.
        /// </summary>
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

        /// <summary>
        /// Initializes the bucket with an IEnumerator and an associated ID.
        /// </summary>
        /// <param name="enumerator">The yielding statement</param>
        /// <param name="id">A unique ID to represent this RoutineContext. You can use GetHashCode() as a unique ID.</param>
        public void Initialize(IEnumerator enumerator, int id) {
            if (id == 0) {
                throw new InvalidOperationException("An ID of 0 is invalid, please use another ID");
            }

            if (ID != 0 || enumerator == null) {
                throw new InvalidOperationException("Cannot reinitialize a RoutineContext that has been initialized!");
            }

            Contexts.Push(enumerator);
            ID = id;
        }
        
        /// <summary>
        /// Pushes a new IEnumerator on top of the stack.
        /// </summary>
        /// <param name="enumerator">Yielding statement to push</param>
        public void Add(IEnumerator enumerator) {
            Contexts.Push(enumerator);
        }

        /// <summary>
        /// Convenience function to check if the bucket is empty.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty() {
            return Contexts.Count == 0;
        }

        /// <summary>
        /// Flushes execution of all active and pending IEnumerators. Resets the ID to 0.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() {
            Contexts.Clear();
            ID = 0;
        }
    }
}
