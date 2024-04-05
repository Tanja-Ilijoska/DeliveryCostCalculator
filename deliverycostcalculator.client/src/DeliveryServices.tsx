/* eslint-disable @typescript-eslint/no-unused-vars */
import { useEffect, useState } from 'react';
import './App.css';
import axios from 'axios';
import AppMenuBar from './AppMenuBar';

interface Country {
    id: number;
    name: string;
    countryType: string;
    costCorrectionPercentage: number;
}

function App() {
    const [countries, setCountries] = useState<Country[]>();

    useEffect(() => {
        populateCountryData();
    }, []);

    const contents = countries === undefined
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
                {countries.map(country =>
                    <tr key={country.id}>
                        <td>{country.name}</td>
                        <td>{country.countryType}</td>
                        <td>{country.costCorrectionPercentage}</td>
                    </tr>
                )}
            </tbody>
        </table>;

    return (

        <><div><AppMenuBar></AppMenuBar></div><div>
            <h1 id="tabelLabel">Countries...</h1>
            <p>This component demonstrates fetching data from the server.</p>
            {contents}
        </div></>
    );


    async function populateCountryData() {
        const url = `/api/countries`;
        await axios.get(url).then(response => {
            const data = response.data;
            setCountries(data);
        });
    }
}

export default App;