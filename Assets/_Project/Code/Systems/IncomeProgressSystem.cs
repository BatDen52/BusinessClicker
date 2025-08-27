using Leopotam.EcsLite;
using UnityEngine;

public class IncomeProgressSystem : IEcsInitSystem, IEcsRunSystem
{
    private readonly StaticDataService _staticData;
    private readonly BusinessCalculator _calculator;

    private EcsWorld _world;
    private EcsFilter _businessFilter;
    private EcsFilter _balanceFilter;

    private EcsPool<BusinessComponent> _businessPool;
    private EcsPool<PlayerBalanceComponent> _balancePool;
    private EcsPool<BusinessChangedEvent> _businessChangedPool;
    private EcsPool<BalanceChangedEvent> _balanceChangedPool;
    private EcsPool<ProgressChangedEvent> _progressChangedPool;

    public IncomeProgressSystem(StaticDataService staticData, BusinessCalculator calculator)
    {
        _staticData = staticData;
        _calculator = calculator;
    }

    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();

        _businessFilter = _world.Filter<BusinessComponent>().End();
        _balanceFilter = _world.Filter<PlayerBalanceComponent>().End();

        _businessPool = _world.GetPool<BusinessComponent>();
        _balancePool = _world.GetPool<PlayerBalanceComponent>();
        _businessChangedPool = _world.GetPool<BusinessChangedEvent>();
        _balanceChangedPool = _world.GetPool<BalanceChangedEvent>();
        _progressChangedPool = _world.GetPool<ProgressChangedEvent>();
    }

    public void Run(IEcsSystems systems)
    {
        const float TargetProgress = 1f;

        if (_balanceFilter.GetEntitiesCount() == 0)
            return;

        int balanceEntity = _balanceFilter.GetRawEntities()[0];
        ref var balance = ref _balancePool.Get(balanceEntity);

        foreach (var businessEntity in _businessFilter)
        {
            ref var business = ref _businessPool.Get(businessEntity);

            if (business.Level <= 0)
                continue;

            var config = _staticData.GetConfig(business.Id);
            business.Progress += Time.deltaTime / config.IncomeDelay;

            if (_progressChangedPool.Has(businessEntity) == false)
                 _progressChangedPool.Add(businessEntity);
            
            if (business.Progress >= TargetProgress)
            {
                balance.Value += _calculator.CalculateIncome(business, config);
                business.Progress = 0f;

                if (_balanceChangedPool.Has(balanceEntity) == false)
                    _balanceChangedPool.Add(balanceEntity);

                if (_businessChangedPool.Has(businessEntity) == false)
                    _businessChangedPool.Add(businessEntity);
            }
        }
    }
}