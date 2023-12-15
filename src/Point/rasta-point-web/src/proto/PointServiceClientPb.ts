/**
 * @fileoverview gRPC-Web generated client stub for point
 * @enhanceable
 * @public
 */

// Code generated by protoc-gen-grpc-web. DO NOT EDIT.
// versions:
// 	protoc-gen-grpc-web v1.4.2
// 	protoc              v4.25.0
// source: point.proto


/* eslint-disable */
// @ts-nocheck


import * as grpcWeb from 'grpc-web';

import * as google_protobuf_empty_pb from 'google-protobuf/google/protobuf/empty_pb';
import * as point_pb from './point_pb';


export class PointClient {
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
    this.hostname_ = hostname.replace(/\/+$/, '');
    this.credentials_ = credentials;
    this.options_ = options;
  }

  methodDescriptorPreventEndPosition = new grpcWeb.MethodDescriptor(
    '/point.Point/PreventEndPosition',
    grpcWeb.MethodType.UNARY,
    point_pb.SimulatedPositionMessage,
    google_protobuf_empty_pb.Empty,
    (request: point_pb.SimulatedPositionMessage) => {
      return request.serializeBinary();
    },
    google_protobuf_empty_pb.Empty.deserializeBinary
  );

  preventEndPosition(
    request: point_pb.SimulatedPositionMessage,
    metadata: grpcWeb.Metadata | null): Promise<google_protobuf_empty_pb.Empty>;

  preventEndPosition(
    request: point_pb.SimulatedPositionMessage,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void): grpcWeb.ClientReadableStream<google_protobuf_empty_pb.Empty>;

  preventEndPosition(
    request: point_pb.SimulatedPositionMessage,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/point.Point/PreventEndPosition',
        request,
        metadata || {},
        this.methodDescriptorPreventEndPosition,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/point.Point/PreventEndPosition',
    request,
    metadata || {},
    this.methodDescriptorPreventEndPosition);
  }

  methodDescriptorPutIntoUnintendedPosition = new grpcWeb.MethodDescriptor(
    '/point.Point/PutIntoUnintendedPosition',
    grpcWeb.MethodType.UNARY,
    point_pb.SimulatedPositionMessage,
    google_protobuf_empty_pb.Empty,
    (request: point_pb.SimulatedPositionMessage) => {
      return request.serializeBinary();
    },
    google_protobuf_empty_pb.Empty.deserializeBinary
  );

  putIntoUnintendedPosition(
    request: point_pb.SimulatedPositionMessage,
    metadata: grpcWeb.Metadata | null): Promise<google_protobuf_empty_pb.Empty>;

  putIntoUnintendedPosition(
    request: point_pb.SimulatedPositionMessage,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void): grpcWeb.ClientReadableStream<google_protobuf_empty_pb.Empty>;

  putIntoUnintendedPosition(
    request: point_pb.SimulatedPositionMessage,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/point.Point/PutIntoUnintendedPosition',
        request,
        metadata || {},
        this.methodDescriptorPutIntoUnintendedPosition,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/point.Point/PutIntoUnintendedPosition',
    request,
    metadata || {},
    this.methodDescriptorPutIntoUnintendedPosition);
  }

  methodDescriptorPutInEndPosition = new grpcWeb.MethodDescriptor(
    '/point.Point/PutInEndPosition',
    grpcWeb.MethodType.UNARY,
    google_protobuf_empty_pb.Empty,
    google_protobuf_empty_pb.Empty,
    (request: google_protobuf_empty_pb.Empty) => {
      return request.serializeBinary();
    },
    google_protobuf_empty_pb.Empty.deserializeBinary
  );

  putInEndPosition(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null): Promise<google_protobuf_empty_pb.Empty>;

  putInEndPosition(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void): grpcWeb.ClientReadableStream<google_protobuf_empty_pb.Empty>;

  putInEndPosition(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/point.Point/PutInEndPosition',
        request,
        metadata || {},
        this.methodDescriptorPutInEndPosition,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/point.Point/PutInEndPosition',
    request,
    metadata || {},
    this.methodDescriptorPutInEndPosition);
  }

  methodDescriptorGetPointPosition = new grpcWeb.MethodDescriptor(
    '/point.Point/GetPointPosition',
    grpcWeb.MethodType.UNARY,
    google_protobuf_empty_pb.Empty,
    point_pb.PointPositionMessage,
    (request: google_protobuf_empty_pb.Empty) => {
      return request.serializeBinary();
    },
    point_pb.PointPositionMessage.deserializeBinary
  );

  getPointPosition(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null): Promise<point_pb.PointPositionMessage>;

  getPointPosition(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: point_pb.PointPositionMessage) => void): grpcWeb.ClientReadableStream<point_pb.PointPositionMessage>;

  getPointPosition(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: point_pb.PointPositionMessage) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/point.Point/GetPointPosition',
        request,
        metadata || {},
        this.methodDescriptorGetPointPosition,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/point.Point/GetPointPosition',
    request,
    metadata || {},
    this.methodDescriptorGetPointPosition);
  }

  methodDescriptorGetDegradedPointPosition = new grpcWeb.MethodDescriptor(
    '/point.Point/GetDegradedPointPosition',
    grpcWeb.MethodType.UNARY,
    google_protobuf_empty_pb.Empty,
    point_pb.PointDegradedPositionMessage,
    (request: google_protobuf_empty_pb.Empty) => {
      return request.serializeBinary();
    },
    point_pb.PointDegradedPositionMessage.deserializeBinary
  );

  getDegradedPointPosition(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null): Promise<point_pb.PointDegradedPositionMessage>;

  getDegradedPointPosition(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: point_pb.PointDegradedPositionMessage) => void): grpcWeb.ClientReadableStream<point_pb.PointDegradedPositionMessage>;

  getDegradedPointPosition(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: point_pb.PointDegradedPositionMessage) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/point.Point/GetDegradedPointPosition',
        request,
        metadata || {},
        this.methodDescriptorGetDegradedPointPosition,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/point.Point/GetDegradedPointPosition',
    request,
    metadata || {},
    this.methodDescriptorGetDegradedPointPosition);
  }

  methodDescriptorEnableTimeout = new grpcWeb.MethodDescriptor(
    '/point.Point/EnableTimeout',
    grpcWeb.MethodType.UNARY,
    google_protobuf_empty_pb.Empty,
    google_protobuf_empty_pb.Empty,
    (request: google_protobuf_empty_pb.Empty) => {
      return request.serializeBinary();
    },
    google_protobuf_empty_pb.Empty.deserializeBinary
  );

  enableTimeout(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null): Promise<google_protobuf_empty_pb.Empty>;

  enableTimeout(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void): grpcWeb.ClientReadableStream<google_protobuf_empty_pb.Empty>;

  enableTimeout(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/point.Point/EnableTimeout',
        request,
        metadata || {},
        this.methodDescriptorEnableTimeout,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/point.Point/EnableTimeout',
    request,
    metadata || {},
    this.methodDescriptorEnableTimeout);
  }

  methodDescriptorSetAbilityToMove = new grpcWeb.MethodDescriptor(
    '/point.Point/SetAbilityToMove',
    grpcWeb.MethodType.UNARY,
    point_pb.AbilityToMoveMessage,
    google_protobuf_empty_pb.Empty,
    (request: point_pb.AbilityToMoveMessage) => {
      return request.serializeBinary();
    },
    google_protobuf_empty_pb.Empty.deserializeBinary
  );

  setAbilityToMove(
    request: point_pb.AbilityToMoveMessage,
    metadata: grpcWeb.Metadata | null): Promise<google_protobuf_empty_pb.Empty>;

  setAbilityToMove(
    request: point_pb.AbilityToMoveMessage,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void): grpcWeb.ClientReadableStream<google_protobuf_empty_pb.Empty>;

  setAbilityToMove(
    request: point_pb.AbilityToMoveMessage,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/point.Point/SetAbilityToMove',
        request,
        metadata || {},
        this.methodDescriptorSetAbilityToMove,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/point.Point/SetAbilityToMove',
    request,
    metadata || {},
    this.methodDescriptorSetAbilityToMove);
  }

  methodDescriptorSendGenericMessage = new grpcWeb.MethodDescriptor(
    '/point.Point/SendGenericMessage',
    grpcWeb.MethodType.UNARY,
    point_pb.GenericSCIMessage,
    google_protobuf_empty_pb.Empty,
    (request: point_pb.GenericSCIMessage) => {
      return request.serializeBinary();
    },
    google_protobuf_empty_pb.Empty.deserializeBinary
  );

  sendGenericMessage(
    request: point_pb.GenericSCIMessage,
    metadata: grpcWeb.Metadata | null): Promise<google_protobuf_empty_pb.Empty>;

  sendGenericMessage(
    request: point_pb.GenericSCIMessage,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void): grpcWeb.ClientReadableStream<google_protobuf_empty_pb.Empty>;

  sendGenericMessage(
    request: point_pb.GenericSCIMessage,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/point.Point/SendGenericMessage',
        request,
        metadata || {},
        this.methodDescriptorSendGenericMessage,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/point.Point/SendGenericMessage',
    request,
    metadata || {},
    this.methodDescriptorSendGenericMessage);
  }

  methodDescriptorReset = new grpcWeb.MethodDescriptor(
    '/point.Point/Reset',
    grpcWeb.MethodType.UNARY,
    google_protobuf_empty_pb.Empty,
    google_protobuf_empty_pb.Empty,
    (request: google_protobuf_empty_pb.Empty) => {
      return request.serializeBinary();
    },
    google_protobuf_empty_pb.Empty.deserializeBinary
  );

  reset(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null): Promise<google_protobuf_empty_pb.Empty>;

  reset(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void): grpcWeb.ClientReadableStream<google_protobuf_empty_pb.Empty>;

  reset(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/point.Point/Reset',
        request,
        metadata || {},
        this.methodDescriptorReset,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/point.Point/Reset',
    request,
    metadata || {},
    this.methodDescriptorReset);
  }

  methodDescriptorEstablishPointMachineState = new grpcWeb.MethodDescriptor(
    '/point.Point/EstablishPointMachineState',
    grpcWeb.MethodType.UNARY,
    point_pb.PointMachineStateMessage,
    google_protobuf_empty_pb.Empty,
    (request: point_pb.PointMachineStateMessage) => {
      return request.serializeBinary();
    },
    google_protobuf_empty_pb.Empty.deserializeBinary
  );

  establishPointMachineState(
    request: point_pb.PointMachineStateMessage,
    metadata: grpcWeb.Metadata | null): Promise<google_protobuf_empty_pb.Empty>;

  establishPointMachineState(
    request: point_pb.PointMachineStateMessage,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void): grpcWeb.ClientReadableStream<google_protobuf_empty_pb.Empty>;

  establishPointMachineState(
    request: point_pb.PointMachineStateMessage,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/point.Point/EstablishPointMachineState',
        request,
        metadata || {},
        this.methodDescriptorEstablishPointMachineState,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/point.Point/EstablishPointMachineState',
    request,
    metadata || {},
    this.methodDescriptorEstablishPointMachineState);
  }

  methodDescriptorGetPointMachineState = new grpcWeb.MethodDescriptor(
    '/point.Point/GetPointMachineState',
    grpcWeb.MethodType.UNARY,
    google_protobuf_empty_pb.Empty,
    point_pb.PointMachineStateMessage,
    (request: google_protobuf_empty_pb.Empty) => {
      return request.serializeBinary();
    },
    point_pb.PointMachineStateMessage.deserializeBinary
  );

  getPointMachineState(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null): Promise<point_pb.PointMachineStateMessage>;

  getPointMachineState(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: point_pb.PointMachineStateMessage) => void): grpcWeb.ClientReadableStream<point_pb.PointMachineStateMessage>;

  getPointMachineState(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: point_pb.PointMachineStateMessage) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/point.Point/GetPointMachineState',
        request,
        metadata || {},
        this.methodDescriptorGetPointMachineState,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/point.Point/GetPointMachineState',
    request,
    metadata || {},
    this.methodDescriptorGetPointMachineState);
  }

}

