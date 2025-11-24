using BLL;
using ConsoleApp;
using MenuSystem;
using DAL;


// Configuration and Controller
var config = GameConfiguration.Classic();

// All Menus here
Menu menuRoot = new Menu("Connect Four", EMenuLevel.Root);
Menu menuChooseConfig =  new Menu("Choose Config", EMenuLevel.First);
Menu menuChoosePlayer = new Menu("Choose Players", EMenuLevel.Deep);


// MenuRoot add items here
menuRoot.AddMenuItem("1", "Choose configuration", menuChooseConfig.Run);


// MenuChooseConfig add items here
menuChooseConfig.AddMenuItem("1", "Classic Connect 4 (7x6)", () =>
{
    config = GameConfiguration.Classic();
    var result = menuChoosePlayer.Run();
    return result;
});
menuChooseConfig.AddMenuItem("2", "Connect 3 (5x4)", () =>
{
    config = GameConfiguration.Connect3();
    var result = menuChoosePlayer.Run();
    return result;
});
menuChooseConfig.AddMenuItem("3", "Connect 5 (9x7)", () =>
{
    config = GameConfiguration.Connect5();
    var result = menuChoosePlayer.Run();
    return result;
});
menuChooseConfig.AddMenuItem("4", "Connect 4 Cylinder (7x6)", () =>
{
    config = GameConfiguration.Connect4Cylinder();
    var result = menuChoosePlayer.Run();
    return result;
});


// MenuChoosePlayer add items here
menuChoosePlayer.AddMenuItem("1", "Human vs Human", () =>
{
    var finalConfig = GameConfiguration.PlayerTypeHumanHuman(config);
    var controller = new GameController(finalConfig);
    controller.GameLoop();
    return "m";
});
menuChoosePlayer.AddMenuItem("2", "Human vs Computer", () =>
{
    var finalConfig = GameConfiguration.PlayerTypeHumanComputer(config);
    var controller = new GameController(finalConfig);
    controller.GameLoop();
    return "m";
});
menuChoosePlayer.AddMenuItem("3", "Computer vs Computer", () =>
{
    var finalConfig = GameConfiguration.PlayerTypeComputerComputer(config);
    var controller = new GameController(finalConfig);
    controller.GameLoop();
    return "m";
});


menuRoot.Run();