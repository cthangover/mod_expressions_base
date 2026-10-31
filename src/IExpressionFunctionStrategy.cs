namespace Cthangover.Mods.Expressions
{
    using System.Collections.Generic;
    using Cthangover.Mods.Stats.Formulas;
    /// <summary>A named numeric function. Evaluate only the operands your function needs through the context.</summary>
    public interface IExpressionFunctionStrategy
    {
        string Name { get; }
        int MinArguments { get; }
        int MaxArguments { get; }
        double Evaluate(ExpressionEvaluationContext context, IReadOnlyList<FormulaNode> arguments);
    }
}
