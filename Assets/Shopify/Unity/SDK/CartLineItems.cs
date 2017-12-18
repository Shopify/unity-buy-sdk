namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System;

    public class CartLineItem {
        public static explicit operator CheckoutLineItemInput(CartLineItem lineItem) {
            return lineItem.GetCheckoutLineItemInput();
        }

        public static explicit operator CheckoutLineItemUpdateInput(CartLineItem lineItem) {
            return lineItem.GetCheckoutLineItemUpdateInput();
        }

        public string VariantId {
            get {
                return _VariantId;
            }
        }

        public decimal Price {
            get {
                return _Price;
            }
        }

        public long Quantity {
            get {
                return _Quantity;
            }

            set {
                if (_Quantity != value) {
                    _Quantity = value;
                    OnChange();
                }
            }
        }

        public IDictionary<string, string> CustomAttributes {
            get {
                return _CustomAttributes;
            }

            set {
                if (value != null) {
                    _CustomAttributes = new ObservableDictionary<string, string>(value, () => { OnChange(); });
                }

                OnChange();
            }
        }

        public string ID = null;
        private string _VariantId;
        private long _Quantity;
        private decimal _Price;
        private ObservableDictionary<string, string> _CustomAttributes;
        private OnCartLineItemChange OnChange;

        public CartLineItem(ProductVariant variant, OnCartLineItemChange onChange, long quantity = 1, IDictionary<string, string> customAttributes = null) :
            this(variant.id(), variant.price(), onChange, quantity, customAttributes) { }

        public CartLineItem(string variantId, decimal variantPrice, OnCartLineItemChange onChange, long quantity = 1, IDictionary<string, string> customAttributes = null) {
            _VariantId = variantId;
            _Quantity = quantity;
            _Price = variantPrice;
            OnChange = onChange;

            if (customAttributes != null) {
                CustomAttributes = new ObservableDictionary<string, string>(customAttributes, () => { OnChange(); });
            }
        }

        public CheckoutLineItemInput GetCheckoutLineItemInput() {
            return new CheckoutLineItemInput(
                quantity: Quantity,
                variantId: VariantId,
                customAttributes: GetAttributeInputs()
            );
        }

        public CheckoutLineItemUpdateInput GetCheckoutLineItemUpdateInput() {
            return new CheckoutLineItemUpdateInput(
                id: ID,
                quantity: Quantity,
                variantId: VariantId,
                customAttributes: GetAttributeInputs()
            );
        }

        private List<AttributeInput> GetAttributeInputs() {
            List<AttributeInput> attributes = null;

            if (CustomAttributes != null) {
                attributes = new List<AttributeInput>();

                foreach (string key in CustomAttributes.Keys) {
                    attributes.Add(new AttributeInput(
                        key: key,
                        value: CustomAttributes[key]
                    ));
                }
            }

            return attributes;
        }
    }

    /// <summary>
    /// Is used to add, update, or delete line items in a <see ref="Cart">Cart </see>.
    /// </summary>
    public class CartLineItems {
        public static List<CheckoutLineItemInput> ConvertToCheckoutLineItemInput(List<CartLineItem> lineItems) {
            var converter = new Converter<CartLineItem, CheckoutLineItemInput>((lineItem) => {
                    return lineItem.GetCheckoutLineItemInput();
                });

            return lineItems.ConvertAll(converter);
        }

        public static List<string> ConvertToLineItemIds(List<CartLineItem> lineItems) {
            var converter = new Converter<CartLineItem, string>((lineItem) => {
                    return lineItem.ID;
                });

            List<string> lineItemIds = lineItems.ConvertAll(converter);

            lineItemIds.RemoveAll(id => id == null);

            return lineItemIds;
        }

        public bool IsSaved {
            get {
                return _IsSaved;
            }
        }

        public decimal Subtotal {
            get {
                decimal subtotalOut = 0;

                foreach (var lineitem in LineItems) {
                    subtotalOut += lineitem.Price * lineitem.Quantity;
                }

                return subtotalOut;
            }
        }

        private bool _IsSaved = false;
        private List<CartLineItem> LineItems = new List<CartLineItem>();
        private Action<string> OnDeleteLineItem;

        public CartLineItems(Action<string> onDeleteLineItem) {
            OnDeleteLineItem = onDeleteLineItem;
        }

        /// <summary>
        /// Adds or updates a line item using a <see ref="ProductVariant">ProductVariant </see>.
        /// </summary>
        /// <param name="variant"><see ref="ProductVariant">ProductVariant </see> whose id will be used to create or update a line item</param>
        /// <param name="quantity">the number of items you'd like to order for variantId</param>
        /// <param name="customAttributes">can be used to define extra information for this line item</param>
        /// \code
        /// // Example that updates the quantity of items to be purchased to 3.
        /// // If no line item exists for `variantId`, then a new line item is created
        /// Cart cart = ShopifyBuy.Client().Cart();
        ///
        /// cart.LineItems.AddOrUpdate(variant, 3);
        /// \endcode
        public void AddOrUpdate(ProductVariant variant, long? quantity = null, Dictionary<string, string> customAttributes = null) {
            AddOrUpdate(variant.id(), variant.price(), quantity, customAttributes);
        }

        public void AddOrUpdate(string variantId, decimal variantPrice, long? quantity = null, Dictionary<string, string> customAttributes = null) {
            CartLineItem input = Get(variantId);

            if (input != null) {
                if (quantity != null) {
                    input.Quantity = (long) quantity;
                }

                if (customAttributes != null) {
                    input.CustomAttributes = customAttributes;
                }
            } else {
                if (quantity == null) {
                    quantity = 1;
                }

                LineItems.Add(
                    new CartLineItem(
                        variantId: variantId,
                        variantPrice: variantPrice,
                        onChange: OnLineItemChange,
                        quantity: (long) quantity,
                        customAttributes : customAttributes
                    )
                );
            }
        }

        /// <summary>
        /// Adds a new line item using a <see ref="Product">Product </see> and selected options. If an existing line item exists for the
        /// variant id, then that line item will be updated.
        /// </summary>
        /// <param name="product">product to check selected options against</param>
        /// <param name="selectedOptions">a Dictionary used to define user selected options</param>
        /// <param name="quantity">the number of items you'd like to order for variantId</param>
        /// <param name="customAttributes">customAttributes can be used to define extra information for this line item</param>
        /// <exception ref="NoMatchingVariantException">Throws when no matching variant could be found for selected options in product</exception>
        /// \code
        /// // Example that updates the quantity of items to be purchased to 3.
        /// // If no line item exists for `variantId`, then a new line item is created
        /// Cart cart = ShopifyBuy.Client().Cart();
        ///
        /// Dictionary<string, string> selectedOptions = new Dictionary<string, string>() {
        ///     {"Size", "Small"},
        ///     {"Color", "Red"}
        /// };
        ///
        /// cart.LineItems.AddOrUpdate(product, selectedOptions, 3);
        /// \endcode
        public void AddOrUpdate(Product product, Dictionary<string, string> selectedOptions, long? quantity = null, Dictionary<string, string> customAttributes = null) {
            var variant = VariantFromSelectedOptions(product, selectedOptions);

            if (variant == null) {
                throw new NoMatchingVariantException("Could not `AddOrUpdate` line item as no matching variant could be found for selected options");
            }

            AddOrUpdate(variant, quantity, customAttributes);
        }

        /// <summary>
        /// Returns all <see ref="CartLineItem">Line Items </see> that have been created.
        /// </summary>
        /// \code
        /// // Example that checks how many line items the cart contains
        /// Cart cart = ShopifyBuy.Client().Cart();
        ///
        /// Debug.Log("The cart has " + cart.LineItems.All().Count + " line items");
        /// \endcode
        public List<CartLineItem> All() {
            return LineItems;
        }

        /// <summary>
        /// Returns one <see ref="CartLineItem">Line Item </see> based on a variant id. If no line item exists for the variant id
        /// <c>null</c> will be returned.
        /// </summary>
        /// <param name="variantId">variant id used to create a line item</param>
        /// \code
        /// // Example that checks the quantity of a line item based on variantId
        /// Cart cart = ShopifyBuy.Client().Cart();
        ///
        /// Debug.Log(cart.LineItems.Get(variantId).quantity);
        /// \endcode
        public CartLineItem Get(string variantId) {
            return LineItems.Find(item => item.VariantId == variantId);
        }

        /// <summary>
        /// Returns one <see ref="CartLineItem">Line Item </see> based on a <see ref="ProductVariant">ProductVariant </see>. If no line item
        /// exists for the variant, <c>null</c> will be returned.
        /// </summary>
        /// <param name="variant">variant whose variant id used to create a line item</param>
        /// \code
        /// // Example that checks the quantity of a line item based on a variant
        /// Cart cart = ShopifyBuy.Client().Cart();
        ///
        /// Debug.Log(cart.LineItems.Get(variant).quantity);
        /// \endcode
        public CartLineItem Get(ProductVariant variant) {
            return Get(variant.id());
        }

        /// <summary>
        /// Returns one <see ref="CartLineItem">Line Item </see> based on a <see ref="Product">product </see> and selected options.
        /// If no line item exists for the matching variant, <c>null</c> will be returned.
        /// </summary>
        /// <param name="product">product whose options will be selected</param>
        /// <param name="selectedOptions">a Dictionary used to define user selected options</param>
        /// \code
        /// // Example that checks the quantity of a line item based on a product and selected options
        /// Cart cart = ShopifyBuy.Client().Cart();
        ///
        /// Dictionary<string, string> selectedOptions = new Dictionary<string, string>() {
        ///     {"Size", "Small"},
        ///     {"Color", "Red"}
        /// };
        ///
        /// Debug.Log(cart.LineItems.Get(product, selectedOptions).quantity);
        /// \endcode
        public CartLineItem Get(Product product, Dictionary<string, string> selectedOptions) {
            string variantId = VariantFromSelectedOptions(product, selectedOptions).id();

            if (variantId == null) {
                return null;
            }

            return Get(variantId);
        }

        /// <summary>
        /// Deletes one <see ref="CartLineItem">Line Item </see> based on a variant id. If a line item was deleted, <c>true</c>
        /// will be returned. If no line items were deleted, <c>false</c> will be returned.
        /// </summary>
        /// <param name="variantId">variant id used to delete a line item</param>
        /// \code
        /// // Example that deletes a line item based on variantId
        /// Cart cart = ShopifyBuy.Client().Cart();
        ///
        /// Debug.Log("Did delete? " + cart.LineItems.Delete(variantId));
        /// \endcode
        public bool Delete(string variantId) {
            int idxToDelete = LineItems.FindIndex(lineItem => lineItem.VariantId == variantId);

            if (idxToDelete == -1) {
                return false;
            } else {
                CartLineItem lineItemRemoved = LineItems[idxToDelete];
                LineItems.RemoveAt(idxToDelete);

                if (lineItemRemoved.ID != null) {
                    OnDeleteLineItem(lineItemRemoved.ID);
                }

                return true;
            }
        }

        /// <summary>
        /// Deletes one <see ref="CartLineItem">Line Item </see> based on a <see ref="ProductVariant">ProductVariant </see>. If a line
        /// item was deleted, <c>true</c> will be returned. If no line items were deleted, <c>false</c> will be returned.
        /// </summary>
        /// <param name="variant"><see ref="ProductVariant">variant </see> to provide the ID to delete a line item</param>
        /// \code
        /// // Example that deletes a line item based on a product variant
        /// Cart cart = ShopifyBuy.Client().Cart();
        ///
        /// Debug.Log("Did delete? " + cart.LineItems.Delete(variant));
        /// \endcode
        public bool Delete(ProductVariant variant) {
            return Delete(variant.id());
        }

        /// <summary>
        /// Deletes one <see ref="CartLineItem">Line Item </see> based on a <see ref="Product">Product </see> and selected options.
        /// If a line item was deleted, <c>true</c> will be returned. If no line item was deleted, <c>false</c> will be returned.
        /// </summary>
        /// <param name="product"><see ref="Product">product </see> whose options will be used to determine which line item is deleted</param>
        /// <param name="selectedOptions">a Dictionary used to define user selected options</param>
        /// \code
        /// // Example that deletes a line item based on a product and selected options
        /// Cart cart = ShopifyBuy.Client().Cart();
        ///
        /// Dictionary<string, string> selectedOptions = new Dictionary<string, string>() {
        ///     {"Size", "Small"},
        ///     {"Color", "Red"}
        /// };
        ///
        /// Debug.Log("Did delete? " + cart.LineItems.Delete(product, selectedOptions));
        /// \endcode
        public bool Delete(Product product, Dictionary<string, string> selectedOptions) {
            string variantId = VariantFromSelectedOptions(product, selectedOptions).id();

            if (variantId == null) {
                return false;
            }

            return Delete(variantId);
        }

        public void UpdateLineItemsFromCheckoutLineItems(List<CheckoutLineItem> checkoutLineItems) {
            foreach (CheckoutLineItem checkoutLineItem in checkoutLineItems) {
                bool didUpdateID = false;
                var checkoutVariant = checkoutLineItem.variant();
                string checkoutLineItemId = checkoutLineItem.id();

                foreach (CartLineItem cartLineItem in LineItems) {
                    if (cartLineItem.VariantId == checkoutVariant.id()) {
                        cartLineItem.ID = checkoutLineItemId;
                        didUpdateID = true;
                        break;
                    }
                }

                if (!didUpdateID) {
                    long quantity = checkoutLineItem.quantity();
                    Dictionary<string, string> customAttributes = null;

                    try {
                        List<Shopify.Unity.Attribute> attributes = checkoutLineItem.customAttributes();
                        customAttributes = new Dictionary<string, string>();

                        foreach (Shopify.Unity.Attribute attribute in attributes) {
                            customAttributes[attribute.key()] = attribute.value();
                        }
#pragma warning disable 0168
                    } catch (NoQueryException error) { }
#pragma warning restore 0168

                    AddOrUpdate(checkoutVariant, quantity, customAttributes);
                    Get(checkoutVariant.id()).ID = checkoutLineItemId;
                }
            }

            _IsSaved = true;
        }

        private ProductVariant VariantFromSelectedOptions(Product product, Dictionary<string, string> selectedOptions) {
            List<ProductVariant> variants = (List<ProductVariant>) product.variants();
            ProductVariant variantOut = null;

            foreach (ProductVariant variant in variants) {
                List<SelectedOption> variantSelectedOptions = variant.selectedOptions();

                if (variantSelectedOptions.Count == selectedOptions.Keys.Count) {
                    variantOut = variant;

                    foreach (SelectedOption variantOption in variantSelectedOptions) {
                        string optionName = variantOption.name();
                        string optionValue = variantOption.value();

                        if (!selectedOptions.ContainsKey(optionName) || selectedOptions[optionName] != optionValue) {
                            variantOut = null;

                            break;
                        }
                    }
                }

                if (variantOut != null) {
                    break;
                }
            }

            return variantOut;
        }

        private void OnLineItemChange() {
            _IsSaved = false;
        }
    }
}
