import React, { Component } from 'react';
import './App.css';
import { LightSignal, LightSignalProps } from './LightSignal';
import { MainAspect, SimulatorState } from './SimulatorState';


interface AppState {
    webSocket: WebSocket | null,
    connected: boolean;
    lightSignals: SimulatorState[];
}

class App extends Component<{}, AppState> {
    constructor(p: {}) {
        super(p);
        this.state = {
            webSocket: null,
            connected: false,
            lightSignals: []
        };
    }

    componentDidMount() {
        var loc = window.location, new_uri;
        if (loc.protocol === "https:") {
            new_uri = "wss:";
        } else {
            new_uri = "ws:";
        }
        new_uri += "//" + loc.host;
        if (loc.pathname.endsWith('/')) {
            new_uri += loc.pathname + "ws";
        } else {
            new_uri += loc.pathname + "/ws";
        }
        const ws = new WebSocket(new_uri);
        this.setState({
            webSocket: ws,
        })
        ws.onopen = (event) => {
            console.log("connected");
        }
        ws.onerror = (error) => {
            console.error(error);
        }
        ws.onmessage = (event) => {
            this.receiveMessage(event);
        };
    }

    componentWillUnmount() {
        if (this.state.webSocket) {
            this.state.webSocket.close();
            this.setState({
                webSocket: null,
            })
        }
    }

    private receiveMessage(event: MessageEvent<any>): any {
        if (typeof event.data !== 'string') {
            return;
        }
        const state = JSON.parse(event.data) as SimulatorState[];
        this.showSignalState(state);
    }

    private showSignalState(state: SimulatorState[]) {
        this.setState({
            connected: true, // state.setup ?? false,
            lightSignals: state
        });
    }

    private showMainAspect(aspect: MainAspect, connected: boolean, id: string, suspended: boolean, setup: boolean): LightSignalProps {
        switch (aspect) {
            case MainAspect.Hp0:
                return {
                    id,
                    connected,
                    suspended,
                    setup,
                    additionalLightTop: false,
                    red: true,
                    green: 'off',
                    yellow: false,
                    bulb04: false,
                    bulb05: false,
                    bulb06: false,
                    additionalLightBottom: false,
                    bulb08: false,
                };
            case MainAspect.Hp0Fahrtanzeiger:
                return {
                    id,
                    connected,
                    suspended,
                    setup,
                    additionalLightTop: false,
                    red: true,
                    green: 'off',
                    yellow: false,
                    bulb04: false,
                    bulb05: false,
                    bulb06: false,
                    additionalLightBottom: false,
                    bulb08: false,
                };
            case MainAspect.Ks1:
                return {
                    id,
                    connected,
                    suspended,
                    setup,
                    additionalLightTop: false,
                    red: false,
                    green: 'on',
                    yellow: false,
                    bulb04: false,
                    bulb05: false,
                    bulb06: false,
                    additionalLightBottom: false,
                    bulb08: false,
                };
            case MainAspect.Ks1Blinking:
                return {
                    id,
                    connected,
                    suspended,
                    setup,
                    additionalLightTop: false,
                    red: false,
                    green: 'blinking',
                    yellow: false,
                    bulb04: false,
                    bulb05: false,
                    bulb06: false,
                    additionalLightBottom: false,
                    bulb08: false,
                };
            case MainAspect.Ks2:
                return {
                    id,
                    connected,
                    suspended,
                    setup,
                    additionalLightTop: false,
                    red: false,
                    green: 'off',
                    yellow: true,
                    bulb04: false,
                    bulb05: false,
                    bulb06: false,
                    additionalLightBottom: false,
                    bulb08: false,
                };
            case MainAspect.Ks2Additional:
                return {
                    id,
                    connected,
                    suspended,
                    setup,
                    additionalLightTop: false,
                    red: false,
                    green: 'off',
                    yellow: true,
                    bulb04: false,
                    bulb05: false,
                    bulb06: false,
                    additionalLightBottom: false,
                    bulb08: false,
                };
            default:
                return {
                    id,
                    connected,
                    suspended,
                    setup,
                    additionalLightTop: false,
                    red: false,
                    green: 'off',
                    yellow: false,
                    bulb04: false,
                    bulb05: false,
                    bulb06: false,
                    additionalLightBottom: true,
                    bulb08: false,
                };
        }
    }

    render() {
        return (
            <div className="container">
                {this.state.lightSignals.map(x =>
                    <LightSignal {...this.showMainAspect(x.mainAspect, this.state.connected, x.id, x.suspended, x.setup)}></LightSignal>
                )}                
            </div>
        );
    }
}

export default App;
