namespace Mordorings;

public static class GuildExtensions
{
    public static int GetIndex(this Guild[] spells, Guild spell) => Array.IndexOf(spells, spell);

    public static Guild? GetByName(this Guild[] spells, string name) => spells.FirstOrDefault(guild => guild.Name == name);

    public static Dictionary<SpellCategory, float> GetGuildSpellCategories(this Guild guild)
    {
        List<int> categories = [];
        for (int i = 0; i <= guild.SpellCaps.GetUpperBound(0); i++)
        {
            if (guild.SpellCaps[i] > 0)
            {
                categories.Add(i);
            }
        }
        Dictionary<SpellCategory, float> retval = [];
        foreach (int category in categories)
        {
            retval[((SpellCategory)category)] = guild.SpellGuildMods[category];
        }
        return retval;
    }
}
