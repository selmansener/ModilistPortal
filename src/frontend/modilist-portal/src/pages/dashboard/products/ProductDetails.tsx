import { Button, Grid } from "@mui/material";
import { useNavigate, useParams } from "react-router-dom";
import Loading from "../../../components/loading/loading";
import { useGetApiV1ProductGetByProductIdQuery } from "../../../store/api";
import { UploadProductImages } from "./components/UploadProductImages";

export default function ProductDetails() {
    const navigate = useNavigate();
    const { productId } = useParams();
    const { data, isLoading, isFetching, error } = useGetApiV1ProductGetByProductIdQuery({
        productId: productId ? parseInt(productId) : 0
    });
    const product = data?.data;

    if (isLoading || isFetching) {
        return <Loading />
    }

    return <Grid item container spacing={2}>
        <Grid item xs={12}>
            <Button onClick={() => navigate(`/dashboard/products/${product?.id}/update`)}>
                Update
            </Button>
        </Grid>
        <Grid item xs={12}>
            Id: {product?.id}
        </Grid>
        <Grid item xs={12}>
            ProductName: {product?.name}
        </Grid>
        <Grid item xs={12}>
            SKU: {product?.sku}
        </Grid>
        <Grid item xs={12}>
            Barcode: {product?.barcode}
        </Grid>
        <Grid item xs={12}>
            Brand: {product?.brand}
        </Grid>
        <Grid item xs={12}>
            Category: {product?.category}
        </Grid>
        <Grid item xs={12}>
            State: {product?.state}
        </Grid>
        <Grid item xs={12}>
            Price: {product?.price}
        </Grid>
        <Grid item xs={12}>
            SalesPrice: {product?.salesPrice}
        </Grid>
        <Grid item xs={12}>
            CreatedAt: {product?.createdAt}
        </Grid>
        <Grid item xs={12}>
            <UploadProductImages productId={product?.id ?? 0} />
        </Grid>
    </Grid>
}