using Braintree;

namespace Web_shop.Utility.BrainTree
{
    public interface IBrainTreeGate
    {
        IBraintreeGateway CreateGateway();

        IBraintreeGateway BraintreeGateway { get; }
    }
}
