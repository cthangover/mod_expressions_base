namespace Cthangover.Mods.Stats.Formulas
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Cthangover.Mods.Expressions;

    /// <summary>Pratt-style parsing from the registered operator vocabulary; functions are resolved through strategies.</summary>
    public class FormulaParser
    {
        private readonly string text;
        private readonly string[] operators;
        private int pos;
        private FormulaParser(string text){this.text=text??"";operators=ExpressionStrategies.OperatorTokens;}
        public static FormulaNode Parse(string text)
        {
            var parser=new FormulaParser(text);parser.SkipSpace();
            if(parser.End)throw parser.Error("empty formula");
            var result=parser.Conditional();parser.SkipSpace();
            if(!parser.End)throw parser.Error("unexpected '"+parser.text[parser.pos]+"'");
            return result;
        }
        private bool End=>pos>=text.Length;
        private FormulaNode Conditional()
        {
            var condition=Binary(0);
            if(!Match("?"))return condition;
            var yes=Conditional();Expect(":");var no=Conditional();
            return new FormulaNode(FormulaNodeKind.Conditional,children:new[]{condition,yes,no});
        }
        private FormulaNode Binary(int minimum)
        {
            var left=Primary();
            while(true)
            {
                var token=Operator();
                if(token==null||!ExpressionStrategies.TryBinary(token,out var strategy)||strategy.Precedence<minimum)return left;
                pos+=token.Length;
                var right=Binary(strategy.Precedence+(strategy.RightAssociative?0:1));
                left=new FormulaNode(FormulaNodeKind.Binary,op:token,children:new[]{left,right});
            }
        }
        private FormulaNode Primary()
        {
            SkipSpace();if(End)throw Error("expected a number, variable or '('");
            var token=Operator();
            if(token!=null&&ExpressionStrategies.TryUnary(token,out var prefix))
            {pos+=token.Length;return new FormulaNode(FormulaNodeKind.Unary,op:token,children:new[]{Binary(prefix.Precedence)});}
            if(Match("(")){var value=Conditional();Expect(")");return value;}
            if(Match("${"))
            {
                var start=pos;while(!End&&text[pos]!='}')pos++;
                if(End)throw Error("unterminated variable reference");
                var name=text.Substring(start,pos-start);pos++;
                if(string.IsNullOrWhiteSpace(name))throw Error("empty variable reference");
                return new FormulaNode(FormulaNodeKind.Variable,name:name);
            }
            if(char.IsDigit(text[pos])||text[pos]=='.')return Number();
            if(char.IsLetter(text[pos])||text[pos]=='_')return Name();
            throw Error("unexpected '"+text[pos]+"'");
        }
        private FormulaNode Number()
        {
            var start=pos;
            while(!End&&(char.IsDigit(text[pos])||text[pos]=='.'))pos++;
            if(!End&&(text[pos]=='e'||text[pos]=='E'))
            {pos++;if(!End&&(text[pos]=='+'||text[pos]=='-'))pos++;while(!End&&char.IsDigit(text[pos]))pos++;}
            var literal=text.Substring(start,pos-start);
            if(!double.TryParse(literal,NumberStyles.Float,CultureInfo.InvariantCulture,out var value))throw Error("bad number '"+literal+"'");
            return new FormulaNode(FormulaNodeKind.Number,value);
        }
        private FormulaNode Name()
        {
            var start=pos;
            while(!End&&(char.IsLetterOrDigit(text[pos])||text[pos]=='_'||text[pos]=='.'))pos++;
            var name=text.Substring(start,pos-start).TrimEnd('.').ToLowerInvariant();
            if(!Match("("))return new FormulaNode(FormulaNodeKind.Variable,name:name);
            var args=new List<FormulaNode>();
            if(!Match(")")){do{args.Add(Conditional());}while(Match(","));Expect(")");}
            if(!ExpressionStrategies.TryFunction(name,out var strategy))throw Error("unknown function '"+name+"'");
            if(args.Count<strategy.MinArguments||strategy.MaxArguments>=0&&args.Count>strategy.MaxArguments)throw Error("'"+name+"' does not take "+args.Count+" argument(s)");
            return new FormulaNode(FormulaNodeKind.Call,name:name,children:args.ToArray());
        }
        private string Operator()
        {SkipSpace();foreach(var token in operators)if(Peek(token))return token;return null;}
        private void SkipSpace(){while(!End&&char.IsWhiteSpace(text[pos]))pos++;}
        private bool Peek(string token)=>text.AsSpan(pos).StartsWith(token.AsSpan(),StringComparison.Ordinal);
        private bool Match(string token){SkipSpace();if(!Peek(token))return false;pos+=token.Length;return true;}
        private void Expect(string token){if(!Match(token))throw Error("expected '"+token+"'");}
        private FormulaException Error(string message)=>new(text,pos,message);
    }
}
