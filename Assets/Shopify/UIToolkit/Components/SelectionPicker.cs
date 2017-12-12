namespace Shopify.UIToolkit {
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using UnityEngine.Events;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for items that are used in the selection picker.
    /// </summary>
    public interface ISelectionPickerItem {
        string Title { get; }
    }

    /// <summary>
    /// A custom selection picker implementation.
    /// </summary>
    public class SelectionPicker : MonoBehaviour {
        [HideInInspector] 
        public ISelectionPickerItem[] Items;

        [HideInInspector]
        public string Title;

        [HideInInspector]
        public UnityEvent onSelectOption = new UnityEvent();

        // Game object references to the prefab'ed UI.
        public GameObject TitleLabel;
        public GameObject ScrollViewContent;
        public GameObject SelectedOptionPrefab;
        public GameObject UnselectedOptionPrefab;

        public ISelectionPickerItem SelectedItem {
            get {
                return Items[SelectedIndex];
            }
        }

        private int SelectedIndex = 0;

        /// <summary>
        /// Constructs a picker from all of the items.
        /// </summary>
        public void Build() {
            TitleLabel.GetComponent<Text>().text = Title;

            // Configure the dropdown to contain options.
            for (var i = 0; i < Items.Length; i++) {
                var item = Items[i];
                AddItem(item, i == SelectedIndex);
            } 

            // Resize the scroll view content view to fit the options snug.
            var contentRectTransform = ScrollViewContent.GetComponent<RectTransform>();
            contentRectTransform.sizeDelta 
                = new Vector2(contentRectTransform.rect.width, CalculateHeightOfContentChildren());
        }

        public void SelectItem() {
            var newSelection = EventSystem.current.currentSelectedGameObject;
            var previousSelection = ScrollViewContent.transform.GetChild(SelectedIndex);

            // Deselect previous selection and new selection containers.
            foreach (Transform child in previousSelection.transform) {
                GameObject.Destroy(child.gameObject);
            }

            foreach (Transform child in newSelection.transform) {
                GameObject.Destroy(child.gameObject);
            }

            InsertSelectionPrefabIntoContainer(UnselectedOptionPrefab, previousSelection.transform.gameObject, SelectedItem);
            SelectedIndex = newSelection.transform.GetSiblingIndex();
            InsertSelectionPrefabIntoContainer(SelectedOptionPrefab, newSelection.transform.gameObject, SelectedItem);

            onSelectOption.Invoke();
        }

        private void AddItem(ISelectionPickerItem item, bool isSelected) {
            var container = new GameObject();
            container.AddComponent<Button>().onClick.AddListener(SelectItem);
            container.name = "Item";
            container.SetActive(true);
            container.transform.SetParent(ScrollViewContent.transform, false);

            var prefab = isSelected ? SelectedOptionPrefab : UnselectedOptionPrefab;
            InsertSelectionPrefabIntoContainer(prefab, container, item);
        }

        private void InsertSelectionPrefabIntoContainer(GameObject prefab, GameObject container, ISelectionPickerItem item) {
            var optionObj = Instantiate(prefab, Vector3.zero, Quaternion.identity);

            optionObj.SetActive(true);
            optionObj.transform.SetParent(container.transform, false);

            var childText = optionObj.transform.GetChild(0).gameObject.GetComponent<Text>();
            childText.text = item.Title;

            var optionObjRectTransform = optionObj.GetComponent<RectTransform>();
            var containerRectTransform = container.GetComponent<RectTransform>();
            if (containerRectTransform == null) {
                containerRectTransform = container.AddComponent<RectTransform>();
            }
            containerRectTransform.sizeDelta = 
                new Vector2(optionObjRectTransform.rect.width, optionObjRectTransform.rect.height);
        }

        private float CalculateHeightOfContentChildren() {
            float contentHeight = 0;
            foreach (Transform child in ScrollViewContent.transform) {
                contentHeight += child.GetComponent<RectTransform>().rect.height;
            }
            return contentHeight;
        }
    }
}

