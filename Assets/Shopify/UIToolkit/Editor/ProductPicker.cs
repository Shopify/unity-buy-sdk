namespace Shopify.UIToolkit.Editor {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using System;
    using System.Linq;

    public interface IProductPickerView {
        void DrawProductLoadingState(SerializedObject serializedObject);
        void DrawProductPicker(SerializedObject serializedObject);
        void DrawProductLoadingError(SerializedObject serializedObject);
        void DrawShopHasNoProductsState(SerializedObject serializedObject);
    }

    public class ProductPicker : IProductPickerView {
        private class ProductNameToGIDMapEntry {
            public string ProductName;
            public string ProductGID;
        }

        public IProductPickerView View;
        private static List<ProductNameToGIDMapEntry> _cachedProductNameToGIDMap;
        private List<ProductNameToGIDMapEntry> _productNameToGIDMap;
        private ShopifyClient _client;
        private ShopifyError _error;

        public bool ProductsFinishedLoading() {
            return _productNameToGIDMap != null || _error != null;
        }

        public ProductPicker(ShopifyClient client) {
            _client = client;
            View = this;
            LoadProducts();
        }

        public void DrawInspectorGUI(SerializedObject serializedObject) {
            if (_error != null) {
                View.DrawProductLoadingError(serializedObject);
                return;
            }

            if (!ProductsFinishedLoading()) {
                View.DrawProductLoadingState(serializedObject);
                return;
            }

            if (_productNameToGIDMap.Count == 0) {
                View.DrawShopHasNoProductsState(serializedObject);
                return;
            }

            View.DrawProductPicker(serializedObject);
        }

        public void LoadProducts() {
            if (_cachedProductNameToGIDMap != null) {
                _productNameToGIDMap = _cachedProductNameToGIDMap;
            }

            _client.products((products, error) => {
                if (error != null) {
                    OnFailedToLoadProducts(error);
                    return;
                }

                OnLoadedProducts(products);
            });
        }

        public void RefreshProducts() {
            _error = null;
            _productNameToGIDMap = null;
            _cachedProductNameToGIDMap = null;
            LoadProducts();
        }

        private void OnFailedToLoadProducts(ShopifyError error) {
            _error = error;
            _cachedProductNameToGIDMap = null;
        }

        private void OnLoadedProducts(List<Product> products) {
            _productNameToGIDMap = new List<ProductNameToGIDMapEntry>();

            foreach(var product in products) {
                _productNameToGIDMap.Add(
                    new ProductNameToGIDMapEntry() {
                        ProductName = product.title(),
                        ProductGID = product.title(),
                    });
            }

            _cachedProductNameToGIDMap = _productNameToGIDMap;
        }

        #region IProductPickerView

        void IProductPickerView.DrawProductLoadingState(SerializedObject serializedObject) {
            EditorGUILayout.LabelField("Loading Products...");
        }

        void IProductPickerView.DrawProductLoadingError(SerializedObject serializedObject) {
            EditorGUILayout.HelpBox("Could not load products from the connected shop.", MessageType.Error);

            if (GUILayout.Button("Reload Products")) {
                RefreshProducts();
            }
        }

        void IProductPickerView.DrawShopHasNoProductsState(SerializedObject serializedObject) {
            EditorGUILayout.HelpBox("The connected shop has no products published for the Shopify SDK.", MessageType.Error);

            if (GUILayout.Button("Reload Products")) {
                RefreshProducts();
            }
        }

        void IProductPickerView.DrawProductPicker(SerializedObject serializedObject) {
            GUILayout.Label("Product");
            EditorGUILayout.BeginHorizontal();

            var selectedProduct = _productNameToGIDMap.FirstOrDefault(
                (x) => {
                    return serializedObject.FindProperty("ProductGID").stringValue == x.ProductGID;
                }
            );

            var selectedIndex = _productNameToGIDMap.IndexOf(selectedProduct);


            EditorGUI.BeginChangeCheck();

            var newIndex = EditorGUILayout.Popup(
                selectedIndex, 
                _productNameToGIDMap.Select((x) => x.ProductName).ToArray()
            );

            if (EditorGUI.EndChangeCheck()) {
                serializedObject.FindProperty("ProductGID").stringValue = _productNameToGIDMap[newIndex].ProductGID;
            }

            if (GUILayout.Button("Refresh", EditorStyles.miniButton, GUILayout.ExpandWidth(false))) {
                RefreshProducts();
            }

            EditorGUILayout.EndHorizontal();
        }

        #endregion
    }
}
