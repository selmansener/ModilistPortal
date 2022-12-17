import { Autocomplete, MenuItem, Select, SelectChangeEvent, TextField } from "@mui/material";
import { useEffect, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { api, useGetApiV1AddressGetDistrictsByCityCodeQuery } from "../../store/api";
import { useAppDispatch } from "../../store/hooks";

export interface DistrictsProps {
    selectedCity?: string,
    value?: string,
    error?: boolean,
    helperText?: string | false,
    onChange: (value: { code?: string, name?: string }) => void;
    onBlur: (value: { code?: string, name?: string }) => void;
}

export function Districts(props: DistrictsProps) {
    const { t } = useTranslation();
    const dispatch = useAppDispatch();
    const { value, error, helperText, onChange, onBlur, selectedCity } = props;
    let { isLoading: districtsLoading, data: districts, error: getDistrictError } = useGetApiV1AddressGetDistrictsByCityCodeQuery({
        cityCode: selectedCity ?? ""
    });
    const ref = useRef<HTMLInputElement>();

    useEffect(() => {
        if (selectedCity) {
            dispatch(api.endpoints.getApiV1AddressGetDistrictsByCityCode.initiate({
                cityCode: selectedCity
            })).then(({ isLoading, data, error }) => {
                districtsLoading = isLoading;
                districts = data;
                getDistrictError = error;
            });

        }
    }, [selectedCity]);

    const getDistrict = () => {
        const district = districts?.find(x => x.name == value);

        if (district) {
            return {
                ...district,
                label: district.name
            }
        }
        else {
            return {
                code: "",
                name: "",
                label: ""
            }
        }
    }

    return (

        <Autocomplete
            id="district"
            value={getDistrict()}
            disabled={districtsLoading}
            onChange={(e, value) => {
                onChange({
                    code: value?.code,
                    name: value?.name
                })
            }}
            onBlur={(e) => {
                const city = districts?.find(x => x.name === ref?.current?.value);
                onBlur({
                    code: city?.code,
                    name: city?.name
                })
            }}
            disablePortal
            options={districts && districts?.length > 0 ? districts?.map(district => {
                return {
                    ...district,
                    label: district.name
                }
            }) : []}
            renderInput={(params) => <TextField
                inputRef={ref}
                {...params}
                name={"district"}
                error={error}
                helperText={helperText}
                label={t("Generic.Address.District")}
            />}
        />
    );
}