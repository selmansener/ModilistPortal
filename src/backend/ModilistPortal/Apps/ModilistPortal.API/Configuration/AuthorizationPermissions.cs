namespace ModilistPortal.API.Configuration
{
    internal struct AuthorizationPermissions
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        public const string Accounts = "Accounts";
        public const string StylePreferences = "StylePreferences";
        public const string Addresses = "Addresses";
        // TODO: Rename as Payments
        public const string PaymentMethods = "PaymentMethods";
        public const string Subscriptions = "Subscriptions";
        public const string Products = "Products";
        public const string SalesOrders = "SalesOrders";
        public const string Returns = "Returns";
        public const string Discounts = "Discounts";
        public const string Development = "Development";
    }
}
