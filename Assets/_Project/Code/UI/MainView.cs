using Leopotam.EcsLite;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainView : MonoBehaviour
{
    [Header("Balance")]
    [SerializeField] private TMP_Text _balanceText;

    [Header("Business Items")]
    [SerializeField] private BusinessItemView _businessItemPrefab;
    [SerializeField] private Transform _businessListContainer;

    private List<BusinessItemView> _businessItems = new();
    private EcsWorld _world;
    private StaticDataService _staticData;

    private void OnDestroy()
    {
        foreach (var item in _businessItems)
        {
            item.LevelUpClicked -= OnLevelUpClicked;
            item.Upgrade1Clicked -= OnUpgrade1Clicked;
            item.Upgrade2Clicked -= OnUpgrade2Clicked;
        }
    }

    public void Initialize(StaticDataService staticData, EcsWorld world)
    {
        _world = world;
        _staticData = staticData;
    }

    public BusinessItemView CreateBusiness(int id)
    {
        var businessItem = Instantiate(_businessItemPrefab, _businessListContainer);
        businessItem.Initialize(id, _staticData);

        businessItem.LevelUpClicked += OnLevelUpClicked;
        businessItem.Upgrade1Clicked += OnUpgrade1Clicked;
        businessItem.Upgrade2Clicked += OnUpgrade2Clicked;

        _businessItems.Add(businessItem);

        return businessItem;
    }

    public void UpdateBalance(int balance)
    {
        _balanceText.text = $"{_staticData.Lables.BalanceText}: ${balance}";
    }

    private void OnLevelUpClicked(int businessId) =>
       SendBuyLevelEvent(businessId);

    private void OnUpgrade1Clicked(int businessId) =>
        SendBuyUpgradeEvent(businessId, UpgradeType.Upgrade1);

    private void OnUpgrade2Clicked(int businessId) =>
        SendBuyUpgradeEvent(businessId, UpgradeType.Upgrade2);

    private void SendBuyLevelEvent(int businessId)
    {
        var buyLevelPool = _world.GetPool<BuyLevelEvent>();

        foreach (var entity in _world.Filter<BusinessComponent>().End())
        {
            ref var business = ref _world.GetPool<BusinessComponent>().Get(entity);
            
            if (business.Id == businessId)
            {
                buyLevelPool.Add(entity);
                break;
            }
        }
    }

    private void SendBuyUpgradeEvent(int businessId, UpgradeType type)
    {
        var upgradePool = _world.GetPool<BuyUpgradeEvent>();

        foreach (var entity in _world.Filter<BusinessComponent>().End())
        {
            ref var business = ref _world.GetPool<BusinessComponent>().Get(entity);
            
            if (business.Id == businessId)
            {
                ref var upgradeEvent = ref upgradePool.Add(entity);
                upgradeEvent.Type = type;
                break;
            }
        }
    }
}