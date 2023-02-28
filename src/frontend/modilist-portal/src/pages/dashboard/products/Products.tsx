import { Button, Grid, Typography } from "@mui/material";
import { Outlet, useNavigate } from "react-router-dom";

export default function Products() {
    const navigate = useNavigate();
    
    return <Grid container spacing={2}>
        <Grid item xs={12} display="flex" justifyContent="space-evenly">
            <Button variant="contained" color="secondary" onClick={() => navigate("/dashboard/products")}>
                <Typography color="white" variant="h6">Products</Typography>
            </Button>
            <Button variant="contained" color="secondary" onClick={() => navigate("/dashboard/products/product-upload-history")}>
                <Typography color="white" variant="h6">Product Upload History</Typography>
            </Button>
            <Button variant="contained" color="secondary" onClick={() => navigate("/dashboard/products/product-variant-upload-history")}>
                <Typography color="white" variant="h6">Product Variant Upload History</Typography>
            </Button>
            <Button variant="contained" color="secondary" onClick={() => navigate("/dashboard/products/new")}>
                <Typography color="white" variant="h6"> New Product</Typography>
            </Button>
            <Button variant="contained" color="secondary" onClick={() => navigate("/dashboard/products/new-upload")}>
                <Typography color="white" variant="h6">New Upload</Typography>
            </Button>
        </Grid>
        <Grid item xs={12}>
            <Outlet />
        </Grid>
    </Grid>
}