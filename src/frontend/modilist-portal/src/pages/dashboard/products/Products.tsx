import { Button, Grid } from "@mui/material";
import { Outlet, useNavigate } from "react-router-dom";

export default function Products() {
    const navigate = useNavigate();

    return <Grid container spacing={2}>
        <Grid item xs={12}>
            <Button onClick={() => navigate("/dashboard/products")}>
                Products
            </Button>
            <Button onClick={() => navigate("/dashboard/products/upload-history")}>
                Upload History
            </Button>
        </Grid>
        <Grid item xs={12}>
            <Outlet />
        </Grid>
    </Grid>
}