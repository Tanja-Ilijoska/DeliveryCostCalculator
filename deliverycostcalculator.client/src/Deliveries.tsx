/* eslint-disable @typescript-eslint/no-unused-vars */
import { useEffect, useState } from 'react';
import './App.css';
import axios from 'axios';

interface Delivery {
    id: number;
    recipient: string;
    distance: number;
    weight: number;
    countryId: number;
    deliveryServiceId: number;
    cost: number;
}

function Deliveries() {
    const [devliveries, setDeliveries] = useState<Delivery[]>();

    useEffect(() => {
        populateDeliveryData();
    }, []);

    const contents = devliveries === undefined
        ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
        : <table className="table table-striped" aria-labelledby="tabelLabel">
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Country Type</th>
                    <th>Cost correction (Percentage)</th>
                </tr>
            </thead>
            <tbody>
                {devliveries.map(d =>
                    <tr key={d.id}>
                        <td>{d.recipient}</td>
                        <td>{d.distance}</td>
                        <td>{d.weight}</td>
                        <td>{d.countryId}</td>
                        <td>{d.deliveryServiceId}</td>
                        <td>{d.cost}</td>
                    </tr>
                )}
            </tbody>
        </table>;

    return (
        <div>
            <h1 id="tabelLabel">Deliveries...</h1>
            <p>This component demonstrates fetching data from the server.</p>
            {contents}
        </div>
    );


    async function populateDeliveryData() {
        const url = `/api/deliveries`;
        await axios.get(url).then(response => {
            const data = response.data;
            setDeliveries(data);
        });
    }
}

export default Deliveries;