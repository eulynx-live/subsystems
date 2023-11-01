import React, { Component } from 'react';
import './InterlockingSystem.css';
import { SimulatorState } from './SimulatorState';
import {stat} from "fs";
import { InterlockingSystemCommand } from './proto/interlocking_system_pb';
import {InterlockingSystemClient} from "./proto/Interlocking_systemServiceClientPb";


interface InterlockingSystemState {
    webSocket: WebSocket | null,
    connected: boolean;
    lineStatus: {[key: string]: string};
    lineDirectionStatus: {[key: string]: string};
    lineDirectionInformation: {[key: string]: string};
}

class InterlockingSystem extends Component<{}, InterlockingSystemState> {
    constructor(p: {}) {
        super(p);
        this.state = {
            webSocket: null,
            connected: false,
            lineStatus: {},
            lineDirectionStatus: {},
            lineDirectionInformation: {}
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
        this.showInterlockingSystemState(state);
    }

    private showInterlockingSystemState(state: SimulatorState) {
        this.setState({
            connected: state.initialized ?? false,
            lineStatus: state.lineStatus,
            lineDirectionInformation: state.lineDirectionInformation,
            lineDirectionStatus: state.lineDirectionStatus
        });
    }

    render() {
        const url = window.location.protocol + '//' + window.location.host + (window.location.pathname.endsWith('/') ? window.location.pathname.slice(0, -1) : window.location.pathname);
        const client = new InterlockingSystemClient(url);
        
        return (
            <div className="interlocking-system">
                <h1>EULYNX Adjacent Interlocking System Simulator</h1>
                <h2>Connection to Interlocking System</h2>
                <p>{this.state.connected ? 'connected' : 'disconnected'}</p>
                {Object.entries(this.state.lineStatus).map(([boundaryId, state]) =>
                    <div key={boundaryId}>
                        <h2>Line information - Boundary {boundaryId}</h2>
                        <h3>Status</h3>
                        <p>Line status: {this.state.lineStatus[boundaryId]}</p>
                        <p>Line direction: {this.state.lineDirectionInformation[boundaryId]}</p>
                        <p>Line direction status: {this.state.lineDirectionStatus[boundaryId]}</p>
                        <h3>Line operation</h3>
                        <button disabled={this.state.lineStatus[boundaryId]=='Occupied' || this.state.lineDirectionInformation[boundaryId]=='Exit'} onClick={async () => {
                            var request = new InterlockingSystemCommand();
                            request.setInterlockingsystem(boundaryId);
                            await client.setBlock(request, null);
                        }}>Block to send train</button>
                        <button disabled={this.state.lineStatus[boundaryId]=='Vacant' || this.state.lineDirectionInformation[boundaryId]=='Entry'} onClick={async () => {
                            var request = new InterlockingSystemCommand();
                            request.setInterlockingsystem(boundaryId);
                            await client.unsetBlock(request, null);
                        }}>Unblock after train arrived</button>
                        <button disabled={this.state.lineStatus[boundaryId]=='Occupied' || this.state.lineDirectionStatus[boundaryId]=='Locked'} onClick={async () => {
                            var request = new InterlockingSystemCommand();
                            request.setInterlockingsystem(boundaryId);
                            await client.initiateDirectionHandover(request, null);
                        }}>Initiate direction change</button>
                    </div>
                )}
            </div>
        );
    }
}



export default InterlockingSystem;
