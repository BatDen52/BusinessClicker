using Leopotam.EcsLite;

public class BuyUpgradeSystem : IEcsInitSystem, IEcsRunSystem
{
    private readonly StaticDataService _staticData;
    private readonly BusinessCalculator _calculator;

    private EcsWorld _world;
    private EcsFilter _upgradeFilter;
    private EcsFilter _balanceFilter;

    private EcsPool<BuyUpgradeEvent> _upgradePool;
    private EcsPool<BusinessComponent> _businessPool;
    private EcsPool<PlayerBalanceComponent> _balancePool;
    private EcsPool<BusinessChangedEvent> _businessChangedPool;
    private EcsPool<BalanceChangedEvent> _balanceChangedPool;

    public BuyUpgradeSystem(StaticDataService staticData, BusinessCalculator calculator)
    {
        _staticData = staticData;
        _calculator = calculator;
    }

    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();

        _upgradeFilter = _world.Filter<BuyUpgradeEvent>().Inc<BusinessComponent>().End();
        _balanceFilter = _world.Filter<PlayerBalanceComponent>().End();

        _upgradePool = _world.GetPool<BuyUpgradeEvent>();
        _businessPool = _world.GetPool<BusinessComponent>();
        _balancePool = _world.GetPool<PlayerBalanceComponent>();
        _businessChangedPool = _world.GetPool<BusinessChangedEvent>();
        _balanceChangedPool = _world.GetPool<BalanceChangedEvent>();
    }

    public void Run(IEcsSystems systems)
    {
        int balanceEntity = -1;
        var balanceEntities = _balanceFilter.GetRawEntities();
        if (balanceEntities.Length > 0)
        {
            balanceEntity = balanceEntities[0];
            ref var balance = ref _balancePool.Get(balanceEntity);

            foreach (var businessEntity in _upgradeFilter)
            {
                ref var upgradeEvent = ref _upgradePool.Get(businessEntity);
                ref var business = ref _businessPool.Get(businessEntity);
                BusinessConfig config = _staticData.GetConfig(business.Id);

                switch (upgradeEvent.Type)
                {
                    case UpgradeType.Upgrade1:
                        HandleUpgrade(config.Upgrade1Cost, ref business, ref business.IsUpgrade1Bought, ref balance.Value, businessEntity, balanceEntity);
                        break;
                    case UpgradeType.Upgrade2:
                        HandleUpgrade(config.Upgrade2Cost, ref business, ref business.IsUpgrade2Bought, ref balance.Value, businessEntity, balanceEntity);
                        break;
                }

                _upgradePool.Del(businessEntity);
            }
        }
    }

    private void HandleUpgrade(int cost, ref BusinessComponent business, ref bool isBought, ref int balance, int businessEntity, int balanceEntity)
    {
        if (_calculator.CanBuyUpgrade(balance, cost, isBought, business.Level))
        {
            balance -= cost;
            isBought = true;

            if (_balanceChangedPool.Has(balanceEntity) == false)
                _balanceChangedPool.Add(balanceEntity);

            if (_businessChangedPool.Has(businessEntity) == false)
                _businessChangedPool.Add(businessEntity);
        }
    }
}