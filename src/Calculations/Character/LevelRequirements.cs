namespace Calculations;

public static class LevelRequirements
{
    public static int GetGoldForNextLevel(double currentLevel, double goldFactor)
    {
        double i = currentLevel * currentLevel * 30.1;
        double j = currentLevel / 10.0;
        double k = goldFactor / 10.0;
        return (int)Math.Floor(Math.Floor(i + j) * (k + 1));
    }

    public static long GetTotalGold(double currentLevel, double goldFactor)
    {
        long retval = 0;
        for (double i = 1; i <= currentLevel; i++)
        {
            retval += GetGoldForNextLevel(i, goldFactor);
        }
        return retval;
    }

    public static int GetXpForNextLevel(double currentLevel, double raceFactor, double guildFactor) =>
        GetExpForNextLevel(currentLevel, raceFactor, guildFactor);

    public static int GetXpForPin(double currentLevel, double raceFactor, double guildFactor) =>
        GetExpForNextLevel(Math.Min(currentLevel + 1, 998), raceFactor, guildFactor);

    private static int GetExpForNextLevel(double level, double raceFactor, double guildFactor)
    {
        double actualRaceFactor = raceFactor + 1;
        double i = level * level * guildFactor * 250 / 3.0;
        double j = (level * 2 - 1) * 344;
        double k = Math.Log(actualRaceFactor) / Math.Log(actualRaceFactor + 1);
        double l = actualRaceFactor / 25.0;
        double value = (i + j - 2) * (k + l + 0.3) / 10.0;
        return (int)Math.Round(value);
    }
}
