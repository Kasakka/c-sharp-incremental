using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using incremental.Models;
using incremental.UI;
using incremental.Utils;

namespace incremental.Game;

public class Game1 : Microsoft.Xna.Framework.Game
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
        
        _graphics.PreferredBackBufferWidth = Constants.Window.Width;
        _graphics.PreferredBackBufferHeight = Constants.Window.Height;
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        _resources = new Dictionary<string, Resource>
        {
            { "Wood", new Resource("Wood", 0, 0, 1, Constants.Resources.WoodBasePrice) },
            { "Stone", new Resource("Stone", 0, 0, 1, Constants.Resources.StoneBasePrice) },
            { "Iron", new Resource("Iron", 0, 0, 1, Constants.Resources.IronBasePrice) },
            { "Copper", new Resource("Copper", 0, 0, 1, Constants.Resources.CopperBasePrice) },
            { "Coal", new Resource("Coal", 0, 0, 1, Constants.Resources.CoalBasePrice) },
            { "Gold", new Resource("Gold", 0, 0, 0) }
        };

        _buttons = new Dictionary<string, Button>();
        int yPos = Constants.UI.PanelPadding + 30;
        foreach (var resource in _resources.Where(r => r.Key != "Gold"))
        {
            _buttons.Add(resource.Key, new Button(
                new Rectangle(Constants.UI.PanelPadding + 10, yPos, 
                            Constants.UI.ResourceButtonWidth, Constants.UI.ButtonHeight), 
                $"Gather {resource.Key}"));
            _buttons.Add($"Sell{resource.Key}", new Button(
                new Rectangle(Constants.UI.PanelPadding + Constants.UI.ResourceButtonWidth + 20, yPos,
                            Constants.UI.SellButtonWidth, Constants.UI.ButtonHeight), 
                $"Sell {resource.Key}"));
            yPos += Constants.UI.ButtonHeight + 10;
        }

        _buttons.Add("SellAll", new Button(
            new Rectangle(Constants.UI.PanelPadding + 10, yPos,
                        Constants.UI.ResourceButtonWidth + Constants.UI.SellButtonWidth + 10, 
                        Constants.UI.ButtonHeight), 
            "Sell All Resources"));

        _upgrades = new Dictionary<string, List<Upgrade>>();
        foreach (var resource in _resources.Where(r => r.Key != "Gold"))
        {
            _upgrades[resource.Key] = new List<Upgrade> {
                new Upgrade($"Better {resource.Key} Tool", $"+1 {resource.Key} per click", 
                          resource.Key, Constants.Upgrades.BaseClickUpgradeCost, 
                          Constants.Upgrades.ClickUpgradeMultiplier, 1, UpgradeType.ClickPower),
                new Upgrade($"{resource.Key} Automator", $"+1 {resource.Key} per second", 
                          resource.Key, Constants.Upgrades.BaseAutoUpgradeCost, 
                          Constants.Upgrades.AutoUpgradeMultiplier, 1, UpgradeType.AutomaticProduction)
            };
        }

        _upgradeButtons = new Dictionary<string, Button>();
        yPos = Constants.UI.PanelPadding + 30;
        foreach (var resourcePair in _upgrades)
        {
            foreach (var upgrade in resourcePair.Value)
            {
                string buttonKey = $"{resourcePair.Key}_{upgrade.Name}";
                _upgradeButtons[buttonKey] = new Button(
                    new Rectangle(400, yPos, Constants.UI.UpgradeButtonWidth, Constants.UI.ButtonHeight),
                    $"{upgrade.Name} ({(int)upgrade.GetCurrentCost()} Gold)"
                );
                yPos += Constants.UI.ButtonHeight + 10;
            }
        }
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _font = Content.Load<SpriteFont>("Fonts/Arial");
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
        GraphicsDevice.Clear(Constants.Colors.Background);

        _spriteBatch.Begin();

        _spriteBatch.Draw(_pixel, new Rectangle(20, 20, 330, 400), Constants.Colors.PanelBackground);
        _spriteBatch.Draw(_pixel, new Rectangle(390, 20, 320, 700), Constants.Colors.PanelBackground);

        _spriteBatch.DrawString(_font, "RESOURCES", new Vector2(30, 25), Constants.Colors.TextLight);
        _spriteBatch.DrawString(_font, "UPGRADES", new Vector2(400, 25), Constants.Colors.TextLight);

        foreach (var pair in _buttons.Concat(_upgradeButtons))
        {
            var button = pair.Value;
            Color buttonColor;
            
            if (pair.Key.StartsWith("Sell"))
            {
                buttonColor = button.WasPressed ? Constants.Colors.SellButtonHover : 
                             button.IsHovered ? Constants.Colors.SellButtonHover : 
                             Constants.Colors.SellButton;
            }
            else if (!pair.Key.Contains("_")) // Resource gathering buttons
            {
                buttonColor = GetResourceButtonColor(pair.Key, button.IsHovered, button.WasPressed);
            }
            else // Upgrade buttons
            {
                buttonColor = button.WasPressed ? Constants.Colors.ButtonPressed :
                             button.IsHovered ? Constants.Colors.ButtonHover :
                             Constants.Colors.ButtonNormal;
            }
            
            _spriteBatch.Draw(_pixel, button.Bounds, buttonColor);
            
            Vector2 textSize = _font.MeasureString(button.Text);
            Vector2 textPos = new Vector2(
                button.Bounds.X + (button.Bounds.Width - textSize.X) / 2,
                button.Bounds.Y + (button.Bounds.Height - textSize.Y) / 2
            );
            _spriteBatch.DrawString(_font, button.Text, textPos, Constants.Colors.TextLight);
        }

        int yOffset = 450;
        foreach (var pair in _resources)
        {
            Color resourceColor = GetResourceColor(pair.Key);
            string text = $"{pair.Key}: {(int)pair.Value.Amount}";
            if (pair.Key != "Gold")
            {
                text += $" (Sells for {(int)pair.Value.SellPrice} Gold each)";
            }
            _spriteBatch.DrawString(_font, text, new Vector2(30, yOffset), resourceColor);
            
            if (pair.Key != "Gold")
            {
                string rateText = $"Per Second: {(int)pair.Value.PerSecond}";
                _spriteBatch.DrawString(_font, rateText, new Vector2(30, yOffset + 20), Constants.Colors.TextGray);
                
                string clickText = $"Per Click: {(int)pair.Value.PerClick}";
                _spriteBatch.DrawString(_font, clickText, new Vector2(200, yOffset + 20), Constants.Colors.TextGray);
            }
            
            yOffset += 50;
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private Color GetResourceButtonColor(string resourceName, bool isHovered, bool isPressed)
    {
        if (isPressed)
        {
            return GetResourceColor(resourceName, true);
        }
        return isHovered ? GetResourceColor(resourceName, true) : GetResourceColor(resourceName);
    }

    private Color GetResourceColor(string resourceName, bool hover = false)
    {
        return resourceName switch
        {
            "Wood" => hover ? Constants.Colors.Resources.WoodHover : Constants.Colors.Resources.Wood,
            "Stone" => hover ? Constants.Colors.Resources.StoneHover : Constants.Colors.Resources.Stone,
            "Iron" => hover ? Constants.Colors.Resources.IronHover : Constants.Colors.Resources.Iron,
            "Copper" => hover ? Constants.Colors.Resources.CopperHover : Constants.Colors.Resources.Copper,
            "Coal" => hover ? Constants.Colors.Resources.CoalHover : Constants.Colors.Resources.Coal,
            "Gold" => Constants.Colors.GoldText,
            _ => Constants.Colors.ButtonNormal
        };
    }

    protected override void UnloadContent()
    {
        _pixel?.Dispose();
        base.UnloadContent();
    }
} 