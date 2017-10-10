# Quick Start

The goal of this quick start guide is to help you get an in-game shop setup as quickly as possible. This can be done easily by using the drop in "Shop Popup" prefab that's included with the examples for the Unity SDK. 

This tutorial is aimed at any developer looking to quickly integrate the sale of physical merchandise into their game. Since this tutorial uses the pre-built shop popup, only basic Unity skills are required in order to get setup.

_For a more detailed breakdown of how the shop popup was created, and steps on creating a similar shop popup of your own, please see the [full tutorial](https://github.com/Shopify/unity-buy-sdk-examples/blob/master/TUTORIAL.md)._


#### Importing The Required Files

We'll start with a blank, default Unity project, as seen above.

The first step is to import the Unity SDK's unitypackage into your project. If you haven't downloaded it yet, you can find it [here](https://github.com/Shopify/unity-buy-sdk/raw/master/shopify-buy.unitypackage).

_If you can't find the relevant files in the `shopify/examples` folder, re-import the unitypackage and make sure the example folder and its sub-folders are checked in the Import Unity Package window when importing_

<img width="1920" alt="screen shot 2017-08-31 at 11 23 18 am" src="https://user-images.githubusercontent.com/1041278/29931639-ca6ba6bc-8e3f-11e7-9585-bfc3b4d171fb.png">



#### Add An Instance Of The Shop Popup Prefab

Next, go into the `Shopify/Examples/Prefabs` folder, and drag an instance of the Shop Popup prefab into the scene.

<img width="1340" alt="shop popup location" src="https://user-images.githubusercontent.com/2158003/31234042-e697fd26-a9bb-11e7-9545-ef0004df4941.png">

<img width="1338" alt="drag shop popup" src="https://user-images.githubusercontent.com/2158003/31234119-13cf1a0e-a9bc-11e7-9546-dbfa6869534c.png">

#### Add the UI eventsystem

In order to be able to interact with the popup, or any other Unity UI component, we need to have an event system present in the scene. The Shop Popup will tell you in it's inspector if it cannot find an event system. You can create an event system by either clicking the provided button in the Shop Popup inspector, or by picking the `GameObject/UI/Event System` option from Unity's built in menus.

_The event system does not need to be configured in any special ways to work with the Shop Popup. The Shop Popup, like any unity UI component, simply needs an event system to process user input._

<img width="326" alt="shop popup no event system" src="https://user-images.githubusercontent.com/2158003/31234645-72dc8c7e-a9bd-11e7-8519-f8571524e508.png">



#### Adding UI To Show The Shop Popup

Now that the Shop Popup is in your scene, we need a way to tell it to show up. We do this by calling the function `ShowPopup()` on the `ShopPopup` component. You can do this from a script or from an event in your game. In this tutorial, we will build a small UI to show the popup.

Add a new Canvas with a Button to your scene, you can do this by first picking `GameObject/UI/Canvas`, followed by `GameObject/UI/Button` from the Unity menus.

<img width="737" alt="menus" src="https://user-images.githubusercontent.com/2158003/31235072-7d2a241a-a9be-11e7-9581-615f45795f10.png">

Once we have a button in the scene, we can change the text of the button to something like "Shop Now" and configure it to open the Shop Popup. The easiest way to tell the button to open the Shop Popup is to use the `OnClick` Event section in the button's inspector to call the `ShopPopup.ShowPopup()` method on the Popup GameObject.

<img width="324" alt="add callback" src="https://user-images.githubusercontent.com/2158003/31235996-d730ff22-a9c0-11e7-94bd-0aaff657ec59.png">
<img width="369" alt="drag shop popup into callback slot" src="https://user-images.githubusercontent.com/2158003/31236000-da3850e4-a9c0-11e7-9390-068c9b48a941.png">
<img width="471" alt="select method to call" src="https://user-images.githubusercontent.com/2158003/31236003-dbed6442-a9c0-11e7-8f6c-66ada2bfe76d.png">



#### Testing The Shop Popup

At this point in the tutorial, we can make sure that the Popup is working properly. The Popup is Pre-Configured to work with a demo shop that we created to showcase the SDK. If you hit play and click the button you just created in the last step, you should see the Shop Popup appear and display the items from the Demo Shop.

_If the popup does not appear, make sure that the button you are clicking is configured to call the ShowPopup method on the `ShopPopup` GameObject in your scene. Also make sure that your button is not accidentally placed under the Shop Popup's Canvas by accident. Any UI you add should belong to a canvas other than the canvas that the Shop Popup prefab contains. If you skip adding your own canvas in the previous steps, Unity will automatically add the button to the first canvas it finds, which in this case is the Shop Popup's canvas._

<img width="930" alt="popup" src="https://user-images.githubusercontent.com/2158003/31236317-c5881ae8-a9c1-11e7-8ab4-981d271b7559.png">



#### Setting Up The Shop Popup Prefab To Work With Your Store

Now that you've verified that the Shop Popup is working, you can configure it to sell items from your shopify store. We can do this by entering a Storefront Access Token and a Shop Domain into the Shop Popup's Inspector. 

Your Shop Domain is the URL you would use to visit your Shop, without `http://` or `https://` in front of it.

The Storefront Access Token allows customers to access your storefront. For more information on getting the Access Token, see [Obtaining a storefront Access Token](https://help.shopify.com/api/storefront-api/getting-started#obtaining-a-storefront-access-token).

<img width="370" alt="access token" src="https://user-images.githubusercontent.com/2158003/31236918-79762288-a9c3-11e7-8b97-4f01a8756bca.png">
