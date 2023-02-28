import { Button, FormControl, Grid, TextField, Typography } from "@mui/material";
import { useParams } from "react-router-dom";
import Loading from "../../../components/loading/loading";
import { ProductDetailsDto, useGetApiV1ProductGetByProductIdQuery } from "../../../store/api";
import * as Yup from "yup";
import { useFormik } from 'formik';
import { useTranslation } from "react-i18next";

export default function UpdateProduct() {
    const { t } = useTranslation();
    const { productId } = useParams();
    const { data, isLoading, isFetching, error } = useGetApiV1ProductGetByProductIdQuery({
        productId: productId ? parseInt(productId) : 0
    });
    const product = data?.data;

    const requiredField = t("FormValidation.RequiredField");
    const schema = Yup.object({
        name: Yup.string().required(requiredField),
        sku: Yup.string().required(requiredField),
        barcode: Yup.string().required(requiredField),
        category: Yup.string().required(requiredField),
        price: Yup.string().required(requiredField),
    });

    const {
        values: tenant,
        handleChange,
        handleBlur,
        touched,
        errors,
        setFieldValue,
        setFieldTouched,
        submitForm
    } = useFormik({
        enableReinitialize: true,
        initialValues: product ?? {
            name: "",
            sku: "",
            barcode: "",
            category: "",
            price: ""
        },
        validationSchema: schema,
        onSubmit: (values) => {
        }
    });
    
    if (isLoading || isFetching || product === undefined) {
        return <Loading />
    }
    //TODO: Add gender, color, size
    return <Grid item container spacing={2}>
        <Grid item xs={4}>
            <FormControl fullWidth>
                <TextField label={<Typography>Name</Typography>}
                    name="name"
                    disabled={isLoading}
                    onChange={handleChange}
                    onBlur={handleBlur}
                    helperText={touched.name && errors.name}
                    error={touched.name && errors.name !== undefined}
                    variant="outlined"
                    value={tenant?.name} />
            </FormControl>
        </Grid>
        <Grid item xs={4}>
            <FormControl fullWidth>
                <TextField label={<Typography>SKU</Typography>}
                    name="sku"
                    disabled={isLoading}
                    onChange={handleChange}
                    onBlur={handleBlur}
                    helperText={touched.sku && errors.sku}
                    error={touched.sku && errors.sku !== undefined}
                    variant="outlined"
                    value={tenant?.sku} />
            </FormControl>
        </Grid>
        <Grid item xs={4}>
            <FormControl fullWidth>
                <TextField label={<Typography>Barcode</Typography>}
                    name="barcode"
                    disabled={isLoading}
                    onChange={handleChange}
                    onBlur={handleBlur}
                    helperText={touched.barcode && errors.barcode}
                    error={touched.barcode && errors.barcode !== undefined}
                    variant="outlined"
                    value={tenant?.barcode} />
            </FormControl>
        </Grid>
        <Grid item xs={4}>
            <FormControl fullWidth>
                <TextField label={<Typography>Category</Typography>}
                    name="category"
                    disabled={isLoading}
                    onChange={handleChange}
                    onBlur={handleBlur}
                    helperText={touched.category && errors.category}
                    error={touched.category && errors.category !== undefined}
                    variant="outlined"
                    value={tenant?.category} />
            </FormControl>
        </Grid>
        <Grid item xs={4}>
            <FormControl fullWidth>
                <TextField label={<Typography>Price</Typography>}
                    name="price"
                    disabled={isLoading}
                    onChange={handleChange}
                    onBlur={handleBlur}
                    helperText={touched.price && errors.price}
                    error={touched.price && errors.price !== undefined}
                    variant="outlined"
                    value={tenant?.price} />
            </FormControl>
        </Grid>
        <Grid item xs={12}>
            <Button onClick={() => {
                submitForm();
            }}>
                Update
            </Button>
        </Grid>
    </Grid>
}