namespace Cthangover.Mods.Expressions
{
    using System;
    using System.Collections.Generic;
    using Cthangover.Mods.Stats.Formulas;
    /// <summary>Inclusive integer random bounds, validated before consuming a random value.</summary>
    internal class RandomIntegerFunction : IExpressionFunctionStrategy
    {
        public string Name=>"rnd";
        public int MinArguments=>2;
        public int MaxArguments=>2;
        public double Evaluate(ExpressionEvaluationContext context,IReadOnlyList<FormulaNode> arguments)
        {
            var lo=context.Value(arguments[0]);var hi=context.Value(arguments[1]);
            if(!double.IsFinite(lo)||!double.IsFinite(hi)||lo!=Math.Truncate(lo)||hi!=Math.Truncate(hi)||lo>hi||lo<int.MinValue||hi>int.MaxValue)
                throw new ExpressionEvaluationException("rnd requires increasing integer bounds in the Int32 range.");
            return context.Random.NextInt64((long)lo,(long)hi+1);
        }
    }
}
