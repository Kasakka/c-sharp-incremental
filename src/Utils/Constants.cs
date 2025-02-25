using Microsoft.Xna.Framework;

namespace incremental.Utils;

public static class Constants
{
    public static class Colors
    {
        public static readonly Color Background = new(34, 40, 49);
        public static readonly Color PanelBackground = new(57, 62, 70);
        public static readonly Color ButtonNormal = new(0, 173, 181);
        public static readonly Color ButtonHover = new(0, 150, 157);
        public static readonly Color ButtonPressed = new(0, 130, 137);
        public static readonly Color SellButton = new(238, 82, 83);
        public static readonly Color SellButtonHover = new(200, 70, 70);
        public static readonly Color TextLight = new(238, 238, 238);
        public static readonly Color TextDark = new(57, 62, 70);
        public static readonly Color TextGray = new(200, 200, 200);
        public static readonly Color GoldText = new(255, 198, 42);

        public static class Resources
        {
            public static readonly Color Wood = new(139, 69, 19);    // Brown
            public static readonly Color WoodHover = new(160, 82, 45);
            public static readonly Color Stone = new(128, 128, 128); // Gray
            public static readonly Color StoneHover = new(169, 169, 169);
            public static readonly Color Iron = new(176, 196, 222);  // Steel Blue
            public static readonly Color IronHover = new(192, 212, 238);
            public static readonly Color Copper = new(184, 115, 51); // Copper
            public static readonly Color CopperHover = new(200, 131, 67);
            public static readonly Color Coal = new(47, 47, 47);     // Dark Gray
            public static readonly Color CoalHover = new(64, 64, 64);
        }
    }

    public static class Window
    {
        public const int Width = 1024;
        public const int Height = 768;
    }

    public static class UI
    {
        public const int ButtonHeight = 40;
        public const int ResourceButtonWidth = 180;
        public const int SellButtonWidth = 120;
        public const int UpgradeButtonWidth = 300;
        public const int PanelPadding = 20;
    }

    public static class Resources
    {
        public const decimal WoodBasePrice = 1;
        public const decimal StoneBasePrice = 2;
        public const decimal IronBasePrice = 5;
        public const decimal CopperBasePrice = 4;
        public const decimal CoalBasePrice = 3;
    }

    public static class Upgrades
    {
        public const decimal BaseClickUpgradeCost = 10;
        public const decimal BaseAutoUpgradeCost = 50;
        public const decimal ClickUpgradeMultiplier = 1.5m;
        public const decimal AutoUpgradeMultiplier = 1.8m;
    }
} 