using NUnit.Framework;
using System;
using System.Collections;

using Random = System.Random;

namespace InitialPrefabs.UniversalRoutines.Tests {

    public class RoutineContextTests {

        RoutineContext context;

        [SetUp]
        public void Setup() {
            context = new RoutineContext();
        }

        [Test]
        public void RoutineAddedByDefault() {
            context = new RoutineContext(WaitFor1SecondOnce(), 25);
            Assert.AreEqual(1, context.Contexts.Count, "Enumerator not added with constructor.");
            Assert.AreEqual(25, context.ID);
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

            Assert.AreEqual(0, context.ID);
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

        [Test]
        public void InitializeEmptyThrowsError() {
            context = new RoutineContext();

            var seed = (int)Math.Floor(DateTime.Now.TimeOfDay.TotalHours);
            context.ID = new Random(seed).Next();

            if (context.ID == 0) {
                context.ID = 1;
            }

            Assert.Throws<InvalidOperationException>(() => {
                context.Initialize(WaitFor1SecondOnce(), GetHashCode());
            });
        }

        [Test]
        public void InitializeWithInvalidID() {
            Assert.Throws<InvalidOperationException>(() => {
                context.Initialize(ChainTesting(), 0);
            });
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
