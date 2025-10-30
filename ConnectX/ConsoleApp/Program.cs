using BLL;
using ConsoleApp;
using MenuSystem;

Console.WriteLine("Hello, ConnectFour!");


var mainMenu0 = new Menu("Connect Four", EMenuLevel.Root);
mainMenu0.AddMenuItem("a", "New Game", () =>
{
    var controller = new GameController(new GameConfiguration());
    controller.GameLoop();
    return "a";
});



var configGameMenu1 = new Menu("Config Game", EMenuLevel.First);
configGameMenu1.AddMenuItem("2", "Set Own Configurations", () =>
{
    var config = GameController.SetGameConfiguration();
    var controller = new GameController(config);
    controller.GameLoop();
    return "a";
});


var chooseClassicConfMenu2 = new Menu("Choose Any Classic Configuration", EMenuLevel.Deep);

var menu3 = new Menu("Menu 3", EMenuLevel.Deep);
menu3.AddMenuItem("a", "Level3 - Option A", () => { Console.WriteLine("Level3 - Option A  was called");
    return "a";
});


mainMenu0.AddMenuItem("1", "Config Game",  configGameMenu1.Run);

configGameMenu1.AddMenuItem("1", "Choose classic configurations", chooseClassicConfMenu2.Run);

chooseClassicConfMenu2.AddMenuItem("1", "Classic connect 4", () => { Console.WriteLine("Connect 4 was called"); return "a";});
chooseClassicConfMenu2.AddMenuItem("2", "Connect 5", () => { Console.WriteLine("Connect 5 was called"); return "a";});
chooseClassicConfMenu2.AddMenuItem("3", "Connect 3", () => { Console.WriteLine("Connect 3 was called"); return "a";});
chooseClassicConfMenu2.AddMenuItem("4", "Connect 4 Cylinder", () => { Console.WriteLine("Connect 4 cylinder was called"); return "a";});


mainMenu0.Run();