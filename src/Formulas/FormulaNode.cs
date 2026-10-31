namespace Cthangover.Mods.Stats.Formulas
{
    using System;
    using System.Collections.Generic;
    using Cthangover.Mods.Expressions;

    /// <summary>Immutable parsed syntax. Operator/function names are resolved at execution, so caches retain no plugin objects.</summary>
    public class FormulaNode
    {
        public FormulaNode(FormulaNodeKind kind,double value=0,string name=null,string op=null,FormulaNode[] children=null)
        {Kind=kind;Value=value;Name=name;Op=op;Children=children??Array.Empty<FormulaNode>();}
        public FormulaNodeKind Kind{get;}
        public double Value{get;}
        public string Name{get;}
        public string Op{get;}
        public IReadOnlyList<FormulaNode> Children{get;}
        /// <summary>Compatible stats behavior: missing variables and zero division evaluate as zero.</summary>
        public double Evaluate(IFormulaScope scope)=>new ExpressionEvaluationContext(scope).Value(this);
        /// <summary>Assignments use strict finite numeric inputs and fail instead of substituting fallback values.</summary>
        public double EvaluateStrict(IFormulaScope scope)=>new ExpressionEvaluationContext(scope,true).Value(this);
        public double EvaluateIn(ExpressionEvaluationContext context)
        {
            switch(Kind)
            {
                case FormulaNodeKind.Number:return Value;
                case FormulaNodeKind.Variable:return context.Read(Name);
                case FormulaNodeKind.Conditional:return context.Value(Children[0])!=0?context.Value(Children[1]):context.Value(Children[2]);
                case FormulaNodeKind.Unary:
                    if(ExpressionStrategies.TryUnary(Op,out var unary))return unary.Evaluate(context,Children[0]);
                    throw new ExpressionEvaluationException("Unary strategy is unavailable: "+Op);
                case FormulaNodeKind.Binary:
                    if(ExpressionStrategies.TryBinary(Op,out var binary))return binary.Evaluate(context,Children[0],Children[1]);
                    throw new ExpressionEvaluationException("Binary strategy is unavailable: "+Op);
                case FormulaNodeKind.Call:
                    if(ExpressionStrategies.TryFunction(Name,out var function))return function.Evaluate(context,Children);
                    throw new ExpressionEvaluationException("Function strategy is unavailable: "+Name);
                default:throw new ExpressionEvaluationException("Unknown expression node kind.");
            }
        }
        public List<string> Variables(){var names=new List<string>();Collect(names);return names;}
        private void Collect(List<string> names)
        {
            if(Kind==FormulaNodeKind.Variable&&!names.Contains(Name))names.Add(Name);
            foreach(var child in Children)child.Collect(names);
        }
    }
}
