using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIMDTest
{
    public class TestWrapper
    {
        private readonly Func<bool> testFunc;
        private readonly string name;

        public TestWrapper(string testName, Func<bool> test)
        {
            testFunc = test;
            name = testName;
        }

        public void Run()
        {
            var t = Stopwatch.GetTimestamp();
            bool succ = testFunc.Invoke();
            t = Stopwatch.GetTimestamp() - t;
            var time = (float)t / Stopwatch.Frequency * 1000;
            Console.WriteLine($"Test: {name}; RunTime: {time} ms; Succ: {succ}");
        }
    }
}
