import { useState } from 'react';
import './App.css';
import { createSignalRContext } from 'react-signalr';
import Point from './Point';

const { Provider, useSignalREffect } = createSignalRContext();

export type PointState = {
  'lastCommandedPointPosition': string,
  'pointPosition': string,
  'degradedPointPosition': string,
  'abilityToMove': string
};

export type SimulatedPointState = {
  'preventedPositionLeft': string,
  'preventedPositionRight': string,
  'degradedPositionLeft': boolean,
  'degradedPositionRight': boolean,
  'simulateTimeoutLeft': boolean,
  'simulateTimeoutRight': boolean,
  'simulateInitializationTimeout': boolean
};

export type SimulatorConfiguration = {
  localId: string,
  localRastaId: number,
  remoteId: string,
  simulatedTransitioningTimeSeconds: number,
  allPointMachinesCrucial: boolean,
  observeAbilityToMove: boolean,
  initialLastCommandedPointPosition: string,
  initialPointPosition: string,
  initialDegradedPointPosition: string,
  initialAbilityToMove: string,
  connectionProtocol: 'EulynxBaseline4R1' | 'EulynxBaseline4R2',
  pdiVersion: string,
  pdiChecksum: string,
};

function App() {
  const [initialized, setInitialized] = useState<boolean>(false);
  const [pointState, setPointState] = useState<PointState | null>(null);
  const [simulatedPointState, setSimulatedPointState] = useState<SimulatedPointState | null>(null);
  const [simulatorConfiguration, setSimulatorConfiguration] = useState<SimulatorConfiguration | null>(null);

  useSignalREffect('ReceiveStatus', (
    receivedInitialized: boolean,
    receivedPointState: PointState,
    receivedSimulatedPointState: SimulatedPointState,
    receivedSimulatorConfiguration: SimulatorConfiguration,
  ) => {
    setInitialized(receivedInitialized);
    setPointState(receivedPointState);
    setSimulatedPointState(receivedSimulatedPointState);
    setSimulatorConfiguration(receivedSimulatorConfiguration);
  }, []);

  return (
    <div className="text-slate-600 dark:text-slate-400 bg-slate-100 dark:bg-slate-900">
      <Provider url="/status" automaticReconnect>
        <Point initialized={initialized} pointState={pointState} simulatedPointState={simulatedPointState} simulatorConfiguration={simulatorConfiguration} />
      </Provider>
    </div>
  );
}

export default App;
