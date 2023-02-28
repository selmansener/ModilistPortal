import { Box, Button, Grid, IconButton, Typography } from "@mui/material";
import { useEffect, useRef, useState } from "react";
import { api, ProductExcelUploadDto } from "../../../store/api";
import { DataGrid, GridCellParams, GridColDef, GridSortModel, GridValueGetterParams } from '@mui/x-data-grid';
import { Pagination, QueryBuilder, SortDirection, SortField, StringFilter, StringFilterOperation } from "dynamic-query-builder-client";
import { useAppDispatch } from "../../../store/hooks";
import format from "date-fns/format";
import { tr } from "date-fns/locale";
import { useNavigate } from "react-router-dom";
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import { useMsal } from "@azure/msal-react";
import { config } from "../../../config";
import axios from "axios";
import { useTranslation } from "react-i18next";

export default function ProductVariantUploadHistory() {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const [queryString, setQueryString] = useState("");
    const { data: queryResult, error, isLoading, isFetching } = api.endpoints.getApiV1ProductQueryVariantUploadHistory.useQueryState({
        dqb: queryString
    });
    const [totalCount, setTotalCount] = useState(0);
    const [pageSize, setPageSize] = useState(25);
    const [currentPage, setCurrentPage] = useState(0);
    const [sortModel, setSortModel] = useState<GridSortModel>([
        {
            field: "createdAt",
            sort: "desc"
        }
    ]);

    const [data, setData] = useState<ProductExcelUploadDto[]>([]);
    const fileInputRef = useRef<HTMLInputElement>(null);
    const [file, setFile] = useState<File | undefined>();
    const { instance: msal } = useMsal();
    const [isUploadLoading, setIsUploadLoading] = useState(false);
    const [isSuccess, setIsSuccess] = useState<boolean | undefined>();
    const { webApi, loginRequest } = config;
    const { cdnExcelTemplates: excelTemplateBaseHost } = config;
    const i18nBase = "Pages.Dashboard.Products.ProductUploadHistory";

    useEffect(() => {
        var sortBy = sortModel.map(x => {
            return new SortField({
                property: x.field,
                by: x.sort == "asc" ? SortDirection.ASC : (x.sort === "desc" ? SortDirection.DESC : SortDirection.NONE)
            })
        });

        const queryBuilder = new QueryBuilder({
            filters: [],
            pagination: new Pagination({
                offset: currentPage * pageSize,
                count: pageSize,
            }),
            sortBy: sortBy,
        });

        var queryString = queryBuilder.build();
        setQueryString(queryString);

        dispatch(api.endpoints.getApiV1ProductQueryVariantUploadHistory.initiate({
            dqb: queryString
        }));

    }, [currentPage, pageSize, sortModel]);

    useEffect(() => {
        if (queryResult?.data) {
            const { count, data } = queryResult.data;
            if (count) {
                setTotalCount(count);
            }

            if (data) {
                setData(data);
            }
        }
    }, [queryResult]);

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
            axios.post(`${webApi}/api/v1/Product/UploadProductExcel`, formData, {
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
        { field: 'id', headerName: 'ID', width: 90 },
        {
            field: 'originalFileName',
            headerName: 'File Name',
            width: 150,
            valueGetter: (params: GridValueGetterParams) => {
                return `${params.row.originalFileName}.${params.row.extension}`
            }
        },
        {
            field: 'fileSize',
            headerName: 'File Size',
            width: 250,
        },
        {
            field: 'createdAt',
            headerName: 'Upload Date',
            width: 250,
            valueGetter: (params: GridValueGetterParams) => {
                return format(new Date(params?.row.createdAt), 'dd.MM.yyyy', { locale: tr })
            }
        },
        {
            field: 'details',
            headerName: 'Actions',
            width: 150,
            renderCell: (params: GridCellParams) => {
                return <Button onClick={() => {
                    navigate(`/dashboard/products/product-upload-history/${params.row.id}`);
                }}>
                    Details
                </Button>
            },
            valueGetter: (params: GridValueGetterParams) => {
                return params.row.id;
            }
        }
    ];

    const templateLink = `${excelTemplateBaseHost}/excel-import/Products.xlsx`;

    return <Grid item container spacing={6}>
        <Grid item xs={12}>
            <Box sx={{ height: 400, width: '100%' }}>
                <DataGrid
                    loading={isLoading || isFetching}
                    rowCount={totalCount}
                    rows={data}
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