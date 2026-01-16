namespace Items
{
    public enum ItemType
    {
        // 实际上是现在有的所有模型
        # region from Miner
        OreGold,
        OreSilver,
        OreCopper,
        OreZinc,
        OreIron,
        OreNickel,
        OreCoal,
        # endregion
        
        # region from Refiner
        RefinedGold,
        RefinedSilver,
        RefinedCopper,
        RefinedZinc,
        RefinedIron,
        RefinedNickel,
        RefinedCoal,
        # endregion
        
        # region from Smelter
        IngotGold,
        IngotSilver,
        IngotCopper,
        IngotZinc,
        IngotIron,
        IngotNickel, 
        # endregion
    }
}