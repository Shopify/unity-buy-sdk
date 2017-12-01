namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System.Collections;
    using System;

    public class CastUtils {
        public static T GetEnumValue<T>(object jsonValue) where T : struct, IConvertible {
            return (T) GetEnumValue(jsonValue, typeof(T));
        }

        private static object GetEnumValue(object jsonValue, Type enumType) {
            object enumValue;

            try {
            enumValue = Enum.Parse(enumType, (string) jsonValue);
            } catch (ArgumentException) {
            enumValue = Enum.Parse(enumType, "UNKNOWN");
            }

            return enumValue;
        }

        public static T CastList<T>(IList list) {
            return (T) CastList(list, typeof(T));
        }

        private static object CastList(IList list, Type listType) {
            IList outList = (IList) Activator.CreateInstance(listType);
            Type outItemType = listType.GetGenericArguments() [0];

            foreach (object item in list) {
                if (item == null) {
                    outList.Add(null);
                } else {
                    if (item is IList) {
                        outList.Add(CastList((IList) item, listType.GetGenericArguments() [0]));
                    } else if (outItemType.IsEnum) {
                        outList.Add(GetEnumValue(item, outItemType));
                    } else if (outItemType.IsValueType) {
                        outList.Add(Convert.ChangeType(item, outItemType));
                    } else if (outItemType.IsClass) {
                        try {
                            outList.Add(Convert.ChangeType(item, outItemType));
                        } catch (InvalidCastException) {
                            outList.Add(Activator.CreateInstance(outItemType, new object[] { item }));
                        }
                    } else {
                        throw new InvalidCastException("Unhandled type of cast");
                    }
                }
            }

            return outList;
        }
    }
}