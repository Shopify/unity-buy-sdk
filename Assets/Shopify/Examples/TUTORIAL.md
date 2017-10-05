<img width="973" alt="screen shot 2017-08-29 at 11 50 49 am" src="https://user-images.githubusercontent.com/1041278/29841406-52275010-8cd3-11e7-812d-c8ed5a2fd951.png">

### Table of contents

- [Shop popup](#Shop-popup)
	- [Products panel](#products-panel)
		- [Product panel line item template](#product-panel-line-item-template)
		- [Image loading](#image-loading)
	- [Product panel](#products-panel)
		- [Image holder template](#Image-holder-template)
	- [Cart panel](#cart-panel)

### Introduction

#### Purpose of this tutorial

The goal of this tutorial is to demonstrate how to setup an in game shop using the Unity Buy SDK. The focus is placed primarily on the interaction with the Unity Buy SDK and, although there is some information about setting up the corresponding UI elements within Unity, those details are mostly left to the game developer.

#### Who this tutorial is for

This tutorial assumes that the reader has a general understanding of Unity game development. In particular, it is assumed the reader has a reasonable familiarity with the built in Unity UI system.

Let's get started with integrating our Shopify store directly into our Unity game!

### Adding the Unity Buy SDK

In order to use the Unity Buy SDK we have to start by adding the [shopify-buy.unitypackage](https://github.com/Shopify/unity-buy-sdk/blob/master/shopify-buy.unitypackage). Once we have dragged the unitypackage into the Assets area of our project, there should be additional Shopify and Plugins folders, which adds in the Unity Buy SDK scripts.

In the next section, we'll start building the shop popup.

### Shop Popup

The shop popup encapsulates the various panels and scripts that make up our in game store.

Starting from the first panel that the player will see, there is the Products Panel, which shows a list of the products which are available. When the player clicks on a product, they are taken to the Product Panel, which shows a more detailed view of the product. From this view, the player will be able to add the product into their cart. Once the player has added the products they are interested in, they can go to the cart view, which will show a summary of the cart contents, and allow them to checkout. We’ll add a few extra views along the way to handle situations like when they cart is empty, or there is an error message we need to display to the player.

We’ll begin by adding in the basic UI framework for the views in the next section.

#### View panels UI

Let's start by putting down a Unity UI canvas. We'll add another canvas within this one called Animation Canvas. Eventually we'll use this canvas to animate the pop up menu into view, let's come back to the animation later.

Next we want to add in the sky background image.

We'll then add in images for the top navigational area background, and then the image for the main content area background.

Let's add in a few Unity UI Panels for each of the views we will need. Add the following panels:

- Products Panel
- Product Panel
- Cart Panel
- Empty Cart Panel

We'll also need to show the cart status, both empty and with products. For now, add in a Unity UI Button called Empty View Cart Button and one called View Cart Button.

#### Setup the view swapping scripts

On the top level canvas, add in a new C# script called Shop Manager. This script is going to be responsible for handling navigation between the different views.

So that we can tell which view we are on while debugging, let's take a moment and add a title text element to each view. In the Products Panel, add label which says Products. On the Product Panel, we'll need a dynamic field which shows the product title. Call that Product Title Text. On the Cart Panel, add in a Cart Panel Section Title which labels the view Cart.

The Empty Cart Panel will be a special case visually, where we won't have a header, and instead just an action button to jump back to products. Add Main Text, with 'Your cart is empty'.

We'll need variable to hold references to each of the panels. Let's add those now.

```cs
public ProductsPanel ProductsPanel;
public ProductPanel ProductPanel;
public CartPanel CartPanel;
public EmptyCartPanel EmptyCartPanel;

// File: ShopPopup.cs
```

When the Shop Manager starts it initiates the connection to our Shopify store.

```cs
// Begin by initializing ShopifyHelper, to make connection to shop
ShopifyHelper.Init (AccessToken, ShopDomain);
// With initialization complete, fetch products
ShopifyHelper.FetchProducts(
	delegate(List<Product> products)
	{
		foreach (var product in products)
		{
			// For each of the products received, add them to the products panel
			ProductsPanel.AddProduct(product);
		}
	},
	delegate
	{
		RenderError("Could not find products.");
	}
);

File: ShopPopup.cs
```

The ShopManager is also responsible routing events from the various panels in order to react to the players inputs.

```cs
// Handle transition to specific product
ProductsPanel.OnShowProduct.AddListener (product =>
{
	ShowPanel(ProductPanel.gameObject);
	ProductPanel.SetCurrentProduct (product);
});
// Handle event from click of the close pop up button
ProductsPanel.OnClosePanel.AddListener (ClosePanel);

// Handle transition back to products list from product view
ProductPanel.OnReturnToProducts.AddListener (() => ShowPanel(ProductsPanel.gameObject));
// Handle transition to the cart view
ProductPanel.OnViewCart.AddListener (() => ShowPanel(CartPanel.gameObject));
// Handles click of the add to cart button, which transitions to the cart view
ProductPanel.OnAddProductToCart.AddListener (CartPanel.AddToCart);

// Handle transition back to products list from cart view
CartPanel.OnReturnToProducts.AddListener (() => ShowPanel(ProductsPanel.gameObject));
// Handles a change in cart quanity, in order to update the cart quantity indicators
CartPanel.OnCartQuantityChanged.AddListener (UpdateCartQuantity);
// Handles a successful checkout, by closing the shop pop up
CartPanel.OnCheckoutSuccess.AddListener(ClosePanel);
// Handles a failed checkout by displaying the error
CartPanel.OnCheckoutFailure.AddListener(RenderError);

// Handle transition back to products list from empty cart view
EmptyCartPanel.OnReturnToProducts.AddListener (() => ShowPanel(ProductsPanel.gameObject));

// Handle transition to cart view, when view cart button is clicked
ViewCartButton.gameObject.GetComponent<Button>().onClick.AddListener (() => ShowPanel(CartPanel.gameObject));
// Handle click on empty view cart button, by transitioning to empty cart view
EmptyViewCartButton.onClick.AddListener (() => ShowPanel(EmptyCartPanel.gameObject));

// Handle click on open popup button by playing animation to show pop up menu
OpenMenuButton.onClick.AddListener (ToggleView);

// File: ShopPopup.cs
```

#### View cart button

The view cart button will be visible across panels, so let's take care of setting it up now. It will need to be rendered in two states, one for when the cart doesn't yet have any items and one for when the cart does have items. When the cart does have items, a badge will appear to show the current number of items in the cart. 

Let's add two buttons to handle each of these states and call them View Cart Panel and Empty View Cart Panel, respectively. 

The Empty View Cart Button is straightforward, it will simply contain an image with the cart icon. 

The View Cart Button needs a few more elements, in order to be able to display the current number of cart items. We will add the cart icon, two badge backgrounds (one for single digits, and one for double digits) and a text element to render the quantity value.

```cs
public void UpdateCartQuantity(int quantity)
{
	Quantity.text = quantity.ToString ();

	SingleDigitBadge.gameObject.SetActive (quantity < 10);
	DoubleDigitBadge.gameObject.SetActive (quantity >= 10);

	gameObject.SetActive(quantity > 0);
}

// File: ViewCartButton.cs
```

We'll add the ViewCartButton script, which will contain a helper which deals with showing the correct badge background.

```cs
// Handles a change in cart quanity, in order to update the cart quantity indicators
CartPanel.OnCartQuantityChanged.AddListener (UpdateCartQuantity);

// File: ShopPopup.cs
```

#### Products Panel

<img width="975" alt="screen shot 2017-08-29 at 11 51 04 am" src="https://user-images.githubusercontent.com/1041278/29841404-5220db2c-8cd3-11e7-8eff-0fb086a6c6af.png">

The Products Panel shows a scrollable list view of the products in our store, with preview information including the title, a snippet of the description, a preview image and the price. In addition to the main scolling view, there are also buttons for closing the popup and to view the cart contents, once some items have been added.

To get started with creating this panel, add in a Unity UI scroll view component. Also add in a button, which will serve as the close button.

For each product which we receive from our shop, we need to instantiate a UI element. This is invoked by 

```cs
foreach (var product in products)
{
	// For each of the products received, add them to the products panel
	ProductsPanel.AddProduct(product);
}

// File: ShopPopup.cs
```

The AddProduct function takes care of this instantiation and adding the new instance to the scroll view content. The product is also passed into the instance so it can handle populating the UI elements with the product details.

```cs
public void AddProduct(Shopify.Unity.Product product) {
	// Create instance of the template
	var instance = Instantiate (ProductsPanelLineItemTemplate);
	// Need to set transform so that scrolling works properly
	instance.transform.SetParent (Content, false);
	// Pass in the product to set the line item attributes
	instance.SetCurrentProduct (product, _lineItems.Count);

	// When the instance is clicked, dispatch up the event to change the view to the product
	instance.OnClick.AddListener (() => OnShowProduct.Invoke (product));

	// Add to our list of line items, as we need to iterate them later to adjust properties
	// such as the separator element visibility
	_lineItems.Add(instance);

	UpdateSeparatorVisibility();
}

// File: ProductsPanel.cs
```

##### Product panel line item template

Let's take a closer look at the product panel line item template. We will duplicate this template game object to serve as the line item for each of our products. 

To setup this template, add in an empty gameobject. To this we will add the following:

- An image element, to display the product image
- Add image element which will go behind this, to display a border around the image
- A text element for the title
- A text element for the price
- A text element for the product description
- Another image element, which will hold the separator line graphic
- A button, which we will use as an invisible hit region, over the entire line item.

Once we've created these elements, we'll attach a script, and add a convenience method that will allow for having these UI element populated when we pass a Product.

Add references to the dynamic UI elements in the attached script

```cs
public Text TitleText;
public Text DescriptionText;
public Image ProductImage;
public Text PriceText;
public Button ClickRegionButton;

// File: ProductPanelLineItem.cs
```

The click region will cover the entire line item area, so that customers can easily click to see details page.

Let's add an event which receivers can register against to know which our line item is clicked.

```cs
public UnityEvent OnClick;

// File: ProductPanelLineItem.cs
```

We'll add a convenience method that take a product and assigns all the properties.

```cs
public void SetCurrentProduct(Shopify.Unity.Product product, int index) {
	gameObject.SetActive (true);

	TitleText.text = product.title();
	DescriptionText.text = StringHelper.Ellipsisize (product.description ());

	List<Shopify.Unity.ProductVariant> variants = (List<Shopify.Unity.ProductVariant>) product.variants();
	PriceText.text = variants [0].price ().ToString("C");

	List<Shopify.Unity.Image> images = (List<Shopify.Unity.Image>) product.images();

	StartCoroutine( ImageHelper.AssignImage(images[0].src(), ProductImage) );
}

// File: ProductPanelLineItem.cs
```

The full code for this element can be found at https://github.com/Shopify/unity-buy-sdk-examples/blob/master/Assets/Scripts/ProductsPanelLineItem.cs

Now that we have the product line item template setup, we need to add in the code to handle making copies of the template to represent the product. The ShopManager takes care of querying the products and passing them into the products view (actually this is kind of weird, we should change this). So we need a function here which takes the product data we have received from the server and handles instantiating the line items.

##### Image loading

In the previous section we used the `ImageHelper.AssignImage` function. In order to fetch images from our online store to use in game, we need to use a coroutine to handle the asynchronous loading. We'll write a helper to take care of this for us.

```cs
public static IEnumerator AssignImage(string url, Image image, Image brokenImage = null)
{
	// If the url isn't in the cache, then we need to make the web request to fetch the image
	if (!TextureCache.ContainsKey(url)) {
		var www = new WWW(url);

		yield return www;

		if (!string.IsNullOrEmpty(www.error))
		{
			Debug.Log(www.error);

			if (brokenImage)
			{
				brokenImage.gameObject.SetActive(true);
			}

			yield break;
		}

		// Once the web request is done, assign the texture into the cache 
		TextureCache[url] = www.texture;
	}

	// As we have already ensured that the url is in the cache, we can now safely pull the texture from the cache
	var texture = TextureCache[url];

	// Turn the texture into a srpite, and assign it to the target image
	image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
	image.preserveAspect = true;

	image.gameObject.SetActive(true);
	if (brokenImage)
	{
		brokenImage.gameObject.SetActive(false);
	}
}

// File: ImageHelper.cs
```

The helper handles the `WWW` call, which fetches the remote image, and then takes care of assigning the texture data into the target image.

#### Product Panel

<img width="976" alt="screen shot 2017-08-29 at 11 51 14 am" src="https://user-images.githubusercontent.com/1041278/29841402-521fb648-8cd3-11e7-92f0-c5157beb4773.png">

Next we want to add a way to show the product details, this will be the function of our Product Panel. The player will be directed to this panel when they click on line items in the Products Panel.

In Shopify, products are modelled as having one or more product variants. These variants allow for having a product called, for example, T-Shirt, which has Small, Medium and Large as variants.

We'll need UI items for the following:

- Product title
- Product description
- Product private
- Dropdown for selecting the variant, when present
- Button to add the product to the cart

When transitioning the view to the Product Panel we will call `SetCurrentProduct` in order to update the panel to show the product information.

```cs
public void SetCurrentProduct(Shopify.Unity.Product product) {
	ProductTitle.text = product.title();

	// Reset the variants dropdown
	VariantsDropdown.ClearOptions ();

	// Parse variant titles into a list of strings and assign to the dropdown as the new options
	var options = new List<string> ();
	var variants = (List<Shopify.Unity.ProductVariant>) product.variants();

	foreach (var variant in variants) {
		options.Add (variant.title());
	}

	VariantsDropdown.AddOptions (options);
	
	// Only need to show the variants dropdown if there are more than one variant to choose from		
	VariantsDropdown.gameObject.SetActive (variants.Count > 1);
	
	// Show the appropriately positioned description text
	ProductTitleDescription.gameObject.SetActive (variants.Count > 1);
	ProductTitleDescNoVariant.gameObject.SetActive (variants.Count <= 1);

	VariantsDropdown.onValueChanged.AddListener (HandleDropdownChange);

	// Assign the first product image to the main product image display
	var images = (List<Shopify.Unity.Image>) product.images();
	StartCoroutine( ImageHelper.AssignImage(images[0].src(), ProductImage));

	RenderVariantImages(images);

	ProductPrice.text = variants [0].price().ToString("C");
	ProductTitleDescription.text = product.description ();
	ProductTitleDescNoVariant.text = product.description ();

	CurrentProduct = product;
	CurrentVariant = variants [0];
}

// File: ProductPanel.cs
```

We call the `RenderVariantImages` method in order to handle creating instances of the ImageHolderTemplate to represent a subset of the variant images.

```cs
private void RenderVariantImages(List<Shopify.Unity.Image> images)
{
	// Clean up the existing thumbnail images
	foreach (var imageHolder in _imageHolders) {
		Destroy (imageHolder);
	}
	_imageHolders.Clear ();
	
	// We only have space for a fixed number of thumbnails, so we render out the first five
	var offset = 0;
	var maxImages = 5;
	if (images.Count < maxImages) {
		maxImages = images.Count;
	}
	foreach (var image in images.GetRange(0,maxImages)) {
		// Generate instance of thumbail template
		var instance = Instantiate (ImageHolderTemplate);
		instance.gameObject.SetActive (true);
		_imageHolders.Add (instance.gameObject);
		var instanceImage = instance.ForegroundButton.GetComponent<Image> ();
		StartCoroutine( 
			ImageHelper.AssignImage(
				image.src(), 
				instanceImage,
				instance.BrokenImageIcon
			)
		);
		instance.transform.SetParent (transform, false);
		// Shift the thumbail over to the right
		instance.transform.position += new Vector3 (offset, 0, 0);
		var instanceButton = instance.ForegroundButton.GetComponent<Button> ();
		instanceButton.onClick.AddListener (() =>
			StartCoroutine( ImageHelper.AssignImage(image.src(), ProductImage))
		);

		offset += 100;
	}
}

// File: ProductPanel.cs
```

When the variant selector is changed we update the interface to reflect the particular variant options, including changing to the correct preview image.

```cs
private void HandleDropdownChange(int option) {
	// Change the current variant to what has been selected 
	var variants = (List<Shopify.Unity.ProductVariant>) CurrentProduct.variants();
	var variant = variants [VariantsDropdown.value];
	CurrentVariant = variant;

	string imageSrc;

	// If the variant has a particular image
	try {
		imageSrc = variant.image ().src ();
	} catch(NullReferenceException) {
		var images = (List<Shopify.Unity.Image>) CurrentProduct.images();
		imageSrc = images.First().src ();
	}

	StartCoroutine( ImageHelper.AssignImage(imageSrc, ProductImage));

	ProductPrice.text = variant.price ().ToString("C");
	ProductTitleDescription.text = CurrentProduct.description ();
	ProductTitleDescNoVariant.text = CurrentProduct.description ();
}

// File: ProductPanel.cs
```

##### Image holder template

We saw in the previous section that for each variant there can be a different image. We'll show the first five of these images. The ImageHolderTemplate serves as the template for these elements.

#### Cart Panel

<img width="976" alt="screen shot 2017-08-29 at 11 51 25 am" src="https://user-images.githubusercontent.com/1041278/29841405-52220a6a-8cd3-11e7-965c-dbd182bf0436.png">

The cart panel shows what is currently in the cart and allows for initiating a checkout.

In order to show the cart content, we use a similar approach to the Product Panel, adding cart panel line items, based off a template to the scroll view.

```cs
public void AddToCart(Product product, ProductVariant variant) {
	if (_cart == null)
	{
		_cart = ShopifyHelper.CreateCart();
	}

	// Handle adding a particular variant to the cart
	// For more information on adding variants to the cart visit
	// https://help.shopify.com/api/sdks/custom-storefront/unity-buy-sdk/getting-started#create-cart-line-items-based-on-selected-options

	var existingLineItem = _cart.LineItems.Get (variant);
	if (existingLineItem == null) {
		_cart.LineItems.AddOrUpdate (variant);

		var instance = Instantiate (CartPanelLineItemTemplate);
		instance.transform.SetParent (Content, false);
		instance.SetCurrentProduct (product, variant, 1);

		instance.OnVariantLineItemQuantityAdjustment.AddListener (HandleVariantLineItemQuantityAdjustment);

		_idCartPanelLineItemMapping.Add (variant.id (), instance);
		_lineItems.Add(instance);
	} else {
		_cart.LineItems.AddOrUpdate (variant, existingLineItem.Quantity + 1);

		var cartPanelLineItem = _idCartPanelLineItemMapping [variant.id ()];
		cartPanelLineItem.Quantity.text = existingLineItem.Quantity.ToString();
	}

	if (!_idVariantMapping.ContainsKey (variant.id ())) {
		_idVariantMapping.Add (variant.id (), variant);
	}

	if (!_idProductMapping.ContainsKey (variant.id ())) {
		_idProductMapping.Add (variant.id (), product);
	}

	DispatchCartQuantityChanged ();
	UpdateSeparatorVisibility();
}

// File: CartPanel.cs
```

Conveniently, we can use the `CheckoutWithWebView` function which will launch the appropriate web based checkout on each platform. For example, on iOS a Safari web view will launch within the application, whereas on desktop the default external browser will be used.

```cs
CheckoutButton.onClick.AddListener (() =>
{
	_cart.CheckoutWithWebView(
		() => {
			OnCheckoutSuccess.Invoke();
			EmptyCart();
		},
		() => {
			OnCheckoutCancelled.Invoke();
		},
		(checkoutError) => {
			OnCheckoutFailure.Invoke(checkoutError.Description);
		}
	);
});

// File: CartPanel.cs
```

#### Empty Cart Panel

<img width="976" alt="screen shot 2017-08-29 at 11 51 38 am" src="https://user-images.githubusercontent.com/1041278/29841403-5220b002-8cd3-11e7-83f6-43050ca8b4ef.png">

The empty cart panel provides some helper text and button to redirect the customer back to the main products view if their cart becomes empty. This can happen if the customer removes all the products from their current on the cart panel view.

```cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EmptyCartPanel : MonoBehaviour {

	public Button BackToProductsButton;
	public Button ContinueButton;

	public UnityEvent OnReturnToProducts;

	private void Start () {
		gameObject.SetActive (false);

		BackToProductsButton.onClick.AddListener (() => OnReturnToProducts.Invoke ());
		ContinueButton.onClick.AddListener (() => OnReturnToProducts.Invoke ());
	}
}

// File: EmptyCartPanel.cs
```

### Error notification

- Error Notification
