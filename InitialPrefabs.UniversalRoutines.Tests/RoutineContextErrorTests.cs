using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using System.Text.RegularExpressions;

namespace InitialPrefabs.UniversalRoutines.Tests {
    public class RoutineContextErrorTests {

        class TestEnumerator {

            public IEnumerator Test() {
                yield return 1;

                throw new System.Exception("Error manually thrown");
            }
        }

        RoutineContext context;

        [SetUp]
        public void Setup() {
            context = new RoutineContext();
        }

        [Test]
        public void ExceptionThrownIsCaught() {
            var testEnumerator = new TestEnumerator();
            context.Initialize(testEnumerator.Test(), 1);
            Assert.AreEqual(1, context.Contexts.Count, "Enumerator not pushed!");

            context.Run();
            context.Run();

            LogAssert.Expect(LogType.Error, new Regex(@"RoutineContext caught an error! Stopping the coroutine and cleaning up!.*"));

            Assert.True(context.IsEmpty(), "ContextRoutine not flushed!");
            Assert.AreEqual(0, context.ID, "ID not resetted!");
        }
    }
}
