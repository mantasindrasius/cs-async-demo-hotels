using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace Lib.Test
{
    [TestClass]
    public class AsyncTest
    {
        [TestMethod]
        public void TestAsyncRunSync()
        {
            var stop = Stopwatch.StartNew();

            Func<Task<bool>> m = async () =>
            {
                Thread.Sleep(1000);
                return true;
            };

            m();

            Assert.IsTrue(stop.ElapsedMilliseconds >= 1000);
        }

        [TestMethod]
        public void TestSimpleAsync()
        {
            var stop = Stopwatch.StartNew();

            Func<Task<bool>> m = async () =>
            {
                var task = Task.Run(() => Thread.Sleep(1000));

                await task;

                return true;
            };

            var resultTask = m();

            Assert.IsTrue(stop.ElapsedMilliseconds < 100);

            resultTask.Wait();

            Assert.IsTrue(stop.ElapsedMilliseconds > 1000);
        }

        [TestMethod]
        public void TestBlockingBeforeAwait()
        {
            var stop = Stopwatch.StartNew();

            Func<Task<bool>> m = async () =>
            {
                var task = Task.Run(() => Thread.Sleep(1000));

                Thread.Sleep(1000);

                await task;

                return true;
            };

            var resultTask = m();

            // Įvykdo pagrindinės gijos Thread.Sleep, tada grąžina Task.
            Assert.IsTrue(stop.ElapsedMilliseconds > 1000);

            resultTask.Wait();

            Debug.Print(stop.ElapsedMilliseconds.ToString());

            // Thread.Sleep įvykdyti konkurenciškai
            Assert.IsTrue(stop.ElapsedMilliseconds < 1100);
        }

        [TestMethod]
        public void TestBlockingAfterAwait()
        {
            var stop = Stopwatch.StartNew();

            Func<Task<bool>> m = async () =>
            {
                var task = Task.Run(() => Thread.Sleep(1000));

                await task;

                Thread.Sleep(1000);

                return true;
            };

            var resultTask = m();

            Assert.IsTrue(stop.ElapsedMilliseconds < 100);

            resultTask.Wait();

            Assert.IsTrue(stop.ElapsedMilliseconds > 2000);
        }

        [TestMethod]
        public void TestFinishedTask()
        {
            var stop = Stopwatch.StartNew();

            Func<Task<bool>> m = async () =>
            {
                var task = Task.Run(() => Thread.Sleep(1000));

                task.Wait();

                await task;

                Thread.Sleep(1000);

                return true;
            };

            var resultTask = m();

            Assert.IsTrue(stop.ElapsedMilliseconds > 2000 && resultTask.IsCompleted);
        }

        class NumberSource
        {
            private IEnumerator<int> source = GetNumbers().GetEnumerator();

            private static IEnumerable<int> GetNumbers()
            {
                yield return 1;
                yield return 2;
                yield return 3;
            }

            public volatile int Current;

            public Task<bool> ReadNext()
            {
                return Task.Run(() =>
                {
                    Thread.Sleep(1000);

                    if (!source.MoveNext())
                    {
                        return false;
                    }

                    Current = source.Current;

                    return true;
                });
            }
        }

        [TestMethod]
        public void TestReaderAsync()
        {
            var stop = Stopwatch.StartNew();

            Func<Task<bool>> m = async () =>
            {
                var source = new NumberSource();

                while (await source.ReadNext())
                {
                    Debug.Print("Read {0} at {1}", source.Current, stop.ElapsedMilliseconds);
                    Thread.Sleep(100);
                }

                return true;
            };

            var resultTask = m();

            Assert.IsTrue(stop.ElapsedMilliseconds < 100);

            resultTask.Wait();

            Assert.IsTrue(stop.ElapsedMilliseconds > 3000);
        }
    }
}
