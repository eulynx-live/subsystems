import React, { Component } from 'react';
import './LevelCrossing.css';
import { SimulatorState } from './SimulatorState';

interface LevelCrossingState {
    webSocket: WebSocket | null,
    connected: boolean;
    states: {[key: string]: string};
}

class LevelCrossing extends Component<{}, LevelCrossingState> {
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
        this.showLevelCrossingState(state);
    }

    private showLevelCrossingState(state: SimulatorState) {
        this.setState({
            connected: state.initialized ?? false,
            states: state.states,
        });
    }

    render() {
        return (
            <div>
                <h1>NeuPro Level Crossing Simulator</h1>
                <h2>Connection to Interlocking</h2>
                <p>{this.state.connected ? 'connected' : 'disconnected'}</p>
                <h2>Track States</h2>
                {Object.entries(this.state.states).map(([id, state]) =>
                    <div key={id}>
                        <h3>{id}</h3>
                        <p>{state}</p>
                    </div>
                )}
            </div>
        );
    }
}

export default LevelCrossing;
