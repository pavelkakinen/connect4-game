using BLL;
using ConsoleApp;
using ConsoleUI;
using DAL;
using DAL.EF;
using DAL.Json;
using Domain;
using MenuSystem;
using Microsoft.EntityFrameworkCore;


// Configuration and Controller
var config = GameConfiguration.Classic();

// ================ DB STAFF =================
bool useDatabase = true;

IRepository<GameState> gameRepository;

if (useDatabase)
{
    var dbContext = GetDbContext();
    gameRepository = new GameStateRepositoryEF(dbContext);
}
else
{
    gameRepository = new GameStateRepositoryJson();
}



// All Menus here
Menu menuRoot = new Menu("Connect Four", EMenuLevel.Root);
Menu menuChooseConfig =  new Menu("Choose Config", EMenuLevel.First);
Menu menuChoosePlayer = new Menu("Choose Players", EMenuLevel.Deep);


// MenuRoot add items here
menuRoot.AddMenuItem("1", "Choose configuration", menuChooseConfig.Run);

menuRoot.AddMenuItem("2", "Load Saved Game", () => 
    SavedGamesMenu.ShowLoadMenu(
        gameRepository,
        (config, gameId) =>
        {
            var gameState = gameRepository.Load(gameId);
            
            var controller = new GameController(
                config, 
                gameState.Player1Name, 
                gameState.Player2Name,
                gameRepository
            );
            
            controller.LoadGame(gameId);
            return controller.GameLoop();
        }
    )
);

menuRoot.AddMenuItem("3", "Delete Saved Games", () => 
    SavedGamesMenu.ShowDeleteMenu(gameRepository));

menuRoot.AddMenuItem("4", "Edit Saved Game", () => 
    SavedGamesMenu.ShowEditMenu(gameRepository));



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
    config.SetP1Type(EPlayerType.Human);
    config.SetP2Type(EPlayerType.Human);
    
    var (p1Name, p2Name) = Ui.GetPlayerNames(isVsComputer: false);
    var controller = new GameController(config, p1Name, p2Name, gameRepository);
    controller.GameLoop();
    return "m";
});
menuChoosePlayer.AddMenuItem("2", "Human vs Computer", () =>
{
    config.SetP1Type(EPlayerType.Human);
    config.SetP2Type(EPlayerType.Computer);
    var (p1Name, p2Name) = Ui.GetPlayerNames(isVsComputer: true);
    var controller = new GameController( config, p1Name, p2Name, gameRepository);
    controller.GameLoop();
    return "m";
});
menuChoosePlayer.AddMenuItem("3", "Computer vs Computer", () =>
{
    config.SetP1Type(EPlayerType.Computer);
    config.SetP2Type(EPlayerType.Computer);
    var controller = new GameController(config, "AI Red", "AI Blue",  gameRepository);
    controller.GameLoop();
    return "m";
});


menuRoot.Run();



// ================ HELPER METHOD =================

static AppDbContext GetDbContext()
{
    var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    homeDirectory = homeDirectory + Path.DirectorySeparatorChar;

    var connectionString = $"Data Source={homeDirectory}connectx.db";

    var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlite(connectionString)
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging()
        .Options;

    var dbContext = new AppDbContext(contextOptions);
    
    // apply any pending migrations (recreates db as needed)
    dbContext.Database.Migrate();
    
    return dbContext;
}
