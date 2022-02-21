using System.Collections.Generic;

namespace Web_shop.Utility
{
    public static class WC
    {
        public const string ImagePath = @"img/products/";

        public const string SessionCart = "ShoppingCartSession";

        public const string SessionInquiryId = "InquirySession";

        public const string AdminRole = "Admin";
        public const string CustomerRole = "Customer";

        public const string AdminEmail = "199stepashka199@gmail.com";

        public const string SuccessNotification = "SuccessNotification";
        public const string ErrorNotification = "ErrorNotification";

        public const string CategoryName = "Category";
        public const string ApplicationTypeName = "ApplicationType";

        public const string Success = "Success";
        public const string Error = "Error";

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        public static readonly IEnumerable<string> ListStatus = new string[]
        {
            StatusApproved,
            StatusCancelled,
            StatusInProcess,
            StatusPending,
            StatusRefunded,
            StatusShipped
        };
    }
}
