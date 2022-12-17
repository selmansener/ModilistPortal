import { Button, FormControl, FormHelperText, Grid, InputLabel, MenuItem, Select, TextField, Typography } from "@mui/material";
import { TenantDto, TenantType, useGetApiV1AddressGetCitiesQuery, useGetApiV1TenantGetQuery, usePostApiV1TenantUpsertMutation } from "../../../store/api";
import { useFormik } from 'formik';
import { useTranslation } from "react-i18next";
import { Cities } from "../../../components/address/Cities";
import { useEffect, useState } from "react";
import { Districts } from "../../../components/address/Districts";
import * as Yup from "yup";
import { validateIdNumber, validateTaxNumber } from "../../../utils/validators";

export default function Tenant() {
    const { t } = useTranslation();
    const { isLoading: getTenantIsLoading, data: initialTenantData, error: getTenantError } = useGetApiV1TenantGetQuery({});
    const { isLoading: getCitiesIsLoading, data: cities, error: getCitiesError } = useGetApiV1AddressGetCitiesQuery({});
    const [upsertTenant, result] = usePostApiV1TenantUpsertMutation();
    const { isLoading: upsertTenantIsLoading, data: upsertTenantData, error: upsertTenantError } = result;
    const isLoading = getTenantIsLoading || upsertTenantIsLoading;
    const [selectedCity, setSelectedCity] = useState<string | undefined>();
    const requiredField = t("FormValidation.RequiredField");

    useEffect(() => {
        if (initialTenantData && cities) {
            const city = cities?.find((x: any) => x.name === initialTenantData.data?.city);
            if (city) {
                setSelectedCity(city.code);
            }
        }
    }, [initialTenantData, cities]);

    const tenantTypes: TenantType[] = [
        "Individual",
        "LimitedLiability",
        "IncorporatedCompany"
    ]

    const schema = Yup.object({
        name: Yup.string().required(requiredField),
        city: Yup.string().required(requiredField),
        district: Yup.string().required(requiredField),
        email: Yup.string().email(t("FormValidation.Email")).required(requiredField),
        phone: Yup.string().required(requiredField),
        type: Yup.string().notOneOf(["None"], requiredField).required(requiredField),
        taxOffice: Yup.string().required(requiredField),
        taxNumber: Yup.string().when("type", {
            is: (type: any) => type != "Individual",
            then: Yup.string().required(requiredField).test({
                test: (value) => {
                    return validateTaxNumber(value);
                },
                message: t("FormValidation.InvalidTaxNumber")
            }),
            otherwise: Yup.string().optional()
        }),
        tckn: Yup.string().when("type", {
            is: (type: any) => type == "Individual",
            then: Yup.string().required(requiredField).test({
                test: (value) => {
                    return validateIdNumber(value);
                },
                message: t("FormValidation.InvalidIdNumber")
            }),
            otherwise: Yup.string().optional()
        })
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
        initialValues: (upsertTenantData?.data ?? initialTenantData?.data ?? {
            name: "",
            city: "",
            district: "",
            email: "",
            phone: "",
            taxNumber: "",
            taxOffice: "",
            tckn: "",
            type: "None"
        }) as TenantDto,
        validationSchema: schema,
        onSubmit: (values) => {
            console.log(values);
            upsertTenant({
                upsertTenant: {
                    ...values
                }
            })
        }
    });

    return <Grid container spacing={2}>
        <Grid item xs={4}>
            <FormControl fullWidth error={touched.type && errors.type !== undefined}>
                <InputLabel id={`tenantType-label`}>{t(`Generic.TenantTypes.Title`)}</InputLabel>
                <Select
                    disabled={isLoading}
                    name={'type'}
                    labelId={`tenantType-label`}
                    id={'tenantType'}
                    value={tenant.type}
                    label={t(`Generic.TenantTypes.Title`)}
                    onChange={handleChange}
                    onBlur={handleBlur}
                >
                    <MenuItem disabled value="None">
                        <em>{t('Generic.Forms.Select')}</em>
                    </MenuItem>
                    {tenantTypes.map(companyType => {
                        return <MenuItem key={companyType} value={companyType.toString()}>{t(`Generic.TenantTypes.${companyType}`)}</MenuItem>
                    })}
                </Select>
                <FormHelperText>{touched.type && errors?.type}</FormHelperText>
            </FormControl>
        </Grid>
        <Grid item xs={4}>
            <FormControl fullWidth>
                <Cities
                    value={tenant?.city}
                    error={touched.city && errors.city !== undefined}
                    helperText={touched.city && errors?.city}
                    onChange={(city) => {
                        setSelectedCity(city.code);

                        setFieldValue("city", city.name);
                    }}
                    onBlur={(city) => {
                        setSelectedCity(city.code);

                        setFieldValue("city", city.name);

                        setFieldTouched("city");
                    }}
                />
            </FormControl>
        </Grid>
        <Grid item xs={4}>
            <FormControl fullWidth>
                <Districts
                    selectedCity={selectedCity}
                    error={touched.district && errors.district !== undefined}
                    helperText={touched.district && errors?.district}
                    value={tenant?.district}
                    onChange={(district) => {
                        setFieldValue("district", district.name);
                    }}
                    onBlur={(district) => {
                        setFieldValue("district", district.name);

                        setFieldTouched("district");
                    }}
                />
            </FormControl>
        </Grid>
        <Grid item xs={6}>
            <FormControl fullWidth>
                <TextField label={<Typography>{t(tenant?.type === "Individual" ? 'Generic.PersonalInfo.FullName' : 'Generic.Tenant.Name')}</Typography>}
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
        <Grid item xs={6}>
            {tenant?.type === "Individual" ?
                <FormControl fullWidth>
                    <TextField label={<Typography>{t('Generic.PersonalInfo.IdNumber')}</Typography>}
                        name="tckn"
                        disabled={isLoading}
                        onChange={handleChange}
                        onBlur={handleBlur}
                        helperText={touched.tckn && errors.tckn}
                        error={touched.tckn && errors.tckn !== undefined}
                        variant="outlined"
                        value={tenant?.tckn} />
                </FormControl>
                :
                <FormControl fullWidth>
                    <TextField label={<Typography>{t('Generic.Tenant.TaxNumber')}</Typography>}
                        name="taxNumber"
                        disabled={isLoading}
                        onChange={handleChange}
                        onBlur={handleBlur}
                        helperText={touched.taxNumber && errors.taxNumber}
                        error={touched.taxNumber && errors.taxNumber !== undefined}
                        variant="outlined"
                        value={tenant?.taxNumber} />
                </FormControl>
            }
        </Grid>
        <Grid item xs={4}>
            <FormControl fullWidth>
                <TextField label={<Typography>{t('Generic.Tenant.TaxOffice')}</Typography>}
                    name="taxOffice"
                    disabled={isLoading}
                    onChange={handleChange}
                    onBlur={handleBlur}
                    helperText={touched.taxOffice && errors.taxOffice}
                    error={touched.taxOffice && errors.taxOffice !== undefined}
                    variant="outlined"
                    value={tenant?.taxOffice} />
            </FormControl>
        </Grid>
        <Grid item xs={4}>
            <FormControl fullWidth>
                <TextField label={<Typography>{t('Generic.Tenant.Phone')}</Typography>}
                    name="phone"
                    disabled={isLoading}
                    onChange={handleChange}
                    onBlur={handleBlur}
                    helperText={touched.phone && errors.phone}
                    error={touched.phone && errors.phone !== undefined}
                    variant="outlined"
                    value={tenant?.phone} />
            </FormControl>
        </Grid>
        <Grid item xs={4}>
            <FormControl fullWidth>
                <TextField label={<Typography>{t('Generic.Tenant.Email')}</Typography>}
                    name="email"
                    disabled={isLoading}
                    onChange={handleChange}
                    onBlur={handleBlur}
                    helperText={touched.email && errors.email}
                    error={touched.email && errors.email !== undefined}
                    variant="outlined"
                    value={tenant?.email} />
            </FormControl>
        </Grid>
        <Grid item xs={12} display="flex" justifyContent="flex-end">
            <Button variant="contained" color="primary"
                onClick={() => {
                    submitForm();
                }}
            >
                {t("Generic.Forms.Submit")}
            </Button>
        </Grid>
    </Grid>
}