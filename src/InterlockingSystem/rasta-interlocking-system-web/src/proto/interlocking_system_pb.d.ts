import * as jspb from 'google-protobuf'



export class InterlockingSystemCommand extends jspb.Message {
  getInterlockingsystem(): string;
  setInterlockingsystem(value: string): InterlockingSystemCommand;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): InterlockingSystemCommand.AsObject;
  static toObject(includeInstance: boolean, msg: InterlockingSystemCommand): InterlockingSystemCommand.AsObject;
  static serializeBinaryToWriter(message: InterlockingSystemCommand, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): InterlockingSystemCommand;
  static deserializeBinaryFromReader(message: InterlockingSystemCommand, reader: jspb.BinaryReader): InterlockingSystemCommand;
}

export namespace InterlockingSystemCommand {
  export type AsObject = {
    interlockingsystem: string,
  }
}

export class InterlockingSystemStateMessage extends jspb.Message {
  getLinestatus(): InterlockingSystemStateMessage.LineStatus;
  setLinestatus(value: InterlockingSystemStateMessage.LineStatus): InterlockingSystemStateMessage;

  getLinedirectioninformation(): InterlockingSystemStateMessage.LineDirectionInformation;
  setLinedirectioninformation(value: InterlockingSystemStateMessage.LineDirectionInformation): InterlockingSystemStateMessage;

  getLinedirectionstatus(): InterlockingSystemStateMessage.LineDirectionStatus;
  setLinedirectionstatus(value: InterlockingSystemStateMessage.LineDirectionStatus): InterlockingSystemStateMessage;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): InterlockingSystemStateMessage.AsObject;
  static toObject(includeInstance: boolean, msg: InterlockingSystemStateMessage): InterlockingSystemStateMessage.AsObject;
  static serializeBinaryToWriter(message: InterlockingSystemStateMessage, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): InterlockingSystemStateMessage;
  static deserializeBinaryFromReader(message: InterlockingSystemStateMessage, reader: jspb.BinaryReader): InterlockingSystemStateMessage;
}

export namespace InterlockingSystemStateMessage {
  export type AsObject = {
    linestatus: InterlockingSystemStateMessage.LineStatus,
    linedirectioninformation: InterlockingSystemStateMessage.LineDirectionInformation,
    linedirectionstatus: InterlockingSystemStateMessage.LineDirectionStatus,
  }

  export enum LineStatus { 
    LINESTATUS_UNDEFINED = 0,
    LINESTATUS_VACANT = 1,
    LINESTATUS_OCCUPIED = 2,
    LINESTATUS_REQUESTFORLINEBLOCKRESET = 3,
  }

  export enum LineDirectionInformation { 
    LINEDIRECTIONINFORMATION_UNDEFINED = 0,
    LINEDIRECTIONINFORMATION_NODIRECTION = 1,
    LINEDIRECTIONINFORMATION_ENTRY = 2,
    LINEDIRECTIONINFORMATION_EXIT = 3,
    LINEDIRECTIONINFORMATION_DIRECTIONREQUEST = 4,
    LINEDIRECTIONINFORMATION_DIRECTIONHANDOVER = 5,
    LINEDIRECTIONINFORMATION_DIRECTIONHANDOVERABORTED = 6,
    LINEDIRECTIONINFORMATION_DISABLELINEBLOCKDIRECTION = 7,
    LINEDIRECTIONINFORMATION_ENABLELINEBLOCKDIRECTION = 8,
  }

  export enum LineDirectionStatus { 
    LINEDIRECTIONSTATUS_UNDEFINED = 0,
    LINEDIRECTIONSTATUS_RELEASED = 1,
    LINEDIRECTIONSTATUS_LOCKED = 2,
    LINEDIRECTIONSTATUS_LINEBLOCKDIRECTIONDISABLED = 3,
  }
}

export class SetInterlockingSystemStateResponse extends jspb.Message {
  getNewstate(): InterlockingSystemStateMessage | undefined;
  setNewstate(value?: InterlockingSystemStateMessage): SetInterlockingSystemStateResponse;
  hasNewstate(): boolean;
  clearNewstate(): SetInterlockingSystemStateResponse;

  getSuccess(): boolean;
  setSuccess(value: boolean): SetInterlockingSystemStateResponse;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): SetInterlockingSystemStateResponse.AsObject;
  static toObject(includeInstance: boolean, msg: SetInterlockingSystemStateResponse): SetInterlockingSystemStateResponse.AsObject;
  static serializeBinaryToWriter(message: SetInterlockingSystemStateResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): SetInterlockingSystemStateResponse;
  static deserializeBinaryFromReader(message: SetInterlockingSystemStateResponse, reader: jspb.BinaryReader): SetInterlockingSystemStateResponse;
}

export namespace SetInterlockingSystemStateResponse {
  export type AsObject = {
    newstate?: InterlockingSystemStateMessage.AsObject,
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

