import React, { Component } from 'react';
import './Point.css';
import * as google_protobuf_empty_pb from 'google-protobuf/google/protobuf/empty_pb';
import { PointClient } from './proto/PointServiceClientPb';

import { PreventedPosition, AbilityToMoveMessage, AbilityToMove, PreventedPositionMessage, DegradedPositionMessage } from './proto/point_pb';
import { PointState, SimulatedPointState, SimulatorConfiguration } from './App';

type PointProps = { initialized: boolean, pointState: PointState | null, simulatedPointState: SimulatedPointState | null, simulatorConfiguration: SimulatorConfiguration | null };

class Point extends Component<PointProps, {}> {
    constructor(p: PointProps) {
        super(p);
        this.state = {
            webSocket: null,
            connected: false,
            position: '',
        };
    }

    render() {
        const url = window.location.protocol + '//' + window.location.host + (window.location.pathname.endsWith('/') ? window.location.pathname.slice(0, -1) : window.location.pathname);
        const client = new PointClient(url);

        return (
            <div className="point">
                <h1>Point Simulator</h1>
                <h2>Connection to Electronic Interlocking</h2>
                <p>{this.props.initialized ? 'connected' : 'disconnected'}</p>
                <h2>Simulator Configuration</h2>
                <p>Observe ability to move: {this.props.simulatorConfiguration?.observeAbilityToMove ? 'yes' : 'no'}</p>
                <p>All point machines crucial: {this.props.simulatorConfiguration?.allPointMachinesCrucial ? 'yes' : 'no'}</p>
                <p>Connection protocol: {this.props.simulatorConfiguration?.connectionProtocol}</p>

                <h2>Point State</h2>
                <p>Last Commanded Point Position: {this.props.pointState?.lastCommandedPointPosition}</p>
                <p>Point Position: {this.props.pointState?.pointPosition}</p>
                <p>Degraded Point Position: {this.props.pointState?.degradedPointPosition}</p>
                <button onClick={async () => {
                    let request = new DegradedPositionMessage();
                    request.setDegradedposition(false);
                    await client.putIntoUnintendedPosition(request, null);
                }}>Set to unintended and not degraded position</button>
                <button onClick={async () => {
                    let request = new DegradedPositionMessage();
                    request.setDegradedposition(true);
                    await client.putIntoUnintendedPosition(request, null);
                }}>Set to unintended and degraded position</button>
                <p>Ability to Move: {this.props.pointState?.abilityToMove}</p>
                {this.props.simulatorConfiguration?.observeAbilityToMove && (
                    <>
                        <button onClick={async () => {
                            let request = new AbilityToMoveMessage();
                            request.setAbility(AbilityToMove.ABLE_TO_MOVE);
                            await client.setAbilityToMove(request, null);
                        }}>Enable ability to move</button>
                        <button onClick={async () => {
                            let request = new AbilityToMoveMessage();
                            request.setAbility(AbilityToMove.UNABLE_TO_MOVE);
                            await client.setAbilityToMove(request, null);
                        }}>Disable ability to move</button>
                    </>
                )}

                <h2>Enabled Irregularities</h2>
                <p>Prevent position left: {this.props.simulatedPointState?.preventedPositionLeft}</p>
                <button onClick={async () => {
                    let request = new PreventedPositionMessage();
                    request.setPosition(PreventedPosition.SETNOENDPOSITION);
                    request.setDegradedposition(false)
                    await client.schedulePreventLeftEndPosition(request, null);
                }}>Prevent position left</button>
                <p>Prevent position right: {this.props.simulatedPointState?.preventedPositionRight}</p>
                <button onClick={async () => {
                    let request = new PreventedPositionMessage();
                    request.setPosition(PreventedPosition.SETNOENDPOSITION);
                    request.setDegradedposition(false)
                    await client.schedulePreventRightEndPosition(request, null);
                }}>Prevent position right</button>
                <p>Degraded position left: {this.props.simulatedPointState?.degradedPositionLeft ? 'yes' : 'no'}</p>
                <button onClick={async () => {
                    let request = new PreventedPositionMessage();
                    request.setPosition(PreventedPosition.SETNOENDPOSITION);
                    request.setDegradedposition(true)
                    await client.schedulePreventLeftEndPosition(request, null);
                }}>Degrade position left</button>
                <p>Degraded position right: {this.props.simulatedPointState?.degradedPositionRight ? 'yes' : 'no'}</p>
                <button onClick={async () => {
                    let request = new PreventedPositionMessage();
                    request.setPosition(PreventedPosition.SETNOENDPOSITION);
                    request.setDegradedposition(true)
                    await client.schedulePreventRightEndPosition(request, null);
                }}>Degrade position right</button>
                <p>Simulate timeout left: {this.props.simulatedPointState?.simulateTimeoutLeft ? 'yes' : 'no'}</p>
                <button onClick={async () => {
                    await client.scheduleTimeoutLeft(new google_protobuf_empty_pb.Empty(), null);
                }}>Simulate timeout on left movement</button>
                <p>Simulate timeout right: {this.props.simulatedPointState?.simulateTimeoutRight ? 'yes' : 'no'}</p>
                <button onClick={async () => {
                    await client.scheduleTimeoutRight(new google_protobuf_empty_pb.Empty(), null);
                }}>Simulate timeout on right movement</button>
                <p></p>
                <button onClick={async () => {
                    await client.reset(new google_protobuf_empty_pb.Empty(), null);
                }}>Reset</button>
            </div>
        );
    }
}

export default Point;
