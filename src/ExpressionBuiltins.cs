namespace Cthangover.Mods.Expressions
{
    using System.Collections.Generic;
    /// <summary>Default vocabulary is supplied as strategies; the parser does not contain a function or operator table.</summary>
    internal static class ExpressionBuiltins
    {
        public static IEnumerable<IExpressionFunctionStrategy> Functions()
        {
            yield return new MathFunctionStrategy("min",1,-1);yield return new MathFunctionStrategy("max",1,-1);
            foreach(var name in new[]{"floor","ceil","round","abs","sqrt","chance"})yield return new MathFunctionStrategy(name,1,1);
            yield return new MathFunctionStrategy("clamp",3,3);yield return new MathFunctionStrategy("if",3,3);
            yield return new MathFunctionStrategy("rand",2,2);yield return new RandomIntegerFunction();
        }
        public static IEnumerable<IExpressionBinaryStrategy> Binary()
        {
            yield return new ArithmeticBinaryStrategy("||",1);yield return new ArithmeticBinaryStrategy("&&",2);
            foreach(var op in new[]{"<=",">=","==","!=","<",">"})yield return new ArithmeticBinaryStrategy(op,3);
            foreach(var op in new[]{"+","-"})yield return new ArithmeticBinaryStrategy(op,4);
            foreach(var op in new[]{"*","/","%"})yield return new ArithmeticBinaryStrategy(op,5);
            yield return new ArithmeticBinaryStrategy("^",7);
        }
        public static IEnumerable<IExpressionUnaryStrategy> Unary()
        {foreach(var op in new[]{"+","-","!"})yield return new ArithmeticUnaryStrategy(op);}
    }
}
