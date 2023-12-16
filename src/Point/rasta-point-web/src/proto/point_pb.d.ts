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

export class GenericSCIMessage extends jspb.Message {
  getMessage(): Uint8Array | string;
  getMessage_asU8(): Uint8Array;
  getMessage_asB64(): string;
  setMessage(value: Uint8Array | string): GenericSCIMessage;

  getDataLength(): number;
  setDataLength(value: number): GenericSCIMessage;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GenericSCIMessage.AsObject;
  static toObject(includeInstance: boolean, msg: GenericSCIMessage): GenericSCIMessage.AsObject;
  static serializeBinaryToWriter(message: GenericSCIMessage, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): GenericSCIMessage;
  static deserializeBinaryFromReader(message: GenericSCIMessage, reader: jspb.BinaryReader): GenericSCIMessage;
}

export namespace GenericSCIMessage {
  export type AsObject = {
    message: Uint8Array | string,
    dataLength: number,
  }
}

export class SimulatedPositionMessage extends jspb.Message {
  getPosition(): PreventedPosition;
  setPosition(value: PreventedPosition): SimulatedPositionMessage;

  getDegradedposition(): PointDegradedPosition;
  setDegradedposition(value: PointDegradedPosition): SimulatedPositionMessage;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): SimulatedPositionMessage.AsObject;
  static toObject(includeInstance: boolean, msg: SimulatedPositionMessage): SimulatedPositionMessage.AsObject;
  static serializeBinaryToWriter(message: SimulatedPositionMessage, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): SimulatedPositionMessage;
  static deserializeBinaryFromReader(message: SimulatedPositionMessage, reader: jspb.BinaryReader): SimulatedPositionMessage;
}

export namespace SimulatedPositionMessage {
  export type AsObject = {
    position: PreventedPosition,
    degradedposition: PointDegradedPosition,
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
  NONE = 0,
  PREVENTEDLEFT = 1,
  PREVENTEDRIGHT = 2,
  PREVENTTRAILED = 3,
}
export enum AbilityToMove { 
  UNDEFINED = 0,
  ABLE_TO_MOVE = 1,
  UNABLE_TO_MOVE = 2,
}
