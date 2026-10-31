namespace Cthangover.Mods.Stats.Formulas
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Cthangover.Mods.Expressions;
    internal class FormulaFunctionArities : IReadOnlyDictionary<string,(int Min,int Max)>
    {
        public (int Min,int Max) this[string name]=>ExpressionStrategies.TryFunction(name,out var fn)?(fn.MinArguments,fn.MaxArguments):throw new KeyNotFoundException(name);
        public IEnumerable<string> Keys=>ExpressionStrategies.Functions.Select(f=>f.Name);
        public IEnumerable<(int Min,int Max)> Values=>ExpressionStrategies.Functions.Select(f=>(f.MinArguments,f.MaxArguments));
        public int Count=>ExpressionStrategies.Functions.Count;
        public bool ContainsKey(string name)=>ExpressionStrategies.TryFunction(name,out _);
        public bool TryGetValue(string name,out (int Min,int Max) value)
        {if(ExpressionStrategies.TryFunction(name,out var fn)){value=(fn.MinArguments,fn.MaxArguments);return true;}value=default;return false;}
        public IEnumerator<KeyValuePair<string,(int Min,int Max)>> GetEnumerator()=>ExpressionStrategies.Functions.Select(f=>new KeyValuePair<string,(int Min,int Max)>(f.Name,(f.MinArguments,f.MaxArguments))).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()=>GetEnumerator();
    }
}
