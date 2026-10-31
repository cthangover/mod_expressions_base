namespace Cthangover.Mods.Expressions
{
    using Cthangover.Mods.Stats.Formulas;
    internal class ArithmeticUnaryStrategy : IExpressionUnaryStrategy
    {
        public string Symbol {get;}
        public int Precedence=>6;
        public ArithmeticUnaryStrategy(string symbol){Symbol=symbol;}
        public double Evaluate(ExpressionEvaluationContext context,FormulaNode operand)
        {var value=context.Value(operand);return Symbol switch {"-"=>-value,"!"=>value==0?1:0,_=>value};}
    }
}
