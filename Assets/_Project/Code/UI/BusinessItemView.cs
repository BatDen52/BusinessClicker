using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BusinessItemView : MonoBehaviour
{
    [Header("Business Info")]
    [SerializeField] private TMP_Text _businessNameText;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private TMP_Text _incomeText;
    [SerializeField] private Image _progressBar;

    [Header("Level Up")]
    [SerializeField] private TMP_Text _levelUpCostText;
    [SerializeField] private Button _levelUpButton;

    [Header("Upgrade 1")]
    [SerializeField] private TMP_Text _upgrade1NameText;
    [SerializeField] private TMP_Text _upgrade1CostText;
    [SerializeField] private Button _upgrade1Button;

    [Header("Upgrade 2")]
    [SerializeField] private TMP_Text _upgrade2NameText;
    [SerializeField] private TMP_Text _upgrade2CostText;
    [SerializeField] private Button _upgrade2Button;

    private int _businessId;
    private StaticDataService _staticData;

    public event Action<int> LevelUpClicked;
    public event Action<int> Upgrade1Clicked;
    public event Action<int> Upgrade2Clicked;

    private void OnDestroy()
    {
        _levelUpButton.onClick.RemoveListener(OnLevelUpClicked);
        _upgrade1Button.onClick.RemoveListener(OnUpgrade1Clicked);
        _upgrade2Button.onClick.RemoveListener(OnUpgrade2Clicked);
    }

    public void Initialize(int businessId, StaticDataService staticData)
    {
        _staticData = staticData;
        _businessId = businessId;

        _levelUpButton.onClick.AddListener(OnLevelUpClicked);
        _upgrade1Button.onClick.AddListener(OnUpgrade1Clicked);
        _upgrade2Button.onClick.AddListener(OnUpgrade2Clicked);

        BusinessLocalization locale = _staticData.GetLocalization(businessId);

        _businessNameText.text = locale.BusinessName;
        _upgrade1NameText.text = locale.Upgrade1Name;
        _upgrade2NameText.text = locale.Upgrade2Name;
    }

    public void UpdateProgressView(float progress)
    {
        _progressBar.fillAmount = progress;
    }

    public void UpdateView(int level, int income, int levelUpCost, bool canBuyLevel, bool isUpgrade1Bought, 
        bool isUpgrade2Bought, int upgrade1Cost, int upgrade2Cost, bool canBuyUpgrade1, bool canBuyUpgrade2)
    {
        _levelText.text = $"{_staticData.Lables.LevelText}: {level}";
        _incomeText.text = $"{_staticData.Lables.IncomeText}: ${income}";

        _levelUpCostText.text = $"{_staticData.Lables.BuyButtonText}\n${levelUpCost}";
        _levelUpButton.interactable = canBuyLevel && level >= 0;

        _upgrade1CostText.text = isUpgrade1Bought ? _staticData.Lables.PurchasedText : $"${upgrade1Cost}";
        _upgrade1Button.interactable = isUpgrade1Bought == false && canBuyUpgrade1;

        _upgrade2CostText.text = isUpgrade2Bought ? _staticData.Lables.PurchasedText : $"${upgrade2Cost}";
        _upgrade2Button.interactable = isUpgrade2Bought == false && canBuyUpgrade2;
    }

    private void OnLevelUpClicked() =>
        LevelUpClicked?.Invoke(_businessId);

    private void OnUpgrade1Clicked() =>
        Upgrade1Clicked?.Invoke(_businessId);

    private void OnUpgrade2Clicked() =>
        Upgrade2Clicked?.Invoke(_businessId);
}