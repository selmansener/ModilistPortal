import { Button, Grid, IconButton, useTheme } from "@mui/material";
import { useRef, useState, useEffect } from "react";
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import HighlightOffIcon from '@mui/icons-material/HighlightOff';
import React from "react";
import { usePostApiV1ProductByProductIdAddImagesMutation } from "../../../../store/api";

export interface UploadProductImagesProps {
    productId: number;
}

export function UploadProductImages(props: UploadProductImagesProps) {
    const { productId } = props;
    const fileInputRef = useRef<HTMLInputElement>(null);
    const [files, setFiles] = useState<File[]>();
    const [addImages, result] = usePostApiV1ProductByProductIdAddImagesMutation();

    const reset = () => {
        if (fileInputRef?.current?.value) {
            fileInputRef.current.value = "";
        }
        setFiles([]);
    }

    useEffect(() => {
        if (result.isSuccess) {
            reset();
        }
    }, [result]);

    return (productId !== 0 ? <Grid item container spacing={2}>
        <Grid item xs={1} sx={{
            display: 'flex',
            justifyContent: 'right'
        }}>
            <IconButton
                disabled={result.isLoading}
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
            {files && <SelectedFiles isLoading={result.isLoading} files={files} onRemove={(index) => {
                files.splice(index, 1);
                setFiles([
                    ...files
                ]);
            }} />}
        </Grid>
        <Grid item xs={3}>
            <Button
                disabled={result.isLoading}
                onClick={() => {
                    addImages({
                        // this request should be multipart formdata but there is an issue with rtk query
                        body: {
                            files: files
                        },

                        productId: productId
                    })
                }}>
                Upload
            </Button>
            <Button
                disabled={result.isLoading}
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