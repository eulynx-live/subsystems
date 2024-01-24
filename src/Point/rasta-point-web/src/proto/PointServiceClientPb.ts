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

  methodDescriptorPutIntoUnintendedPosition = new grpcWeb.MethodDescriptor(
    '/point.Point/PutIntoUnintendedPosition',
    grpcWeb.MethodType.UNARY,
    point_pb.DegradedPositionMessage,
    google_protobuf_empty_pb.Empty,
    (request: point_pb.DegradedPositionMessage) => {
      return request.serializeBinary();
    },
    google_protobuf_empty_pb.Empty.deserializeBinary
  );

  putIntoUnintendedPosition(
    request: point_pb.DegradedPositionMessage,
    metadata: grpcWeb.Metadata | null): Promise<google_protobuf_empty_pb.Empty>;

  putIntoUnintendedPosition(
    request: point_pb.DegradedPositionMessage,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void): grpcWeb.ClientReadableStream<google_protobuf_empty_pb.Empty>;

  putIntoUnintendedPosition(
    request: point_pb.DegradedPositionMessage,
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

  methodDescriptorPutIntoTrailedPosition = new grpcWeb.MethodDescriptor(
    '/point.Point/PutIntoTrailedPosition',
    grpcWeb.MethodType.UNARY,
    point_pb.DegradedPositionMessage,
    google_protobuf_empty_pb.Empty,
    (request: point_pb.DegradedPositionMessage) => {
      return request.serializeBinary();
    },
    google_protobuf_empty_pb.Empty.deserializeBinary
  );

  putIntoTrailedPosition(
    request: point_pb.DegradedPositionMessage,
    metadata: grpcWeb.Metadata | null): Promise<google_protobuf_empty_pb.Empty>;

  putIntoTrailedPosition(
    request: point_pb.DegradedPositionMessage,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void): grpcWeb.ClientReadableStream<google_protobuf_empty_pb.Empty>;

  putIntoTrailedPosition(
    request: point_pb.DegradedPositionMessage,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/point.Point/PutIntoTrailedPosition',
        request,
        metadata || {},
        this.methodDescriptorPutIntoTrailedPosition,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/point.Point/PutIntoTrailedPosition',
    request,
    metadata || {},
    this.methodDescriptorPutIntoTrailedPosition);
  }

  methodDescriptorPutIntoNoEndPosition = new grpcWeb.MethodDescriptor(
    '/point.Point/PutIntoNoEndPosition',
    grpcWeb.MethodType.UNARY,
    point_pb.DegradedPositionMessage,
    google_protobuf_empty_pb.Empty,
    (request: point_pb.DegradedPositionMessage) => {
      return request.serializeBinary();
    },
    google_protobuf_empty_pb.Empty.deserializeBinary
  );

  putIntoNoEndPosition(
    request: point_pb.DegradedPositionMessage,
    metadata: grpcWeb.Metadata | null): Promise<google_protobuf_empty_pb.Empty>;

  putIntoNoEndPosition(
    request: point_pb.DegradedPositionMessage,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void): grpcWeb.ClientReadableStream<google_protobuf_empty_pb.Empty>;

  putIntoNoEndPosition(
    request: point_pb.DegradedPositionMessage,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/point.Point/PutIntoNoEndPosition',
        request,
        metadata || {},
        this.methodDescriptorPutIntoNoEndPosition,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/point.Point/PutIntoNoEndPosition',
    request,
    metadata || {},
    this.methodDescriptorPutIntoNoEndPosition);
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

  methodDescriptorSendSciMessage = new grpcWeb.MethodDescriptor(
    '/point.Point/SendSciMessage',
    grpcWeb.MethodType.UNARY,
    point_pb.SciMessage,
    google_protobuf_empty_pb.Empty,
    (request: point_pb.SciMessage) => {
      return request.serializeBinary();
    },
    google_protobuf_empty_pb.Empty.deserializeBinary
  );

  sendSciMessage(
    request: point_pb.SciMessage,
    metadata: grpcWeb.Metadata | null): Promise<google_protobuf_empty_pb.Empty>;

  sendSciMessage(
    request: point_pb.SciMessage,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void): grpcWeb.ClientReadableStream<google_protobuf_empty_pb.Empty>;

  sendSciMessage(
    request: point_pb.SciMessage,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/point.Point/SendSciMessage',
        request,
        metadata || {},
        this.methodDescriptorSendSciMessage,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/point.Point/SendSciMessage',
    request,
    metadata || {},
    this.methodDescriptorSendSciMessage);
  }

  methodDescriptorSendSciMessagePDIError = new grpcWeb.MethodDescriptor(
    '/point.Point/SendSciMessagePDIError',
    grpcWeb.MethodType.UNARY,
    google_protobuf_empty_pb.Empty,
    google_protobuf_empty_pb.Empty,
    (request: google_protobuf_empty_pb.Empty) => {
      return request.serializeBinary();
    },
    google_protobuf_empty_pb.Empty.deserializeBinary
  );

  sendSciMessagePDIError(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null): Promise<google_protobuf_empty_pb.Empty>;

  sendSciMessagePDIError(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void): grpcWeb.ClientReadableStream<google_protobuf_empty_pb.Empty>;

  sendSciMessagePDIError(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/point.Point/SendSciMessagePDIError',
        request,
        metadata || {},
        this.methodDescriptorSendSciMessagePDIError,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/point.Point/SendSciMessagePDIError',
    request,
    metadata || {},
    this.methodDescriptorSendSciMessagePDIError);
  }

  methodDescriptorSendSciMessageContentError = new grpcWeb.MethodDescriptor(
    '/point.Point/SendSciMessageContentError',
    grpcWeb.MethodType.UNARY,
    google_protobuf_empty_pb.Empty,
    google_protobuf_empty_pb.Empty,
    (request: google_protobuf_empty_pb.Empty) => {
      return request.serializeBinary();
    },
    google_protobuf_empty_pb.Empty.deserializeBinary
  );

  sendSciMessageContentError(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null): Promise<google_protobuf_empty_pb.Empty>;

  sendSciMessageContentError(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void): grpcWeb.ClientReadableStream<google_protobuf_empty_pb.Empty>;

  sendSciMessageContentError(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/point.Point/SendSciMessageContentError',
        request,
        metadata || {},
        this.methodDescriptorSendSciMessageContentError,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/point.Point/SendSciMessageContentError',
    request,
    metadata || {},
    this.methodDescriptorSendSciMessageContentError);
  }

  methodDescriptorSendSciMessageFormalError = new grpcWeb.MethodDescriptor(
    '/point.Point/SendSciMessageFormalError',
    grpcWeb.MethodType.UNARY,
    google_protobuf_empty_pb.Empty,
    google_protobuf_empty_pb.Empty,
    (request: google_protobuf_empty_pb.Empty) => {
      return request.serializeBinary();
    },
    google_protobuf_empty_pb.Empty.deserializeBinary
  );

  sendSciMessageFormalError(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null): Promise<google_protobuf_empty_pb.Empty>;

  sendSciMessageFormalError(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void): grpcWeb.ClientReadableStream<google_protobuf_empty_pb.Empty>;

  sendSciMessageFormalError(
    request: google_protobuf_empty_pb.Empty,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/point.Point/SendSciMessageFormalError',
        request,
        metadata || {},
        this.methodDescriptorSendSciMessageFormalError,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/point.Point/SendSciMessageFormalError',
    request,
    metadata || {},
    this.methodDescriptorSendSciMessageFormalError);
  }

  methodDescriptorOverrideSciMessage = new grpcWeb.MethodDescriptor(
    '/point.Point/OverrideSciMessage',
    grpcWeb.MethodType.UNARY,
    point_pb.SciMessage,
    google_protobuf_empty_pb.Empty,
    (request: point_pb.SciMessage) => {
      return request.serializeBinary();
    },
    google_protobuf_empty_pb.Empty.deserializeBinary
  );

  overrideSciMessage(
    request: point_pb.SciMessage,
    metadata: grpcWeb.Metadata | null): Promise<google_protobuf_empty_pb.Empty>;

  overrideSciMessage(
    request: point_pb.SciMessage,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void): grpcWeb.ClientReadableStream<google_protobuf_empty_pb.Empty>;

  overrideSciMessage(
    request: point_pb.SciMessage,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/point.Point/OverrideSciMessage',
        request,
        metadata || {},
        this.methodDescriptorOverrideSciMessage,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/point.Point/OverrideSciMessage',
    request,
    metadata || {},
    this.methodDescriptorOverrideSciMessage);
  }

  methodDescriptorSchedulePreventRightEndPosition = new grpcWeb.MethodDescriptor(
    '/point.Point/SchedulePreventRightEndPosition',
    grpcWeb.MethodType.UNARY,
    point_pb.PreventedPositionMessage,
    google_protobuf_empty_pb.Empty,
    (request: point_pb.PreventedPositionMessage) => {
      return request.serializeBinary();
    },
    google_protobuf_empty_pb.Empty.deserializeBinary
  );

  schedulePreventRightEndPosition(
    request: point_pb.PreventedPositionMessage,
    metadata: grpcWeb.Metadata | null): Promise<google_protobuf_empty_pb.Empty>;

  schedulePreventRightEndPosition(
    request: point_pb.PreventedPositionMessage,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void): grpcWeb.ClientReadableStream<google_protobuf_empty_pb.Empty>;

  schedulePreventRightEndPosition(
    request: point_pb.PreventedPositionMessage,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/point.Point/SchedulePreventRightEndPosition',
        request,
        metadata || {},
        this.methodDescriptorSchedulePreventRightEndPosition,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/point.Point/SchedulePreventRightEndPosition',
    request,
    metadata || {},
    this.methodDescriptorSchedulePreventRightEndPosition);
  }

  methodDescriptorSchedulePreventLeftEndPosition = new grpcWeb.MethodDescriptor(
    '/point.Point/SchedulePreventLeftEndPosition',
    grpcWeb.MethodType.UNARY,
    point_pb.PreventedPositionMessage,
    google_protobuf_empty_pb.Empty,
    (request: point_pb.PreventedPositionMessage) => {
      return request.serializeBinary();
    },
    google_protobuf_empty_pb.Empty.deserializeBinary
  );

  schedulePreventLeftEndPosition(
    request: point_pb.PreventedPositionMessage,
    metadata: grpcWeb.Metadata | null): Promise<google_protobuf_empty_pb.Empty>;

  schedulePreventLeftEndPosition(
    request: point_pb.PreventedPositionMessage,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void): grpcWeb.ClientReadableStream<google_protobuf_empty_pb.Empty>;

  schedulePreventLeftEndPosition(
    request: point_pb.PreventedPositionMessage,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/point.Point/SchedulePreventLeftEndPosition',
        request,
        metadata || {},
        this.methodDescriptorSchedulePreventLeftEndPosition,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/point.Point/SchedulePreventLeftEndPosition',
    request,
    metadata || {},
    this.methodDescriptorSchedulePreventLeftEndPosition);
  }

  methodDescriptorScheduleInitializationTimeout = new grpcWeb.MethodDescriptor(
    '/point.Point/ScheduleInitializationTimeout',
    grpcWeb.MethodType.UNARY,
    point_pb.EnableInitializationTimeoutMessage,
    google_protobuf_empty_pb.Empty,
    (request: point_pb.EnableInitializationTimeoutMessage) => {
      return request.serializeBinary();
    },
    google_protobuf_empty_pb.Empty.deserializeBinary
  );

  scheduleInitializationTimeout(
    request: point_pb.EnableInitializationTimeoutMessage,
    metadata: grpcWeb.Metadata | null): Promise<google_protobuf_empty_pb.Empty>;

  scheduleInitializationTimeout(
    request: point_pb.EnableInitializationTimeoutMessage,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void): grpcWeb.ClientReadableStream<google_protobuf_empty_pb.Empty>;

  scheduleInitializationTimeout(
    request: point_pb.EnableInitializationTimeoutMessage,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: google_protobuf_empty_pb.Empty) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/point.Point/ScheduleInitializationTimeout',
        request,
        metadata || {},
        this.methodDescriptorScheduleInitializationTimeout,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/point.Point/ScheduleInitializationTimeout',
    request,
    metadata || {},
    this.methodDescriptorScheduleInitializationTimeout);
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

