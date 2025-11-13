using BLL;
using ConsoleApp;
using DAL;
using MenuSystem;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var config = GameConfiguration.Classic();
var controller = new GameController(config);

var mainMenu = new Menu("=== Connect Four ===", EMenuLevel.Root);

var predefinedConfigMenu = new Menu("=== Predefined Config ===", EMenuLevel.First);

var playerTypeMenu = new Menu("=== Player Type Menu ===", EMenuLevel.Deep);

var configMenu = new Menu("=== All Configs ===", EMenuLevel.First);


mainMenu.AddMenuItem("1", "Play Game", () =>
{
    predefinedConfigMenu.Run();
    return "p";
});

mainMenu.AddMenuItem("2", "Set Own Configurations", () =>
{
    var config = GameController.SetGameConfiguration();
    var controller = new GameController(config);
    return controller.GameLoop();
});

mainMenu.AddMenuItem("3", "All Configurations", configMenu.Run);



predefinedConfigMenu.AddMenuItem("1", "Classic Connect 4 (7x6, Win: 4)", () =>
{
    config = GameConfiguration.Classic();
    controller = new GameController(config);
    return playerTypeMenu.Run();
});

predefinedConfigMenu.AddMenuItem("2", "Connect 3 (5x4, Win: 3)", () =>
{
    config = GameConfiguration.Connect3();
    controller = new GameController(config);
    return playerTypeMenu.Run();
});

predefinedConfigMenu.AddMenuItem("3", "Connect 5 (9x7, Win: 5)", () =>
{
    config = GameConfiguration.Connect5();
    controller = new GameController(config);
    return playerTypeMenu.Run();
});

predefinedConfigMenu.AddMenuItem("4", "Cylinder Connect 4 (7x6, Win: 4)", () =>
{
    config = GameConfiguration.Connect4Cylinder();
    controller = new GameController(config);
    return playerTypeMenu.Run();
});




playerTypeMenu.AddMenuItem("1", "Human vs Human", () =>
{
    config = GameConfiguration.PlayerTypeHumanHuman(config);
    controller = new GameController(config);
    return controller.GameLoop();
});
playerTypeMenu.AddMenuItem("2", "Human vs Computer", () =>
{
    config = GameConfiguration.PlayerTypeHumanComputer(config);
    controller = new GameController(config);
    return controller.GameLoop();
});
playerTypeMenu.AddMenuItem("3", "Computer vs Computer", () =>
{
    config = GameConfiguration.PlayerTypeComputerComputer(config);
    controller = new GameController(config);
    return controller.GameLoop();
});




var configRepo = new ConfigRepositoryJson();
configMenu.AddMenuItem("1", "Load", () =>
{
    var count = 0;
    var data = configRepo.List();
    foreach (var item in data)
    {
        Console.WriteLine(count + ": " + item);
        count++;
    }
    Thread.Sleep(2000);

    return "abc";
});
configMenu.AddMenuItem("2", "Edit", () =>
{
    return "abc";
});
configMenu.AddMenuItem("3", "Create", () =>
{
    configRepo.Save(new GameConfiguration { Name = "classical" });
    return "abc";
});
configMenu.AddMenuItem("4", "Delete", () => { return "abc";});



mainMenu.Run();