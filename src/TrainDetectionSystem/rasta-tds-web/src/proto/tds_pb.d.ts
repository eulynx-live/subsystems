import * as jspb from 'google-protobuf'



export class TpsCommand extends jspb.Message {
  getTps(): string;
  setTps(value: string): TpsCommand;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): TpsCommand.AsObject;
  static toObject(includeInstance: boolean, msg: TpsCommand): TpsCommand.AsObject;
  static serializeBinaryToWriter(message: TpsCommand, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): TpsCommand;
  static deserializeBinaryFromReader(message: TpsCommand, reader: jspb.BinaryReader): TpsCommand;
}

export namespace TpsCommand {
  export type AsObject = {
    tps: string,
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

