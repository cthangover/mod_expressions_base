namespace Cthangover.Mods.Expressions
{
    using Cthangover.Mods.Stats.Formulas;
    /// <summary>A prefix operator. Its precedence determines how much of the following expression it consumes.</summary>
    public interface IExpressionUnaryStrategy
    {
        string Symbol { get; }
        int Precedence { get; }
        double Evaluate(ExpressionEvaluationContext context, FormulaNode operand);
    }
}
