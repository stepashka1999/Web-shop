using Microsoft.Extensions.Options;
using Braintree;

namespace Web_shop.Utility.BrainTree
{
    public class BrainTreeGate : IBrainTreeGate
    {
        private readonly BrainTreeSettings _options;
        private IBraintreeGateway _brainTreeGateWay;

        public IBraintreeGateway BraintreeGateway
        {
            get
            {
                if (_brainTreeGateWay == null)
                {
                    _brainTreeGateWay = CreateGateway();
                }
                
                return _brainTreeGateWay;
            }
        }

        public BrainTreeGate(IOptions<BrainTreeSettings> options)
        {
            _options = options.Value;
        }

        public IBraintreeGateway CreateGateway()
        {
            return new BraintreeGateway(_options.Environment, _options.MerchantId, _options.PublicKey, _options.PrivateKey);
        }
    }
}
