using Microsoft.Xna.Framework;

namespace incremental.Models;

public class Resource
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public decimal PerSecond { get; set; }
    public decimal PerClick { get; set; }
    public decimal BasePrice { get; private set; }

    public Resource(string name, decimal amount, decimal perSecond, decimal perClick, decimal basePrice = 0)
    {
        Name = name;
        Amount = amount;
        PerSecond = perSecond;
        PerClick = perClick;
        BasePrice = basePrice;
    }

    public void Click() => Amount += PerClick;

    public decimal SellPrice => BasePrice;

    public decimal Sell(decimal amount)
    {
        if (amount > Amount) amount = Amount;
        Amount -= amount;
        return amount * SellPrice;
    }

    public void Update(GameTime gameTime)
    {
        float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Amount += PerSecond * (decimal)deltaSeconds;
    }
} 