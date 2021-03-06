using System.Collections.Generic;
using Reactor.Entities;
using Reactor.Extensions;
using Reactor.Groups;
using Reactor.Pools;
using UniRx;

namespace Reactor.Systems.Executor.Handlers
{
    public class SetupSystemHandler : ISetupSystemHandler
    {
        public IPoolManager PoolManager { get; private set; }

        public SetupSystemHandler(IPoolManager poolManager)
        {
            PoolManager = poolManager;
        }

        public IEnumerable<SubscriptionToken> Setup(ISetupSystem system)
        {
            var subscriptions = new List<SubscriptionToken>();
            var entities = PoolManager.GetEntitiesFor(system.TargetGroup);
            entities.ForEachRun(x =>
            {
                var possibleSubscription = ProcessEntity(system, x);
                if (possibleSubscription != null)
                {
                    subscriptions.Add(possibleSubscription);
                }
            });

            return subscriptions;
        }

        public SubscriptionToken ProcessEntity(ISetupSystem system, IEntity entity)
        {
            var groupPredicate = system.TargetGroup as IHasPredicate;

            if (groupPredicate == null)
            {
                system.Setup(entity);
                return null;
            }

            if (groupPredicate.CanProcessEntity(entity))
            {
                system.Setup(entity);
                return null;
            }

            var subscription = entity.WaitForPredicateMet(groupPredicate.CanProcessEntity)
                .Subscribe(system.Setup);

            var subscriptionToken = new SubscriptionToken(entity, subscription);
            return subscriptionToken;
        }
    }
}