using System.Collections;
using NUnit.Framework;
using UnityEngine;

namespace InitialPrefabs.UniversalRoutines.Tests {

    public class ContextRunnerTests {

        ContextRunner contextRunner;

        [SetUp]
        public void Setup() {
            contextRunner = new ContextRunner(5);

            Assert.AreEqual(5, contextRunner.Active.Capacity);
            Assert.AreEqual(5, contextRunner.Free.Capacity);
        }

        [Test]
        public void PushCoroutine() {
            contextRunner.PushRoutine(Test(), GetHashCode());
            Assert.AreEqual(1, contextRunner.Active.Count, "ContextRunner did not store an IEnumerator to track");
        }

        [Test]
        public void StopCoroutine() {
            contextRunner.PushRoutine(Test(), 1);
            contextRunner.Stop(1);

            Assert.AreEqual(0, contextRunner.Active.Count, "Active list did not pool the count");
            Assert.AreEqual(5, contextRunner.Free.Count, "Free list did not pool the recently freed object");
        }

        [Test]
        public void StopAll() {
            for (int i = 0; i < 5; i++) {
                contextRunner.PushRoutine(Test(), i + 1);
            }
            Assert.AreEqual(5, contextRunner.Active.Count, "Did not push 5 coroutines");
            Assert.AreEqual(0, contextRunner.Free.Count, "Did not use 5 free slots");
            contextRunner.StopAll();

            Assert.AreEqual(0, contextRunner.Active.Count, "Did not stop all coroutines");
        }

        [Test]
        public void PushCoroutineWithoutFreeSlots() {
            contextRunner = new ContextRunner(0);
            Assert.AreEqual(0, contextRunner.Active.Count, "Allocated when shouldn't have");
            Assert.AreEqual(0, contextRunner.Free.Count, "Allocated when shouldn't have");

            contextRunner.PushRoutine(Test(), 1);
            Assert.AreEqual(1, contextRunner.Active.Count, "Did not allocate a free slot.");
        }

        [Test]
        public void ContextRunnerCleansItselfUp() {
            contextRunner.PushRoutine(Test(), 1);
            Assert.AreEqual(1, contextRunner.Active.Count, "Did not use a free index");
            Assert.AreEqual(4, contextRunner.Free.Count, "Did not use a free resource");

            for (int i = 0; i < 2; i++) {
                contextRunner.Run();
            }

            Assert.AreEqual(0, contextRunner.Active.Count, "Did not free itself after finishing");
            Assert.AreEqual(5, contextRunner.Free.Count, "Did not free itself after finishing");
        }

        IEnumerator Test() {
            yield return 1;
        }
    }
}
