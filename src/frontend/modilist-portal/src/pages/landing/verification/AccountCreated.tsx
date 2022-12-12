import { Grid, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";
import ForwardToInboxIcon from '@mui/icons-material/ForwardToInbox';

export default function AccountCreated() {
    const { t } = useTranslation();

    return (
        <Grid container spacing={2} textAlign="center" mt={4}>
            <Grid item xs={12}>
                <Typography variant="h4">
                    {t("Pages.AccountCreated.WelcomeMessage")}
                </Typography>
            </Grid>
            <Grid item xs={12}>
                <Typography>
                    {t("Pages.AccountCreated.CheckMailMessage")}
                </Typography>
            </Grid>
            <Grid item xs={12}>
                <ForwardToInboxIcon color="info" sx={{
                    fontSize: 72
                }} />
            </Grid>
        </Grid>
    )
}