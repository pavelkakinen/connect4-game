namespace DAL.Helpers;

public static class FilesystemHelpers
{
    private const string AppName = "ConnectX";
    
    private static string GetAppDirectory()
    {
        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var appDirectory = Path.Combine(homeDirectory, AppName);
        
        if (!Directory.Exists(appDirectory))
        {
            Directory.CreateDirectory(appDirectory);
        }
        
        return appDirectory;
    }
    
    public static string GetGamesDirectory()
    {
        var gamesDir = Path.Combine(GetAppDirectory(), "saved_games");
        
        if (!Directory.Exists(gamesDir))
        {
            Directory.CreateDirectory(gamesDir);
        }
        
        return gamesDir;
    }
}