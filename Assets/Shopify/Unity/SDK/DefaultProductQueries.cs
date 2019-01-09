namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System;
    using Shopify.Unity.GraphQL;

    /// <summary>
    /// Generates default queries for <see ref="ShopifyClient.products">ShopifyClient.products </see>.
    /// </summary>
    public class DefaultProductQueries {
        public void ShopProducts(QueryRootQuery query, Dictionary<string,int> imageResolutions, int first = 20, string after = null) {
            query.products(pc => pc
                .edges(e => e
                    .node((p) => Product(p, imageResolutions))
                    .cursor()
                )
                .pageInfo(pi => pi
                    .hasNextPage()
                ),
                first : first, after : after
            );
        }

        public void Product(ProductQuery product, Dictionary<string, int> imageResolutions) {
            product
                .id()
                .title()
                .description()
                .descriptionHtml()
                .options(pn => pn
                    .name()
                    .values()
                )
                .variants(
                    pvc => ProductVariantConnection(pvc, imageResolutions),
                    first : DefaultQueries.MaxPageSize
                )
                .collections(
                    pcc => CollectionConnection(pcc),
                    first : DefaultQueries.MaxPageSize
                )
                .images(
                    ic => ImageConnection(ic, imageResolutions),
                    first : DefaultQueries.MaxPageSize
                );
        }

        public void ImageConnection(ImageConnectionQuery imageConnection, Dictionary<string, int> imageResolutions) {
            imageConnection
                .edges(ie => ie
                    .node(i => AliasedTransformedSrcImages(i.altText().transformedSrc(), imageResolutions))
                    .cursor()
                )
                .pageInfo(pi => pi
                    .hasNextPage()
                );
        }

        public void AliasedTransformedSrcImages(ImageQuery imageQuery, Dictionary<string, int> imageResolutions) {
            foreach (string alias in imageResolutions.Keys) {
                imageQuery.transformedSrc(
                    maxWidth: imageResolutions[alias],
                    maxHeight: imageResolutions[alias],
                    alias: alias
                );
            }
        }

        public void ProductVariantConnection(ProductVariantConnectionQuery variantConnection, Dictionary<string, int> imageResolutions) {
            variantConnection
                .edges(pve => pve
                    .node(pnv => ProductVariant(pnv, imageResolutions))
                    .cursor()
                )
                .pageInfo(pvp => pvp
                    .hasNextPage()
                );
        }

        public void CollectionConnection(CollectionConnectionQuery collectionConnection) {
            collectionConnection
                .edges(pce => pce
                    .node(pnc => Collection(pnc))
                    .cursor()
                )
                .pageInfo(pcp => pcp
                    .hasNextPage()
                );
        }

        public void Image(ImageQuery image) {
            image
                .altText()
                .transformedSrc();
        }

        public void ProductVariant(ProductVariantQuery variant, Dictionary<string, int> imageResolutions) {
            variant
                .id()
                .availableForSale()
                .image(i => AliasedTransformedSrcImages(i.altText().transformedSrc(), imageResolutions))
                .price()
                .title()
                .weight()
                .selectedOptions(pnvso => pnvso
                    .name()
                    .value()
                )
                .weightUnit();
        }

        public void Collection(CollectionQuery collection) {
            collection
                .id()
                .title()
                .updatedAt();
        }
    }
}
