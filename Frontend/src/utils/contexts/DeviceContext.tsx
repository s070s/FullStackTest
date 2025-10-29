//**  Context to determine if the device is touch-capable or desktop */
import React, { createContext, useContext, useEffect, useState } from "react";

type DeviceType = "touch" | "desktop";

const DeviceContext = createContext<DeviceType>("desktop");

export const useDevice = () => useContext(DeviceContext);

export const DeviceProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [deviceType, setDeviceType] = useState<DeviceType>("desktop");

    useEffect(() => {
        const isTouch =
            "ontouchstart" in window ||
            navigator.maxTouchPoints > 0 ||
            window.matchMedia("(pointer: coarse)").matches;
        setDeviceType(isTouch ? "touch" : "desktop");
    }, []);

    return (
        <DeviceContext.Provider value={deviceType}>
            {children}
        </DeviceContext.Provider>
    );
};