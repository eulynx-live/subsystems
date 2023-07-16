export interface SimulatorState
{
    initialized: boolean;
    lineStatus: {[key: string]: string};
    lineDirectionInformation: {[key: string]: string};
    lineDirectionStatus: {[key: string]: string};
};
