namespace Shopify.UIToolkit {
    using System;
    using System.Collections.Generic;
    using Shopify.Unity;

    public class ProductCache {
        private string _cursor = null;
        private bool _complete = false;

        private List<Product> _products;

        public ProductCache() {
            _products = new List<Product> ();
        }

        public string Cursor {
          get {
            return _cursor;
          }
        }

        public bool Complete {
          get {
            return _complete;
          }
        }

        public int Count {
          get {
            return _products.Count;
          }
        }

        public void Add(Product[] products, string after) {
            if (string.IsNullOrEmpty(after)) {
                _complete = true;
            }

            _cursor = after;

            foreach (var product in products) {
                _products.Add(product);
            }
        }
    }
}
