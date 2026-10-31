namespace Cthangover.Mods.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cthangover.Mods.Stats.Formulas;
    internal class MathFunctionStrategy : IExpressionFunctionStrategy
    {
        public string Name {get;}
        public int MinArguments {get;}
        public int MaxArguments {get;}
        public MathFunctionStrategy(string name,int min,int max){Name=name;MinArguments=min;MaxArguments=max;}
        public double Evaluate(ExpressionEvaluationContext context,IReadOnlyList<FormulaNode> arguments)
        {
            if(Name=="if"&&context.Strict)return context.Value(arguments[0])!=0?context.Value(arguments[1]):context.Value(arguments[2]);
            var args=new double[arguments.Count];for(var i=0;i<args.Length;i++)args[i]=context.Value(arguments[i]);
            switch(Name)
            {
                case "min":return args.Min();case "max":return args.Max();case "floor":return Math.Floor(args[0]);
                case "ceil":return Math.Ceiling(args[0]);case "round":return Math.Round(args[0],MidpointRounding.AwayFromZero);
                case "abs":return Math.Abs(args[0]);
                case "sqrt":
                    if(context.Strict&&args[0]<0)throw new ExpressionEvaluationException("Square root of a negative number.");
                    return args[0]<=0?0:Math.Sqrt(args[0]);
                case "clamp":return Math.Clamp(args[0],Math.Min(args[1],args[2]),Math.Max(args[1],args[2]));
                case "if":return args[0]!=0?args[1]:args[2];
                case "chance":return context.Random.NextDouble()*100<args[0]?1:0;
                case "rand":
                    var lo=(int)Math.Ceiling(Math.Min(args[0],args[1]));var hi=(int)Math.Floor(Math.Max(args[0],args[1]));
                    return hi<lo?lo:context.Random.Next(lo,hi+1);
                default:throw new ExpressionEvaluationException("Unknown standard function: "+Name);
            }
        }
    }
}
