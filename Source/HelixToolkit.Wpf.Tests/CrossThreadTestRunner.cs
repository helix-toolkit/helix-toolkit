using System.Reflection;
using System.Security.Permissions;

namespace HelixToolkit.Wpf.Tests;

/// <summary>
/// http://www.peterprovost.org/blog/post/NUnit-and-Multithreaded-Tests-CrossThreadTestRunner.aspx
/// </summary>
public sealed class CrossThreadTestRunner
{
    private Exception? lastException;

    public static void RunInSTA(Action action)
    {
        var r = new CrossThreadTestRunner();
        r.Run(new ThreadStart(action), ApartmentState.STA);
    }

    public static void RunInSTAThrowException(Action action)
    {
        Exception? exception = null;

        void action2()
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        }

        RunInSTA(action2);

        if (exception != null)
        {
            throw exception;
        }
    }

    //public void RunInMTA(ThreadStart userDelegate)
    //{
    //    this.Run(userDelegate, ApartmentState.MTA);
    //}

    //public void RunInSTA(ThreadStart userDelegate)
    //{
    //    this.Run(userDelegate, ApartmentState.STA);
    //}

    //[ReflectionPermission(SecurityAction.Demand)]
    private static void ThrowExceptionPreservingStack(Exception exception)
    {
        var remoteStackTraceString = typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);
        if (remoteStackTraceString != null)
        {
            remoteStackTraceString.SetValue(exception, exception.StackTrace + Environment.NewLine);
        }

        throw exception;
    }

    private void Run(ThreadStart userDelegate, ApartmentState apartmentState)
    {
        this.lastException = null;

        var thread = new Thread(userDelegate.Invoke);
        thread.SetApartmentState(apartmentState);

        thread.Start();
        thread.Join();

        if (this.ExceptionWasThrown())
        {
            ThrowExceptionPreservingStack(this.lastException!);
        }
    }

    private bool ExceptionWasThrown()
    {
        return this.lastException != null;
    }
}
