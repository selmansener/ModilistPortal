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
        <Container>
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

const SalesOrdersPage = React.lazy(() => import("../../pages/dashboard/salesOrders/SalesOrders"));
const ProductsPage = React.lazy(() => import("../../pages/dashboard/products/Products"));
const ReturnsPage = React.lazy(() => import("../../pages/dashboard/returns/Returns"));
const TenantPage = React.lazy(() => import("../../pages/dashboard/tenant/Tenant"));

export const dashboardRoutes: RouteConfig = {
    path: "/dashboard",
    element: <DashboardLayout />,
    isPublic: false,
    leafNodes: [
        {
            path: "/sales-orders",
            element: <SalesOrdersPage />
        },
        {
            path: "/products",
            element: <ProductsPage />
        },
        {
            path: "/returns",
            element: <ReturnsPage />
        },
        {
            path: "/products",
            element: <TenantPage />
        }
    ]
}