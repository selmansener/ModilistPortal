import { useMsal } from "@azure/msal-react";
import React from "react";
import { useEffect } from "react";
import { Outlet } from "react-router-dom";
import { config } from "../../config";
import { RouteConfig } from "../../router/routes";

export default function LandingLayout() {
    const { instance } = useMsal();

    // TODO: don't redirect to login on load
    useEffect(() => {
        instance.loginRedirect(config.loginRequest)
            .catch(e => {
                console.log(e);
            });
    }, []);

    return (
        <React.Fragment>
            <Outlet />
        </React.Fragment>
    );
}

export const unauthenticatedLayoutRoutes: RouteConfig = {
    path: "/",
    element: <LandingLayout />,
    isPublic: true
}