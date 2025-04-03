import axios from 'axios';

export interface WeatherData {
    //id: number;
    country: string;
    city: string;
    temperature: number;
    dateCreated: string;
}

const apiClient = axios.create({
    baseURL: 'https://localhost:7184/api',
    headers: { 'Content-Type': 'application/json' },
});

export const getWeatherData = async (): Promise<WeatherData[]> => {
    const response = await apiClient.get<WeatherData[]>('/weather');
    return response.data;
};