import { Button, FormControl, Grid, InputLabel, MenuItem, Select, SelectChangeEvent, TextField } from "@mui/material";
import { BooleanFilter, BooleanFilterOperation, DateFilter, DateFilterOperation, LogicalOperator, NumericFilter, NumericFilterOperation, Pagination, QueryBuilder, SortDirection, SortField, StringFilter, StringFilterOperation } from "dynamic-query-builder-client";
import React, { ChangeEvent, useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

export type QueryFilter = NumericFilter | StringFilter | DateFilter | BooleanFilter;

type PropertyType = "string" | "number" | "float" | "date" | "boolean" | "enum";

export interface PropertyDefinition {
    name: string;
    title: string;
    type: PropertyType;
    enumValues?: { value: string, title: string }[];
}

interface FilterElementProps {
    id: number;
    properties: PropertyDefinition[];
    queryFilter?: QueryFilter;
    onChange: (index: number, queryFilter: QueryFilter) => void;
    onRemove: (index: number) => void;
}

const typeOperationMapping = {
    "string": [
        "In",
        "Equals",
        "Contains",
        "NotEqual",
        "EndsWith",
        "StartsWith",
    ],
    "number": [
        "In",
        "Equals",
        "NotEqual",
        "LessThan",
        "LessThanOrEqual",
        "GreaterThan",
        "GreaterThanOrEqual",
    ],
    "boolean": [
        "Equals",
        "NotEqual",
    ],
    "date": [
        "Equals",
        "NotEqual",
        "GreaterThan",
        "GreaterThanOrEqual",
        "LessThan",
        "LessThanOrEqual",
    ],
    "enum": [
        "In",
        "Equals",
        "NotEqual",
    ],
    "float": [
        "In",
        "Equals",
        "NotEqual",
        "LessThan",
        "LessThanOrEqual",
        "GreaterThan",
        "GreaterThanOrEqual",
    ]
}

function FilterElement(props: FilterElementProps) {
    const { t } = useTranslation();
    const { id, properties, onChange, onRemove } = props;
    const [availableOperations, setAvailableOperations] = useState<string[]>([]);
    const [selectedProperty, setSelectedProperty] = useState<PropertyDefinition>();
    const [selectedOperation, setSelectedOperation] = useState<string>("");
    const [filterValue, setFilterValue] = useState<string>("");

    const handlePropertyChange = (event: SelectChangeEvent) => {
        const selected = event.target.value;
        const selectedProp = properties.find(prop => prop.name === selected);
        if (selectedProp) {
            setAvailableOperations(typeOperationMapping[selectedProp.type]);
            setSelectedProperty(properties.find(prop => prop.name === selected));
            setSelectedOperation("");
            setFilterValue("");
        }
    };

    const handleOperationChange = (event: SelectChangeEvent) => {
        const selected = event.target.value;
        setSelectedOperation(selected);
        setFilterValue("");
    }

    const handleEnumValueChange = (event: SelectChangeEvent) => {
        const value = event.target.value;

        setFilterValue(value);
    }

    const handleValueChange = (event: ChangeEvent<HTMLInputElement>) => {
        const value = event.target.value;

        setFilterValue(value);
    }

    useEffect(() => {
        if (filterValue) {
            switch (selectedProperty?.type) {
                case "string":
                case "enum":
                    onChange(id, new StringFilter({
                        property: selectedProperty.name,
                        op: selectedOperation as StringFilterOperation,
                        value: filterValue
                    }))
                    break;
                case "number":
                    onChange(id, new NumericFilter({
                        property: selectedProperty.name,
                        op: selectedOperation as NumericFilterOperation,
                        value: parseInt(filterValue)
                    }))
                    break;
                case "float":
                    onChange(id, new NumericFilter({
                        property: selectedProperty.name,
                        op: selectedOperation as NumericFilterOperation,
                        value: parseFloat(filterValue)
                    }))
                    break;
                case "date":
                    onChange(id, new DateFilter({
                        property: selectedProperty.name,
                        op: selectedOperation as DateFilterOperation,
                        value: filterValue
                    }))
                    break;
                case "boolean":
                    onChange(id, new BooleanFilter({
                        property: selectedProperty.name,
                        op: selectedOperation as BooleanFilterOperation,
                        value: filterValue as any
                    }))
                    break;
                default:
                    break;
            }
        }
    }, [filterValue]);

    return <Grid item xs={12} container spacing={2} key={id}>
        <Grid item xs={3}>
            <FormControl fullWidth
                size="small">
                <InputLabel id={`${id}-prop-name-label`}>{t("Filters.PropertyName")}</InputLabel>
                <Select
                    labelId={`${id}-prop-name-label`}
                    id={`${id}-prop-name`}
                    value={selectedProperty?.name ?? ""}
                    label="Property Name"
                    onChange={handlePropertyChange}
                >
                    <MenuItem value={""} disabled>{t("Generic.Forms.Select")}</MenuItem>
                    {properties.map(properties => <MenuItem key={properties.name} value={properties.name}>{properties.title}</MenuItem>)}
                </Select>
            </FormControl>
        </Grid>
        <Grid item xs={3}>
            <FormControl fullWidth
                size="small">
                <InputLabel id={`${id}-operation-label`}>{t("Filters.OperationName")}</InputLabel>
                <Select
                    disabled={!selectedProperty}
                    labelId={`${id}-operation-label`}
                    id={`${id}-operation`}
                    value={selectedOperation}
                    label={t("Filters.OperationName")}
                    onChange={handleOperationChange}
                >
                    <MenuItem value={""}>{t("Generic.Forms.Select")}</MenuItem>
                    {selectedProperty && availableOperations.map(operation => <MenuItem key={operation} value={operation}>{t(`Filters.Operations.${selectedProperty?.type}.${operation}`)}</MenuItem>)}
                </Select>
            </FormControl>
        </Grid>
        <Grid item xs={3}>
            <FormControl fullWidth size="small">
                {selectedProperty?.type === "enum" ?
                    <React.Fragment>
                        <InputLabel id={`${id}-enum-label`}>{t("Filters.FilterValue")}</InputLabel>
                        <Select
                            disabled={!selectedOperation}
                            labelId={`${id}-enum-label`}
                            id={`${id}-enum`}
                            value={filterValue}
                            label={t("Filters.FilterValue")}
                            onChange={handleEnumValueChange}
                        >
                            <MenuItem value={""}>{t("Generic.Forms.Select")}</MenuItem>
                            {selectedProperty.enumValues && selectedProperty.enumValues.map(enumVal => <MenuItem key={enumVal.value} value={enumVal.value}>{enumVal.title}</MenuItem>)}
                        </Select>
                    </React.Fragment> :
                    <TextField
                        size="small"
                        disabled={!selectedOperation}
                        label={t("Filters.FilterValue")}
                        value={filterValue}
                        onChange={handleValueChange}
                    />}
            </FormControl>
        </Grid>
        <Grid item xs={3} display="flex" justifyContent={"flex-start"}>
            <Button size="small" variant="contained" color="error" onClick={() => onRemove(id)} sx={{
                mr: 2
            }}>
                Kaldır
            </Button>
        </Grid>
    </Grid>
}

export interface ModelDefinition {
    properties: PropertyDefinition[];
}

export interface FilterProps {
    model: ModelDefinition;
    onApply: (queryFilters: QueryFilter[]) => void;
}

export function Filters(props: FilterProps) {
    const { model, onApply } = props;
    const [queryFilters, setQueryFilters] = useState<QueryFilter[]>([]);
    // const [properties, setProperties] = useState<PropertyDefinition[]>([]);
    const [filterElements, setFilterElements] = useState<FilterElementProps[]>([]);

    // useEffect(() => {
    //     const _properties = [];
    //     for (const key in model) {
    //         if (Object.prototype.hasOwnProperty.call(model, key)) {
    //             const element = model[key];
    //             _properties.push({
    //                 name: key,
    //                 type: element
    //             });
    //         }
    //     }

    //     if (properties.length === 0) {
    //         setProperties(_properties);
    //     }
    // }, [model, properties]);

    useEffect(() => {
        setFilterElements([
            {
                id: 0,
                properties: model.properties,
            } as FilterElementProps
        ])
    }, [model]);

    const handleAddFilter = () => {
        const currentLength = filterElements.length;
        setFilterElements([
            ...filterElements,
            {
                id: currentLength,
                properties: model.properties,
            } as FilterElementProps
        ])
    }

    const handleChange = (index: number, queryFilter: QueryFilter) => {
        if (queryFilter.value === undefined) {
            return;
        }

        if (queryFilters[index]) {
            const newFilters = queryFilters.map((_queryFilter, i) => i === index ? queryFilter : _queryFilter);
            setQueryFilters([
                ...newFilters
            ]);
        }
        else {
            setQueryFilters([
                ...queryFilters,
                queryFilter
            ]);
        }
    }

    const handleRemove = (index: number) => {
        if (filterElements[index]) {
            filterElements.splice(index, 1);
            setFilterElements([
                ...filterElements
            ]);
        }

        if (queryFilters[index]) {
            queryFilters.splice(index, 1);
            setQueryFilters([
                ...queryFilters
            ]);
        }
    }

    const handleApplyFilters = () => {
        onApply(queryFilters);
    }

    return <Grid container spacing={2}>
        <Grid item xs={12}>
            <Button>
                Filters
            </Button>
        </Grid>
        <Grid item xs={12} container spacing={2}>
            {filterElements.map((filter) => {
                return <FilterElement key={filter.id} {...{
                    id: filter.id,
                    properties: model.properties,
                    queryFilter: filter.queryFilter,
                    onChange: handleChange,
                    onRemove: handleRemove
                } as FilterElementProps} />;
            })}
        </Grid>
        <Grid item xs={6} display="flex" justifyContent={"flex-start"}>
            <Button variant="outlined" onClick={handleAddFilter}>
                Filtre Ekle
            </Button>
        </Grid>
        <Grid item xs={6} display="flex" justifyContent={"flex-end"}>
            <Button variant="outlined" onClick={handleApplyFilters}>
                Uygula
            </Button>
        </Grid>
    </Grid>
}