using UnityEngine;

public class BusinessCalculator
{
    private const float PercentCoefficient = 100f;

    public int CalculateIncome(BusinessComponent business, BusinessConfig config)
    {
        var baseIncome = config.BaseIncome * business.Level;
        var multiplier = 1f;

        if (business.IsUpgrade1Bought)
            multiplier += config.Upgrade1Multiplier / PercentCoefficient;

        if (business.IsUpgrade2Bought)
            multiplier += config.Upgrade2Multiplier / PercentCoefficient;

        return Mathf.RoundToInt(baseIncome * multiplier);
    }

    public int CalculateLevelUpCost(int currentLevel, BusinessConfig config)
    {
        return (currentLevel + 1) * config.BaseCost;
    }

    public bool CanBuyLevelUp(int balance, int currentLevel, BusinessConfig config)
    {
        return balance >= CalculateLevelUpCost(currentLevel, config);
    }

    public bool CanBuyUpgrade(int balance, int upgradeCost, bool isUpgradeBought, int businessLevel)
    {
        return !isUpgradeBought && balance >= upgradeCost && businessLevel > 0;
    }
}
