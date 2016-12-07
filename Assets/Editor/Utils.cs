namespace Shopify.Tests {
    using System;
    using System.Linq;

    public class Utils {
        public static bool DoesExist(string className) {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from type in assembly.GetTypes()
            where type.Name == className
            select type).Any();
        }
    }
}