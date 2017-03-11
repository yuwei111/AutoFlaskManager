namespace FlaskManager.Flask_Components
{
    public enum FlaskAction : int
    {
        IGNORE = 0,         // ignore mods and don't give error
        NONE,               // flask isn't initilized.
        LIFE,               //life, Blood of the Karui
        MANA,               //mana, Doedre's Elixir, 
                            //Zerphi's Last Breath, Lavianga's Spirit

        HYBRID,             //hybrid flasks,

        DEFENSE,            //bismuth, jade, stibnite, granite,
                            //amethyst, ruby, sapphire, topaz,
                            // aquamarine, quartz, Sin's Rebirth, 
                            //Coruscating Elixir, Forbidden Taste,Rumi's Concoction
                            //MODS: iron skin, reflexes, gluttony,
                            // craving, resistance

        UTILITY,            //Doedre's Elixir, Zerphi's Last Breath, Lavianga's Spirit

        SPEEDRUN,           //quick silver, MOD: adrenaline,

        OFFENSE,            //silver, sulphur, basalt, diamond,Taste of Hate, 
                            //Kiara's Determination, Lion's Roar, The Overflowing Chalice, 
                            //The Sorrow of the Divine,Rotgut, Witchfire Brew, Atziri's Promise, 
                            //Dying Sun,Vessel of Vinktar
                            //MOD: Fending

        POISON_IMMUNE,      // MOD: curing
        FREEZE_IMMUNE,      // MOD: heat
        IGNITE_IMMUNE,      // MOD: dousing
        SHOCK_IMMUNE,       // MOD: grounding
        BLEED_IMMUNE,       // MOD: staunching
        CURSE_IMMUNE,       // MOD: warding
        UNIQUE_FLASK,       // All the milk shakes
    }
}
