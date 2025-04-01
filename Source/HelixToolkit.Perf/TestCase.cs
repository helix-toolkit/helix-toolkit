
namespace HelixToolkit.Perf;
sealed class TestCase
{
    public string Name;
    public Action Action;
    public TestCase(string name, Action action)
    {
        Name = name;
        Action = action;
    }

    public static readonly List<TestCase> Tests = new();
    public static void Add(string name, Action action)
    {
        Tests.Add(new TestCase(name, action));
    }
}