using System;
using System.Collections;
using NUnit.Framework;

namespace InitialPrefabs.UniversalRoutines.Tests {

    public class RoutineContextTests {

        RoutineContext context;

        [SetUp]
        public void Setup() {
            context = new RoutineContext();
        }

        // TODO: Add tests to destroy an IEnumerator and catch if there is an error.

        [Test]
        public void RoutineAddedByDefault() {
            context = new RoutineContext(WaitFor1SecondOnce());

            Assert.AreEqual(1, context.Contexts.Count, "Enumerator not added with constructor.");
        }

        [Test]
        public void RoutinePushed() {
            context.Add(WaitFor1SecondOnce());

            Assert.False(context.IsEmpty());
            Assert.AreEqual(1, context.Contexts.Count);

            context.Add(WaitFor1SecondOnce());
            Assert.AreEqual(2, context.Contexts.Count);

            context.Reset();
            Assert.True(context.IsEmpty());
        }

        [Test]
        public void RoutineChaining() {
            context.Add(ChainTesting());
            Assert.AreEqual(1, context.Contexts.Count);

            var start = DateTime.Now;
            TimeSpan diff = DateTime.Now - start;

            while (diff.TotalSeconds < 3f) {
                context.Run();
                diff = DateTime.Now - start;
            }

            Assert.IsTrue(context.IsEmpty(), 
                $"Routine Context did not flush itself, {context.Contexts.Count} remaining");
        }

        IEnumerator WaitFor1SecondOnce() {
            yield return new WaitForNSeconds(1);
        }

        IEnumerator ChainTesting() {
            yield return new WaitForNSeconds(0);
            yield return new WaitForNSeconds(0);
        }
    }
}
