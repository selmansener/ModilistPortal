import { Autocomplete, TextField } from "@mui/material";
import { useRef } from "react";
import { useTranslation } from "react-i18next";
import { useGetApiV1AddressGetCitiesQuery } from "../../store/api";

export interface CitiesProps {
    value?: string,
    error?: boolean,
    helperText?: string | false,
    onChange: (value: { code?: string, name?: string }) => void;
    onBlur: (value: { code?: string, name?: string }) => void;
}

export function Cities(props: CitiesProps) {
    const { t } = useTranslation();
    const { isLoading, data: cities, error: getCitiesError } = useGetApiV1AddressGetCitiesQuery({});
    const { value, error, helperText, onChange, onBlur } = props;
    const ref = useRef<HTMLInputElement>();

    const getCity = () => {
        const city = cities?.find(x => x.name === value);
        if (city) {
            return {
                ...city,
                label: city.name
            }
        }
        else {
            return {
                name: "",
                code: "",
                label: ""
            }
        }
    }

    return (
        <Autocomplete
            id="city"
            disabled={isLoading}
            value={getCity()}
            onChange={(e, value) => {
                onChange({
                    code: value?.code,
                    name: value?.name
                })
            }}
            onBlur={(e) => {
                const city = cities?.find(x => x.name === ref?.current?.value);
                onBlur({
                    code: city?.code,
                    name: city?.name
                })
            }}
            disablePortal
            options={cities && cities?.length > 0 ? cities?.map(city => {
                return {
                    ...city,
                    label: city.name
                }
            }) : []}
            renderInput={(params) => <TextField
                inputRef={ref}
                {...params}
                name={"city"}
                error={error}
                helperText={helperText}
                label={t("Generic.Address.City")}
            />}
        />
    );
}