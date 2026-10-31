namespace Cthangover.Mods.Expressions.Tests
{
    using System.Collections.Generic;
    using Cthangover.Mods.Stats.Formulas;
    internal class TripleFunction : IExpressionFunctionStrategy
    {
        public string Name=>"unit_triple";
        public int MinArguments=>1;
        public int MaxArguments=>1;
        public double Evaluate(ExpressionEvaluationContext context,IReadOnlyList<FormulaNode> args)=>context.Value(args[0])*3;
    }
}
