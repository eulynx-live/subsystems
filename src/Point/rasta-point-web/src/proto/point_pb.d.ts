import * as jspb from 'google-protobuf'

import * as google_protobuf_empty_pb from 'google-protobuf/google/protobuf/empty_pb';


export class PreventedPositionMessage extends jspb.Message {
  getPosition(): PreventedPosition;
  setPosition(value: PreventedPosition): PreventedPositionMessage;

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

export class PointMachineStateMessage extends jspb.Message {
  getPointposition(): PointPosition;
  setPointposition(value: PointPosition): PointMachineStateMessage;

  getTarget(): PointMachineStateMessage.Target;
  setTarget(value: PointMachineStateMessage.Target): PointMachineStateMessage;

  getAbilitytomove(): PointMachineStateMessage.AbilityToMove;
  setAbilitytomove(value: PointMachineStateMessage.AbilityToMove): PointMachineStateMessage;

  getLastpointposition(): PointMachineStateMessage.LastPointPosition;
  setLastpointposition(value: PointMachineStateMessage.LastPointPosition): PointMachineStateMessage;

  getCrucial(): PointMachineStateMessage.Crucial;
  setCrucial(value: PointMachineStateMessage.Crucial): PointMachineStateMessage;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): PointMachineStateMessage.AsObject;
  static toObject(includeInstance: boolean, msg: PointMachineStateMessage): PointMachineStateMessage.AsObject;
  static serializeBinaryToWriter(message: PointMachineStateMessage, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): PointMachineStateMessage;
  static deserializeBinaryFromReader(message: PointMachineStateMessage, reader: jspb.BinaryReader): PointMachineStateMessage;
}

export namespace PointMachineStateMessage {
  export type AsObject = {
    pointposition: PointPosition,
    target: PointMachineStateMessage.Target,
    abilitytomove: PointMachineStateMessage.AbilityToMove,
    lastpointposition: PointMachineStateMessage.LastPointPosition,
    crucial: PointMachineStateMessage.Crucial,
  }

  export enum Target { 
    TARGET_UNDEFINED = 0,
    TARGET_LEFT = 1,
    TARGET_RIGHT = 2,
    TARGET_NONE = 3,
  }

  export enum AbilityToMove { 
    ABILITYTOMOVE_UNDEFINED = 0,
    ABILITYTOMOVE_ABLE = 1,
    ABILITYTOMOVE_UNABLE = 2,
  }

  export enum LastPointPosition { 
    LASTPOINTPOSITION_UNDEFINED = 0,
    LASTPOINTPOSITION_NONE = 1,
  }

  export enum Crucial { 
    CRUCIAL_UNDEFINED = 0,
    CRUCIAL_CRUCIAL = 1,
    CRUCIAL_NONCRUCIAL = 2,
  }
}

export enum PointPosition { 
  RIGHT = 0,
  LEFT = 1,
  NOENDPOSITION = 2,
  UNINTENDEDPOSITION = 3,
}
export enum PreventedPosition { 
  PREVENTEDLEFT = 0,
  PREVENTEDRIGHT = 1,
  TRAILED = 2,
  NONE = 3,
}
