import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App";
import { AuthProvider } from "./utils/contexts/AuthContext";
import { DeviceProvider } from "./utils/contexts/DeviceContext";
import { BrowserRouter} from 'react-router-dom';
import "./index.css";
//StrictMode causes useEffect and therefore api calls to run twice in dev mode only
ReactDOM.createRoot(document.getElementById("root")!).render(
    <React.StrictMode>
        <BrowserRouter>
            <AuthProvider>
                <DeviceProvider>
                    <App />
                </DeviceProvider>
            </AuthProvider>
        </BrowserRouter>

    </React.StrictMode>
);
