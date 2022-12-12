import { Button, Grid, Typography } from "@mui/material";
import React, { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";

import MarkEmailReadIcon from '@mui/icons-material/MarkEmailRead';
import { useMsal } from "@azure/msal-react";

export default function AccountVerified() {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const { instance: msal } = useMsal();
    const accounts = msal.getAllAccounts();
    const [counter, setCounter] = useState(5);

    useEffect(() => {
        if (accounts.length > 0) {
            const timer = setTimeout(() => {
                navigate("/welcome/gender", {
                    replace: true
                });
            }, 5000);
            return () => clearTimeout(timer);
        }
    }, []);

    useEffect(() => {
        if (accounts.length > 0) {
            counter > 0 && setTimeout(() => setCounter(counter - 1), 1000);
        }
    }, [counter]);

    return (
        <Grid container spacing={2} textAlign="center" mt={4}>
            <Grid item xs={12}>
                <Typography variant="h4">
                    {t("Pages.AccountVerified.SuccessMessage")}
                </Typography>
            </Grid>
            <Grid item xs={12}>
                <Typography>
                    {t("Pages.AccountVerified.Thanks")}
                </Typography>
            </Grid>
            <Grid item xs={12}>
                <Typography>
                    {t("Pages.AccountVerified.NavigationMessage")}
                </Typography>
            </Grid>
            <Grid item xs={12}>
                <MarkEmailReadIcon color="success" sx={{
                    fontSize: 72
                }} />
            </Grid>
            {accounts.length > 0 && <React.Fragment>
                <Grid item xs={12}>
                    <Typography>
                        {t("Pages.AccountVerified.Timer", { counter: counter })}
                    </Typography>
                </Grid>
                <Grid item xs={12}>
                    <Button variant="contained" color="secondary" onClick={() => {
                        navigate("/welcome/gender", {
                            replace: true
                        });
                    }}>
                        {t("Pages.AccountVerified.NavigateToForm")}
                    </Button>
                </Grid>
            </React.Fragment>}
            {accounts.length === 0 && <React.Fragment>
                <Grid item xs={12}>
                    <Button variant="contained" color="secondary" onClick={() => {
                        navigate("/", {
                            replace: true
                        });
                    }}>
                        {t("Pages.AccountVerified.Login")}
                    </Button>
                </Grid>
            </React.Fragment>}
        </Grid>
    )
}