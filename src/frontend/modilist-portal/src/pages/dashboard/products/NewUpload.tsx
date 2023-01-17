import { Button, Grid, IconButton } from "@mui/material";
import { useRef, useState } from "react";
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import { usePostApiV1ProductUploadProductExcelMutation } from "../../../store/api";

export default function NewUpload() {
    const fileInputRef = useRef<HTMLInputElement>(null);
    const [file, setFile] = useState<File | undefined>();
    const [uploadExcel, result] = usePostApiV1ProductUploadProductExcelMutation();

    return <Grid item container spacing={2}>
        <Grid item xs={12}>

            <IconButton
                color="primary" aria-label="upload picture" component="label">
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
                <CloudUploadIcon />
            </IconButton>

        </Grid>
        <Grid item xs={12}>
            <Button onClick={() => {
                // TODO: use axios
                uploadExcel({
                    body: {
                        file: file
                    }
                })
            }}>
                Upload
            </Button>
        </Grid>
    </Grid>
}