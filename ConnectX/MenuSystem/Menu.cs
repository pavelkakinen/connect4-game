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

            var userInput = "";
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    option = (option == 0 ? MenuItems.Count - 1 : option - 1);
                    break;
                case ConsoleKey.DownArrow:
                    option = (option == MenuItems.Count - 1 ? 0 : option + 1);
                    break;
                case ConsoleKey.Enter:
                    userInput = menuItemsList[option].Key;
                    break;
            }
            
            if (userInput == null)
            {
                Console.WriteLine("Invalid input. Please try again.");
                continue;
            }

            userChoice = userInput.Trim().ToLower();

            if (userChoice == "x" || userChoice == "m" || userChoice == "b")
            {
                menuRunning = false;
            }
            else
            {
                if (MenuItems.ContainsKey(userChoice))
                {
                    var returnValueFromMethodToRun = MenuItems[userChoice].MethodToRun?.Invoke();

                    if (returnValueFromMethodToRun == "x")
                    {
                        menuRunning = false;
                        userChoice = "x";
                    }
                    else if (returnValueFromMethodToRun == "m" && Level != EMenuLevel.Root)
                    {
                        menuRunning = false;
                        userChoice = "m";
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please try again.");
                }
            }
        } while (menuRunning);
        
        return userChoice;
    }

    private void DisplayMenu(List<MenuItem> menuItemsList, int option)
    {
        Console.WriteLine(Title);
        Console.WriteLine("-----------------------------");
        for (int i = 0; i < menuItemsList.Count; i++)
        {
            Console.WriteLine(option == i ? $" > {menuItemsList[i]}" : $"   {menuItemsList[i]}");
        }
    }
}
