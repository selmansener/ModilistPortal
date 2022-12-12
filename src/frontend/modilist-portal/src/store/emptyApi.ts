// Or from '@reduxjs/toolkit/query' if not using the auto-generated hooks
import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react'
import { InteractionRequiredAuthError, PublicClientApplication } from '@azure/msal-browser';
import { RootState } from './store';

import { msal } from '../App';
import { config } from '../config';

// initialize an empty api service that we'll inject endpoints into later as needed
export const emptySplitApi = createApi({
    baseQuery: fetchBaseQuery({
        baseUrl: 'https://localhost:5088/',
        prepareHeaders: (headers, { getState }) => {
            const state = getState() as RootState;
            const { token, account } = state.auth;

            console.log("token", state.auth.token);

            if (!token) {
                return headers;
            }

            if (account) {
                msal.acquireTokenSilent({
                    ...config.loginRequest,
                    account
                }).then((result) => {
                }).catch((error) => {
                    if (error instanceof InteractionRequiredAuthError) {
                        msal.logoutRedirect({
                            account
                        });
                    }
                });
            }


            console.log("account", state.auth.account);

            headers.set("Authorization", `Bearer ${state.auth.token}`);

            return headers;

        },
    }),
    endpoints: () => ({}),
})