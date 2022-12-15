import { AuthenticatedTemplate, UnauthenticatedTemplate, useMsal } from "@azure/msal-react"
import { AppBar, Box, Button, Toolbar } from "@mui/material"
import React from "react"
import { NavLink } from "react-router-dom";
import { config } from "../../config";

function AuthenticationButton() {
    const { instance: msal } = useMsal();

    const logout = () => {
        const account = msal.getAllAccounts()[0];

        msal.logoutRedirect({
            account
        });
    }

    const login = () => {
        msal.loginRedirect(config.loginRequest)
            .catch(e => {
                console.log(e);
            });
    }

    return <React.Fragment>
        <AuthenticatedTemplate>
            <Button onClick={logout}>
                Logout
            </Button>
        </AuthenticatedTemplate>
        <UnauthenticatedTemplate>
            <Button onClick={login}>
                Login
            </Button>
        </UnauthenticatedTemplate>
    </React.Fragment>
}

export function Header() {

    return <React.Fragment>
        <AppBar position="fixed" color="inherit">
            <Toolbar>
                <Box sx={{
                    display: 'flex',
                    justifyContent: 'flex-start',
                    flexGrow: 1,
                }}>
                    <NavLink to="/">
                        <img width={200} src="/originalhorizontallogoslogan.svg" />
                    </NavLink>
                </Box>
                <AuthenticationButton />
            </Toolbar>
        </AppBar>
    </React.Fragment>
}