import { useState } from 'react';
import './App.css';
import Point from './Point';
import { createSignalRContext } from 'react-signalr';

const { Provider, useSignalREffect } = createSignalRContext();

export type PointState = {
    'lastCommandedPointPosition': 'NoEndPosition',
    'pointPosition': 'NoEndPosition',
    'degradedPointPosition': 'NotApplicable',
    'abilityToMove': 'AbleToMove'
};

export type SimulatedPointState = {
    "preventedPositionLeft": "DoNotPrevent",
    "preventedPositionRight": "DoNotPrevent",
    "degradedPositionLeft": boolean,
    "degradedPositionRight": boolean,
    "simulateTimeoutLeft": boolean,
    "simulateTimeoutRight": boolean
};

export type SimulatorConfiguration = {
    observeAbilityToMove: boolean,
    allPointMachinesCrucial: boolean,
    connectionProtocol: 'EulynxBaseline4R1' | 'EulynxBaseline4R2',
};

function App() {
    const [initialized, setInitialized] = useState<boolean>(false);
    const [pointState, setPointState] = useState<PointState | null>(null);
    const [simulatedPointState, setSimulatedPointState] = useState<SimulatedPointState | null>(null);
    const [simulatorConfiguration, setSimulatorConfiguration] = useState<SimulatorConfiguration | null>(null);

    useSignalREffect("ReceiveStatus", (initialized: boolean, pointState: PointState, simulatedPointState: SimulatedPointState, simulatorConfiguration: SimulatorConfiguration) => {
        setInitialized(initialized);
        setPointState(pointState);
        setSimulatedPointState(simulatedPointState);
        setSimulatorConfiguration(simulatorConfiguration);
    }, []);

    return (
        <div className="container">
            <Provider url="/status" automaticReconnect={true} >
                <Point initialized={initialized} pointState={pointState} simulatedPointState={simulatedPointState} simulatorConfiguration={simulatorConfiguration} />
            </Provider>
        </div>
    );
}

export default App;
