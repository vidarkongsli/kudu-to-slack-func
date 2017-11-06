using System;

namespace Kudu2SlackFuncApp
{
    public static class FunctionalExtensions
    {
        public static TOut Map<TIn, TOut>(this TIn @this, Func<TIn, TOut> map) where TIn : class
        {
            return @this == null ? default(TOut) : map(@this);
        }

        public static T Tee<T>(this T @this, Action<T> action) where T : class
        {
            if (@this != null) action(@this);
            return @this;
        }
    }
}
