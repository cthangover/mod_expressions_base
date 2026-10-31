namespace Cthangover.Mods.Expressions
{
    using System;
    /// <summary>A valid expression whose runtime inputs or result are invalid.</summary>
    public class ExpressionEvaluationException : Exception
    {
        public ExpressionEvaluationException(string message) : base(message) { }
    }
}
