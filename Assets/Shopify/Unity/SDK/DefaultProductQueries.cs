namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System;
    using Shopify.Unity.GraphQL;

    /// <summary>
    /// Generates default queries for <see ref="ShopifyClient.products">ShopifyClient.products </see>.
    /// </summary>
    public class DefaultProductQueries {
        public void ShopProducts(QueryRootQuery query, Dictionary<string,int> imageResolutions, int first = 20, string after = null) {
            query.shop(s => s
                .products(pc => pc
                    .edges(e => e
                        .node((p) => Product(p, imageResolutions))
                        .cursor()
                    )
                    .pageInfo(pi => pi
                        .hasNextPage()
                    ),
                    first : first, after : after
                )
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
            foreach (string alias in imageResolutions.Keys) {
                imageConnection
                    .edges(ie => ie
                        .node(i => i
                            .transformedSrc(
                                maxWidth: imageResolutions[alias],
                                maxHeight: imageResolutions[alias],
                                alias: alias
                            )
                        )
                        .cursor()
                    )
                    .pageInfo(pi => pi
                        .hasNextPage()
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
                .image(pnvi => pnvi
                    .altText()
                    .transformedSrc()
                )
                .price()
                .title()
                .weight()
                .selectedOptions(pnvso => pnvso
                    .name()
                    .value()
                )
                .weightUnit();

            foreach (string alias in imageResolutions.Keys) {
                variant.image(
                    pnvi => pnvi
                    .altText()
                    .transformedSrc(
                        maxWidth : imageResolutions[alias],
                        maxHeight : imageResolutions[alias],
                        alias: alias
                    )
                );
            }
        }

        public void Collection(CollectionQuery collection) {
            collection
                .id()
                .title()
                .updatedAt();
        }
    }
}
