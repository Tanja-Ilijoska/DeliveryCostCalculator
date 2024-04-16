
import React, { useEffect, useState } from "react";
import { useFormik } from "formik";
import { Button, InputLabel, MenuItem, Select, SelectChangeEvent, TextField, Typography } from "@mui/material";
import axios from "axios";
import DeliveryServices from "./DeliveryServices";



interface IFormValues {
    recipient: string;
    distance: number;
    weight: number;
    countryId: string;
    deliveryServiceId: number;
    cost: number;
}

const initialValues: IFormValues = {
    recipient: "",
    distance: 0,
    weight: 0,
    countryId: "",
    deliveryServiceId: 0,
    cost: 0,
};

interface costParameters {
    distance: number,
    weight: number,
    countryId: string,
    deliveryServiceId: string,
}

interface Country {
    id: number;
    name: string;
    countryType: string;
    costCorrectionPercentage: string;
}

const App = () => {
    const onSubmit = (values: IFormValues) => {
        // Handle form submission logic here
        // like making an API call or updating the store

        console.log(values);
        axios.post('/api/delivery', values);
    };

    const [countries, setCountries] = useState<Country[]>();
    const [countryId, setCountry] = React.useState('');

    const [deliveryServices, setDeliveryServices] = useState<DeliveryServices[]>();
    const [deliveryServiceId, setDeliveryService] = React.useState('');

    const [distance, setDistance] = React.useState(0);
    const [weight, setWeight] = React.useState(0);


    const [cost, setCost] = React.useState(0);

    useEffect(() => {
        populateCountryData();
        populateDeliveryServicesData();
    }, []);

    useEffect(() => {
        setWeight(formik.values.weight);
        setDistance(formik.values.distance);

        if (weight != 0 && distance != 0 && countryId != null && deliveryServiceId != null) {
        const parameters: costParameters = {
            weight: weight,
            distance: distance,
            countryId: countryId,
            deliveryServiceId: deliveryServiceId
        }
            getDeliveryCost(parameters);
        }
    }, [distance, weight, countryId, deliveryServiceId, weight, distance]);

    const handleChangeCountry = (event: SelectChangeEvent) => {
        setCountry(event.target.value);
    };

    const handleChangeDeliveryService = (event: SelectChangeEvent) => {
        setDeliveryService(event.target.value);
    };


    const formik = useFormik<IFormValues>({
        initialValues,
        onSubmit,
    });

    return (
        <div className="displayFlex">
            <React.Fragment>
                <form autoComplete="off" onSubmit={formik.handleSubmit}>
                    <Typography variant="h5" fontWeight={600} mb={2}>
                        Login Form
                    </Typography>
                    <TextField
                        label="Recipient"
                        variant="outlined"
                        color="primary"
                        type="text"
                        name="recipient"
                        sx={{ mb: 3 }}
                        fullWidth
                        value={formik.values.recipient}
                        onChange={formik.handleChange}
                        onBlur={formik.handleBlur}
                        error={formik.touched.recipient && Boolean(formik.errors.recipient)}
                        helperText={formik.touched.recipient && formik.errors.recipient}
                    />
                    <TextField
                        label="Distance"
                        variant="outlined"
                        color="primary"
                        type="number"
                        name="distance"
                        placeholder=""
                        value={formik.values.distance}
                        onChange={formik.handleChange}
                        onBlur={formik.handleBlur}
                        error={
                            formik.touched.distance && Boolean(formik.errors.distance)
                        }
                        helperText={
                            formik.touched.distance && formik.errors.distance
                        }
                        fullWidth
                        sx={{ mb: 3 }}
                    />
                    <TextField
                        label="Weight"
                        variant="outlined"
                        color="primary"
                        type="number"
                        name="weight"
                        placeholder=""
                        value={formik.values.weight}
                        onChange={formik.handleChange}
                        onBlur={formik.handleBlur}
                        error={
                            formik.touched.weight && Boolean(formik.errors.weight)
                        }
                        helperText={
                            formik.touched.weight && formik.errors.weight
                        }
                        fullWidth
                        sx={{ mb: 3 }}
                    />

                    <InputLabel id="country-label">Country</InputLabel>
                    <Select
                        labelId="country-label"
                        variant="outlined"
                        color="primary"
                        type="number"
                        name="countryId"
                        placeholder="Country"
                        value={countryId}
                        onChange={handleChangeCountry}
                        onBlur={formik.handleBlur}
                        error={
                            formik.touched.countryId && Boolean(formik.errors.countryId)
                        }
                        fullWidth
                        sx={{ mb: 3 }}
                    >
                        {countries?.map(({ id, name }, index) => (
                            <MenuItem key={index} value={id}>
                                {name}
                            </MenuItem>
                        ))}
                    </Select>

                    <InputLabel id="ds-label">Delivery Service</InputLabel>
                    <Select
                        labelId="ds-label"
                        variant="outlined"
                        color="primary"
                        type="number"
                        name="deliveryServiceId"
                        placeholder="Delivery Service"
                        value={deliveryServiceId}
                        onChange={handleChangeDeliveryService}
                        onBlur={formik.handleBlur}
                        error={
                            formik.touched.deliveryServiceId && Boolean(formik.errors.deliveryServiceId)
                        }
                        fullWidth
                        sx={{ mb: 3 }}
                    >
                        {deliveryServices?.map(({ id, name }, index) => (
                            <MenuItem key={index} value={id}>
                                {name}
                            </MenuItem>
                        ))}
                    </Select>

                    <Typography>
                        Price: { cost }
                    </Typography>

                    <Button
                        variant="contained"
                        color="primary"
                        type="submit"
                        fullWidth
                        size="medium"
                        disabled={!formik.isValid}
                    >
                        Register
                    </Button>
                </form>
            </React.Fragment>
        </div>
    );
async function populateCountryData() {
    const url = `/api/countries`;
    await axios.get(url).then(response => {
        setCountries(response.data);
    });
    }

    async function populateDeliveryServicesData() {
        const url = `/api/deliveryServices`;
        await axios.get(url).then(response => {
            setDeliveryServices(response.data);
        });
    }
    async function getDeliveryCost(costparams: costParameters) {
        const url = `/api/deliveryCost`;
        await axios.post(url, costparams).then(response => {
            setCost(response.data);
        });
    }
};
export default App;

