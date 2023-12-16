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

  methodDescriptorSendTimeoutMessage = new grpcWeb.MethodDescriptor(
    '/point.Point/SendTimeoutMessage',
    grpcWeb.MethodType.UNARY,
    google_protobuf_empty_pb.Empty,
    google_protobuf_empty_pb.Empty,
    (request: google_protobuf_empty_pb.Empty) => {
      return request.serializeBinary();
    },
    google_protobuf_empty_pb.Empty.deserializeBinary
  );

  sendTimeoutMessage(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null): Promise<google_protobuf_empty_pb.Empty>;

  sendTimeoutMessage(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void): grpcWeb.ClientReadableStream<google_protobuf_empty_pb.Empty>;

  sendTimeoutMessage(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/point.Point/SendTimeoutMessage',
        request,
        metadata || {},
        this.methodDescriptorSendTimeoutMessage,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/point.Point/SendTimeoutMessage',
    request,
    metadata || {},
    this.methodDescriptorSendTimeoutMessage);
  }

  methodDescriptorSendAbilityToMoveMessage = new grpcWeb.MethodDescriptor(
    '/point.Point/SendAbilityToMoveMessage',
    grpcWeb.MethodType.UNARY,
    point_pb.AbilityToMoveMessage,
    google_protobuf_empty_pb.Empty,
    (request: point_pb.AbilityToMoveMessage) => {
      return request.serializeBinary();
    },
    google_protobuf_empty_pb.Empty.deserializeBinary
  );

  sendAbilityToMoveMessage(
    request: point_pb.AbilityToMoveMessage,
    metadata: grpcWeb.Metadata | null): Promise<google_protobuf_empty_pb.Empty>;

  sendAbilityToMoveMessage(
    request: point_pb.AbilityToMoveMessage,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void): grpcWeb.ClientReadableStream<google_protobuf_empty_pb.Empty>;

  sendAbilityToMoveMessage(
    request: point_pb.AbilityToMoveMessage,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/point.Point/SendAbilityToMoveMessage',
        request,
        metadata || {},
        this.methodDescriptorSendAbilityToMoveMessage,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/point.Point/SendAbilityToMoveMessage',
    request,
    metadata || {},
    this.methodDescriptorSendAbilityToMoveMessage);
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

}

