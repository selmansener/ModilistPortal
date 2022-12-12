import { AppBar, Box, Container, Grid, Toolbar } from "@mui/material";
import React from "react";
import { Helmet } from "react-helmet";
import { useTranslation } from "react-i18next";
import { Outlet } from "react-router-dom";
import { RouteConfig } from "../../router/routes";

export function VerificationLayout() {
    const { t } = useTranslation();

    return (
        <Box display="flex" flexDirection="column">
            <Box component="main" minHeight={760}>
                <Helmet>
                    <title>{t("Pages.Titles.AccountVerification")}</title>
                </Helmet>
                <AppBar position="static">
                    <Toolbar>
                        <Box sx={{
                            display: 'flex',
                            justifyContent: 'center',
                            flexGrow: 1
                        }}>
                            <img width={200} src="/whitehorizontallogo.svg"  />
                        </Box>
                    </Toolbar>
                </AppBar>
                <Container maxWidth="xl" sx={{
                    mt: 2,
                    mb: 2
                }}>
                    <Grid container>
                        <Outlet />
                    </Grid>
                </Container>
            </Box>
        </Box>
    )
}

const AccountCreatedPage = React.lazy(() => import("../../pages/landing/verification/AccountCreated"));
const AccountVerifiedPage = React.lazy(() => import("../../pages/landing/verification/AccountVerified"));
const AccountVerificationFailedPage = React.lazy(() => import("../../pages/landing/verification/AccountVerificationFailed"));

export const verificationLayoutRoutes: RouteConfig = {
    path: "/verification",
    element: <VerificationLayout />,
    leafNodes: [
        {
            path: "account-created",
            element: <AccountCreatedPage />
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
}