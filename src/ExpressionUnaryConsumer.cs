namespace Cthangover.Mods.Expressions
{
    using Cthangover.Core.Mods.Consumers;
    public class ExpressionUnaryConsumer : ModTypeConsumer<IExpressionUnaryStrategy>
    {
        public override string Id => "expressions_base.unary";
        protected override void Add(IExpressionUnaryStrategy instance,string modId)=>ExpressionStrategies.Register(instance);
        protected override void Remove(IExpressionUnaryStrategy instance,string modId)=>ExpressionStrategies.Unregister(instance);
    }
}
