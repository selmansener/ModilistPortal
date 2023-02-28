import { Form } from "react-router-dom";
import { usePostApiV1ProductCreateProductMutation } from "../../../store/api";
import { Button, FormControl, Grid, TextField, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";
import Loading from "../../../components/loading/loading";
import * as Yup from "yup";
import { useFormik } from 'formik';
import { useNavigate } from "react-router-dom";
import { useEffect } from "react";


export default function NewProduct() {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const [createProduct, result] = usePostApiV1ProductCreateProductMutation();
    const requiredField = t("FormValidation.RequiredField");
    const schema = Yup.object({
        name: Yup.string().required(requiredField),
        sku: Yup.string().required(requiredField),
        barcode: Yup.string().required(requiredField),
        brandName: Yup.string().required(requiredField),
        category: Yup.string().required(requiredField),
        price: Yup.number().required(requiredField),
        salesPrice: Yup.number().required(requiredField),
        gender: Yup.string().required(requiredField),
        size: Yup.string().required(requiredField),
        color: Yup.string().required(requiredField),
    });
    const { isLoading, error, isSuccess } = result;

    const {
        values: product,
        handleChange,
        handleBlur,
        touched,
        errors,
        submitForm
    } = useFormik({
        enableReinitialize: true,
        initialValues: {
            name: "",
            sku: "",
            barcode: "",
            brandName: "",
            category: "",
            price: undefined,
            salesPrice: undefined,
            gender: undefined,
            size: "",
            color: "",
        },
        validationSchema: schema,
        onSubmit: (values) => {
            console.log(values.brandName);
            createProduct({
                createProduct: {
                    name: values.name,
                    sku: values.sku,
                    barcode: values.barcode,
                    brandName: values.brandName,
                    category: values.category,
                    price: values.price,
                    salesPrice: values.salesPrice,
                    gender: values.gender,
                    size: values.size,
                    color: values.color,
                }
            })
        }
    })

    useEffect(() => {
        if (isSuccess) {
            navigate("/dashboard/Products");
        }
    }, [isSuccess]);

    return <Grid container spacing={2}>
        <Grid item xs={12}>
            <FormControl fullWidth>
                <TextField label={<Typography>{t("Pages.Dashboard.Products.NewProduct.Name")}</Typography>}
                    name="name"
                    disabled={isLoading}
                    onChange={handleChange}
                    onBlur={handleBlur}
                    helperText={touched.name && errors.name}
                    error={touched.name && errors.name !== undefined}
                    variant="outlined"
                    value={product?.name} />
            </FormControl>
        </Grid>
        <Grid item xs={12}>
            <FormControl fullWidth>
                <TextField label={<Typography>{t("Pages.Dashboard.Products.NewProduct.SKU")}</Typography>}
                    name="sku"
                    disabled={isLoading}
                    onChange={handleChange}
                    onBlur={handleBlur}
                    helperText={touched.sku && errors.sku}
                    error={touched.sku && errors.sku !== undefined}
                    variant="outlined"
                    value={product?.sku} />
            </FormControl>
        </Grid>
        <Grid item xs={12}>
            <FormControl fullWidth>
                <TextField label={<Typography>{t("Pages.Dashboard.Products.NewProduct.Barcode")}</Typography>}
                    name="barcode"
                    disabled={isLoading}
                    onChange={handleChange}
                    onBlur={handleBlur}
                    helperText={touched.barcode && errors.barcode}
                    error={touched.barcode && errors.barcode !== undefined}
                    variant="outlined"
                    value={product?.barcode} />
            </FormControl>
        </Grid>
        <Grid item xs={12}>
            <FormControl fullWidth>
                <TextField label={<Typography>{t("Pages.Dashboard.Products.NewProduct.Brand")}</Typography>}
                    name="brandName"
                    disabled={isLoading}
                    onChange={handleChange}
                    onBlur={handleBlur}
                    helperText={touched.brandName && errors.brandName}
                    error={touched.brandName && errors.brandName !== undefined}
                    variant="outlined"
                    value={product?.brandName} />
            </FormControl>
        </Grid>
        <Grid item xs={12}>
            <FormControl fullWidth>
                <TextField label={<Typography>{t("Pages.Dashboard.Products.NewProduct.Category")}</Typography>}
                    name="category"
                    disabled={isLoading}
                    onChange={handleChange}
                    onBlur={handleBlur}
                    helperText={touched.category && errors.category}
                    error={touched.category && errors.category !== undefined}
                    variant="outlined"
                    value={product?.category} />
            </FormControl>
        </Grid>
        <Grid item xs={12}>
            <FormControl fullWidth>
                <TextField label={<Typography>{t("Pages.Dashboard.Products.NewProduct.Price")}</Typography>}
                    name="price"
                    disabled={isLoading}
                    onChange={handleChange}
                    onBlur={handleBlur}
                    helperText={touched.price && errors.price}
                    error={touched.price && errors.price !== undefined}
                    variant="outlined"
                    value={product?.price} />
            </FormControl>
        </Grid>
        <Grid item xs={12}>
            <FormControl fullWidth>
                <TextField label={<Typography>{t("Pages.Dashboard.Products.NewProduct.SalesPrice")}</Typography>}
                    name="salesPrice"
                    disabled={isLoading}
                    onChange={handleChange}
                    onBlur={handleBlur}
                    helperText={touched.salesPrice && errors.salesPrice}
                    error={touched.salesPrice && errors.salesPrice !== undefined}
                    variant="outlined"
                    value={product?.salesPrice} />
            </FormControl>
        </Grid>
        <Grid item xs={12}>
            <FormControl fullWidth>
                <TextField label={<Typography>{t("Pages.Dashboard.Products.NewProduct.Gender")}</Typography>}
                    name="gender"
                    disabled={isLoading}
                    onChange={handleChange}
                    onBlur={handleBlur}
                    helperText={touched.gender && errors.gender}
                    error={touched.gender && errors.gender !== undefined}
                    variant="outlined"
                    value={product?.gender} />
            </FormControl>
        </Grid>
        <Grid item xs={12}>
            <FormControl fullWidth>
                <TextField label={<Typography>{t("Pages.Dashboard.Products.NewProduct.Size")}</Typography>}
                    name="size"
                    disabled={isLoading}
                    onChange={handleChange}
                    onBlur={handleBlur}
                    helperText={touched.size && errors.size}
                    error={touched.size && errors.size !== undefined}
                    variant="outlined"
                    value={product?.size} />
            </FormControl>
        </Grid>
        <Grid item xs={12}>
            <FormControl fullWidth>
                <TextField label={<Typography>{t("Pages.Dashboard.Products.NewProduct.Color")}</Typography>}
                    name="color"
                    disabled={isLoading}
                    onChange={handleChange}
                    onBlur={handleBlur}
                    helperText={touched.color && errors.color}
                    error={touched.color && errors.color !== undefined}
                    variant="outlined"
                    value={product?.color} />
            </FormControl>
        </Grid>
        <Grid item xs={12}>
            <Button onClick={() => submitForm()}>
                {t("Generic.Forms.Submit")}
            </Button>
        </Grid>
    </Grid>
}