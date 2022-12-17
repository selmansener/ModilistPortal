import { useMsal } from "@azure/msal-react";
import { config } from "../../config";
import { useAppDispatch } from "../hooks";
import { authSlice } from "./auth";

export function Authentication() {
    const { instance: msal } = useMsal();
    const dispatch = useAppDispatch();
    const { loginRequest } = config;

    if (!msal) {
        return <div>
            Loading...
        </div>
    }

    const accounts = msal.getAllAccounts();
    if (accounts.length > 0) {
        const account = accounts[0];
        msal.acquireTokenSilent({
            ...loginRequest,
            account
        }).then((result: any) => {
            dispatch(authSlice.actions.setToken(result.accessToken))
            dispatch(authSlice.actions.setActiveAccount(result.account))
        });
    }

    return <></>
}