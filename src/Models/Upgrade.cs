namespace incremental.Models;

public enum UpgradeType
{
    ClickPower,
    AutomaticProduction
}

public class Upgrade
{
    public string Name { get; }
    public string Description { get; }
    public string Resource { get; }
    public decimal BaseCost { get; }
    public decimal CostMultiplier { get; }
    public decimal Effect { get; }
    public UpgradeType Type { get; }
    public int Level { get; private set; }

    public Upgrade(string name, string description, string resource, decimal baseCost, 
                  decimal costMultiplier, decimal effect, UpgradeType type)
    {
        Name = name;
        Description = description;
        Resource = resource;
        BaseCost = baseCost;
        CostMultiplier = costMultiplier;
        Effect = effect;
        Type = type;
        Level = 0;
    }

    public decimal GetCurrentCost() => BaseCost * (decimal)System.Math.Pow((double)CostMultiplier, Level);
    public decimal GetEffect() => Effect * Level;
    public bool CanAfford(decimal gold) => gold >= GetCurrentCost();
    public void Purchase() => Level++;
} 