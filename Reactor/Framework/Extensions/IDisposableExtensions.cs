﻿using System;
using System.Collections.Generic;
using Reactor.Systems.Executor;

namespace Reactor.Extensions
{
    public static class IDisposableExtensions
    {
        public static void DisposeAll(this IEnumerable<IDisposable> disposables)
        { disposables.ForEachRun(x => x.Dispose()); }

        public static void DisposeAll(this IEnumerable<SubscriptionToken> disposables)
        { disposables.ForEachRun(x => x.Disposable.Dispose()); }

        public static void AddTo(this IDisposable currentDisposable, ICollection<IDisposable> disposables)
        { disposables.Add(currentDisposable); }
    }
}