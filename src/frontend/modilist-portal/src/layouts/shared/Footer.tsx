import { AuthenticatedTemplate, UnauthenticatedTemplate } from "@azure/msal-react";
import React from "react";

export function Footer() {
    return <React.Fragment>
        <AuthenticatedTemplate>
        </AuthenticatedTemplate>
        <UnauthenticatedTemplate>
        </UnauthenticatedTemplate>
    </React.Fragment>
}