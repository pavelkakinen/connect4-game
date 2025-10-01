namespace MenuSystem;

public class Menu
{
    // exit, back1, return to main
    private string Title { get; set; } = default!; 
    
    private Dictionary<string, MenuItem> MenuItems { get; set; } = new();
    
    private EMenuLevel Level { get; set; }
    
    public void addMenuItem(string key, string value, Func<string> methodToRun)
    {
        if (MenuItems.ContainsKey(key))
        {
            throw new ArgumentException($"Duplicate key: {key}");
        }

        MenuItems[key] = new MenuItem() { Key = key, Value = value, MethodToRun = methodToRun };
    }

    public Menu(string title, EMenuLevel level)
    {
        Title = title;
        Level = level;

        switch (level)
        {
            case EMenuLevel.Root:
                MenuItems["x"] = new MenuItem() {Key = "x",  Value = "Exit"};
                break;
            case EMenuLevel.First:
                MenuItems["m"] = new MenuItem() {Key = "m",  Value = "Return to main menu"};
                MenuItems["x"] = new MenuItem() {Key = "x",  Value = "Exit"};
                break;
            case EMenuLevel.Deep:
                MenuItems["b"] = new MenuItem() {Key = "b",  Value = "Back to previous menu"};
                MenuItems["m"] = new MenuItem() {Key = "m",  Value = "Return to main menu"};
                MenuItems["x"] = new MenuItem() {Key = "x",  Value = "Exit"};
                break;
        }
    }


    public string Run()
    {
        Console.Clear();
        var menuRunning = true;
        var userChoice = "";
        
        do
        {
            DisplayMenu();
            Console.Write("Select an option: ");
            var input = Console.ReadLine();

            if (input == null)
            {
                Console.WriteLine("Invalid input. Please try again.");
                continue;
            }

            userChoice = input.Trim().ToLower();
            
            if (userChoice == "x" ||  userChoice == "m" || userChoice == "b")
            {
                // TODO: handle exit, return to main menu, or back
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
                    } else if (returnValueFromMethodToRun == "m" && Level != EMenuLevel.Root)
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

    public void DisplayMenu()
    {
        Console.WriteLine(Title);
        Console.WriteLine("----------------------");
        foreach (var item in MenuItems.Values)
        {
            Console.WriteLine(item);
        }
    }
}