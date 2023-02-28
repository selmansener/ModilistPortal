import { Button, Grid, IconButton } from "@mui/material";
import { useEffect, useRef, useState } from "react";
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import { usePostApiV1ProductUploadProductExcelMutation } from "../../../store/api";
import axios from "axios";
import { useMsal } from "@azure/msal-react";
import { config } from "../../../config";

export default function NewUpload() {
    const fileInputRef = useRef<HTMLInputElement>(null);
    const [file, setFile] = useState<File | undefined>();
    const { instance: msal } = useMsal();
    const [isLoading, setIsLoading] = useState(false);
    const [isSuccess, setIsSuccess] = useState<boolean | undefined>();
    const { webApi, loginRequest } = config;

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
        
        if (!file) {
            return;
        }

        const accounts = msal.getAllAccounts();
        const account = accounts.length > 0 ? accounts[0] : null;

        if (account === null) {

            return;
        }

        setIsLoading(true);

        msal.acquireTokenSilent({
            ...loginRequest,
            account: account
        }).then((tokenResponse) => {

            const formData = new FormData();

            formData.append("file", file);

            axios.post(`${webApi}/api/v1/Product/UploadProductExcel`, formData, {
                headers: {
                    contentType: "multipart/formdata",
                    authorization: `Bearer ${tokenResponse.accessToken}`
                }
            }).then((response) => {
                console.log(response);
                setIsLoading(false);
                setIsSuccess(true);
            }).catch((err) => {
                setIsLoading(false);
            });
        }).catch(err => {
            setIsSuccess(false);
        });
    }

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
            <Button
                disabled={isLoading}
                onClick={handleOnClick}
            >
                Upload
            </Button>
        </Grid>
    </Grid>
}