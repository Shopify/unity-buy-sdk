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
        void DrawProductPicker(SerializedObject serializedObject, Dictionary<string, string> productNameToGIDMap);
        void DrawProductLoadingError(SerializedObject serializedObject);
        void DrawShopHasNoProductsState(SerializedObject serializedObject);
    }

    public interface IProductPickerController {
        void OnShouldRefreshProductList();
    }

    public class ProductPicker : IProductPickerController {
        public delegate void ProductListUpdatedHandler();
        public event ProductListUpdatedHandler OnProductListUpdated;

        public ShopifyClient Client;
        private IProductPickerView _view;
        private static Dictionary<string, string> _cachedProductNameToGIDMap;
        private Dictionary<string, string> _productNameToGIDMap;
        private ShopifyError _error;

        public bool ProductsFinishedLoading() {
            return _productNameToGIDMap != null || _error != null;
        }

        public ProductPicker(ShopifyClient client, IProductPickerView view = null) {
            _view = view ?? new ProductPickerView(this);
            Client = client;
            LoadProducts();
        }

        public void DrawInspectorGUI(SerializedObject serializedObject) {
            if (_error != null) {
                _view.DrawProductLoadingError(serializedObject);
                return;
            }

            if (!ProductsFinishedLoading()) {
                _view.DrawProductLoadingState(serializedObject);
                return;
            }

            if (_productNameToGIDMap.Count == 0) {
                _view.DrawShopHasNoProductsState(serializedObject);
                return;
            }

            _view.DrawProductPicker(serializedObject, _productNameToGIDMap);
        }

        public void LoadProducts() {
            if (_cachedProductNameToGIDMap != null) {
                _productNameToGIDMap = _cachedProductNameToGIDMap;
                if (OnProductListUpdated != null) OnProductListUpdated();
            }

            LoadAllProducts();
        }

        public void OnShouldRefreshProductList() {
            RefreshProducts();
        }

        private void LoadAllProducts(string lastPageAfter = null, List<Product> allProducts = null) {
            allProducts = allProducts ?? new List<Product>();

            Client.products((products, error, after) => {
                if (error != null) {
                    OnFailedToLoadProducts(error);
                    return;
                }

                if (after == null) {
                    OnLoadedProducts(products);
                    return;
                }

                allProducts.AddRange(products);
                LoadAllProducts(after, allProducts);
            }, after: lastPageAfter);
        }

        private void RefreshProducts() {
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
            _productNameToGIDMap = new Dictionary<string, string>();

            foreach(var product in products) {
                _productNameToGIDMap[product.title()] = product.id();
            }

            _cachedProductNameToGIDMap = _productNameToGIDMap;

            if (OnProductListUpdated != null) OnProductListUpdated();
        }
    }

    public class ProductPickerView : IProductPickerView {
        private IProductPickerController _controller;

        public ProductPickerView(IProductPickerController controller) {
            _controller = controller;
        }

        public void DrawProductLoadingState(SerializedObject serializedObject) {
            EditorGUILayout.LabelField("Loading Products...");
        }

        public void DrawProductLoadingError(SerializedObject serializedObject) {
            EditorGUILayout.HelpBox("Could not load products from the connected shop.", MessageType.Error);

            if (GUILayout.Button("Reload Products")) {
                _controller.OnShouldRefreshProductList();
            }
        }

        public void DrawShopHasNoProductsState(SerializedObject serializedObject) {
            EditorGUILayout.HelpBox("The connected shop has no products published for the Shopify SDK.", MessageType.Error);

            if (GUILayout.Button("Reload Products")) {
                _controller.OnShouldRefreshProductList();
            }
        }

        public void DrawProductPicker(SerializedObject serializedObject, Dictionary<string, string> productNameToGIDMap) {
            GUILayout.Label("Product");
            EditorGUILayout.BeginHorizontal();

            var selectedProduct = productNameToGIDMap.FirstOrDefault((x) => {
                    return serializedObject.FindProperty("ProductGID").stringValue == x.Value;
            });

            var productNameToGIDList = productNameToGIDMap.ToList();

            var selectedIndex = productNameToGIDList.IndexOf(selectedProduct);
            EditorGUI.BeginChangeCheck();

            var newIndex = EditorGUILayout.Popup(
                selectedIndex, 
                productNameToGIDList.Select((x) => x.Key).ToArray()
            );

            if (EditorGUI.EndChangeCheck()) {
                serializedObject.FindProperty("ProductGID").stringValue = productNameToGIDList[newIndex].Value;
            }

            if (GUILayout.Button("Refresh", EditorStyles.miniButton, GUILayout.ExpandWidth(false))) {
                _controller.OnShouldRefreshProductList();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
