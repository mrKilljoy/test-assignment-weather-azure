import React, { useState, useEffect } from 'react';
import { getWeatherData, WeatherData } from '../services/weatherService';
import { Bar } from 'react-chartjs-2';
import { Chart, CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend } from 'chart.js';

Chart.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend);

const WeatherDashboard: React.FC = () => {
    const [weatherItems, setWeatherItems] = useState<WeatherData[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const fetchData = async () => {
        try {
            const data = await getWeatherData();
            setWeatherItems(data);
            setLoading(false);
        } catch (err) {
            setError('Failed to fetch weather data');
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchData();
        const interval = setInterval(fetchData, 60000); // Every minute
        return () => clearInterval(interval); // Cleanup on unmount
    }, []);

    if (loading) return <p>Loading...</p>;
    if (error) return <p>{error}</p>;

    // prepare chart data (min+max temperatures for each city)
    const cities = Array.from(new Set(weatherItems.map(d => d.city)));
    const minTemps = cities.map(city => Math.min(...weatherItems.filter(d => d.city === city).map(d => d.temperature)));
    const maxTemps = cities.map(city => Math.max(...weatherItems.filter(d => d.city === city).map(d => d.temperature)));
    
    // table data
    const itemsMap = new Map<string, WeatherData>();
    weatherItems.forEach(item => {
        const key = `${item.country}-${item.city}`;
        if (!itemsMap.has(key) || itemsMap.get(key)!.dateCreated < item.dateCreated) {
            itemsMap.set(key, item);
        }
    });
    const sortedItems = Array.from(itemsMap.values()).sort((a, b) => {
        if (a.country !== b.country) return a.country.localeCompare(b.country);
        return a.city.localeCompare(b.city);
    });

    // chart data
    const chartData = {
        labels: cities,
        datasets: [
            {
                label: 'Min Temperature',
                data: minTemps,
                backgroundColor: 'rgba(54, 162, 235, 0.2)',
                borderColor: 'rgba(54, 162, 235, 1)',
                borderWidth: 1,
            },
            {
                label: 'Max Temperature',
                data: maxTemps,
                backgroundColor: 'rgba(255, 99, 132, 0.2)',
                borderColor: 'rgba(255, 99, 132, 1)',
                borderWidth: 1,
            },
        ],
    };

    return (
        <div style={{ padding: '20px' }}>
            <h3>Weather Dashboard</h3>
            <table style={{ width: '100%', borderCollapse: 'collapse', marginBottom: '20px' }}>
                <thead>
                    <tr style={{ backgroundColor: '#f2f2f2' }}>
                        <th style={{ padding: '8px', border: '1px solid #ddd' }}>Country</th>
                        <th style={{ padding: '8px', border: '1px solid #ddd' }}>City</th>
                        <th style={{ padding: '8px', border: '1px solid #ddd' }}>Temperature</th>
                        <th style={{ padding: '8px', border: '1px solid #ddd' }}>Last Update</th>
                    </tr>
                </thead>
                <tbody>
                    {sortedItems.map(data => (
                        <tr key={data.city}>
                            <td style={{ padding: '8px', border: '1px solid #ddd' }}>{data.country}</td>
                            <td style={{ padding: '8px', border: '1px solid #ddd' }}>{data.city}</td>
                            <td style={{ padding: '8px', border: '1px solid #ddd' }}>{data.temperature}</td>
                            <td style={{ padding: '8px', border: '1px solid #ddd' }}>
                                {new Date(data.dateCreated).toISOString().slice(0, 19)}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
            <div>
                <h4>Min/Max Temperatures</h4>
                <Bar data={chartData} options={{ scales: { y: { beginAtZero: true } } }} />
            </div>
        </div>
    );
};

export default WeatherDashboard;