namespace incremental.Utils;

public static class Constants
{
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