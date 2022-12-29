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
const ProductUploadHistoryPage = React.lazy(() => import("../../pages/dashboard/products/UploadHistory"));
const ReturnsPage = React.lazy(() => import("../../pages/dashboard/returns/Returns"));
const TenantPage = React.lazy(() => import("../../pages/dashboard/tenant/Tenant"));


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
                    path: "upload-history",
                    element: <ProductUploadHistoryPage />
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