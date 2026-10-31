namespace Cthangover.Mods.Expressions.Tests
{
    using Cthangover.Core.Mods.Testing;
    using Cthangover.Mods.Stats.Formulas;

    /// <summary>Language contracts independent of dialog state, inventory, RPG stats and rendering.</summary>
    public class ExpressionStrategyTests
    {
        [ModTest("expressions/strict_arithmetic_references_and_short_circuit")]
        static void Arithmetic(ModTestContext ctx)
        {
            var scope=new NumericTestScope();scope.Values["Value"]=7;
            ModAssert.CloseTo(14,Formula.Parse("2+3*4").EvaluateStrict(scope),what:"precedence");
            ModAssert.CloseTo(-21,Formula.Parse("-(1+2)*${Value}").EvaluateStrict(scope),what:"case-preserving reference");
            ModAssert.CloseTo(-4,Formula.Parse("-2^2").EvaluateStrict(scope),what:"power before unary minus");
            ModAssert.CloseTo(512,Formula.Parse("2^3^2").EvaluateStrict(scope),what:"right associative power");
            ModAssert.CloseTo(1002,Formula.Parse("1e3+2").EvaluateStrict(scope),what:"scientific numbers");
            ModAssert.CloseTo(0,Formula.Parse("0 && 1/0").EvaluateStrict(scope),what:"logical short circuit");
            ModAssert.CloseTo(7,Formula.Parse("if(0,1/0,7)").EvaluateStrict(scope),what:"strict conditional function is lazy");
            ModAssert.True(Fails("1/0",scope)&&Fails("unknown+1",scope)&&Fails("10^1000",scope),"invalid inputs never silently become zero");
            ModAssert.CloseTo(0,Formula.Parse("1/0").Evaluate(scope),what:"legacy zero division remains forgiving");
            ModAssert.CloseTo(0,Formula.Parse("unknown").Evaluate(scope),what:"legacy missing names remain zero");
        }
        [ModTest("expressions/rnd_inclusive_endpoints_and_invalid_bounds")]
        static void Random(ModTestContext ctx)
        {
            var random=new BoundaryRandom();var scope=new NumericTestScope { Random=random };
            var expression=Formula.Parse("rnd(3,5)");
            ModAssert.Equal(0,random.Calls,"parsing does not roll");
            ModAssert.CloseTo(3,expression.EvaluateStrict(scope),what:"inclusive lower bound");
            random.Upper=true;
            ModAssert.CloseTo(5,expression.EvaluateStrict(scope),what:"inclusive upper bound");
            ModAssert.CloseTo(3,Formula.Parse("rnd(3,3)").EvaluateStrict(scope),what:"equal bounds");
            ModAssert.CloseTo(int.MaxValue,Formula.Parse("rnd(2147483647,2147483647)").EvaluateStrict(scope),what:"upper bound does not overflow");
            var calls=random.Calls;
            ModAssert.True(Fails("rnd(5,3)",scope)&&Fails("rnd(1.5,3)",scope)&&Fails("rnd(-2147483649,3)",scope),"bad ranges rejected");
            ModAssert.Equal(calls,random.Calls,"invalid ranges do not consume randomness");
        }
        [ModTest("expressions/mod_strategies_and_cache_invalidation")]
        static void Strategies(ModTestContext ctx)
        {
            var function=new TripleFunction();var duplicate=new TripleFunction();var op=new JoinOperator();var scope=new NumericTestScope();
            ModAssert.False(Formula.TryParse("unit_triple(4)",out _,out _),"function starts unavailable");
            try
            {
                ModAssert.True(ExpressionStrategies.Register(function),"mod function registered");
                ModAssert.False(ExpressionStrategies.Register(duplicate),"first strategy wins");
                ExpressionStrategies.Unregister(duplicate);
                var cached=Formula.Parse("unit_triple(4)+1");
                ModAssert.CloseTo(13,cached.EvaluateStrict(scope),what:"function participates in expressions");
                ModAssert.True(ExpressionStrategies.Register(op),"mod operator registered");
                ModAssert.CloseTo(42,Formula.Parse("1+2%%3*4").EvaluateStrict(scope),what:"custom precedence and longest operator token");
                ExpressionStrategies.Unregister(function);
                ModAssert.False(Formula.TryParse("unit_triple(4)",out _,out _),"unload invalidates parsed vocabulary");
                var rejected=false;try{cached.EvaluateStrict(scope);}catch(ExpressionEvaluationException){rejected=true;}
                ModAssert.True(rejected,"an existing tree never retains or invokes an unloaded plugin");
            }
            finally{ExpressionStrategies.Unregister(function);ExpressionStrategies.Unregister(op);}
        }
        private static bool Fails(string text,NumericTestScope scope)
        {try{Formula.Parse(text).EvaluateStrict(scope);return false;}catch(ExpressionEvaluationException){return true;}}
    }
}
