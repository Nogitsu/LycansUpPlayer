using BepInEx.Configuration;

namespace LycansUpPlayer.Utils;

public class Settings(ConfigFile config)
{
    private readonly ConfigEntry<int> playerCount = config.Bind("General", "PlayerCount", 10, "Nombres de joueurs acceptés");

    public int getDefaultPlayerCount()
    {
        return (int)playerCount.DefaultValue;
    }

    public int getPlayerCount()
    {
        return playerCount.Value;
    }

    public void setPlayerCount(int value)
    {
        playerCount.Value = value;
    }
}