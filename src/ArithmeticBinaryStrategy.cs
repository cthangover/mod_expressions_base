namespace Cthangover.Mods.Expressions
{
    using System;
    using Cthangover.Mods.Stats.Formulas;
    internal class ArithmeticBinaryStrategy : IExpressionBinaryStrategy
    {
        public string Symbol {get;}
        public int Precedence {get;}
        public bool RightAssociative=>Symbol=="^";
        public ArithmeticBinaryStrategy(string symbol,int precedence){Symbol=symbol;Precedence=precedence;}
        public double Evaluate(ExpressionEvaluationContext context,FormulaNode left,FormulaNode right)
        {
            var a=context.Value(left);
            if(Symbol=="&&")return a!=0&&context.Value(right)!=0?1:0;
            if(Symbol=="||")return a!=0||context.Value(right)!=0?1:0;
            var b=context.Value(right);
            if(context.Strict&&b==0&&(Symbol=="/"||Symbol=="%"))throw new ExpressionEvaluationException("Division or remainder by zero.");
            return Symbol switch {
                "+"=>a+b,"-"=>a-b,"*"=>a*b,"/"=>b==0?0:a/b,"%"=>b==0?0:a%b,"^"=>Math.Pow(a,b),
                "<"=>a<b?1:0,">"=>a>b?1:0,"<="=>a<=b?1:0,">="=>a>=b?1:0,"=="=>a==b?1:0,"!="=>a!=b?1:0,
                _=>throw new ExpressionEvaluationException("Unsupported arithmetic operator: "+Symbol)
            };
        }
    }
}
