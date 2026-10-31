namespace Cthangover.Mods.Expressions
{
    using System;
    using Cthangover.Mods.Stats.Formulas;
    /// <summary>One evaluation policy and variable/random source shared by all nested strategies.</summary>
    public class ExpressionEvaluationContext
    {
        private readonly IFormulaScope scope;
        private readonly Random random;
        public bool Strict { get; }
        public Random Random => random ?? scope?.Random ?? System.Random.Shared;
        public ExpressionEvaluationContext(IFormulaScope scope, bool strict = false, Random random = null)
        { this.scope=scope; Strict=strict; this.random=random; }
        public double Read(string name)
        {
            if (scope != null && scope.TryGet(name,out var value)) return Checked(value);
            if (Strict) throw new ExpressionEvaluationException("Unknown numeric variable: " + name);
            return 0;
        }
        public double Value(FormulaNode node) => Checked(node.EvaluateIn(this));
        public double Checked(double value)
        {
            if (Strict && !double.IsFinite(value)) throw new ExpressionEvaluationException("Expression result is not a finite number.");
            return value;
        }
    }
}
