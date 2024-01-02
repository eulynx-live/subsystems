import React, { useState } from 'react';
import _uniqueId from 'lodash/uniqueId';
import './Point.css';
import * as google_protobuf_empty_pb from 'google-protobuf/google/protobuf/empty_pb';
import { PointClient } from './proto/PointServiceClientPb';

import { PreventedPosition, AbilityToMoveMessage, AbilityToMove, PreventedPositionMessage, DegradedPositionMessage, EnableMovementFailedMessage } from './proto/point_pb';
import { PointState, SimulatedPointState, SimulatorConfiguration } from './App';

type PointProps = { initialized: boolean, pointState: PointState | null, simulatedPointState: SimulatedPointState | null, simulatorConfiguration: SimulatorConfiguration | null };

function Toggle({ active, label, disabled, onChange }: { active: boolean, disabled?: boolean, onChange?: (active: boolean) => void, label: string }) {
    const [id] = useState(_uniqueId('prefix-'));
    return (
        <div className="flex items-center m-3">
            <div className="form-switch">
                <input id={id} type="checkbox" checked={active} className="sr-only" onChange={() => onChange?.(!active)} disabled={disabled} />
                {/* <input  type="checkbox" defaultChecked={true} onChange={x => console.log('test')} /> */}
                <label className="bg-slate-400 dark:bg-slate-700" htmlFor={id}>
                    <span className="bg-white shadow-sm" aria-hidden="true"></span>
                    <span className="sr-only">{label}</span>
                </label>
            </div>
            <div className="text-sm ml-2">{label}</div>
        </div>
    )
}

function ButtonGroup({ items }: { items: { active: boolean, label: string, disabled?: boolean, onClick?: (index: number) => void }[] }) {
    const activeClassName = "btn bg-slate-200 dark:bg-slate-900 border-slate-200 dark:border-slate-700 text-indigo-500 rounded-none first:rounded-l last:rounded-r";
    const defaultClassName = "btn bg-white dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-600 dark:text-slate-300 rounded-none first:rounded-l last:rounded-r";
    const enabledClassName = " hover:bg-slate-50 dark:hover:bg-slate-900";
    const disabledClassName = " cursor-auto";

    return (
        <div className="flex flex-wrap items-center">
            <div className="m-1.5">
                <div className="flex flex-wrap -space-x-px">
                    {items.map((item, index) => (
                        <button key={index} type="button" onClick={() => item.disabled || item.onClick?.(index)} className={(item.active ? activeClassName : defaultClassName) + (item.disabled ? disabledClassName : enabledClassName)}>{item.label}</button>
                    ))}
                </div>
            </div>
        </div>
    )
}

function Point(props: PointProps) {
    const sendCommand = async (sender: (client: PointClient) => Promise<any>) => {
        const url = window.location.protocol + '//' + window.location.host + (window.location.pathname.endsWith('/') ? window.location.pathname.slice(0, -1) : window.location.pathname);
        const client = new PointClient(url);
        try {
            await sender(client)
        } catch (e) {
            alert(e);
        }
    };

    const unintendedLabel = props.simulatorConfiguration?.connectionProtocol === 'EulynxBaseline4R1' ? 'Trailed' : 'Unintended';

    return (
        <div className="p-10">
            <h1 className="text-xl font-semibold text-slate-800 dark:text-slate-100 pb-4">Point Simulator</h1>

            <div className="grid grid-cols-12 gap-6">

                <div className="col-span-full sm:col-span-6 xl:col-span-3 bg-white dark:bg-slate-800 shadow-lg rounded-sm border border-slate-200 dark:border-slate-700 p-4">
                    <h2 className="text-l font-semibold text-slate-800 dark:text-slate-100">Simulator Configuration</h2>
                    <div className="m-3 text-sm inline-flex font-medium bg-slate-100 dark:bg-slate-300 text-slate-600 dark:text-slate-600 rounded-full text-center px-2.5 py-1">{props.simulatorConfiguration?.connectionProtocol ?? 'Loading...'}</div>
                    <Toggle label="Observe Ability to Move" active={props.simulatorConfiguration?.observeAbilityToMove || false} disabled />
                    <Toggle label="All Point Machines Crucial" active={props.simulatorConfiguration?.allPointMachinesCrucial || false} disabled />
                </div>

                <div className="col-span-full sm:col-span-6 xl:col-span-4 bg-white dark:bg-slate-800 shadow-lg rounded-sm border border-slate-200 dark:border-slate-700 p-4">
                    <h2 className="text-l font-semibold text-slate-800 dark:text-slate-100">Point State</h2>
                    <div className="py-3">
                        <div className="text-xs font-semibold text-slate-400 dark:text-slate-500 uppercase">
                            Electronic Interlocking Connection
                        </div>
                        <div className="m-3 text-sm inline-flex font-medium bg-slate-100 dark:bg-slate-300 text-slate-600 dark:text-slate-600 rounded-full text-center px-2.5 py-1">{props.initialized ? 'connected' : 'disconnected'}</div>
                    </div>

                    <div className="py-3">
                        <div className="text-xs font-semibold text-slate-400 dark:text-slate-500 uppercase">
                            End Position
                        </div>
                        <ButtonGroup items={[
                            { label: 'Left', active: props.pointState?.pointPosition === 'Left' ?? false, disabled: true },
                            { label: 'No End Position', active: props.pointState?.pointPosition === 'NoEndPosition' ?? false, disabled: true },
                            {
                                label: unintendedLabel,
                                active: props.pointState?.pointPosition === 'UnintendedPosition' ?? false, disabled: !(props.pointState?.pointPosition === 'Left' || props.pointState?.pointPosition === 'Right'),
                                onClick: () => sendCommand(async (client) => {
                                    if (props.simulatorConfiguration?.connectionProtocol === 'EulynxBaseline4R1') {
                                        await client.putIntoTrailedPosition(new DegradedPositionMessage().setDegradedposition(false), null);
                                    } else if (props.simulatorConfiguration?.connectionProtocol === 'EulynxBaseline4R2') {
                                        await client.putIntoUnintendedPosition(new DegradedPositionMessage().setDegradedposition(false), null);
                                    }
                                })
                            },
                            { label: 'Right', active: props.pointState?.pointPosition === 'Right' ?? false, disabled: true },
                        ]} />
                    </div>

                    <div className="py-3">
                        <div className="text-xs font-semibold text-slate-400 dark:text-slate-500 uppercase">
                            Degraded Point Position
                        </div>
                        <ButtonGroup items={[
                            { label: 'Left', active: props.pointState?.degradedPointPosition === 'DegradedLeft' ?? false, disabled: true },
                            { label: 'Not Degraded', active: props.pointState?.degradedPointPosition === 'NotDegraded' ?? false, disabled: true },
                            { label: 'N/A', active: props.pointState?.degradedPointPosition === 'NotApplicable' ?? false, disabled: true },
                            { label: 'Right', active: props.pointState?.degradedPointPosition === 'DegradedRight' ?? false, disabled: true },
                        ]} />
                    </div>

                    <div className="py-3">
                        <div className="text-xs font-semibold text-slate-400 dark:text-slate-500 uppercase">
                            Last Commanded Point Position
                        </div>
                        <ButtonGroup items={[
                            { label: 'Left', active: props.pointState?.lastCommandedPointPosition === 'Left' ?? false, disabled: true },
                            { label: 'Right', active: props.pointState?.lastCommandedPointPosition === 'Right' ?? false, disabled: true },
                        ]} />
                    </div>

                    {props.simulatorConfiguration?.observeAbilityToMove && (
                        <div className="py-3">
                            <div className="text-xs font-semibold text-slate-400 dark:text-slate-500 uppercase">
                                Ability to Move
                            </div>
                            <ButtonGroup items={[
                                { label: 'Able to Move', active: props.pointState?.abilityToMove === 'AbleToMove' ?? false, onClick: () => sendCommand((client) => client.setAbilityToMove(new AbilityToMoveMessage().setAbility(AbilityToMove.ABLE_TO_MOVE), null)) },
                                { label: 'Unable to Move', active: props.pointState?.abilityToMove === 'UnableToMove' ?? false, onClick: () => sendCommand((client) => client.setAbilityToMove(new AbilityToMoveMessage().setAbility(AbilityToMove.UNABLE_TO_MOVE), null)) },
                            ]} />
                        </div>
                    )}
                </div>

                <div className="col-span-full sm:col-span-7 xl:col-span-5 bg-white dark:bg-slate-800 shadow-lg rounded-sm border border-slate-200 dark:border-slate-700 p-4">
                    <h2 className="text-l font-semibold text-slate-800 dark:text-slate-100">Simulate Irregularities</h2>

                    <div className="py-3">
                        <div className="text-xs font-semibold text-slate-400 dark:text-slate-500 uppercase">
                            Prevent left end position
                        </div>

                        <Toggle label="Trigger Movement Failed" active={props.simulatedPointState?.simulateTimeoutLeft || false}
                            onChange={(enable) => sendCommand((client) => client.scheduleTimeoutLeft(new EnableMovementFailedMessage().setEnablemovementfailed(enable), null))} />

                        <ButtonGroup items={[
                            {
                                label: 'Do not prevent', active: props.simulatedPointState?.preventedPositionLeft === 'DoNotPrevent' ?? false,
                                onClick: () => sendCommand((client) => client.schedulePreventLeftEndPosition(new PreventedPositionMessage().setPosition(PreventedPosition.DONOTPREVENT).setDegradedposition(props.simulatedPointState?.degradedPositionLeft ?? false), null))
                            },
                            {
                                label: unintendedLabel, active: props.simulatedPointState?.preventedPositionLeft === 'SetUnintendedOrTrailed' ?? false,
                                onClick: () => sendCommand((client) => client.schedulePreventLeftEndPosition(new PreventedPositionMessage().setPosition(PreventedPosition.SETUNINTENDEDORTRAILED).setDegradedposition(props.simulatedPointState?.degradedPositionLeft ?? false), null))
                            },
                            {
                                label: 'No End Position', active: props.simulatedPointState?.preventedPositionLeft === 'SetNoEndPosition' ?? false,
                                onClick: () => sendCommand((client) => client.schedulePreventLeftEndPosition(new PreventedPositionMessage().setPosition(PreventedPosition.SETNOENDPOSITION).setDegradedposition(props.simulatedPointState?.degradedPositionLeft ?? false), null))
                            },
                        ]} />

                        <Toggle label="Degraded Left Position" active={props.simulatedPointState?.degradedPositionLeft || false}
                            onChange={(enable) => sendCommand((client) => client.schedulePreventLeftEndPosition(new PreventedPositionMessage().setDegradedposition(enable).setPosition({
                                'DoNotPrevent': PreventedPosition.DONOTPREVENT,
                                'SetUnintendedOrTrailed': PreventedPosition.SETUNINTENDEDORTRAILED,
                                'SetNoEndPosition': PreventedPosition.SETNOENDPOSITION,
                                'none': null
                            }[props.simulatedPointState?.preventedPositionLeft ?? 'none'] ?? PreventedPosition.DONOTPREVENT), null))}
                        />
                    </div>

                    <div className="py-3">
                        <div className="text-xs font-semibold text-slate-400 dark:text-slate-500 uppercase">
                            Prevent right end position
                        </div>

                        <Toggle label="Trigger Movement Failed" active={props.simulatedPointState?.simulateTimeoutRight || false}
                            onChange={(enable) => sendCommand((client) => client.scheduleTimeoutRight(new EnableMovementFailedMessage().setEnablemovementfailed(enable), null))} />

                        <ButtonGroup items={[
                            {
                                label: 'Do not prevent', active: props.simulatedPointState?.preventedPositionRight === 'DoNotPrevent' ?? false,
                                onClick: () => sendCommand((client) => client.schedulePreventRightEndPosition(new PreventedPositionMessage().setPosition(PreventedPosition.DONOTPREVENT).setDegradedposition(props.simulatedPointState?.degradedPositionRight ?? false), null))
                            },
                            {
                                label: unintendedLabel, active: props.simulatedPointState?.preventedPositionRight === 'SetUnintendedOrTrailed' ?? false,
                                onClick: () => sendCommand((client) => client.schedulePreventRightEndPosition(new PreventedPositionMessage().setPosition(PreventedPosition.SETUNINTENDEDORTRAILED).setDegradedposition(props.simulatedPointState?.degradedPositionRight ?? false), null))
                            },
                            {
                                label: 'No End Position', active: props.simulatedPointState?.preventedPositionRight === 'SetNoEndPosition' ?? false,
                                onClick: () => sendCommand((client) => client.schedulePreventRightEndPosition(new PreventedPositionMessage().setPosition(PreventedPosition.SETNOENDPOSITION).setDegradedposition(props.simulatedPointState?.degradedPositionRight ?? false), null))
                            },
                        ]} />

                        <Toggle label="Degraded Right Position" active={props.simulatedPointState?.degradedPositionRight || false}
                            onChange={(enable) => sendCommand((client) => client.schedulePreventRightEndPosition(new PreventedPositionMessage().setDegradedposition(enable).setPosition({
                                'DoNotPrevent': PreventedPosition.DONOTPREVENT,
                                'SetUnintendedOrTrailed': PreventedPosition.SETUNINTENDEDORTRAILED,
                                'SetNoEndPosition': PreventedPosition.SETNOENDPOSITION,
                                'none': null
                            }[props.simulatedPointState?.preventedPositionRight ?? 'none'] ?? PreventedPosition.DONOTPREVENT), null))}
                        />
                    </div>

                    <div>
                        <div className="text-xs font-semibold text-slate-400 dark:text-slate-500 uppercase">
                            Connectivity
                        </div>
                        <button
                            type="button"
                            onClick={() => sendCommand((client) => client.reset(new google_protobuf_empty_pb.Empty(), null))}
                            className="m-3 btn dark:bg-slate-800 border-slate-200 dark:border-slate-700 hover:border-slate-300 dark:hover:border-slate-600 text-rose-500">Reset RaSTA Connection</button>
                    </div>
                </div>

            </div>

        </div>
    );
}

export default Point;
