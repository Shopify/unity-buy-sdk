using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Publishing {
    public static string[] DirectoriesInPackage = new string[] {
        "Assets/ExternalDependencyManager/",
        "Assets/ExternalDependencyManager/Editor/",
        "Assets/Shopify/",
        "Assets/Shopify/Examples/",
        "Assets/Shopify/Examples/3DAssets/",
        "Assets/Shopify/Examples/3DAssets/Environment/",
        "Assets/Shopify/Examples/3DAssets/Environment/Materials/",
        "Assets/Shopify/Examples/3DAssets/Environment/Textures/",
        "Assets/Shopify/Examples/3DAssets/VendingMachine/",
        "Assets/Shopify/Examples/3DAssets/VendingMachine/Materials/",
        "Assets/Shopify/Examples/3DAssets/VendingMachine/Textures/",
        "Assets/Shopify/Examples/Animation/",
        "Assets/Shopify/Examples/Animation/Camera/",
        "Assets/Shopify/Examples/Animation/Canvas/",
        "Assets/Shopify/Examples/Animation/Error Panel/",
        "Assets/Shopify/Examples/Editor/",
        "Assets/Shopify/Examples/Font/",
        "Assets/Shopify/Examples/Images/",
        "Assets/Shopify/Examples/Images/HomeScreen/",
        "Assets/Shopify/Examples/Images/NewIcons/",
        "Assets/Shopify/Examples/PostEffects/",
        "Assets/Shopify/Examples/Prefabs/",
        "Assets/Shopify/Examples/Scenes/",
        "Assets/Shopify/Examples/Scenes/Vending Machine Example/",
        "Assets/Shopify/Examples/Scripts/",
        "Assets/Shopify/Examples/Scripts/Components/",
        "Assets/Shopify/Examples/Scripts/Helpers/",
        "Assets/Shopify/Examples/Scripts/LineItems/",
        "Assets/Shopify/Examples/Scripts/Panels/",
        "Assets/Shopify/Plugins/",
        "Assets/Shopify/Plugins/Android/",
        "Assets/Shopify/Plugins/Android/libs/",
        "Assets/Shopify/Plugins/Editor/",
        "Assets/Shopify/Plugins/iOS/",
        "Assets/Shopify/Plugins/iOS/Shopify/",
        "Assets/Shopify/Unity/",
        "Assets/Shopify/Unity/Editor/",
        "Assets/Shopify/Unity/Editor/BuildPipeline/",
        "Assets/Shopify/Unity/Generated/",
        "Assets/Shopify/Unity/Generated/GraphQL/",
        "Assets/Shopify/Unity/SDK/",
        "Assets/Shopify/Unity/SDK/Android/",
        "Assets/Shopify/Unity/SDK/Editor/",
        "Assets/Shopify/Unity/SDK/iOS/",
        "Assets/Shopify/Unity/Static/",
        "Assets/Shopify/Unity/Static/Editor/",
        "Assets/Shopify/Unity/Static/Editor/Resources/",
        "Assets/Shopify/Unity/UI/",
        "Assets/Shopify/Unity/Vendor/"
    };

    public static void Publish(string packageName) {
        var filesToIncludeInPackage = new List<string>();

        foreach(var dir in DirectoriesInPackage) {
            var files = Directory.GetFiles(dir);
            filesToIncludeInPackage.AddRange(files);
        }

        AssetDatabase.ExportPackage(filesToIncludeInPackage.ToArray(), packageName, ExportPackageOptions.Default);
    }

    public static void PublishFromCommandLine() {
        var packageName = System.Environment.GetEnvironmentVariable("UNITY_PACKAGE");
        Publish(packageName);
    }

    [MenuItem("Build/Publish Unitypackage")]
    public static void PublishFromUnityMenu() {
        Publish("shopify-buy.unitypackage");
    }
}
