using BLL;
using ConsoleApp;
using MenuSystem;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var mainMenu = new Menu("=== Connect Four ===", EMenuLevel.Root);

var predefinedConfigMenu = new Menu("=== Predefined Config ===", EMenuLevel.First);



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



predefinedConfigMenu.AddMenuItem("1", "Classic Connect 4 (7x6, Win: 4)", () =>
{
    var config = GameConfiguration.Classic();
    var controller = new GameController(config);
    return controller.GameLoop();
});

predefinedConfigMenu.AddMenuItem("2", "Connect 3 (5x4, Win: 3)", () =>
{
    var config = GameConfiguration.Connect3();
    var controller = new GameController(config);
    return controller.GameLoop();
});

predefinedConfigMenu.AddMenuItem("3", "Connect 5 (9x7, Win: 5)", () =>
{
    var config = GameConfiguration.Connect5();
    var controller = new GameController(config);
    return controller.GameLoop();
});

predefinedConfigMenu.AddMenuItem("4", "Cylinder Connect 4 (7x6, Win: 4)", () =>
{
    var config = GameConfiguration.Connect4Cylinder();
    var controller = new GameController(config);
    return controller.GameLoop();
});




mainMenu.Run();