import { Box, Grid, Typography } from "@mui/material";
import { Link, useLocation } from "react-router-dom";
import { api, QueryProductDto, QueryProductExcelRowDto } from "../../../../store/api";
import { LogicalOperator, BooleanFilterOperation, Pagination, QueryBuilder, SortDirection, SortField, StringFilter, StringFilterOperation } from "dynamic-query-builder-client";
import { DataGrid, GridCellParams, GridColDef, GridSortModel, GridValueGetterParams } from '@mui/x-data-grid';
import { useEffect, useState } from "react";
import { useAppDispatch } from "../../../../store/hooks";
import format from "date-fns/format";
import { tr } from "date-fns/locale";
import { Filters, ModelDefinition, QueryFilter } from "../../../../components/filters/Filters";

export default function ProductGrid() {
    const dispatch = useAppDispatch();
    const [queryString, setQueryString] = useState("");
    const location = useLocation();
    const [totalCount, setTotalCount] = useState(0);
    const [pageSize, setPageSize] = useState(25);
    const [currentPage, setCurrentPage] = useState(0);
    const [sortModel, setSortModel] = useState<GridSortModel>([
        {
            field: "createdAt",
            sort: "desc"
        }
    ]);
    const [queryFilters, setQueryFilters] = useState<QueryFilter[]>([]);
    const { data: queryResult, error, isLoading, isFetching } = api.endpoints.getApiV1ProductQuery.useQueryState({
        dqb: queryString
    });

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

    const [data, setData] = useState<QueryProductDto[]>([]);

    useEffect(() => {
        var sortBy = sortModel.map(x => {
            return new SortField({
                property: x.field,
                by: x.sort === "asc" ? SortDirection.ASC : (x.sort === "desc" ? SortDirection.DESC : SortDirection.NONE)
            })
        });

        const queryBuilder = new QueryBuilder({
            filters: queryFilters,
            pagination: new Pagination({
                offset: currentPage * pageSize,
                count: pageSize,
            }),
            sortBy: sortBy,
        });

        var queryString = `&${queryBuilder.build()}`;
        setQueryString(encodeURI(queryString));

        dispatch(api.endpoints.getApiV1ProductQuery.initiate({
            dqb: queryString
        }));

    }, [currentPage, pageSize, sortModel, productState, queryFilters]);

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

    const handleFiltersChange = (queryFilters: QueryFilter[]) => {
        setQueryFilters(queryFilters);
    }

    const productModel: ModelDefinition = {
        properties: [
            {
                name: "name",
                title: "Ürün Adı",
                type: "string"
            },
            {
                name: "sku",
                title: "Stok Kodu",
                type: "string"
            },
            {
                name: "barcode",
                title: "Barkod",
                type: "string"
            },
            {
                name: "brand",
                title: "Marka",
                type: "string"
            },
            {
                name: "category",
                title: "Kategori",
                type: "string"
            },
            {
                name: "gender",
                title: "Cinsiyet",
                type: "enum",
                enumValues: [
                    {
                        title: "Erkek",
                        value: "Male"
                    },
                    {
                        title: "Kadın",
                        value: "Female"
                    },
                    {
                        title: "Unisex",
                        value: "Unisex"
                    }
                ]
            },
            {
                name: "size",
                title: "Beden",
                type: "string"
            },
            {
                name: "price",
                title: "Fiyat",
                type: "number"
            },
        ]
    }

    return <Grid item container spacing={2}>
        <Grid item xs={12}>
            <Filters model={productModel} onApply={handleFiltersChange} />
        </Grid>
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