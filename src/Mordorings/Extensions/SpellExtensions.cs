namespace Mordorings;

public static class SpellExtensions
{
    public static Spell? GetById(this Spell[] spells, int id) => spells.FirstOrDefault(spell => spell.Id == id);

    public static int GetIndex(this Spell[] spells, Spell spell) => Array.IndexOf(spells, spell);

    public static Spell? GetByName(this Spell[] spells, string name) => spells.FirstOrDefault(spell => spell.Name == name);
}
