namespace Cthangover.Mods.Expressions
{
    using Cthangover.Core.Mods.Consumers;
    /// <summary>Discover function strategies through the existing mod load/unload pipeline.</summary>
    public class ExpressionFunctionConsumer : ModTypeConsumer<IExpressionFunctionStrategy>
    {
        public override string Id => "expressions_base.functions";
        protected override void Add(IExpressionFunctionStrategy instance,string modId)=>ExpressionStrategies.Register(instance);
        protected override void Remove(IExpressionFunctionStrategy instance,string modId)=>ExpressionStrategies.Unregister(instance);
    }
}
