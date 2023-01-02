import { Grid, Tab, Tabs } from "@mui/material";
import { useEffect, useState } from "react";
import { Outlet, useLocation, useNavigate, useParams } from "react-router-dom";

export default function ProductList() {
    const location = useLocation();
    const productStateIndexMap = {
        "": 0,
        "products": 0,
        "active": 1,
        "in-review": 2,
        "out-of-stock": 3,
        "missing-info": 4,
        "rejected": 5,
        "passive": 6,
    };

    const indexProductStateMap = {
        0: "",
        1: "active",
        2: "in-review",
        3: "out-of-stock",
        4: "missing-info",
        5: "rejected",
        6: "passive",
    }

    const pathVariables = location.pathname.split('/');
    const last = pathVariables[pathVariables.length - 1];
    const navigate = useNavigate();
    const [selectedTabsIndex, setSelectedTabsIndex] = useState<number>(productStateIndexMap[last as keyof typeof productStateIndexMap]);

    const handleTabsChange = (event: React.SyntheticEvent, newValue: number) => {
        navigate(`/dashboard/products/${indexProductStateMap[newValue as keyof typeof indexProductStateMap] ?? ""}`);

        setSelectedTabsIndex(newValue);
    };

    return <Grid item container spacing={2}>
        <Grid item xs={12}>
            <Tabs
                value={selectedTabsIndex}
                onChange={handleTabsChange}
                textColor="primary"
                indicatorColor="primary"
                centered
                variant='fullWidth'
                color="primary"
                aria-label="basic tabs example">
                <Tab
                    label={"All"}
                />
                <Tab
                    label={"Active"}
                />
                <Tab
                    label={"InReview"}
                />
                <Tab
                    label={"OutOfStock"}
                />
                <Tab
                    label={"MissingInfo"}
                />
                <Tab
                    label={"Rejected"}
                />
                <Tab
                    label={"Passive"}
                />
            </Tabs>
        </Grid>
        <Grid item xs={12}>
            <Outlet />
        </Grid>
    </Grid>
}