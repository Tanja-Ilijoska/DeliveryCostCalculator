/* eslint-disable @typescript-eslint/no-unused-vars */
import { useEffect, useState } from 'react';
import './App.css';
import axios from 'axios';

interface DeliveryServices {
    id: number;
    name: string;
    formula: string;

}

function DeliveryServices() {
    const [deliveryServices, setDeliveryServices] = useState<DeliveryServices[]>();

    useEffect(() => {
        populateDeliveryServices();
    }, []);

    console.log(deliveryServices)
    const contents = deliveryServices === undefined
        ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
        : <table className="table table-striped" aria-labelledby="tabelLabel">
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Formula</th>
                </tr>
            </thead>
            <tbody>
                {deliveryServices.map(deliveryService =>
                    <tr key={deliveryService.id}>
                        <td>{deliveryService.name}</td>
                        <td>{deliveryService.formula}</td>
                    </tr>
                )}
            </tbody>
        </table>;

    return (
        <div>
            <h1 id="tabelLabel">Countries...</h1>
            <p>This component demonstrates fetching data from the server.</p>
            {contents}
        </div>
    );


    async function populateDeliveryServices() {
        const url = `/api/deliveryServices`;
        await axios.get(url).then(response => {
            const data = response.data;
            setDeliveryServices(data);
        });
    }
}

export default DeliveryServices;