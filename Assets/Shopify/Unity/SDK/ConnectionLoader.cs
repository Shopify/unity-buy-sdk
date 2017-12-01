namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System.Collections;
    using System.Reflection;
    using System;
    using Shopify.Unity.GraphQL;

    public struct ConnectionQueryInfo {
        public ConnectionQueryInfo(GetConnectionFromResponseDelegate getConnection, BuildQueryOnEdgesNodeDelegate query, string after = null) {
            GetConnection = getConnection;
            Query = query;
            After = after;
        }

        public string After;
        public GetConnectionFromResponseDelegate GetConnection;
        public BuildQueryOnEdgesNodeDelegate Query;
    }

    public class ConnectionLoader {
        private delegate void QueryLoop(QueryLoop loop);

        private QueryLoader Loader;

        public ConnectionLoader(QueryLoader loader) {
            Loader = loader;
        }

        public void QueryConnection(BuildQueryOnConnectionLoopDelegate getQuery, GetConnectionFromResponseDelegate getConnection, QueryResponseHandler callback) {
            QueryResponse mergedResponse = null;
            QueryRootQuery query = getQuery(null);

            QueryLoop queryLoop = (loop) => {
                Loader.Query(query, (response) => {
                    var error = (ShopifyError) response;
                    if (error != null) {
                        callback(response);
                    } else {
                        if (mergedResponse == null) {
                            mergedResponse = response;
                        } else {
                            MergeConnections(getConnection(mergedResponse.data), getConnection(response.data));
                        }

                        query = getQuery(response);

                        if (query != null) {
                            loop(loop);
                        } else {
                            callback(mergedResponse);
                        }
                    }
                });
            };

            queryLoop(queryLoop);
        }

        public void QueryConnectionsOnNodes(List<Node> nodes, List<ConnectionQueryInfo> connectionInfos, BuildQueryOnNodeDelegate buildQuery, ResponseNodeHandler callback) {
            List<int> indicesQueried = new List<int>();
            List<List<ConnectionQueryInfo>> connectionInfoQueried = new List<List<ConnectionQueryInfo>>();
            QueryRootQuery query = new QueryRootQuery();

            for (int i = 0; i < nodes.Count; i++) {
                Node node = nodes[i];
                List<ConnectionQueryInfo> connectionInfosToBuildQuery = GetConnectionInfosToBuildQueriesFrom(connectionInfos, node);

                if (connectionInfosToBuildQuery.Count > 0) {
                    indicesQueried.Add(i);
                    connectionInfoQueried.Add(connectionInfosToBuildQuery);

                    buildQuery(query, connectionInfosToBuildQuery, node.id(), GetAliasForIdx(i));
                }
            }

            if (indicesQueried.Count == 0) {
                callback(nodes, null);
            } else {
                Loader.Query(query, (response) => {
                    var error = (ShopifyError) response;
                    if (error != null) {
                        callback(null, error);
                    } else {
                        for (int i = 0; i < indicesQueried.Count; i++) {
                            int idxQueried = indicesQueried[i];
                            List<ConnectionQueryInfo> infos = connectionInfoQueried[i];
                            object baseResult = nodes[idxQueried];
                            object resultToMerge = response.data.node(GetAliasForIdx(idxQueried));

                            foreach (ConnectionQueryInfo info in infos) {
                                object baseConnection = info.GetConnection(baseResult);
                                object connectionToMerge = info.GetConnection(resultToMerge);

                                MergeConnections(baseConnection, connectionToMerge);
                            }
                        }

                        QueryConnectionsOnNodes(nodes, connectionInfos, buildQuery, callback);
                    }
                });
            }
        }

        private string GetAliasForIdx(int idx) {
            return String.Format("a{0}", idx);
        }

        private List<ConnectionQueryInfo> GetConnectionInfosToBuildQueriesFrom(List<ConnectionQueryInfo> infos, object responseObject) {
            List<ConnectionQueryInfo> infosToQuery = new List<ConnectionQueryInfo>();

            foreach (ConnectionQueryInfo info in infos) {
                object connection = info.GetConnection(responseObject);
                string after = GetAfter(connection);

                if (after != null) {
                    infosToQuery.Add(new ConnectionQueryInfo(info.GetConnection, info.Query, after));
                }
            }

            return infosToQuery;
        }

        private string GetAfter(object connection) {
            string after = null;

            Type connectionType = connection.GetType();

            MethodInfo pageInfoMethod = connectionType.GetMethod("pageInfo");
            PageInfo pageInfo = (PageInfo) pageInfoMethod.Invoke(connection, null);

            MethodInfo edgesMethod = connectionType.GetMethod("edges");

            if (pageInfo.hasNextPage()) {
                IList edges = edgesMethod.Invoke(connection, null) as IList;

                if (edges.Count > 0) {
                    object lastEdge = edges[edges.Count - 1];

                    MethodInfo cursorMethod = lastEdge.GetType().GetMethod("cursor");

                    after = (string) cursorMethod.Invoke(edges[edges.Count - 1], null);
                }
            }

            return after;
        }

        private void MergeConnections(object baseConnection, object connectionToMerge) {
            MethodInfo methodAddFromConnection = baseConnection.GetType().GetMethod("AddFromConnection");

            // this will merge connectionToMerge into baseConnection
            methodAddFromConnection.Invoke(baseConnection, new object[] { connectionToMerge });
        }
    }
}