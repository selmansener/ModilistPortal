import { Box, Grid } from "@mui/material";
import { useParams } from "react-router-dom";
import { DataGrid, GridCellParams, GridColDef, GridSortModel, GridValueGetterParams } from '@mui/x-data-grid';
import { useEffect, useState } from "react";
import { useAppDispatch } from "../../../store/hooks";
import format from "date-fns/format";
import { tr } from "date-fns/locale";
import { useNavigate } from "react-router-dom";
import { api, QueryProductExcelRowDto } from "../../../store/api";
import { Pagination, QueryBuilder, SortDirection, SortField, StringFilter, StringFilterOperation } from "dynamic-query-builder-client";

export default function UploadHistoryDetails() {
    const { productExcelUploadId } = useParams();

    const dispatch = useAppDispatch();
    const [queryString, setQueryString] = useState("");
    const { data: queryResult, error, isLoading, isFetching } = api.endpoints.getApiV1ProductQueryUploadHistoryDetailsByProductExcelUploadId.useQueryState({
        productExcelUploadId: productExcelUploadId ? parseInt(productExcelUploadId) : 0,
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

    const [data, setData] = useState<QueryProductExcelRowDto[]>([]);

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

        dispatch(api.endpoints.getApiV1ProductQueryUploadHistoryDetailsByProductExcelUploadId.initiate({
            productExcelUploadId: productExcelUploadId ? parseInt(productExcelUploadId) : 0,
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

    const columns: GridColDef[] = [
        {
            field: 'id',
            headerName: 'ID',
        },
        {
            field: 'rowId',
            headerName: 'Satır No.',
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

    return <Grid item container spacing={2}>
        <Grid item xs={12}>
            <Box sx={{ height: 400 }}>
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
    </Grid>
}