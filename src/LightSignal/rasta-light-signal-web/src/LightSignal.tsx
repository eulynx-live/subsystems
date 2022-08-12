import React, { Component } from 'react';
import './LightSignal.css';
import { SimulatorState, MainAspect } from './SimulatorState';

interface BulbProps {
    color: string;
    size: 'small' | 'normal';
    left: number, // percent
    top: number, // percent
    on: boolean,
    blinking?: boolean,
}


class Bulb extends Component<BulbProps, {}> {
    render() {
        const {size, color, top, left, on, blinking} = this.props;
        const dimensions = size === 'normal' ? '6vh' : '3vh';
        return (
            <div
                className={blinking ? 'blinking' : ''}
                style={{
                    backgroundColor: on ? color : "#555555",
                    height: `calc(${dimensions})`,
                    width: `calc(${dimensions})`,
                    borderRadius: '50%',
                    display: 'inline-block',
                    position: 'absolute',
                    transform: "translate(-50%, -50%)",
                    top: `${top}%`,
                    left: `${left}%`,
                }} >
            </div>
        );
    }
}

interface LightSignalState {
    webSocket: WebSocket | null,
    connected: boolean;
    additionalLightTop: boolean;
    red: boolean;
    green: 'off' | 'on' | 'blinking';
    yellow: boolean;
    bulb04: boolean;
    bulb05: boolean;
    bulb06: boolean;
    additionalLightBottom: boolean;
    bulb08: boolean;
}

class LightSignal extends Component<{}, LightSignalState> {
    constructor(p: {}) {
        super(p);
        this.state = {
            webSocket: null,
            connected: false,
            additionalLightTop: false,
            red: false,
            green: 'off',
            yellow: false,
            bulb04: false,
            bulb05: false,
            bulb06: false,
            additionalLightBottom: false,
            bulb08: false,
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
        this.showSignalState(state);
    }

    private showSignalState(state: SimulatorState) {
        this.setState({
            connected: true, // state.setup ?? false,
        });

        this.showMainAspect(state.mainAspect);
    }

    private showMainAspect(aspect: MainAspect) {
        switch (aspect) {
            case MainAspect.Hp0:
                this.setState({
                    additionalLightTop: false,
                    red: true,
                    green: 'off',
                    yellow: false,
                    bulb04: false,
                    bulb05: false,
                    bulb06: false,
                    additionalLightBottom: false,
                    bulb08: false,
                });
                break;
            case MainAspect.Hp0Fahrtanzeiger:
                this.setState({
                    additionalLightTop: false,
                    red: true,
                    green: 'off',
                    yellow: false,
                    bulb04: false,
                    bulb05: false,
                    bulb06: false,
                    additionalLightBottom: false,
                    bulb08: false,
                });
                break;
            case MainAspect.Ks1:
                this.setState({
                    additionalLightTop: false,
                    red: false,
                    green: 'on',
                    yellow: false,
                    bulb04: false,
                    bulb05: false,
                    bulb06: false,
                    additionalLightBottom: false,
                    bulb08: false,
                });
                break;
            case MainAspect.Ks1Blinking:
                this.setState({
                    additionalLightTop: false,
                    red: false,
                    green: 'blinking',
                    yellow: false,
                    bulb04: false,
                    bulb05: false,
                    bulb06: false,
                    additionalLightBottom: false,
                    bulb08: false,
                });
                break;
            case MainAspect.Ks2:
                this.setState({
                    additionalLightTop: false,
                    red: false,
                    green: 'off',
                    yellow: true,
                    bulb04: false,
                    bulb05: false,
                    bulb06: false,
                    additionalLightBottom: false,
                    bulb08: false,
                });
                break;
            case MainAspect.Ks2Additional:
                this.setState({
                    additionalLightTop: false,
                    red: false,
                    green: 'off',
                    yellow: true,
                    bulb04: false,
                    bulb05: false,
                    bulb06: false,
                    additionalLightBottom: false,
                    bulb08: false,
                });
                break;
            default:
                this.setState({
                    additionalLightTop: false,
                    red: false,
                    green: 'off',
                    yellow: false,
                    bulb04: false,
                    bulb05: false,
                    bulb06: false,
                    additionalLightBottom: true,
                    bulb08: false,
                });
        }
    }

    render() {
        return (
            <div className="light-signal">
                <div className="plate">
                    <Bulb
                        color="#ffffff"
                        size="small"
                        top={15}
                        left={30}
                        blinking={false}
                        on={this.state.connected ? this.state.additionalLightTop : false}
                    ></Bulb>
                    <Bulb
                        color="#ff0000"
                        size="normal"
                        top={30}
                        left={50}
                        blinking={false}
                        on={this.state.connected ? this.state.red : false}
                    ></Bulb>
                    <Bulb
                        color="#00ff00"
                        size="normal"
                        top={50}
                        left={27.5}
                        blinking={this.state.connected ? this.state.green === 'blinking' : false}
                        on={this.state.connected ? this.state.green === 'on' || this.state.green === 'blinking' : false}
                    ></Bulb>
                    <Bulb
                        color="#ffd503"
                        size="normal"
                        top={50}
                        left={72.5}
                        blinking={false}
                        on={this.state.connected ? this.state.yellow : false}
                    ></Bulb>

                    <Bulb
                        color="#ffff00"
                        size="small"
                        top={70}
                        left={32.5}
                        blinking={false}
                        on={this.state.connected ? this.state.bulb04 : false}
                    ></Bulb>
                    <Bulb
                        color="#ffffff"
                        size="small"
                        top={70}
                        left={50}
                        blinking={false}
                        on={this.state.connected ? this.state.bulb05 : false}
                    ></Bulb>
                    <Bulb
                        color="#ffff00"
                        size="small"
                        top={70}
                        left={67.5}
                        blinking={false}
                        on={this.state.connected ? this.state.bulb06 : false}
                    ></Bulb>

                    <Bulb
                        color="#ffffff"
                        size="small"
                        top={85}
                        left={15}
                        blinking={false}
                        on={this.state.connected ? this.state.additionalLightBottom : false}
                    ></Bulb>
                    <Bulb
                        color="#ffff00"
                        size="small"
                        top={85}
                        left={50}
                        blinking={false}
                        on={this.state.connected ? this.state.bulb08 : false}
                    ></Bulb>
                </div>
                <div className="break"></div>
                <div className="mast-sign-white">
                    <div className="mast-sign-red"></div>
                </div>
                <div className="break"></div>
                <div className="announcement-indicator">
                    <svg viewBox="0 130 82 155" xmlns="http://www.w3.org/2000/svg">
                        <g>
                            <path transform="rotate(-180 40.098 178.798)" d="m1.0975,356.29813l38.99997,-355l39.00003,355l-78,0z" stroke="#0f0f00" strokeWidth="3px" fill="#ffff00"/>
                        </g>
                    </svg>
                </div>
            </div>
        );
    }
}

export default LightSignal;
