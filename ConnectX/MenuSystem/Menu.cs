namespace MenuSystem;

public class Menu
{
    private string Title { get; set; } = default!;
    private SortedDictionary<string, MenuItem> MenuItems { get; set; } = new();
    private EMenuLevel Level { get; set; }

    public void AddMenuItem(string key, string value, Func<string>? MethodToRun)
    {
        if (MenuItems.ContainsKey(key))
        {
            throw new Exception($"Duplicate key: {key}");
        }

        MenuItems[key] = new MenuItem() { Key = key, Value = value,  MethodToRun = MethodToRun };
    }

    public Menu(string title, EMenuLevel level)
    {
        Title = title;
        Level = level;

        switch (level)
        {
            case EMenuLevel.Root:
                MenuItems["x"] = new MenuItem() {Key = "x", Value = "Exit"};
                break;
            case EMenuLevel.First:
                MenuItems["b"] = new MenuItem() {Key = "b", Value = "Back"};
                MenuItems["x"] = new MenuItem() {Key = "x", Value = "Exit"};
                break;
            case EMenuLevel.Deep:
                MenuItems["m"] = new MenuItem() {Key = "m", Value = "Return To Main Menu"};
                MenuItems["b"] = new MenuItem() {Key = "b", Value = "Back"};
                MenuItems["x"] = new MenuItem() {Key = "x", Value = "Exit"};
                break;
            case EMenuLevel.Pause:
                break;
        }
    }


    public string Run()
    {
        var menuRunning = true;
        var userChoice = "";
        var option = 0;
        var menuItemsList = MenuItems.Values.ToList();

        do
        {
            Console.Clear();
            (int left, int top) = Console.GetCursorPosition();
            Console.SetCursorPosition(left, top);
            DisplayMenu(menuItemsList, option);

            var keyInfo = Console.ReadKey(true);
            (option, userChoice) = HandleKeyInput(keyInfo, option, menuItemsList);

            if (MenuItems.ContainsKey(userChoice))
            {
                (menuRunning, userChoice) = ProcessMenuSelection(userChoice);
            }
        } while (menuRunning);

        return userChoice;
    }

    private (int option, string userChoice) HandleKeyInput(ConsoleKeyInfo keyInfo, int option, List<MenuItem> menuItemsList)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.UpArrow:
                return (option == 0 ? MenuItems.Count - 1 : option - 1, "");
            case ConsoleKey.DownArrow:
                return (option == MenuItems.Count - 1 ? 0 : option + 1, "");
            case ConsoleKey.Enter:
                return (option, menuItemsList[option].Key);
            default:
                return (option, "");
        }
    }

    private (bool menuRunning, string userChoice) ProcessMenuSelection(string userChoice)
    {
        if (userChoice == "x" || userChoice == "m" || userChoice == "b")
            return (false, userChoice);

        var returnValue = MenuItems[userChoice].MethodToRun?.Invoke() ?? "";

        if (Level == EMenuLevel.Pause && returnValue == "continue")
            return (false, "continue");

        if (returnValue == "x")
            return (false, "x");

        if (returnValue == "m")
        {
            if (Level == EMenuLevel.Root)
                return (true, "");

            return (false, "m");
        }

        if (returnValue == "b")
            return (true, "");

        return (true, "");
    }

    private void DisplayMenu(List<MenuItem> menuItemsList, int option)
    {
        Console.WriteLine($"=== {Title} ===");
        Console.WriteLine("---------------------------");
        for (int i = 0; i < menuItemsList.Count; i++)
        {
            Console.WriteLine(option == i ? $" > {menuItemsList[i]}" : $"   {menuItemsList[i]}");
        }
    }
}
