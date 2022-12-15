import { AuthenticatedTemplate, UnauthenticatedTemplate } from "@azure/msal-react";
import { Container } from "@mui/material";
import React from "react";
import { Outlet } from "react-router-dom";
import { RouteConfig } from "../../router/routes";
import { Footer } from "../shared/Footer";
import { Header } from "../shared/Header";

export default function LandingLayout() {
    return <React.Fragment>
        <Header />
        <Container sx={{
            mt: 8
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

const VerificationPage = React.lazy(() => import("../../pages/landing/verification/Verification"));
const AccountCreatedPage = React.lazy(() => import("../../pages/landing/verification/AccountCreated"));
const AccountVerifiedPage = React.lazy(() => import("../../pages/landing/verification/AccountVerified"));
const AccountVerificationFailedPage = React.lazy(() => import("../../pages/landing/verification/AccountVerificationFailed"));
const MainPage = React.lazy(() => import("../../pages/landing/main/Main"));

export const landingRoutes: RouteConfig = {
    path: "/",
    element: <LandingLayout />,
    leafNodes: [
        {
            path: "verification",
            element: <VerificationPage />,
            leafNodes: [
                {
                    path: "account-created",
                    element: <AccountCreatedPage />,
                },
                {
                    path: "account-verified",
                    element: <AccountVerifiedPage />
                },
                {
                    path: "account-verification-failed",
                    element: <AccountVerificationFailedPage />
                },
            ]
        },
        {
            path: "",
            element: <MainPage />
        },
    ]
}