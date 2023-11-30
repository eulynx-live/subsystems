import React, { Component } from 'react';
import './Point.css';
import * as google_protobuf_empty_pb from 'google-protobuf/google/protobuf/empty_pb';
import { SimulatorState } from './SimulatorState';
import { PointClient } from './proto/PointServiceClientPb';

import { PreventedPositionMessage, PreventedPosition, PointPosition } from './proto/point_pb';

interface PointState {
    webSocket: WebSocket | null,
    connected: boolean;
    position: string;
}

class Point extends Component<{}, PointState> {
    constructor(p: {}) {
        super(p);
        this.state = {
            webSocket: null,
            connected: false,
            position: '',
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
        this.showPointState(state);
    }

    private showPointState(state: SimulatorState) {
        this.setState({
            connected: state.initialized ?? false,
            position: state.position,
        });
    }

    render() {
        const url = window.location.protocol + '//' + window.location.host + (window.location.pathname.endsWith('/') ? window.location.pathname.slice(0, -1) : window.location.pathname);
        const client = new PointClient(url);

        return (
            <div className="point">
                <h1>EULYNX Point Simulator</h1>
                <h2>Connection to Interlocking</h2>
                <p>{this.state.connected ? 'connected' : 'disconnected'}</p>
                <h2>Position</h2>
                <p>{this.state.position}</p>
                <button onClick={async () => {
                    let request = new PreventedPositionMessage();
                    request.setPosition(PreventedPosition.PREVENTEDLEFT);
                    await client.preventEndPosition(request, null);
                }}>Prevent position left</button>
                <button onClick={async () => {
                    let request = new PreventedPositionMessage();
                    request.setPosition(PreventedPosition.PREVENTEDRIGHT);
                    await client.preventEndPosition(request, null);
                }}>Prevent position right</button>
                <p></p>
                <button onClick={async () => {
                    let request = new PreventedPositionMessage();
                    request.setPosition(PreventedPosition.TRAILED);
                    await client.preventEndPosition(request, null);
                }}>Set to unintended position</button>
                <p></p>
                <button onClick={async () => {
                    await client.putInEndPosition(new google_protobuf_empty_pb.Empty(), null);
                }}>Finalize position</button>
            </div>
        );
    }
}

export default Point;
