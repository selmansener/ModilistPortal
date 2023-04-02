import { Box, Grid, Typography } from "@mui/material";
import { Link, useLocation, useParams, useSearchParams } from "react-router-dom";
import { api, QueryProductDto, QueryProductExcelRowDto } from "../../../../store/api";
import { LogicalOperator, BooleanFilterOperation, Pagination, QueryBuilder, SortDirection, SortField, StringFilter, StringFilterOperation, DateFilter, NumericFilter, BooleanFilter, NumericFilterOperation, DateFilterOperation } from "dynamic-query-builder-client";
import { DataGrid, GridCellParams, GridColDef, GridSortDirection, GridSortModel, GridValueGetterParams } from '@mui/x-data-grid';
import { useEffect, useState } from "react";
import { useAppDispatch } from "../../../../store/hooks";
import format from "date-fns/format";
import { tr } from "date-fns/locale";
import { Filters, ModelDefinition, QueryFilter } from "../../../../components/filters/Filters";

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

export default function ProductGrid() {
    const [searchParams, setSearchParams] = useSearchParams();
    const dispatch = useAppDispatch();
    const [queryString, setQueryString] = useState(searchParams.get("dqb") ?? "");
    const location = useLocation();
    const [totalCount, setTotalCount] = useState(0);
    const [pageSize, setPageSize] = useState(25);
    const [currentPage, setCurrentPage] = useState(0);
    const [pagination, setPagination] = useState<Pagination>(new Pagination({
        offset: 0,
        count: 25
    }));
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
    const pathVariables = location.pathname.split('/');
    const last = pathVariables[pathVariables.length - 1];
    const productState = productStates.indexOf(last) > -1 ? last : "";

    const [data, setData] = useState<QueryProductDto[]>([]);

    useEffect(() => {
        const dqb = searchParams.get("dqb");

        if (dqb === null || dqb === "") {
            const sortBy = new SortField({
                property: "createdAt",
                by: SortDirection.DESC
            });

            const pagination = new Pagination({
                offset: 0,
                count: 25
            });

            const queryBuilder = new QueryBuilder({
                filters: [],
                pagination,
                sortBy
            });

            var queryString = encodeURI(`&${queryBuilder.build()}`);
            setQueryString(queryString);

            searchParams.set("dqb", queryString);
            setSearchParams(searchParams);
        }
        else {
            const queryElements = dqb.split("&").filter(el => el !== "");
            const sortString = queryElements.find(el => el.startsWith("s="));
            const offsetString = queryElements.find(el => el.startsWith("offset="));
            const countString = queryElements.find(el => el.startsWith("count="));

            queryElements?.splice(queryElements.indexOf(sortString as string), 1);
            queryElements?.splice(queryElements.indexOf(offsetString as string), 1);
            queryElements?.splice(queryElements.indexOf(countString as string), 1);

            console.log("queryElements", queryElements);

            if (queryElements.length % 3 === 0) {
                const queryFilterParameters = [];
                for (let i = 1; i <= queryElements.length; i++) {

                    if (i % 3 === 0) {
                        const operation = queryElements[i - 3];
                        const property = queryElements[i - 2];
                        const value = queryElements[i - 1];
                        queryFilterParameters.push({
                            o: operation.replace("o=", ""),
                            p: property.replace("p=", ""),
                            v: value.replace("v=", "")
                        });
                    }
                }

                const newFilters = [];
                for (let i = 0; i < queryFilterParameters.length; i++) {
                    const element = queryFilterParameters[i];
                    if (typeof element.v === "string") {
                        newFilters.push(new StringFilter({
                            op: element.o as StringFilterOperation,
                            property: element.p as string,
                            value: element.v
                        }));
                    }
                    else if (typeof element.v === "number") {
                        newFilters.push(new NumericFilter({
                            op: element.o as NumericFilterOperation,
                            property: element.p as string,
                            value: element.v
                        }));
                    }
                    else if (typeof element.v === "boolean") {
                        newFilters.push(new BooleanFilter({
                            op: element.o as BooleanFilterOperation,
                            property: element.p as string,
                            value: element.v
                        }));
                    }
                    else {
                        // TODO: find a solid way to check if typeof element value is Date and throw exception in else.
                        newFilters.push(new DateFilter({
                            op: element.o as DateFilterOperation,
                            property: element.p as string,
                            value: element.v
                        }));
                    }
                }

                setQueryFilters(newFilters);
            }
        }
    }, []);

    useEffect(() => {
        const dqb = searchParams.get("dqb");

        if (dqb !== null && dqb !== "") {
            const queryElements = dqb.split("&").filter(el => el !== "");
            const sortString = queryElements.find(el => el.startsWith("s="));
            const sortElements = sortString?.replace("s=", "")?.split(",");
            const offsetString = queryElements.find(el => el.startsWith("offset="));
            const countString = queryElements.find(el => el.startsWith("count="));

            const newPagination = new Pagination({
                offset: parseInt(offsetString?.replace("offset=", "") ?? "0"),
                count: parseInt(countString?.replace("count=", "") ?? "25")
            });

            let sortBy;
            if (sortElements && sortElements.length > 0) {
                sortBy = new SortField({
                    property: sortElements.at(0) as string,
                    by: sortElements?.at(1) as SortDirection
                });
            }

            queryElements?.splice(queryElements.indexOf(sortString as string), 1);
            queryElements?.splice(queryElements.indexOf(offsetString as string), 1);
            queryElements?.splice(queryElements.indexOf(countString as string), 1);

            if (queryElements.length % 3 === 0) {
                const queryFilterParameters = [];
                for (let i = 1; i <= queryElements.length; i++) {

                    if (i % 3 === 0) {
                        const operation = queryElements[i - 3];
                        const property = queryElements[i - 2];
                        const value = queryElements[i - 1];
                        queryFilterParameters.push({
                            o: operation.replace("o=", ""),
                            p: property.replace("p=", ""),
                            v: value.replace("v=", "")
                        });
                    }
                }

                const newFilters = [];
                for (let i = 0; i < queryFilterParameters.length; i++) {
                    const element = queryFilterParameters[i];
                    if (typeof element.v === "string") {
                        newFilters.push(new StringFilter({
                            op: element.o as StringFilterOperation,
                            property: element.p as string,
                            value: element.v
                        }));
                    }
                    else if (typeof element.v === "number") {
                        newFilters.push(new NumericFilter({
                            op: element.o as NumericFilterOperation,
                            property: element.p as string,
                            value: element.v
                        }));
                    }
                    else if (typeof element.v === "boolean") {
                        newFilters.push(new BooleanFilter({
                            op: element.o as BooleanFilterOperation,
                            property: element.p as string,
                            value: element.v
                        }));
                    }
                    else {
                        // TODO: find a solid way to check if typeof element value is Date and throw exception in else.
                        newFilters.push(new DateFilter({
                            op: element.o as DateFilterOperation,
                            property: element.p as string,
                            value: element.v
                        }));
                    }
                }

                const queryBuilder = new QueryBuilder({
                    filters: newFilters,
                    pagination: newPagination,
                    sortBy
                });

                //setQueryFilters(newFilters);

                var queryString = encodeURI(`&${queryBuilder.build()}`);
                setQueryString(queryString);

                dispatch(api.endpoints.getApiV1ProductQuery.initiate({
                    dqb: queryString
                }));
            }
        }
    }, [searchParams]);

    useEffect(() => {
        if (productState !== "") {

            const existingFilter = queryFilters.find(filter => filter.property === "State");

            if (!existingFilter) {
                const filters = [
                    ...queryFilters,
                    new StringFilter({
                        op: StringFilterOperation.Equals,
                        property: "State",
                        value: productStateMaps[productState as keyof typeof productStateMaps]
                    })
                ];

                setQueryFilters(filters);
            }
            else {
                if (productStateMaps[productState as keyof typeof productStateMaps] !== existingFilter?.value) {
                    existingFilter.value = productStateMaps[productState as keyof typeof productStateMaps];
                    setQueryFilters([
                        ...queryFilters
                    ]);
                }
            }
        }
        else {
            const existingFilter = queryFilters.find(filter => filter.property === "State");
            if (existingFilter) {
                queryFilters.splice(queryFilters.indexOf(existingFilter, 1));
                setQueryFilters([
                    ...queryFilters
                ]);
            }
        }
    }, [productState, queryFilters])

    useEffect(() => {
        var sortBy = sortModel.map(x => {
            return new SortField({
                property: x.field,
                by: x.sort === "asc" ? SortDirection.ASC : (x.sort === "desc" ? SortDirection.DESC : SortDirection.NONE)
            })
        });

        const queryBuilder = new QueryBuilder({
            filters: queryFilters,
            pagination,
            sortBy: sortBy,
        });

        var queryString = encodeURI(`&${queryBuilder.build()}`);
        setQueryString(queryString);

        searchParams.set("dqb", queryString);
        setSearchParams(searchParams);

    }, [pagination, sortModel, queryFilters]);

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

    const handleFiltersChange = (queryFilters: QueryFilter[]) => {
        setQueryFilters(queryFilters);
    }

    return <Grid item container spacing={2}>
        <Grid item xs={12}>
            <Filters model={productModel} filters={queryFilters.filter(qf => qf.property !== "State")} onApply={handleFiltersChange} />
        </Grid>
        <Grid item xs={12}>
            <Box sx={{ height: 400 }}>
                <DataGrid
                    loading={isLoading || isFetching}
                    rowCount={totalCount}
                    rows={data}
                    columns={columns}
                    pageSize={pagination.count}
                    onPageChange={(newPage) => {

                        setPagination(new Pagination({
                            offset: newPage * pagination.count,
                            count: pagination.count
                        }))

                        // setCurrentPage(newPage);
                    }}
                    paginationMode="server"
                    onPageSizeChange={(newPageSize) => {
                        
                        setPagination(new Pagination({
                            offset: pagination.currentPage * newPageSize,
                            count: newPageSize
                        }))

                        // setPageSize(newPageSize)
                    }}
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