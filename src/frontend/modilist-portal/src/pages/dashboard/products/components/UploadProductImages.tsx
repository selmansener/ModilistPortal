import { Button, Grid, IconButton, useTheme } from "@mui/material";
import { useRef, useState, useEffect } from "react";
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import HighlightOffIcon from '@mui/icons-material/HighlightOff';
import React from "react";
import { usePostApiV1ProductByProductIdAddImagesMutation } from "../../../../store/api";
import axios from "axios";
import { config } from "../../../../config";
import { useMsal } from "@azure/msal-react";

export interface UploadProductImagesProps {
    productId: number;
}

export function UploadProductImages(props: UploadProductImagesProps) {
    const { productId } = props;
    const fileInputRef = useRef<HTMLInputElement>(null);
    const [files, setFiles] = useState<File[]>();
    const [isLoading, setIsLoading] = useState(false);
    const [isSuccess, setIsSuccess] = useState<boolean | undefined>();
    const { webApi, loginRequest } = config;
    const { instance: msal } = useMsal();


    const reset = () => {
        if (fileInputRef?.current?.value) {
            fileInputRef.current.value = "";
        }
        setFiles([]);
    }

    useEffect(() => {
        if (isSuccess) {
            reset();
        }
    }, [isSuccess]);

    return (productId !== 0 ? <Grid item container spacing={2}>
        <Grid item xs={1} sx={{
            display: 'flex',
            justifyContent: 'right'
        }}>
            <IconButton
                disabled={isLoading}
                color="primary" aria-label="upload picture" component="label">
                <input
                    ref={fileInputRef}
                    hidden
                    onChange={(e) => {
                        if (e.target.files) {
                            const selectedFiles = Array.from(e.target.files);
                            setFiles(selectedFiles);
                        }
                    }}
                    multiple
                    accept="image/*"
                    type="file" />
                <CloudUploadIcon />
            </IconButton>
        </Grid>
        <Grid item container spacing={2} xs={8}>
            {files && <SelectedFiles isLoading={isLoading} files={files} onRemove={(index) => {
                files.splice(index, 1);
                setFiles([
                    ...files
                ]);
            }} />}
        </Grid>
        <Grid item xs={3}>
            <Button
                disabled={isLoading}
                onClick={() => {
                    if (!files) {
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
                        for (let i = 0; i < files.length; i++) {
                            const file = files[i];
                            formData.append("files", file);
                        }

                        axios.post(`${webApi}/api/v1/Product/${productId}/AddImages`, formData, {
                            headers: {
                                contentType: "multipart/formdata",
                                authorization: `Bearer ${tokenResponse.accessToken}`
                            }
                        }).then((response) => {
                            console.log(response.data);
                            setIsLoading(false);
                            setIsSuccess(true);
                        }).catch((err) => {
                            setIsLoading(false);
                        });
                    }).catch(err => {
                        setIsSuccess(false);
                    });
                }}>
                Upload
            </Button>
            <Button
                disabled={isLoading}
                onClick={() => {
                    reset();
                }}>
                Clear All
            </Button>
        </Grid>
    </Grid> : <></>)
}

interface SelectedFilesProps {
    isLoading: boolean;
    files: File[];
    onRemove: (index: number) => void;
}

function SelectedFiles(props: SelectedFilesProps) {
    const { isLoading, files, onRemove } = props;

    const selectedFiles: React.ReactNode[] = [];

    for (let i = 0; i < files.length; i++) {
        const file = files[i];

        selectedFiles.push(
            <React.Fragment key={i}>
                <Grid item xs={11}>
                    {file.name}
                </Grid>
                <Grid item xs={1}>
                    <IconButton
                        disabled={isLoading}
                        onClick={() => onRemove(i)}>
                        <HighlightOffIcon color="error" />
                    </IconButton>
                </Grid>
            </React.Fragment>
        );
    }

    return <React.Fragment>
        {selectedFiles}
    </React.Fragment>;
}