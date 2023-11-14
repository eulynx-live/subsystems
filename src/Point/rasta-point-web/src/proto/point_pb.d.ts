import * as jspb from 'google-protobuf'



export class PointDegradedMessage extends jspb.Message {
  getPosition(): PointPosition;
  setPosition(value: PointPosition): PointDegradedMessage;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): PointDegradedMessage.AsObject;
  static toObject(includeInstance: boolean, msg: PointDegradedMessage): PointDegradedMessage.AsObject;
  static serializeBinaryToWriter(message: PointDegradedMessage, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): PointDegradedMessage;
  static deserializeBinaryFromReader(message: PointDegradedMessage, reader: jspb.BinaryReader): PointDegradedMessage;
}

export namespace PointDegradedMessage {
  export type AsObject = {
    position: PointPosition,
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

export class SetPointMachineStateResponse extends jspb.Message {
  getNewstate(): PointMachineStateMessage | undefined;
  setNewstate(value?: PointMachineStateMessage): SetPointMachineStateResponse;
  hasNewstate(): boolean;
  clearNewstate(): SetPointMachineStateResponse;

  getSuccess(): boolean;
  setSuccess(value: boolean): SetPointMachineStateResponse;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): SetPointMachineStateResponse.AsObject;
  static toObject(includeInstance: boolean, msg: SetPointMachineStateResponse): SetPointMachineStateResponse.AsObject;
  static serializeBinaryToWriter(message: SetPointMachineStateResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): SetPointMachineStateResponse;
  static deserializeBinaryFromReader(message: SetPointMachineStateResponse, reader: jspb.BinaryReader): SetPointMachineStateResponse;
}

export namespace SetPointMachineStateResponse {
  export type AsObject = {
    newstate?: PointMachineStateMessage.AsObject,
    success: boolean,
  }
}

export class Nothing extends jspb.Message {
  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Nothing.AsObject;
  static toObject(includeInstance: boolean, msg: Nothing): Nothing.AsObject;
  static serializeBinaryToWriter(message: Nothing, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Nothing;
  static deserializeBinaryFromReader(message: Nothing, reader: jspb.BinaryReader): Nothing;
}

export namespace Nothing {
  export type AsObject = {
  }
}

export enum PointPosition { 
  RIGHT = 0,
  LEFT = 1,
  NOENDPOSITION = 2,
  TRAILED = 3,
}
