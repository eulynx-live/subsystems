import React, { useState } from 'react';
import _uniqueId from 'lodash/uniqueId';
import './Point.css';
import * as google_protobuf_empty_pb from 'google-protobuf/google/protobuf/empty_pb';
import { PointClient } from './proto/PointServiceClientPb';
import logo from './logo.png';

import {
  PreventedPosition, AbilityToMoveMessage, AbilityToMove, PreventedPositionMessage, DegradedPositionMessage, EnableMovementFailedMessage,
  EnableInitializationTimeoutMessage,
} from './proto/point_pb';
import { PointState, SimulatedPointState, SimulatorConfiguration } from './App';

type PointProps = { initialized: boolean, pointState: PointState | null, simulatedPointState: SimulatedPointState | null, simulatorConfiguration: SimulatorConfiguration | null };

function PropertyLabel({ children }: { children: React.ReactNode }) {
  return (
    <div className="m-3 text-sm inline-flex font-medium bg-slate-100 dark:bg-slate-300 text-slate-600 dark:text-slate-600 rounded-full text-center px-2.5 py-1">
      {children}
    </div>
  );
}

function SemiBoldPropertyLabel({ children }: { children: React.ReactNode }) {
  return (
    <div className="text-xs font-semibold text-slate-400 dark:text-slate-500 uppercase">
      {children}
    </div>
  );
}

function Toggle({
  active, label, disabled, onChange,
}: { active: boolean, disabled?: boolean, onChange?: (active: boolean) => void, label: string }) {
  const [id] = useState(_uniqueId('prefix-'));
  return (
    <div className="flex items-center m-3">
      <div className="form-switch">
        <input id={id} type="checkbox" checked={active} className="sr-only" onChange={() => onChange?.(!active)} disabled={disabled} />
        <label className="bg-slate-400 dark:bg-slate-700" htmlFor={id}>
          <span className="bg-white shadow-sm" aria-hidden="true" />
          <span className="sr-only">{label}</span>
        </label>
      </div>
      <div className="text-sm ml-2">{label}</div>
    </div>
  );
}

function ButtonGroup({ items }: { items: { active: boolean, label: string, disabled?: boolean, onClick?: (index: number) => void }[] }) {
  const activeClassName = 'btn bg-slate-200 dark:bg-slate-900 border-slate-200 dark:border-slate-700 text-indigo-500 rounded-none first:rounded-l last:rounded-r';
  const defaultClassName = 'btn bg-white dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-600 dark:text-slate-300 rounded-none first:rounded-l last:rounded-r';
  const enabledClassName = ' hover:bg-slate-50 dark:hover:bg-slate-900';
  const disabledClassName = ' cursor-auto';

  return (
    <div className="flex flex-wrap items-center">
      <div className="m-1.5">
        <div className="flex sm:flex-row -space-x-px">
          {items.map((item, index) => (
            <button key={item.label} type="button" onClick={() => item.disabled || item.onClick?.(index)} className={(item.active ? activeClassName : defaultClassName) + (item.disabled ? disabledClassName : enabledClassName)}>{item.label}</button>
          ))}
        </div>
      </div>
    </div>
  );
}

function Point({
  simulatorConfiguration, pointState, simulatedPointState, initialized,
}: PointProps) {
  const sendCommand = async (sender: (client: PointClient) => Promise<unknown>) => {
    const url = `${window.location.protocol}//${window.location.host}${window.location.pathname.endsWith('/') ? window.location.pathname.slice(0, -1) : window.location.pathname}`;
    const client = new PointClient(url);
    try {
      await sender(client);
    } catch (e) {
      // eslint-disable-next-line no-alert
      alert(e);
    }
  };

  const unintendedLabel = simulatorConfiguration?.connectionProtocol === 'EulynxBaseline4R1' ? 'Trailed' : 'Unintended';

  return (
    <div className="p-10">
      <div className="sm:flex sm:justify-between sm:items-center mb-5">

        {/* Left: Title */}
        <div className="grid grid-flow-col sm:auto-cols-max justify-start sm:justify-end gap-2 mb-4 sm:mb-0">
          <img src={logo} width="32" height="32" alt="logo" />
          <h1 className="text-xl font-semibold text-slate-800 dark:text-slate-100 pb-4">
            Point Simulator
          </h1>
        </div>

        {/* Right: Actions */}
        <div className="grid grid-flow-col sm:auto-cols-max justify-start sm:justify-end gap-2">
          <a href="https://redirect.systemslab21.com/PointSimulatorDocs" target="_blank" rel="noreferrer" className="btn bg-white dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-600 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-900">
            <svg xmlns="http://www.w3.org/2000/svg" className="icon icon-tabler icon-tabler-book mr-2 stroke-slate-600 dark:stroke-slate-300" width="16" height="16" viewBox="0 0 24 24" strokeWidth="1.5" fill="none" strokeLinecap="round" strokeLinejoin="round">
              <path stroke="none" d="M0 0h24v24H0z" fill="none" />
              <path d="M3 19a9 9 0 0 1 9 0a9 9 0 0 1 9 0" />
              <path d="M3 6a9 9 0 0 1 9 0a9 9 0 0 1 9 0" />
              <path d="M3 6l0 13" />
              <path d="M12 6l0 13" />
              <path d="M21 6l0 13" />
            </svg>
            Readme
          </a>
          <a href="/point.proto" download className="btn bg-white dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-600 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-900">
            <svg xmlns="http://www.w3.org/2000/svg" className="icon icon-tabler icon-tabler-file-download mr-2 stroke-slate-600 dark:stroke-slate-300" width="16" height="16" viewBox="0 0 24 24" strokeWidth="1.5" stroke="#2c3e50" fill="none" strokeLinecap="round" strokeLinejoin="round">
              <path stroke="none" d="M0 0h24v24H0z" fill="none" />
              <path d="M14 3v4a1 1 0 0 0 1 1h4" />
              <path d="M17 21h-10a2 2 0 0 1 -2 -2v-14a2 2 0 0 1 2 -2h7l5 5v11a2 2 0 0 1 -2 2z" />
              <path d="M12 17v-6" />
              <path d="M9.5 14.5l2.5 2.5l2.5 -2.5" />
            </svg>
            Download .proto
          </a>
        </div>
      </div>

      <div className="grid grid-cols-12 gap-6">

        <div className="col-span-full sm:col-span-6 xl:col-span-4 bg-white dark:bg-slate-800 shadow-lg rounded-sm border border-slate-200 dark:border-slate-700 p-4">
          <h2 className="text-l font-semibold text-slate-800 dark:text-slate-100">Simulator Configuration</h2>
          <div className="py-3">
            <SemiBoldPropertyLabel>
              Connection Protocol
            </SemiBoldPropertyLabel>
            <PropertyLabel>
              {simulatorConfiguration?.connectionProtocol ?? 'Loading...'}
            </PropertyLabel>
            <SemiBoldPropertyLabel>
              Point technical identifier
            </SemiBoldPropertyLabel>
            <PropertyLabel>
              {simulatorConfiguration?.localId}
            </PropertyLabel>
            <SemiBoldPropertyLabel>
              Electronic interlocking technical identifier
            </SemiBoldPropertyLabel>
            <PropertyLabel>
              {simulatorConfiguration?.remoteId}
            </PropertyLabel>
            <SemiBoldPropertyLabel>
              Simulated Point Transitioning Time
            </SemiBoldPropertyLabel>
            <PropertyLabel>
              {simulatorConfiguration?.simulatedTransitioningTimeSeconds}
              {' '}
              s
            </PropertyLabel>
            <div className="flex flex-row items-center">
              <SemiBoldPropertyLabel>
                PDI Version
              </SemiBoldPropertyLabel>
              <PropertyLabel>
                {simulatorConfiguration?.pdiVersion}
              </PropertyLabel>
              <SemiBoldPropertyLabel>
                PDI Checksum
              </SemiBoldPropertyLabel>
              <PropertyLabel>
                {simulatorConfiguration?.pdiChecksum}
              </PropertyLabel>
            </div>
            <Toggle label="Observe Ability to Move" active={simulatorConfiguration?.observeAbilityToMove || false} disabled />
            <Toggle label="All Point Machines are Crucial" active={simulatorConfiguration?.allPointMachinesCrucial || false} disabled />
          </div>
        </div>

        <div className="col-span-full sm:col-span-6 xl:col-span-4 bg-white dark:bg-slate-800 shadow-lg rounded-sm border border-slate-200 dark:border-slate-700 p-4">
          <h2 className="text-l font-semibold text-slate-800 dark:text-slate-100">Point State</h2>
          <div className="py-3">
            <SemiBoldPropertyLabel>
              Electronic Interlocking Connection
            </SemiBoldPropertyLabel>
            <div className="m-3 text-sm inline-flex font-medium bg-slate-100 dark:bg-slate-300 text-slate-600 dark:text-slate-600 rounded-full text-center px-2.5 py-1">{initialized ? 'connected' : 'disconnected'}</div>
          </div>

          <div className="py-3">
            <SemiBoldPropertyLabel>
              End Position
            </SemiBoldPropertyLabel>
            <ButtonGroup items={[
              { label: 'Left', active: pointState?.pointPosition === 'Left' ?? false, disabled: true },
              {
                label: 'No End Position',
                active: pointState?.pointPosition === 'NoEndPosition' ?? false,
                disabled: !(pointState?.pointPosition === 'Left' || pointState?.pointPosition === 'Right'),
                onClick: () => sendCommand(async (client) => {
                  await client.putIntoNoEndPosition(new DegradedPositionMessage().setDegradedposition(false), null);
                }),
              },
              {
                label: unintendedLabel,
                active: pointState?.pointPosition === 'UnintendedPosition' ?? false,
                disabled: !(pointState?.pointPosition === 'Left' || pointState?.pointPosition === 'Right'),
                onClick: () => sendCommand(async (client) => {
                  if (simulatorConfiguration?.connectionProtocol === 'EulynxBaseline4R1') {
                    await client.putIntoTrailedPosition(new DegradedPositionMessage().setDegradedposition(false), null);
                  } else if (simulatorConfiguration?.connectionProtocol === 'EulynxBaseline4R2') {
                    await client.putIntoUnintendedPosition(new DegradedPositionMessage().setDegradedposition(false), null);
                  }
                }),
              },
              { label: 'Right', active: pointState?.pointPosition === 'Right' ?? false, disabled: true },
            ]}
            />
          </div>

          <div className="py-3">
            <SemiBoldPropertyLabel>
              Degraded Point Position
            </SemiBoldPropertyLabel>
            <ButtonGroup items={[
              { label: 'Left', active: pointState?.degradedPointPosition === 'DegradedLeft' ?? false, disabled: true },
              { label: 'Not Degraded', active: pointState?.degradedPointPosition === 'NotDegraded' ?? false, disabled: true },
              { label: 'N/A', active: pointState?.degradedPointPosition === 'NotApplicable' ?? false, disabled: true },
              { label: 'Right', active: pointState?.degradedPointPosition === 'DegradedRight' ?? false, disabled: true },
            ]}
            />
          </div>

          <div className="py-3">
            <SemiBoldPropertyLabel>
              Last Commanded Point Position
            </SemiBoldPropertyLabel>
            <ButtonGroup items={[
              { label: 'Left', active: pointState?.lastCommandedPointPosition === 'Left' ?? false, disabled: true },
              { label: 'Right', active: pointState?.lastCommandedPointPosition === 'Right' ?? false, disabled: true },
            ]}
            />
          </div>

          {simulatorConfiguration?.observeAbilityToMove && (
            <div className="py-3">
              <SemiBoldPropertyLabel>
                Ability to Move
              </SemiBoldPropertyLabel>
              <ButtonGroup items={[
                {
                  label: 'Able to Move',
                  active: pointState?.abilityToMove === 'AbleToMove' ?? false,
                  onClick: () => sendCommand((client) => client.setAbilityToMove(new AbilityToMoveMessage().setAbility(AbilityToMove.ABLE_TO_MOVE), null)),
                },
                {
                  label: 'Unable to Move',
                  active: pointState?.abilityToMove === 'UnableToMove' ?? false,
                  onClick: () => sendCommand((client) => client.setAbilityToMove(new AbilityToMoveMessage().setAbility(AbilityToMove.UNABLE_TO_MOVE), null)),
                },
              ]}
              />
            </div>
          )}
        </div>

        <div className="col-span-full sm:col-span-12 xl:col-span-8 bg-white dark:bg-slate-800 shadow-lg rounded-sm border border-slate-200 dark:border-slate-700 p-4">
          <h2 className="text-l font-semibold text-slate-800 dark:text-slate-100">Simulate Irregularities</h2>
          <div className="grid grid-flow-col">
            <div className="py-3">
              <SemiBoldPropertyLabel>
                Prevent left end position
              </SemiBoldPropertyLabel>

              <Toggle
                label="Trigger Movement Failed"
                active={simulatedPointState?.simulateTimeoutLeft || false}
                onChange={(enable) => sendCommand((client) => client.scheduleTimeoutLeft(new EnableMovementFailedMessage().setEnablemovementfailed(enable), null))}
              />

              <ButtonGroup items={[
                {
                  label: 'Do not prevent',
                  active: simulatedPointState?.preventedPositionLeft === 'DoNotPrevent' ?? false,
                  onClick: () => sendCommand((client) => client.schedulePreventLeftEndPosition(
                    new PreventedPositionMessage()
                      .setPosition(PreventedPosition.DONOTPREVENT)
                      .setDegradedposition(simulatedPointState?.degradedPositionLeft ?? false),
                    null,
                  )),
                },
                {
                  label: unintendedLabel,
                  active: simulatedPointState?.preventedPositionLeft === 'SetUnintendedOrTrailed' ?? false,
                  onClick: () => sendCommand((client) => client.schedulePreventLeftEndPosition(
                    new PreventedPositionMessage()
                      .setPosition(PreventedPosition.SETUNINTENDEDORTRAILED)
                      .setDegradedposition(simulatedPointState?.degradedPositionLeft ?? false),
                    null,
                  )),
                },
                {
                  label: 'No End Position',
                  active: simulatedPointState?.preventedPositionLeft === 'SetNoEndPosition' ?? false,
                  onClick: () => sendCommand((client) => client.schedulePreventLeftEndPosition(
                    new PreventedPositionMessage()
                      .setPosition(PreventedPosition.SETNOENDPOSITION)
                      .setDegradedposition(simulatedPointState?.degradedPositionLeft ?? false),
                    null,
                  )),
                },
              ]}
              />

              <Toggle
                label="Degraded Left Position"
                active={simulatedPointState?.degradedPositionLeft || false}
                onChange={(enable) => sendCommand((client) => client.schedulePreventLeftEndPosition(new PreventedPositionMessage().setDegradedposition(enable).setPosition({
                  DoNotPrevent: PreventedPosition.DONOTPREVENT,
                  SetUnintendedOrTrailed: PreventedPosition.SETUNINTENDEDORTRAILED,
                  SetNoEndPosition: PreventedPosition.SETNOENDPOSITION,
                  none: null,
                }[simulatedPointState?.preventedPositionLeft ?? 'none'] ?? PreventedPosition.DONOTPREVENT), null))}
              />
            </div>

            <div className="py-3">
              <div className="text-xs font-semibold text-slate-400 dark:text-slate-500 uppercase">
                Prevent right end position
              </div>

              <Toggle
                label="Trigger Movement Failed"
                active={simulatedPointState?.simulateTimeoutRight || false}
                onChange={(enable) => sendCommand((client) => client.scheduleTimeoutRight(new EnableMovementFailedMessage().setEnablemovementfailed(enable), null))}
              />

              <ButtonGroup items={[
                {
                  label: 'Do not prevent',
                  active: simulatedPointState?.preventedPositionRight === 'DoNotPrevent' ?? false,
                  onClick: () => sendCommand((client) => client.schedulePreventRightEndPosition(
                    new PreventedPositionMessage()
                      .setPosition(PreventedPosition.DONOTPREVENT)
                      .setDegradedposition(simulatedPointState?.degradedPositionRight ?? false),
                    null,
                  )),
                },
                {
                  label: unintendedLabel,
                  active: simulatedPointState?.preventedPositionRight === 'SetUnintendedOrTrailed' ?? false,
                  onClick: () => sendCommand((client) => client.schedulePreventRightEndPosition(
                    new PreventedPositionMessage()
                      .setPosition(PreventedPosition.SETUNINTENDEDORTRAILED)
                      .setDegradedposition(simulatedPointState?.degradedPositionRight ?? false),
                    null,
                  )),
                },
                {
                  label: 'No End Position',
                  active: simulatedPointState?.preventedPositionRight === 'SetNoEndPosition' ?? false,
                  onClick: () => sendCommand((client) => client.schedulePreventRightEndPosition(
                    new PreventedPositionMessage()
                      .setPosition(PreventedPosition.SETNOENDPOSITION)
                      .setDegradedposition(simulatedPointState?.degradedPositionRight ?? false),
                    null,
                  )),
                },
              ]}
              />

              <Toggle
                label="Degraded Right Position"
                active={simulatedPointState?.degradedPositionRight || false}
                onChange={(enable) => sendCommand((client) => client.schedulePreventRightEndPosition(
                  new PreventedPositionMessage()
                    .setDegradedposition(enable)
                    .setPosition({
                      DoNotPrevent: PreventedPosition.DONOTPREVENT,
                      SetUnintendedOrTrailed: PreventedPosition.SETUNINTENDEDORTRAILED,
                      SetNoEndPosition: PreventedPosition.SETNOENDPOSITION,
                      none: null,
                    }[simulatedPointState?.preventedPositionRight ?? 'none'] ?? PreventedPosition.DONOTPREVENT),
                  null,
                ))}
              />
            </div>
          </div>
          <div>
            <SemiBoldPropertyLabel>
              Connectivity
            </SemiBoldPropertyLabel>
            <div>
              <button
                type="button"
                onClick={() => sendCommand((client) => client.reset(new google_protobuf_empty_pb.Empty(), null))}
                className="m-3 btn dark:bg-slate-800 border-slate-200 dark:border-slate-700 hover:border-slate-300 dark:hover:border-slate-600 text-rose-500"
              >
                Reset RaSTA Connection

              </button>
              <div>
                <Toggle
                  label="Enable Initialization Timeout"
                  active={simulatedPointState?.simulateInitializationTimeout || false}
                  onChange={
                    (enable) => sendCommand((client) => client.scheduleInitializationTimeout(new EnableInitializationTimeoutMessage().setEnableinitializationtimeout(enable), null))
                  }
                />
              </div>
            </div>
          </div>
        </div>

      </div>

    </div>
  );
}

export default Point;
