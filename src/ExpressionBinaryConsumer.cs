namespace Cthangover.Mods.Expressions
{
    using Cthangover.Core.Mods.Consumers;
    public class ExpressionBinaryConsumer : ModTypeConsumer<IExpressionBinaryStrategy>
    {
        public override string Id => "expressions_base.binary";
        protected override void Add(IExpressionBinaryStrategy instance,string modId)=>ExpressionStrategies.Register(instance);
        protected override void Remove(IExpressionBinaryStrategy instance,string modId)=>ExpressionStrategies.Unregister(instance);
    }
}
