public class Upgrade
{
    public string Name { get; }
    public string Description { get; }
    public string AffectedResource { get; }
    public decimal BaseCost { get; }
    public decimal CostMultiplier { get; }
    public decimal BaseEffect { get; }
    public UpgradeType Type { get; }
    public int Level { get; private set; }

    public Upgrade(string name, string description, string affectedResource, 
                  decimal baseCost, decimal costMultiplier, decimal baseEffect, 
                  UpgradeType type)
    {
        Name = name;
        Description = description;
        AffectedResource = affectedResource;
        BaseCost = baseCost;
        CostMultiplier = costMultiplier;
        BaseEffect = baseEffect;
        Type = type;
        Level = 0;
    }

    public decimal GetCurrentCost()
    {
        return BaseCost * (decimal)System.Math.Pow((double)CostMultiplier, Level);
    }

    public decimal GetEffect()
    {
        return BaseEffect * Level;
    }

    public bool CanAfford(decimal resourceAmount)
    {
        return resourceAmount >= GetCurrentCost();
    }

    public void Purchase()
    {
        Level++;
    }
}

public enum UpgradeType
{
    ClickPower,
    AutomaticProduction
} 