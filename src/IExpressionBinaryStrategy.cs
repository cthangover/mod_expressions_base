namespace Cthangover.Mods.Expressions
{
    using Cthangover.Mods.Stats.Formulas;
    /// <summary>A binary operator with explicit binding strength; higher precedence binds first.</summary>
    public interface IExpressionBinaryStrategy
    {
        string Symbol { get; }
        int Precedence { get; }
        bool RightAssociative => false;
        double Evaluate(ExpressionEvaluationContext context, FormulaNode left, FormulaNode right);
    }
}
