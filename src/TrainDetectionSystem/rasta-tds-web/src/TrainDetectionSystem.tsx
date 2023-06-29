import React, { Component } from 'react';
import './TrainDetectionSystem.css';
import { SimulatorState } from './SimulatorState';
import { TrainDetectionSystemClient } from './proto/TdsServiceClientPb';
import { TpsCommand } from './proto/tds_pb';

interface TrainDetectionSystemState {
    webSocket: WebSocket | null,
    connected: boolean;
    states: {[key: string]: string};
}

class TrainDetectionSystem extends Component<{}, TrainDetectionSystemState> {
    constructor(p: {}) {
        super(p);
        this.state = {
            webSocket: null,
            connected: false,
            states: {},
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
        const state = JSON.parse(event.data) as SimulatorState;
        this.showTrainDetectionSystemState(state);
    }

    private showTrainDetectionSystemState(state: SimulatorState) {
        this.setState({
            connected: state.initialized ?? false,
            states: state.states,
        });
    }

    render() {
        const url = window.location.protocol + '//' + window.location.host + (window.location.pathname.endsWith('/') ? window.location.pathname.slice(0, -1) : window.location.pathname);
        const client = new TrainDetectionSystemClient(url);

        return (
            <div className="tds">
                <h1>EULYNX Train Detection System Simulator</h1>
                <h2>Connection to Interlocking</h2>
                <p>{this.state.connected ? 'connected' : 'disconnected'}</p>
                <h2>Track Section States</h2>
                <div className="tdsStates">
                    {Object.entries(this.state.states).map(([id, state]) =>
                        <div key={id}>
                            <h3>{id}</h3>
                            <p>{state}</p>
                            <button onClick={async () => {
                                var request = new TpsCommand();
                                request.setTps(id);

                                await client.increaseAxleCount(request, null);
                            }}>Set occupied</button>
                            <button onClick={async () => {
                                var request = new TpsCommand();
                                request.setTps(id);

                                await client.decreaseAxleCount(request, null);
                            }}>Set vacant</button>
                        </div>
                    )}
                    </div>
            </div>
        );
    }
}

export default TrainDetectionSystem;
