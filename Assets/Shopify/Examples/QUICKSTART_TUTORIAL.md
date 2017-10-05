### Introduction

#### Purpose of this tutorial

The goal of this tutorial is to get an in-game shop setup as quickly as possible by leveraging the pre-built shop popup in this example project. For a more detailed breakdown of how the shop popup was created, and steps on creating a similar shop popup of your own, please see the [full tutorial](https://github.com/Shopify/unity-buy-sdk-examples/blob/master/TUTORIAL.md).

#### Who this tutorial is for

This tutorial is aimed at any developer looking to quickly integrate the sale of physical merchandise into their game. Since this tutorial uses the pre-built shop popup, only basic Unity skills are required in order to get setup.

### Integrating the shop popup

#### Add the unitypackage

<img width="2032" alt="screen shot 2017-08-31 at 11 22 19 am" src="https://user-images.githubusercontent.com/1041278/29931638-ca5a9502-8e3f-11e7-959e-38b65b2d85e4.png">

We'll start with a blank, default Unity project, as seen above.

Our first step is to add in the shop popup unitypackage, which can be downloaded [here](https://github.com/Shopify/unity-buy-sdk-examples/blob/master/UnityBuySDKShopPopup.unitypackage). Drag this into the Assets area of your project.

<img width="1920" alt="screen shot 2017-08-31 at 11 23 18 am" src="https://user-images.githubusercontent.com/1041278/29931639-ca6ba6bc-8e3f-11e7-9585-bfc3b4d171fb.png">

When prompted, import the package contents.

#### Add an instance of the Shop Popup prefab

<img width="2032" alt="screen shot 2017-08-31 at 11 24 03 am" src="https://user-images.githubusercontent.com/1041278/29931636-ca56452e-8e3f-11e7-935b-de7a267473f1.png">

Next, go into the Prefabs folder, and drag an instance of the Shop Popup prefab into the scene.

In the options for the Shop Popup prefab, we can set our Access Token and Shop Domain. For more information on getting the Access Token, see [Obtaining a storefront Access Token]([https://help.shopify.com/api/storefront-api/getting-started#obtaining-a-storefront-access-token).

#### Add the UI eventsystem

<img width="2032" alt="screen shot 2017-08-31 at 11 24 20 am" src="https://user-images.githubusercontent.com/1041278/29931637-ca57106c-8e3f-11e7-8f4d-c7fb2f92ac1e.png">

In order for the UI events to fire, we must add in a UI > Eventsystem game object.

#### Test the shop popup

<img width="2032" alt="screen shot 2017-08-31 at 11 24 33 am" src="https://user-images.githubusercontent.com/1041278/29931635-ca538bcc-8e3f-11e7-8153-c07a7f313371.png">

With these steps completed, the shop popup should be ready to use. There is a hidden button which triggers the shop popup, click in the middle of the view and the shop popup should appear. 



