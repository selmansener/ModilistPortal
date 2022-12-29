import { Box, Grid } from "@mui/material";
import { useEffect, useState } from "react";
import { ProductExcelUploadDto, useGetApiV1ProductQueryUploadHistoryQuery } from "../../../store/api";
import { DataGrid, GridColDef, GridValueGetterParams } from '@mui/x-data-grid';

export default function UploadHistory() {
    const { isLoading, data: queryResult, error } = useGetApiV1ProductQueryUploadHistoryQuery({});
    const [totalCount, setTotalCount] = useState(0);
    
    const [data, setData] = useState<ProductExcelUploadDto[]>([]);
    const rowsPerPage = 2;

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
    ];

    return <Grid item container spacing={2}>
        <Grid item xs={12}>
            <Box sx={{ height: 400, width: '100%' }}>
                <DataGrid
                    rows={data}
                    columns={columns}
                    pageSize={2}
                    onPageChange={(newPage) => console.log(newPage)}
                    paginationMode="server"
                    // onPageSizeChange={(newPageSize) => console.log(newPageSize)}
                    rowsPerPageOptions={[2]}
                    disableSelectionOnClick
                />
            </Box>
        </Grid>
    </Grid>
}