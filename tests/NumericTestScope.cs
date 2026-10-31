namespace Cthangover.Mods.Expressions.Tests
{
    using System;
    using System.Collections.Generic;
    using Cthangover.Mods.Stats.Formulas;
    internal class NumericTestScope : IFormulaScope
    {
        public Dictionary<string,double> Values {get;}=new();
        public Random Random {get;set;}=new Random(17);
        public bool TryGet(string name,out double value)=>Values.TryGetValue(name,out value);
    }
}
