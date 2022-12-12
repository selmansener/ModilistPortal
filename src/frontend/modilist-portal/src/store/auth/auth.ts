import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { AccountInfo } from '@azure/msal-browser';

interface MsalState {
    token?: string,
    account?: AccountInfo | null,
}

const initialState: MsalState = {
    token: undefined,
    account: undefined,
}

export const authSlice = createSlice({
    name: 'auth',
    initialState,
    reducers: {
        setToken: (state, action: PayloadAction<string>) => {
            state.token = action.payload
        },
        setActiveAccount: (state, action: PayloadAction<AccountInfo | null>) => {
            state.account = action.payload
        }
    }
});

export default authSlice.reducer;