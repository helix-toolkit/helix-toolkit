
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

    public static void RunAll()
    {
        foreach (var test in Tests)
        {
            MathSettings.EnableSIMD = true;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"---------Running: {test.Name}---------");
            Console.ForegroundColor = ConsoleColor.White;
            Perf.Profile(test.Name, test.Action);
            MathSettings.EnableSIMD = false;
            Perf.Profile(test.Name, test.Action);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"---------End: {test.Name}---------\n\n");
        }
    }
}