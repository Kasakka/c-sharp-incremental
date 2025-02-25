using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace incremental;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;
    private Texture2D _pixel;
    
    private Dictionary<string, Resource> _resources;
    private Dictionary<string, Button> _buttons;
    private Dictionary<string, List<Upgrade>> _upgrades;
    private Dictionary<string, Button> _upgradeButtons;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        _graphics.PreferredBackBufferWidth = 1024;
        _graphics.PreferredBackBufferHeight = 768;
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        _resources = new Dictionary<string, Resource>
        {
            { "Wood", new Resource("Wood", 0, 0, 1, 1) },
            { "Stone", new Resource("Stone", 0, 0, 1, 2) },
            { "Iron", new Resource("Iron", 0, 0, 1, 5) },
            { "Copper", new Resource("Copper", 0, 0, 1, 4) },
            { "Coal", new Resource("Coal", 0, 0, 1, 3) },
            { "Gold", new Resource("Gold", 0, 0, 0) }
        };

        _buttons = new Dictionary<string, Button>();
        int yPos = 50;
        foreach (var resource in _resources.Where(r => r.Key != "Gold"))
        {
            _buttons.Add(resource.Key, new Button(new Rectangle(30, yPos, 180, 40), $"Gather {resource.Key}"));
            _buttons.Add($"Sell{resource.Key}", new Button(new Rectangle(220, yPos, 120, 40), $"Sell {resource.Key}"));
            yPos += 50;
        }

        _buttons.Add("SellAll", new Button(new Rectangle(30, yPos, 310, 40), "Sell All Resources"));

        _upgrades = new Dictionary<string, List<Upgrade>>();
        foreach (var resource in _resources.Where(r => r.Key != "Gold"))
        {
            _upgrades[resource.Key] = new List<Upgrade> {
                new Upgrade($"Better {resource.Key} Tool", $"+1 {resource.Key} per click", 
                          resource.Key, 10, 1.5m, 1, UpgradeType.ClickPower),
                new Upgrade($"{resource.Key} Automator", $"+1 {resource.Key} per second", 
                          resource.Key, 50, 1.8m, 1, UpgradeType.AutomaticProduction)
            };
        }

        _upgradeButtons = new Dictionary<string, Button>();
        yPos = 50;
        foreach (var resourcePair in _upgrades)
        {
            foreach (var upgrade in resourcePair.Value)
            {
                string buttonKey = $"{resourcePair.Key}_{upgrade.Name}";
                _upgradeButtons[buttonKey] = new Button(
                    new Rectangle(400, yPos, 300, 40),
                    $"{upgrade.Name} ({(int)upgrade.GetCurrentCost()} Gold)"
                );
                yPos += 50;
            }
        }
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _font = Content.Load<SpriteFont>("Arial");
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        MouseState mouseState = Mouse.GetState();
        
        foreach (var pair in _buttons)
        {
            if (pair.Value.Update(mouseState))
            {
                if (pair.Key == "SellAll")
                {
                    foreach (var resource in _resources.Where(r => r.Key != "Gold"))
                    {
                        decimal goldEarned = resource.Value.Sell(resource.Value.Amount);
                        _resources["Gold"].Amount += goldEarned;
                    }
                }
                else if (pair.Key.StartsWith("Sell"))
                {
                    string resourceName = pair.Key.Substring(4);
                    decimal goldEarned = _resources[resourceName].Sell(_resources[resourceName].Amount);
                    _resources["Gold"].Amount += goldEarned;
                }
                else
                {
                    _resources[pair.Key].Click();
                }
            }
        }

        foreach (var resourcePair in _upgrades)
        {
            string resourceKey = resourcePair.Key;
            foreach (var upgrade in resourcePair.Value)
            {
                string buttonKey = $"{resourceKey}_{upgrade.Name}";
                if (_upgradeButtons[buttonKey].Update(mouseState))
                {
                    if (upgrade.CanAfford(_resources["Gold"].Amount))
                    {
                        _resources["Gold"].Amount -= upgrade.GetCurrentCost();
                        upgrade.Purchase();
                        
                        if (upgrade.Type == UpgradeType.ClickPower)
                        {
                            _resources[resourceKey].PerClick = 1 + upgrade.GetEffect();
                        }
                        else if (upgrade.Type == UpgradeType.AutomaticProduction)
                        {
                            _resources[resourceKey].PerSecond = upgrade.GetEffect();
                        }
                        
                        _upgradeButtons[buttonKey].Text = $"{upgrade.Name} ({(int)upgrade.GetCurrentCost()} Gold)";
                    }
                }
            }
        }

        foreach (var resource in _resources.Values)
        {
            resource.Update(gameTime);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        _spriteBatch.Draw(_pixel, new Rectangle(20, 20, 330, 400), new Color(0, 0, 0, 50));
        _spriteBatch.Draw(_pixel, new Rectangle(390, 20, 320, 700), new Color(0, 0, 0, 50));

        _spriteBatch.DrawString(_font, "RESOURCES", new Vector2(30, 25), Color.Black);
        _spriteBatch.DrawString(_font, "UPGRADES", new Vector2(400, 25), Color.Black);

        foreach (var pair in _buttons.Concat(_upgradeButtons))
        {
            var button = pair.Value;
            var color = button.WasPressed ? Color.DarkGray : 
                       (pair.Key.StartsWith("Sell") ? new Color(255, 200, 200) : Color.White);
            _spriteBatch.Draw(_pixel, button.Bounds, color);
            
            Vector2 textSize = _font.MeasureString(button.Text);
            Vector2 textPos = new Vector2(
                button.Bounds.X + (button.Bounds.Width - textSize.X) / 2,
                button.Bounds.Y + (button.Bounds.Height - textSize.Y) / 2
            );
            _spriteBatch.DrawString(_font, button.Text, textPos, Color.Black);
        }

        int yOffset = 450;
        foreach (var pair in _resources)
        {
            string text = $"{pair.Key}: {(int)pair.Value.Amount}";
            if (pair.Key != "Gold")
            {
                text += $" (Sells for {(int)pair.Value.SellPrice} Gold each)";
            }
            _spriteBatch.DrawString(_font, text, new Vector2(30, yOffset), Color.Black);
            
            if (pair.Key != "Gold")
            {
                string rateText = $"Per Second: {(int)pair.Value.PerSecond}";
                _spriteBatch.DrawString(_font, rateText, new Vector2(30, yOffset + 20), Color.DarkGray);
                
                string clickText = $"Per Click: {(int)pair.Value.PerClick}";
                _spriteBatch.DrawString(_font, clickText, new Vector2(200, yOffset + 20), Color.DarkGray);
            }
            
            yOffset += 50;
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        _pixel?.Dispose();
        base.UnloadContent();
    }
}

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
