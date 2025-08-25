namespace Calculations;

public static class SpellCost
{
    public static int GetInitialCost(Spell spell, Guild guild) =>
        DoSpellLevelCalculation(spell, guild, 0);

    public static int GetSpellCost(Spell spell, Guild guild, double characterSpellLevel) =>
        DoSpellLevelCalculation(spell, guild, characterSpellLevel);

    private static int DoSpellLevelCalculation(Spell spell, Guild guild, double characterSpellLevel)
    {
        float guildMod = CalculateGuildMod(spell, guild);
        if (characterSpellLevel == 0)
        {
            characterSpellLevel = spell.Level;
        }
        double spellLevel = spell.Level / characterSpellLevel;
        return (int)(spellLevel * spell.Cost * guildMod * 1.9 - 0.0001);
    }

    private static float CalculateGuildMod(Spell spell, Guild guild)
    {
        float guildMod = guild.SpellGuildMods[spell.Category];
        if (guildMod == 0)
        {
            guildMod = 1;
        }
        return guildMod;
    }
}
