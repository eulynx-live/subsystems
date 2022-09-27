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

export interface LightSignalProps {
    connected: boolean;
    setup: boolean;
    id: string;
    suspended: boolean;
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

export class LightSignal extends Component<LightSignalProps, {}> {
    constructor(p: LightSignalProps) {
        super(p);
    }

    suspend() {
        var loc = window.location, new_uri;
        new_uri = loc.protocol;
        new_uri += "//" + loc.host;
        if (loc.pathname.endsWith('/')) {
            new_uri += loc.pathname + "suspend";
        } else {
            new_uri += loc.pathname + "/suspend";
        }

        new_uri += "/" + this.props.id;

        fetch(new_uri, { method: "post" });
    }

    activate() {
        var loc = window.location, new_uri;
        new_uri = loc.protocol;
        new_uri += "//" + loc.host;
        if (loc.pathname.endsWith('/')) {
            new_uri += loc.pathname + "activate";
        } else {
            new_uri += loc.pathname + "/activate";
        }

        new_uri += "/" + this.props.id;

        fetch(new_uri, { method: "post" });
    }

    render() {
        return (
            <div className="light-signal-wrapper">
                <h1>{this.props.id}</h1>

                {this.props.setup && !this.props.suspended && <button onClick={() => this.suspend()}>Suspend</button>}
                {this.props.connected && this.props.suspended && <button onClick={() => this.activate()}>Activate</button>}

                <div className="light-signal">
                    <div className="plate">
                        <Bulb
                            color="#ffffff"
                            size="small"
                            top={15}
                            left={30}
                            blinking={false}
                            on={this.props.connected ? this.props.additionalLightTop : false}
                        ></Bulb>
                        <Bulb
                            color="#ff0000"
                            size="normal"
                            top={30}
                            left={50}
                            blinking={false}
                            on={this.props.connected ? this.props.red : false}
                        ></Bulb>
                        <Bulb
                            color="#00ff00"
                            size="normal"
                            top={50}
                            left={27.5}
                            blinking={this.props.connected ? this.props.green === 'blinking' : false}
                            on={this.props.connected ? this.props.green === 'on' || this.props.green === 'blinking' : false}
                        ></Bulb>
                        <Bulb
                            color="#ffd503"
                            size="normal"
                            top={50}
                            left={72.5}
                            blinking={false}
                            on={this.props.connected ? this.props.yellow : false}
                        ></Bulb>

                        <Bulb
                            color="#ffff00"
                            size="small"
                            top={70}
                            left={32.5}
                            blinking={false}
                            on={this.props.connected ? this.props.bulb04 : false}
                        ></Bulb>
                        <Bulb
                            color="#ffffff"
                            size="small"
                            top={70}
                            left={50}
                            blinking={false}
                            on={this.props.connected ? this.props.bulb05 : false}
                        ></Bulb>
                        <Bulb
                            color="#ffff00"
                            size="small"
                            top={70}
                            left={67.5}
                            blinking={false}
                            on={this.props.connected ? this.props.bulb06 : false}
                        ></Bulb>

                        <Bulb
                            color="#ffffff"
                            size="small"
                            top={85}
                            left={15}
                            blinking={false}
                            on={this.props.connected ? this.props.additionalLightBottom : false}
                        ></Bulb>
                        <Bulb
                            color="#ffff00"
                            size="small"
                            top={85}
                            left={50}
                            blinking={false}
                            on={this.props.connected ? this.props.bulb08 : false}
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
            </div>
        );
    }
}
