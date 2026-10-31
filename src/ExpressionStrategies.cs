namespace Cthangover.Mods.Expressions
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Cthangover.Mods.Stats.Formulas;
    /// <summary>First registered strategy wins, as with DSL commands. Cached syntax never retains plugin instances.</summary>
    public static class ExpressionStrategies
    {
        private static readonly ConcurrentDictionary<string,IExpressionFunctionStrategy> functions=new(StringComparer.OrdinalIgnoreCase);
        private static readonly ConcurrentDictionary<string,IExpressionBinaryStrategy> binary=new(StringComparer.Ordinal);
        private static readonly ConcurrentDictionary<string,IExpressionUnaryStrategy> unary=new(StringComparer.Ordinal);
        static ExpressionStrategies()
        {
            foreach(var fn in ExpressionBuiltins.Functions()) functions.TryAdd(fn.Name,fn);
            foreach(var op in ExpressionBuiltins.Binary()) binary.TryAdd(op.Symbol,op);
            foreach(var op in ExpressionBuiltins.Unary()) unary.TryAdd(op.Symbol,op);
        }
        public static IReadOnlyCollection<IExpressionFunctionStrategy> Functions => functions.Values.ToArray();
        public static string[] OperatorTokens => binary.Keys.Concat(unary.Keys).Distinct().OrderByDescending(s=>s.Length).ThenBy(s=>s,StringComparer.Ordinal).ToArray();
        public static bool TryFunction(string name,out IExpressionFunctionStrategy value)=>functions.TryGetValue(name,out value);
        public static bool TryBinary(string token,out IExpressionBinaryStrategy value)=>binary.TryGetValue(token,out value);
        public static bool TryUnary(string token,out IExpressionUnaryStrategy value)=>unary.TryGetValue(token,out value);
        public static bool Register(IExpressionFunctionStrategy strategy)
        {
            if(string.IsNullOrWhiteSpace(strategy?.Name)||strategy.Name.Any(c=>!char.IsLetterOrDigit(c)&&c!='_')||char.IsDigit(strategy.Name[0])||strategy.MinArguments<0||strategy.MaxArguments>=0&&strategy.MaxArguments<strategy.MinArguments)
                throw new ArgumentException("Invalid expression function signature.");
            var added=functions.TryAdd(strategy.Name,strategy);if(added)Formula.ClearCache();return added;
        }
        public static bool Register(IExpressionBinaryStrategy strategy)
        {
            ValidateOperator(strategy?.Symbol,strategy?.Precedence??0);
            var added=binary.TryAdd(strategy.Symbol,strategy);if(added)Formula.ClearCache();return added;
        }
        public static bool Register(IExpressionUnaryStrategy strategy)
        {
            ValidateOperator(strategy?.Symbol,strategy?.Precedence??0);
            var added=unary.TryAdd(strategy.Symbol,strategy);if(added)Formula.ClearCache();return added;
        }
        public static void Unregister(IExpressionFunctionStrategy strategy)
        {if(functions.TryGetValue(strategy.Name,out var current)&&ReferenceEquals(current,strategy)){functions.TryRemove(strategy.Name,out _);Formula.ClearCache();}}
        public static void Unregister(IExpressionBinaryStrategy strategy)
        {if(binary.TryGetValue(strategy.Symbol,out var current)&&ReferenceEquals(current,strategy)){binary.TryRemove(strategy.Symbol,out _);Formula.ClearCache();}}
        public static void Unregister(IExpressionUnaryStrategy strategy)
        {if(unary.TryGetValue(strategy.Symbol,out var current)&&ReferenceEquals(current,strategy)){unary.TryRemove(strategy.Symbol,out _);Formula.ClearCache();}}
        private static void ValidateOperator(string symbol,int precedence)
        {
            if(string.IsNullOrEmpty(symbol)||symbol.Any(c=>!"+-*/%^<>=!&|~".Contains(c))||precedence<1||precedence>100)
                throw new ArgumentException("Operators need a punctuation symbol and precedence in 1..100.");
        }
    }
}
