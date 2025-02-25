using Microsoft.Xna.Framework;

public class Resource
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public decimal PerSecond { get; set; }
    public decimal PerClick { get; set; }
    public decimal SellPrice { get; set; }

    public Resource(string name, decimal amount = 0, decimal perSecond = 0, decimal perClick = 1, decimal sellPrice = 0.1m)
    {
        Name = name;
        Amount = amount;
        PerSecond = perSecond;
        PerClick = perClick;
        SellPrice = sellPrice;
    }

    public void Update(GameTime gameTime)
    {
        decimal deltaTime = (decimal)gameTime.ElapsedGameTime.TotalSeconds;
        Amount += PerSecond * deltaTime;
    }

    public void Click()
    {
        Amount += PerClick;
    }

    public decimal Sell(decimal amount)
    {
        if (amount > Amount) amount = Amount;
        Amount -= amount;
        return amount * SellPrice;
    }
} 