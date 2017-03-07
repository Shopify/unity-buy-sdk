namespace Shopify.Unity {
    using System.Collections.Generic;
    using System.Text;
    using Shopify.Unity.SDK;
    using Shopify.Unity.GraphQL;

    public delegate void PurchaseSessionSyncHandler(string purchaseSessionID, List<string> errors, string httpError);

    public class PurchaseSessionLineItems {
        private List<LineItemInput> LineItems = new List<LineItemInput>();

        public void Set(string variantId, long? quantity = null, List<AttributeInput> customAttributes = null) {
            LineItemInput input = Get(variantId);

            if (input != null) {
                if (quantity != null) {
                    input.quantity = (long) quantity;
                }

                if (customAttributes != null) {
                    input.customAttributes = customAttributes;
                }
            } else {
                if (quantity == null) {
                    quantity = 1;
                }

                LineItems.Add(
                    new LineItemInput(
                        variantId: variantId,
                        quantity: (long) quantity,
                        customAttributes: customAttributes
                    )
                );
            }
        }

        public void Set(ProductVariant variant, long? quantity = null, List<AttributeInput> customAttributes = null) {
            Set(variant.id(), quantity, customAttributes);
        }

        public List<LineItemInput> GetAll() {
            return LineItems;
        }

        public LineItemInput Get(string variantId) {
            return LineItems.Find(item => item.variantId == variantId);
        }

        public LineItemInput Get(ProductVariant variant) {
            return Get(variant.id());
        }

        public bool Delete(string variantId) {
            int idxToDelete = LineItems.FindIndex(lineItem => lineItem.variantId == variantId);

            if (idxToDelete == -1) {
                return false;
            } else {
                LineItems.RemoveAt(idxToDelete);

                return true;
            }
        }

        public bool Delete(ProductVariant variant) {
            return Delete(variant.id());
        }
    }

    public class PurchaseSession {
        public PurchaseSessionLineItems LineItems {
            get {
                return _LineItems;
            }
        }

        private PurchaseSessionLineItems _LineItems;

        private ShopifyClient Client;

        public PurchaseSession(ShopifyClient client) {
            Client = client;
            _LineItems = new PurchaseSessionLineItems();
        }

        public string WebCheckoutLink(string note = null) {
            StringBuilder url = new StringBuilder();
            bool hasLineItem = false;

            url.Append("http://");
            url.Append(Client.Domain);
            url.Append("/cart/");

            foreach(LineItemInput lineItem in LineItems.GetAll()) {
                if (hasLineItem) {
                    url.Append(",");
                }

                hasLineItem = true;

                string[] variandIdSplit = lineItem.variantId.Split('/');

                // variant id's are in this form:
                // gid://shopify/ProductVariant/123123
                url.Append(variandIdSplit[variandIdSplit.Length - 1]);
                url.Append(":");
                url.Append(lineItem.quantity);
            }

            if (note != null) {
                url.Append("?");
                url.Append("note=");
                url.Append(note);
            }

            return url.ToString();
        }
    }
    }
