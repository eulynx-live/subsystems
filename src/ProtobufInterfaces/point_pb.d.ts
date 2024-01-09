import * as jspb from 'google-protobuf'

import * as google_protobuf_empty_pb from 'google-protobuf/google/protobuf/empty_pb';


export class AbilityToMoveMessage extends jspb.Message {
  getAbility(): AbilityToMove;
  setAbility(value: AbilityToMove): AbilityToMoveMessage;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AbilityToMoveMessage.AsObject;
  static toObject(includeInstance: boolean, msg: AbilityToMoveMessage): AbilityToMoveMessage.AsObject;
  static serializeBinaryToWriter(message: AbilityToMoveMessage, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AbilityToMoveMessage;
  static deserializeBinaryFromReader(message: AbilityToMoveMessage, reader: jspb.BinaryReader): AbilityToMoveMessage;
}

export namespace AbilityToMoveMessage {
  export type AsObject = {
    ability: AbilityToMove,
  }
}

export class SciMessage extends jspb.Message {
  getMessage(): Uint8Array | string;
  getMessage_asU8(): Uint8Array;
  getMessage_asB64(): string;
  setMessage(value: Uint8Array | string): SciMessage;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): SciMessage.AsObject;
  static toObject(includeInstance: boolean, msg: SciMessage): SciMessage.AsObject;
  static serializeBinaryToWriter(message: SciMessage, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): SciMessage;
  static deserializeBinaryFromReader(message: SciMessage, reader: jspb.BinaryReader): SciMessage;
}

export namespace SciMessage {
  export type AsObject = {
    message: Uint8Array | string,
  }
}

export class PreventedPositionMessage extends jspb.Message {
  getPosition(): PreventedPosition;
  setPosition(value: PreventedPosition): PreventedPositionMessage;

  getDegradedposition(): boolean;
  setDegradedposition(value: boolean): PreventedPositionMessage;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): PreventedPositionMessage.AsObject;
  static toObject(includeInstance: boolean, msg: PreventedPositionMessage): PreventedPositionMessage.AsObject;
  static serializeBinaryToWriter(message: PreventedPositionMessage, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): PreventedPositionMessage;
  static deserializeBinaryFromReader(message: PreventedPositionMessage, reader: jspb.BinaryReader): PreventedPositionMessage;
}

export namespace PreventedPositionMessage {
  export type AsObject = {
    position: PreventedPosition,
    degradedposition: boolean,
  }
}

export class DegradedPositionMessage extends jspb.Message {
  getDegradedposition(): boolean;
  setDegradedposition(value: boolean): DegradedPositionMessage;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): DegradedPositionMessage.AsObject;
  static toObject(includeInstance: boolean, msg: DegradedPositionMessage): DegradedPositionMessage.AsObject;
  static serializeBinaryToWriter(message: DegradedPositionMessage, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): DegradedPositionMessage;
  static deserializeBinaryFromReader(message: DegradedPositionMessage, reader: jspb.BinaryReader): DegradedPositionMessage;
}

export namespace DegradedPositionMessage {
  export type AsObject = {
    degradedposition: boolean,
  }
}

export class EnableMovementFailedMessage extends jspb.Message {
  getEnablemovementfailed(): boolean;
  setEnablemovementfailed(value: boolean): EnableMovementFailedMessage;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): EnableMovementFailedMessage.AsObject;
  static toObject(includeInstance: boolean, msg: EnableMovementFailedMessage): EnableMovementFailedMessage.AsObject;
  static serializeBinaryToWriter(message: EnableMovementFailedMessage, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): EnableMovementFailedMessage;
  static deserializeBinaryFromReader(message: EnableMovementFailedMessage, reader: jspb.BinaryReader): EnableMovementFailedMessage;
}

export namespace EnableMovementFailedMessage {
  export type AsObject = {
    enablemovementfailed: boolean,
  }
}

export class EnableInitializationTimeoutMessage extends jspb.Message {
  getEnableinitializationtimeout(): boolean;
  setEnableinitializationtimeout(value: boolean): EnableInitializationTimeoutMessage;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): EnableInitializationTimeoutMessage.AsObject;
  static toObject(includeInstance: boolean, msg: EnableInitializationTimeoutMessage): EnableInitializationTimeoutMessage.AsObject;
  static serializeBinaryToWriter(message: EnableInitializationTimeoutMessage, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): EnableInitializationTimeoutMessage;
  static deserializeBinaryFromReader(message: EnableInitializationTimeoutMessage, reader: jspb.BinaryReader): EnableInitializationTimeoutMessage;
}

export namespace EnableInitializationTimeoutMessage {
  export type AsObject = {
    enableinitializationtimeout: boolean,
  }
}

export class PointPositionMessage extends jspb.Message {
  getPosition(): PointPosition;
  setPosition(value: PointPosition): PointPositionMessage;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): PointPositionMessage.AsObject;
  static toObject(includeInstance: boolean, msg: PointPositionMessage): PointPositionMessage.AsObject;
  static serializeBinaryToWriter(message: PointPositionMessage, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): PointPositionMessage;
  static deserializeBinaryFromReader(message: PointPositionMessage, reader: jspb.BinaryReader): PointPositionMessage;
}

export namespace PointPositionMessage {
  export type AsObject = {
    position: PointPosition,
  }
}

export class PointDegradedPositionMessage extends jspb.Message {
  getPosition(): PointDegradedPosition;
  setPosition(value: PointDegradedPosition): PointDegradedPositionMessage;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): PointDegradedPositionMessage.AsObject;
  static toObject(includeInstance: boolean, msg: PointDegradedPositionMessage): PointDegradedPositionMessage.AsObject;
  static serializeBinaryToWriter(message: PointDegradedPositionMessage, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): PointDegradedPositionMessage;
  static deserializeBinaryFromReader(message: PointDegradedPositionMessage, reader: jspb.BinaryReader): PointDegradedPositionMessage;
}

export namespace PointDegradedPositionMessage {
  export type AsObject = {
    position: PointDegradedPosition,
  }
}

export enum PointPosition { 
  RIGHT = 0,
  LEFT = 1,
  NOENDPOSITION = 2,
  UNINTENDEDPOSITION = 3,
}
export enum PointDegradedPosition { 
  DEGRADEDRIGHT = 0,
  DEGRADEDLEFT = 1,
  NOTDEGRADED = 2,
  NOTAPPLICABLE = 3,
}
export enum PreventedPosition { 
  DONOTPREVENT = 0,
  SETUNINTENDEDORTRAILED = 1,
  SETNOENDPOSITION = 2,
}
export enum AbilityToMove { 
  ABLE_TO_MOVE = 0,
  UNABLE_TO_MOVE = 1,
}
