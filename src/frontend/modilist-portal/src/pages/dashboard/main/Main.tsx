import { Button, Grid } from "@mui/material";
import { useNavigate } from "react-router-dom";

export default function Main() {
    const navigate = useNavigate();

    return <Grid container spacing={2}>
        <Grid item xs={12}>
            <Button onClick={() => navigate("/dashboard/tenant")}>
                Tenant Details
            </Button>
            <Button onClick={() => navigate("/dashboard/products")}>
                Products
            </Button>
            <Button onClick={() => navigate("/dashboard/sales-orders")}>
                SalesOrders
            </Button>
        </Grid>
    </Grid>
}