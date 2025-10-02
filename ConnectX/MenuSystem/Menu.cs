namespace MenuSystem;

public class Menu
{
    private string Title { get; set; } = default!;
    private Dictionary<string, MenuItem> MenuItems { get; set; } = new();
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
                MenuItems["m"] = new MenuItem() {Key = "m", Value = "Return To Main Menu"};
                MenuItems["x"] = new MenuItem() {Key = "x", Value = "Exit"};
                break;
            case EMenuLevel.Deep:
                MenuItems["b"] = new MenuItem() {Key = "b", Value = "Back To Previous Menu"};
                MenuItems["m"] = new MenuItem() {Key = "m", Value = "Return To Main Menu"};
                MenuItems["x"] = new MenuItem() {Key = "x", Value = "Exit"};
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
            Console.Write("Please make a selection: ");
            
            var userInput = Console.ReadLine();
            if (userInput == null)
            {
                Console.WriteLine("Invalid input. Please try again.");
                continue;
            }

            userChoice = userInput.Trim().ToLower();

            if (userChoice == "x" || userChoice == "m" || userChoice == "b")
            {
                // TODO: Handle exit, return to main menu, or back
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
                    else if (returnValueFromMethodToRun == "b" && Level != EMenuLevel.Deep)
                    {
                        menuRunning = false;
                        userChoice = "";
                    }
                    else if (returnValueFromMethodToRun == "m" && Level != EMenuLevel.First)
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
        private void DisplayMenu()
        {
            
            Console.WriteLine(Title);
            Console.WriteLine("-----------------------------");
            foreach (var item in MenuItems.Values)
            {
                Console.WriteLine(item);
            }
        }

}