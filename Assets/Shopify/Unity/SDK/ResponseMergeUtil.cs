namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System.Collections;
    using System.Linq;
    using System;

    public class ResponseMergeUtil {
        public static void DoNotMergeIfExists(string field, Dictionary<string, object> into, Dictionary<string, object> responseA, Dictionary<string, object> responseB) {
            if (!responseA.ContainsKey(field)) {
                into[field] = responseB[field];
            }
        }

        public static void MergeOverwrite(string field, Dictionary<string, object> into, Dictionary<string, object> responseA, Dictionary<string, object> responseB) {
            into[field] = responseB[field];
        }

        public static void MergeListsConcat(string field, Dictionary<string, object> into, Dictionary<string, object> responseA, Dictionary<string, object> responseB) {
            List<object> newList = new List<object>();

            newList.AddRange(((IList) responseA[field]).Cast<object>());
            newList.AddRange(((IList) responseB[field]).Cast<object>());

            into[field] = newList;
        }

        private Dictionary<string, MergeFieldDelegate> FieldMergers = new Dictionary<string, MergeFieldDelegate>();
        private Dictionary<string, ResponseMergeUtil> ObjectMergers = new Dictionary<string, ResponseMergeUtil>();

        public void AddFieldMerger(string field, MergeFieldDelegate merger) {
            FieldMergers[field] = merger;
        }

        public void AddObjectMerger(string field, ResponseMergeUtil merger) {
            ObjectMergers[field] = merger;
        }

        public Dictionary<string, object> Merge(Dictionary<string, object> responseA, Dictionary<string, object> responseB) {
            Dictionary<string, object> mergedResponse = new Dictionary<string, object>(responseA);

            foreach (string field in responseB.Keys) {
                if (FieldMergers.ContainsKey(field)) {
                    FieldMergers[field](field, mergedResponse, responseA, responseB);
                } else if (ObjectMergers.ContainsKey(field)) {
                    mergedResponse[field] = ObjectMergers[field].Merge(
                        (Dictionary<string, object>) responseA[field],
                        (Dictionary<string, object>) responseB[field]
                    );
                } else {
                    MergeOverwrite(field, mergedResponse, responseA, responseB);
                }
            }

            return mergedResponse;
        }
    }
}