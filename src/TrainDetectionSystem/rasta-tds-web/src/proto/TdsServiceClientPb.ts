/**
 * @fileoverview gRPC-Web generated client stub for ixl
 * @enhanceable
 * @public
 */

// GENERATED CODE -- DO NOT EDIT!


/* eslint-disable */
// @ts-nocheck


import * as grpcWeb from 'grpc-web';

import * as tds_pb from './tds_pb';


export class TrainDetectionSystemClient {
  client_: grpcWeb.AbstractClientBase;
  hostname_: string;
  credentials_: null | { [index: string]: string; };
  options_: null | { [index: string]: any; };

  constructor (hostname: string,
               credentials?: null | { [index: string]: string; },
               options?: null | { [index: string]: any; }) {
    if (!options) options = {};
    if (!credentials) credentials = {};
    options['format'] = 'text';

    this.client_ = new grpcWeb.GrpcWebClientBase(options);
    this.hostname_ = hostname;
    this.credentials_ = credentials;
    this.options_ = options;
  }

  methodInfoIncreaseAxleCount = new grpcWeb.AbstractClientBase.MethodInfo(
    tds_pb.Nothing,
    (request: tds_pb.TpsCommand) => {
      return request.serializeBinary();
    },
    tds_pb.Nothing.deserializeBinary
  );

  increaseAxleCount(
    request: tds_pb.TpsCommand,
    metadata: grpcWeb.Metadata | null): Promise<tds_pb.Nothing>;

  increaseAxleCount(
    request: tds_pb.TpsCommand,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.Error,
               response: tds_pb.Nothing) => void): grpcWeb.ClientReadableStream<tds_pb.Nothing>;

  increaseAxleCount(
    request: tds_pb.TpsCommand,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.Error,
               response: tds_pb.Nothing) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/ixl.TrainDetectionSystem/IncreaseAxleCount',
        request,
        metadata || {},
        this.methodInfoIncreaseAxleCount,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/ixl.TrainDetectionSystem/IncreaseAxleCount',
    request,
    metadata || {},
    this.methodInfoIncreaseAxleCount);
  }

  methodInfoDecreaseAxleCount = new grpcWeb.AbstractClientBase.MethodInfo(
    tds_pb.Nothing,
    (request: tds_pb.TpsCommand) => {
      return request.serializeBinary();
    },
    tds_pb.Nothing.deserializeBinary
  );

  decreaseAxleCount(
    request: tds_pb.TpsCommand,
    metadata: grpcWeb.Metadata | null): Promise<tds_pb.Nothing>;

  decreaseAxleCount(
    request: tds_pb.TpsCommand,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.Error,
               response: tds_pb.Nothing) => void): grpcWeb.ClientReadableStream<tds_pb.Nothing>;

  decreaseAxleCount(
    request: tds_pb.TpsCommand,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.Error,
               response: tds_pb.Nothing) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/ixl.TrainDetectionSystem/DecreaseAxleCount',
        request,
        metadata || {},
        this.methodInfoDecreaseAxleCount,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/ixl.TrainDetectionSystem/DecreaseAxleCount',
    request,
    metadata || {},
    this.methodInfoDecreaseAxleCount);
  }

}

