namespace Cthangover.Mods.Expressions.Tests
{
    using System;
    internal class BoundaryRandom : Random
    {
        public bool Upper {get;set;}
        public int Calls {get;private set;}
        public override long NextInt64(long minValue,long maxValue){Calls++;return Upper?maxValue-1:minValue;}
    }
}
