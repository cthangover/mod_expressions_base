namespace Cthangover.Mods.Stats.Formulas
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cthangover.Mods.Expressions;
    /// <summary>Compatibility facade; functions are independent strategies shared by all expression consumers.</summary>
    public static class FormulaFunctions
    {
        public static readonly IReadOnlyDictionary<string,(int Min,int Max)> Arity=new FormulaFunctionArities();
        public static bool Accepts(string name,int count)=>ExpressionStrategies.TryFunction(name,out var fn)&&count>=fn.MinArguments&&(fn.MaxArguments<0||count<=fn.MaxArguments);
        public static double Call(string name,double[] args,Random random)
            =>ExpressionStrategies.TryFunction(name,out var fn)?fn.Evaluate(new ExpressionEvaluationContext(null,false,random),args.Select(v=>new FormulaNode(FormulaNodeKind.Number,v)).ToArray()):0;
    }
}
