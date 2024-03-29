import { Box, Button, Grid, IconButton, Typography } from "@mui/material";
import { DataGrid, GridCellParams, GridColDef, GridSortModel, GridValueGetterParams } from "@mui/x-data-grid";
import { LogicalOperator, BooleanFilterOperation, Pagination, QueryBuilder, SortDirection, SortField, StringFilter, StringFilterOperation } from "dynamic-query-builder-client";
import { useEffect, useRef, useState } from "react";
import { Link, useLocation, useNavigate, useParams } from "react-router-dom";
import Loading from "../../../components/loading/loading";
import { api, QueryProductDto, useGetApiV1ProductGetByProductIdQuery, useGetApiV1ProductQueryProductVariantsByProductIdQuery } from "../../../store/api";
import { useAppDispatch } from "../../../store/hooks";
import { UploadProductImages } from "./components/UploadProductImages";
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import format from "date-fns/format";
import { tr } from "date-fns/locale";
import { t } from "i18next";
import { useMsal } from "@azure/msal-react";
import { config } from "../../../config";
import { useTranslation } from "react-i18next";
import axios from "axios";

export default function ProductDetails() {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const [queryString, setQueryString] = useState("");
    const location = useLocation();
    const [totalCount, setTotalCount] = useState(0);
    const [pageSize, setPageSize] = useState(25);
    const [currentPage, setCurrentPage] = useState(0);
    const fileInputRef = useRef<HTMLInputElement>(null);
    const [file, setFile] = useState<File | undefined>();
    const { instance: msal } = useMsal();
    const [isUploadLoading, setIsUploadLoading] = useState(false);
    const [isSuccess, setIsSuccess] = useState<boolean | undefined>();
    const { webApi, loginRequest } = config;
    const { cdnExcelTemplates: excelTemplateBaseHost } = config;
    const i18nBase = "Pages.Dashboard.Products.ProductUploadHistory";
    const [sortModel, setSortModel] = useState<GridSortModel>([
        {
            field: "createdAt",
            sort: "desc"
        }
    ]);
    const { productId } = useParams();
    const { data, isLoading, isFetching, error } = useGetApiV1ProductGetByProductIdQuery({
        productId: productId ? parseInt(productId) : 0
    });
    const product = data?.data;
    const { data: variantsData, isLoading: isLoadingVariants, isFetching: isFetchingVariants, error: errorVariants } = useGetApiV1ProductQueryProductVariantsByProductIdQuery({
        productId: productId ? parseInt(productId) : 0
    });

    const [dataVariants, setDataVariants] = useState<QueryProductDto[]>([]);

    const productStates = [
        "",
        "active",
        "in-review",
        "out-of-stock",
        "missing-info",
        "rejected",
        "passive",
    ];

    const productStateMaps = {
        "active": "Available",
        "in-review": "InReview",
        "out-of-stock": "OutOfStock",
        "missing-info": "MissingInfo",
        "rejected": "Rejected",
        "passive": "Passive",
    };

    const pathVariables = location.pathname.split('/');
    const last = pathVariables[pathVariables.length - 1];
    const productState = productStates.indexOf(last) > -1 ? last : "";
    const templateLink = `${excelTemplateBaseHost}/excel-import/SingleProductVariantImport.xlsx`;

    useEffect(() => {
        if (variantsData?.data) {
            const { count, data } = variantsData.data;
            if (count) {
                setTotalCount(count);
            }

            if (data) {
                setDataVariants(data);
            }
        }
    }, [variantsData]);

    useEffect(() => {
        var sortBy = sortModel.map(x => {
            return new SortField({
                property: x.field,
                by: x.sort == "asc" ? SortDirection.ASC : (x.sort === "desc" ? SortDirection.DESC : SortDirection.NONE)
            })
        });

        const filters = productState === "" || productState === undefined ? [] : [
            new StringFilter({
                op: StringFilterOperation.Equals,
                property: "state",
                value: productStateMaps[productState as keyof typeof productStateMaps],
            })
        ]

        const queryBuilder = new QueryBuilder({
            filters: filters,
            pagination: new Pagination({
                offset: currentPage * pageSize,
                count: pageSize,
            }),
            sortBy: sortBy,
        });

        var queryString = `&${queryBuilder.build()}`;
        setQueryString(queryString);

        dispatch(api.endpoints.getApiV1ProductQuery.initiate({
            dqb: queryString
        }));

    }, [currentPage, pageSize, sortModel, productState]);

    const reset = () => {
        if (fileInputRef?.current?.value) {
            fileInputRef.current.value = "";
        }
        setFile(undefined);
    }

    useEffect(() => {
        if (isSuccess) {
            reset();
        }
    }, [isSuccess]);

    const handleOnClick = () => {
        console.log("clicked")
        if (!file) {
            return;
        }

        const accounts = msal.getAllAccounts();
        const account = accounts.length > 0 ? accounts[0] : null;

        if (account === null) {

            return;
        }

        setIsUploadLoading(true);

        msal.acquireTokenSilent({
            ...loginRequest,
            account: account
        }).then((tokenResponse) => {

            const formData = new FormData();

            formData.append("file", file);
            console.log("file to upload", file)
            axios.post(`${webApi}/api/v1/Product/UploadProductVariantByProductIdExcel/${productId}`, formData, {
                headers: {
                    contentType: "multipart/formdata",
                    authorization: `Bearer ${tokenResponse.accessToken}`
                }
            }).then((response) => {
                console.log(response);
                setIsUploadLoading(false);
                setIsSuccess(true);
            }).catch((err) => {
                console.log(err)
                setIsUploadLoading(false);
            });
        }).catch(err => {
            setIsSuccess(false);
        });
    }

    const columns: GridColDef[] = [
        {
            field: 'id',
            headerName: 'ID',
            renderCell: (params: GridCellParams) => {
                return <Link to={`/dashboard/products/${params.row.id}`}>
                    {params.row.id}
                </Link>
            }
        },
        {
            field: 'name',
            headerName: 'Ürün Adı',

        },
        {
            field: 'sku',
            headerName: 'Stok Kodu',

        },
        {
            field: 'barcode',
            headerName: 'Barkod',

        },
        {
            field: 'brand',
            headerName: 'Marka',

        },
        {
            field: 'category',
            headerName: 'Kategori',

        },
        {
            field: 'gender',
            headerName: 'Cinsiyet',
        },
        {
            field: 'color',
            headerName: 'Renk',
        },
        {
            field: 'size',
            headerName: 'Beden',
        },
        {
            field: 'state',
            headerName: 'Durum',
        },
        {
            field: 'price',
            headerName: 'Fiyat',
        },
        {
            field: 'salesPrice',
            headerName: 'Satış Fiyatı',
        },
        {
            field: 'createdAt',
            headerName: 'Oluşturulma Tarihi',

            valueGetter: (params: GridValueGetterParams) => {
                return format(new Date(params?.row.createdAt), 'dd.MM.yyyy', { locale: tr })
            }
        },
    ];


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
        <Grid item xs={12}>
            <Box sx={{ height: 400, width: '100%' }}>
                <DataGrid
                    loading={isLoading || isFetching}
                    rowCount={totalCount}
                    rows={dataVariants}
                    columns={columns}
                    pageSize={pageSize}
                    onPageChange={(newPage) => setCurrentPage(newPage)}
                    paginationMode="server"
                    onPageSizeChange={(newPageSize) => setPageSize(newPageSize)}
                    rowsPerPageOptions={[10, 25, 50, 100]}
                    disableSelectionOnClick
                    sortModel={sortModel}
                    onSortModelChange={(model, details) => {
                        setSortModel(model);
                    }}
                />
            </Box>
        </Grid>
        <Grid item container xs={12}>
            <Grid item container xs={12} display="flex" justifyContent="center">
                <Box display="flex" flexDirection="column" alignItems="center" justifyContent="center"
                    padding="50px"
                    sx={{
                        border: '3px dashed grey',
                        borderRadius: '10px'
                    }}
                >
                    <Typography variant="h6" align="center">
                        {t(`${i18nBase}.UploadDescription1`)}
                        <a
                            onClick={e => e.stopPropagation()} href={templateLink} target="_blank" download
                        >
                            {t(`${i18nBase}.UploadDescription2`)}
                        </a>
                        {t(`${i18nBase}.UploadDescription3`)}
                    </Typography>
                    <Grid item xs={12}>
                        <IconButton
                            color="primary" aria-label="upload picture" component="label" sx={{
                                fontSize: '100px'
                            }}>
                            <input
                                ref={fileInputRef}
                                hidden
                                onChange={(e) => {

                                    if (e.target.files && e.target.files.length > 0) {
                                        setFile(e.target.files[0]);
                                    }
                                }}
                                accept="application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                                type="file" />
                            <CloudUploadIcon sx={{
                                fontSize: '150px'
                            }} />
                        </IconButton>
                    </Grid>
                    <Grid item xs={12}>
                        <Button
                            disabled={isUploadLoading}
                            onClick={handleOnClick}
                            color="primary"
                            variant="contained"
                            sx={{
                                fontSize: '20px'
                            }}
                        >
                            Upload
                        </Button>
                    </Grid>
                </Box>
            </Grid>
        </Grid>
    </Grid>
}