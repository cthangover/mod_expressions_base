namespace Cthangover.Mods.Expressions.Tests
{
    using Cthangover.Mods.Stats.Formulas;
    internal class JoinOperator : IExpressionBinaryStrategy
    {
        public string Symbol=>"%%";
        public int Precedence=>4;
        public double Evaluate(ExpressionEvaluationContext context,FormulaNode left,FormulaNode right)=>context.Value(left)*10+context.Value(right);
    }
}
