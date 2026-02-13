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
bool useDatabase = false;

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

// Helper methods
string RunSavedGamesAction(Func<string> action)
{
    var result = action();
    return string.IsNullOrEmpty(result) ? "m" : result;
}

string SelectConfig(GameConfiguration newConfig)
{
    config = newConfig;
    return menuChoosePlayer.Run();
}


// MenuRoot add items here
menuRoot.AddMenuItem("1", "Choose configuration", menuChooseConfig.Run);

menuRoot.AddMenuItem("2", "Load Saved Game", () =>
    RunSavedGamesAction(() => SavedGamesMenu.ShowLoadMenu(
        gameRepository,
        (cfg, gameId) =>
        {
            var gameState = gameRepository.Load(gameId);

            var controller = new GameController(
                cfg,
                gameState.Player1Name,
                gameState.Player2Name,
                gameRepository
            );

            controller.LoadGame(gameId);
            return controller.GameLoop();
        }
    ))
);

menuRoot.AddMenuItem("3", "Delete Saved Games", () =>
    RunSavedGamesAction(() => SavedGamesMenu.ShowDeleteMenu(gameRepository))
);

menuRoot.AddMenuItem("4", "Edit Saved Game", () =>
    RunSavedGamesAction(() => SavedGamesMenu.ShowEditMenu(gameRepository))
);



// MenuChooseConfig add items here
menuChooseConfig.AddMenuItem("1", "Classic Connect 4 (7x6)", () => SelectConfig(GameConfiguration.Classic()));
menuChooseConfig.AddMenuItem("2", "Connect 3 (5x4)", () => SelectConfig(GameConfiguration.Connect3()));
menuChooseConfig.AddMenuItem("3", "Connect 5 (9x7)", () => SelectConfig(GameConfiguration.Connect5()));
menuChooseConfig.AddMenuItem("4", "Connect 4 Cylinder (7x6)", () => SelectConfig(GameConfiguration.Connect4Cylinder()));


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
