import { AuthenticatedTemplate, UnauthenticatedTemplate } from "@azure/msal-react";
import { Container } from "@mui/material";
import React from "react";
import { Outlet } from "react-router-dom";
import { RouteConfig } from "../../router/routes";
import { Footer } from "../shared/Footer";
import { Header } from "../shared/Header";

export function DashboardLayout() {
    return <React.Fragment>
        <Header />
        <Container sx={{
            mt: 15
        }}>
            <AuthenticatedTemplate>
                <Outlet />
            </AuthenticatedTemplate>
            <UnauthenticatedTemplate>
                <Outlet />
            </UnauthenticatedTemplate>
        </Container>
        <Footer />
    </React.Fragment>
}

const MainPage = React.lazy(() => import("../../pages/dashboard/main/Main"));
const SalesOrdersPage = React.lazy(() => import("../../pages/dashboard/salesOrders/SalesOrders"));
const ProductsPage = React.lazy(() => import("../../pages/dashboard/products/Products"));
const ProductListPage = React.lazy(() => import("../../pages/dashboard/products/ProductList"));
const ProductUploadHistoryPage = React.lazy(() => import("../../pages/dashboard/products/ProductUploadHistory"));
const ProductUploadHistoryDetailsPage = React.lazy(() => import("../../pages/dashboard/products/ProductUploadHistoryDetails"));
const ProductVariantUploadHistoryPage = React.lazy(() => import("../../pages/dashboard/products/ProductVariantUploadHistory"));
const ReturnsPage = React.lazy(() => import("../../pages/dashboard/returns/Returns"));
const TenantPage = React.lazy(() => import("../../pages/dashboard/tenant/Tenant"));
const ProductListGridPage = React.lazy(() => import("../../pages/dashboard/products/components/ProductGrid"));
const ProductDetailsPage = React.lazy(() => import("../../pages/dashboard/products/ProductDetails"));
const NewProductPage = React.lazy(() => import("../../pages/dashboard/products/NewProduct"));
const NewProductUploadPage = React.lazy(() => import("../../pages/dashboard/products/NewProductUpload"));
const UpdateProductPage = React.lazy(() => import("../../pages/dashboard/products/UpdateProduct"));

export const dashboardRoutes: RouteConfig = {
    path: "/dashboard",
    element: <DashboardLayout />,
    isPublic: false,
    leafNodes: [
        {
            path: "",
            element: <MainPage />
        },
        {
            path: "sales-orders",
            element: <SalesOrdersPage />
        },
        {
            path: "products",
            element: <ProductsPage />,
            leafNodes: [
                {
                    path:"",
                    element: <ProductListPage />,
                    leafNodes: [
                        {
                            path:"",
                            element: <ProductListGridPage />
                        },
                        {
                            path:"active",
                            element: <ProductListGridPage />
                        },
                        {
                            path:"in-review",
                            element: <ProductListGridPage />
                        },
                        {
                            path:"out-of-stock",
                            element: <ProductListGridPage />
                        },
                        {
                            path:"missing-info",
                            element: <ProductListGridPage />
                        },
                        {
                            path:"rejected",
                            element: <ProductListGridPage />
                        },
                        {
                            path:"passive",
                            element: <ProductListGridPage />
                        }
                    ]
                },
                {
                    path: "new",
                    element: <NewProductPage />
                },
                {
                    path: ":productId",
                    element: <ProductDetailsPage />
                },
                {
                    path: ":productId/update",
                    element: <UpdateProductPage />
                },
                {
                    path: "new-upload",
                    element: <NewProductUploadPage />
                },
                {
                    path: "product-upload-history",
                    element: <ProductUploadHistoryPage />
                },
                {
                    path: "product-upload-history/:productExcelUploadId",
                    element: <ProductUploadHistoryDetailsPage />
                },
                {
                    path: "product-variant-upload-history",
                    element: <ProductVariantUploadHistoryPage />
                }
            ]
        },
        {
            path: "returns",
            element: <ReturnsPage />
        },
        {
            path: "tenant",
            element: <TenantPage />
        }
    ]
}