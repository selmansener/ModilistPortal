import { AuthenticatedTemplate, UnauthenticatedTemplate } from "@azure/msal-react";
import { Grid, Typography } from "@mui/material";
import React from "react";

export default function Main() {
    return <Grid container spacing={2}>
        <AuthenticatedTemplate>
            <Grid item xs={12}>
                <Typography>This is landing page. AuthenticatedTemplate</Typography>
            </Grid>
        </AuthenticatedTemplate>
        <UnauthenticatedTemplate>
            <Grid item xs={12}>
                <Typography>This is landing page. UnauthenticatedTemplate</Typography>
            </Grid>
        </UnauthenticatedTemplate>
    </Grid>
}