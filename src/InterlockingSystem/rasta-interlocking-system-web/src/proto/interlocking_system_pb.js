// source: interlocking_system.proto
/**
 * @fileoverview
 * @enhanceable
 * @suppress {missingRequire} reports error on implicit type usages.
 * @suppress {messageConventions} JS Compiler reports an error if a variable or
 *     field starts with 'MSG_' and isn't a translatable message.
 * @public
 */
// GENERATED CODE -- DO NOT EDIT!
/* eslint-disable */
// @ts-nocheck

var jspb = require('google-protobuf');
var goog = jspb;
var global =
    (typeof globalThis !== 'undefined' && globalThis) ||
    (typeof window !== 'undefined' && window) ||
    (typeof global !== 'undefined' && global) ||
    (typeof self !== 'undefined' && self) ||
    (function () { return this; }).call(null) ||
    Function('return this')();

goog.exportSymbol('proto.interlocking_system.InterlockingSystemCommand', null, global);
goog.exportSymbol('proto.interlocking_system.InterlockingSystemStateMessage', null, global);
goog.exportSymbol('proto.interlocking_system.InterlockingSystemStateMessage.LineDirectionInformation', null, global);
goog.exportSymbol('proto.interlocking_system.InterlockingSystemStateMessage.LineDirectionStatus', null, global);
goog.exportSymbol('proto.interlocking_system.InterlockingSystemStateMessage.LineStatus', null, global);
goog.exportSymbol('proto.interlocking_system.Nothing', null, global);
goog.exportSymbol('proto.interlocking_system.SetInterlockingSystemStateResponse', null, global);
/**
 * Generated by JsPbCodeGenerator.
 * @param {Array=} opt_data Optional initial data array, typically from a
 * server response, or constructed directly in Javascript. The array is used
 * in place and becomes part of the constructed object. It is not cloned.
 * If no data is provided, the constructed object will be empty, but still
 * valid.
 * @extends {jspb.Message}
 * @constructor
 */
proto.interlocking_system.InterlockingSystemCommand = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, null, null);
};
goog.inherits(proto.interlocking_system.InterlockingSystemCommand, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.interlocking_system.InterlockingSystemCommand.displayName = 'proto.interlocking_system.InterlockingSystemCommand';
}
/**
 * Generated by JsPbCodeGenerator.
 * @param {Array=} opt_data Optional initial data array, typically from a
 * server response, or constructed directly in Javascript. The array is used
 * in place and becomes part of the constructed object. It is not cloned.
 * If no data is provided, the constructed object will be empty, but still
 * valid.
 * @extends {jspb.Message}
 * @constructor
 */
proto.interlocking_system.InterlockingSystemStateMessage = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, null, null);
};
goog.inherits(proto.interlocking_system.InterlockingSystemStateMessage, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.interlocking_system.InterlockingSystemStateMessage.displayName = 'proto.interlocking_system.InterlockingSystemStateMessage';
}
/**
 * Generated by JsPbCodeGenerator.
 * @param {Array=} opt_data Optional initial data array, typically from a
 * server response, or constructed directly in Javascript. The array is used
 * in place and becomes part of the constructed object. It is not cloned.
 * If no data is provided, the constructed object will be empty, but still
 * valid.
 * @extends {jspb.Message}
 * @constructor
 */
proto.interlocking_system.SetInterlockingSystemStateResponse = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, null, null);
};
goog.inherits(proto.interlocking_system.SetInterlockingSystemStateResponse, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.interlocking_system.SetInterlockingSystemStateResponse.displayName = 'proto.interlocking_system.SetInterlockingSystemStateResponse';
}
/**
 * Generated by JsPbCodeGenerator.
 * @param {Array=} opt_data Optional initial data array, typically from a
 * server response, or constructed directly in Javascript. The array is used
 * in place and becomes part of the constructed object. It is not cloned.
 * If no data is provided, the constructed object will be empty, but still
 * valid.
 * @extends {jspb.Message}
 * @constructor
 */
proto.interlocking_system.Nothing = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, null, null);
};
goog.inherits(proto.interlocking_system.Nothing, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.interlocking_system.Nothing.displayName = 'proto.interlocking_system.Nothing';
}



if (jspb.Message.GENERATE_TO_OBJECT) {
/**
 * Creates an object representation of this proto.
 * Field names that are reserved in JavaScript and will be renamed to pb_name.
 * Optional fields that are not set will be set to undefined.
 * To access a reserved field use, foo.pb_<name>, eg, foo.pb_default.
 * For the list of reserved names please see:
 *     net/proto2/compiler/js/internal/generator.cc#kKeyword.
 * @param {boolean=} opt_includeInstance Deprecated. whether to include the
 *     JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @return {!Object}
 */
proto.interlocking_system.InterlockingSystemCommand.prototype.toObject = function(opt_includeInstance) {
  return proto.interlocking_system.InterlockingSystemCommand.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.interlocking_system.InterlockingSystemCommand} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.interlocking_system.InterlockingSystemCommand.toObject = function(includeInstance, msg) {
  var f, obj = {
    interlockingsystem: jspb.Message.getFieldWithDefault(msg, 1, "")
  };

  if (includeInstance) {
    obj.$jspbMessageInstance = msg;
  }
  return obj;
};
}


/**
 * Deserializes binary data (in protobuf wire format).
 * @param {jspb.ByteSource} bytes The bytes to deserialize.
 * @return {!proto.interlocking_system.InterlockingSystemCommand}
 */
proto.interlocking_system.InterlockingSystemCommand.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.interlocking_system.InterlockingSystemCommand;
  return proto.interlocking_system.InterlockingSystemCommand.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.interlocking_system.InterlockingSystemCommand} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.interlocking_system.InterlockingSystemCommand}
 */
proto.interlocking_system.InterlockingSystemCommand.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = /** @type {string} */ (reader.readString());
      msg.setInterlockingsystem(value);
      break;
    default:
      reader.skipField();
      break;
    }
  }
  return msg;
};


/**
 * Serializes the message to binary data (in protobuf wire format).
 * @return {!Uint8Array}
 */
proto.interlocking_system.InterlockingSystemCommand.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.interlocking_system.InterlockingSystemCommand.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.interlocking_system.InterlockingSystemCommand} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.interlocking_system.InterlockingSystemCommand.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getInterlockingsystem();
  if (f.length > 0) {
    writer.writeString(
      1,
      f
    );
  }
};


/**
 * optional string interlockingSystem = 1;
 * @return {string}
 */
proto.interlocking_system.InterlockingSystemCommand.prototype.getInterlockingsystem = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 1, ""));
};


/**
 * @param {string} value
 * @return {!proto.interlocking_system.InterlockingSystemCommand} returns this
 */
proto.interlocking_system.InterlockingSystemCommand.prototype.setInterlockingsystem = function(value) {
  return jspb.Message.setProto3StringField(this, 1, value);
};





if (jspb.Message.GENERATE_TO_OBJECT) {
/**
 * Creates an object representation of this proto.
 * Field names that are reserved in JavaScript and will be renamed to pb_name.
 * Optional fields that are not set will be set to undefined.
 * To access a reserved field use, foo.pb_<name>, eg, foo.pb_default.
 * For the list of reserved names please see:
 *     net/proto2/compiler/js/internal/generator.cc#kKeyword.
 * @param {boolean=} opt_includeInstance Deprecated. whether to include the
 *     JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @return {!Object}
 */
proto.interlocking_system.InterlockingSystemStateMessage.prototype.toObject = function(opt_includeInstance) {
  return proto.interlocking_system.InterlockingSystemStateMessage.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.interlocking_system.InterlockingSystemStateMessage} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.interlocking_system.InterlockingSystemStateMessage.toObject = function(includeInstance, msg) {
  var f, obj = {
    linestatus: jspb.Message.getFieldWithDefault(msg, 1, 0),
    linedirectioninformation: jspb.Message.getFieldWithDefault(msg, 2, 0),
    linedirectionstatus: jspb.Message.getFieldWithDefault(msg, 3, 0)
  };

  if (includeInstance) {
    obj.$jspbMessageInstance = msg;
  }
  return obj;
};
}


/**
 * Deserializes binary data (in protobuf wire format).
 * @param {jspb.ByteSource} bytes The bytes to deserialize.
 * @return {!proto.interlocking_system.InterlockingSystemStateMessage}
 */
proto.interlocking_system.InterlockingSystemStateMessage.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.interlocking_system.InterlockingSystemStateMessage;
  return proto.interlocking_system.InterlockingSystemStateMessage.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.interlocking_system.InterlockingSystemStateMessage} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.interlocking_system.InterlockingSystemStateMessage}
 */
proto.interlocking_system.InterlockingSystemStateMessage.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = /** @type {!proto.interlocking_system.InterlockingSystemStateMessage.LineStatus} */ (reader.readEnum());
      msg.setLinestatus(value);
      break;
    case 2:
      var value = /** @type {!proto.interlocking_system.InterlockingSystemStateMessage.LineDirectionInformation} */ (reader.readEnum());
      msg.setLinedirectioninformation(value);
      break;
    case 3:
      var value = /** @type {!proto.interlocking_system.InterlockingSystemStateMessage.LineDirectionStatus} */ (reader.readEnum());
      msg.setLinedirectionstatus(value);
      break;
    default:
      reader.skipField();
      break;
    }
  }
  return msg;
};


/**
 * Serializes the message to binary data (in protobuf wire format).
 * @return {!Uint8Array}
 */
proto.interlocking_system.InterlockingSystemStateMessage.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.interlocking_system.InterlockingSystemStateMessage.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.interlocking_system.InterlockingSystemStateMessage} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.interlocking_system.InterlockingSystemStateMessage.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getLinestatus();
  if (f !== 0.0) {
    writer.writeEnum(
      1,
      f
    );
  }
  f = message.getLinedirectioninformation();
  if (f !== 0.0) {
    writer.writeEnum(
      2,
      f
    );
  }
  f = message.getLinedirectionstatus();
  if (f !== 0.0) {
    writer.writeEnum(
      3,
      f
    );
  }
};


/**
 * @enum {number}
 */
proto.interlocking_system.InterlockingSystemStateMessage.LineStatus = {
  LINESTATUS_UNDEFINED: 0,
  LINESTATUS_VACANT: 1,
  LINESTATUS_OCCUPIED: 2,
  LINESTATUS_REQUESTFORLINEBLOCKRESET: 3
};

/**
 * @enum {number}
 */
proto.interlocking_system.InterlockingSystemStateMessage.LineDirectionInformation = {
  LINEDIRECTIONINFORMATION_UNDEFINED: 0,
  LINEDIRECTIONINFORMATION_NODIRECTION: 1,
  LINEDIRECTIONINFORMATION_ENTRY: 2,
  LINEDIRECTIONINFORMATION_EXIT: 3,
  LINEDIRECTIONINFORMATION_DIRECTIONREQUEST: 4,
  LINEDIRECTIONINFORMATION_DIRECTIONHANDOVER: 5,
  LINEDIRECTIONINFORMATION_DIRECTIONHANDOVERABORTED: 6,
  LINEDIRECTIONINFORMATION_DISABLELINEBLOCKDIRECTION: 7,
  LINEDIRECTIONINFORMATION_ENABLELINEBLOCKDIRECTION: 8
};

/**
 * @enum {number}
 */
proto.interlocking_system.InterlockingSystemStateMessage.LineDirectionStatus = {
  LINEDIRECTIONSTATUS_UNDEFINED: 0,
  LINEDIRECTIONSTATUS_RELEASED: 1,
  LINEDIRECTIONSTATUS_LOCKED: 2,
  LINEDIRECTIONSTATUS_LINEBLOCKDIRECTIONDISABLED: 3
};

/**
 * optional LineStatus lineStatus = 1;
 * @return {!proto.interlocking_system.InterlockingSystemStateMessage.LineStatus}
 */
proto.interlocking_system.InterlockingSystemStateMessage.prototype.getLinestatus = function() {
  return /** @type {!proto.interlocking_system.InterlockingSystemStateMessage.LineStatus} */ (jspb.Message.getFieldWithDefault(this, 1, 0));
};


/**
 * @param {!proto.interlocking_system.InterlockingSystemStateMessage.LineStatus} value
 * @return {!proto.interlocking_system.InterlockingSystemStateMessage} returns this
 */
proto.interlocking_system.InterlockingSystemStateMessage.prototype.setLinestatus = function(value) {
  return jspb.Message.setProto3EnumField(this, 1, value);
};


/**
 * optional LineDirectionInformation lineDirectionInformation = 2;
 * @return {!proto.interlocking_system.InterlockingSystemStateMessage.LineDirectionInformation}
 */
proto.interlocking_system.InterlockingSystemStateMessage.prototype.getLinedirectioninformation = function() {
  return /** @type {!proto.interlocking_system.InterlockingSystemStateMessage.LineDirectionInformation} */ (jspb.Message.getFieldWithDefault(this, 2, 0));
};


/**
 * @param {!proto.interlocking_system.InterlockingSystemStateMessage.LineDirectionInformation} value
 * @return {!proto.interlocking_system.InterlockingSystemStateMessage} returns this
 */
proto.interlocking_system.InterlockingSystemStateMessage.prototype.setLinedirectioninformation = function(value) {
  return jspb.Message.setProto3EnumField(this, 2, value);
};


/**
 * optional LineDirectionStatus lineDirectionStatus = 3;
 * @return {!proto.interlocking_system.InterlockingSystemStateMessage.LineDirectionStatus}
 */
proto.interlocking_system.InterlockingSystemStateMessage.prototype.getLinedirectionstatus = function() {
  return /** @type {!proto.interlocking_system.InterlockingSystemStateMessage.LineDirectionStatus} */ (jspb.Message.getFieldWithDefault(this, 3, 0));
};


/**
 * @param {!proto.interlocking_system.InterlockingSystemStateMessage.LineDirectionStatus} value
 * @return {!proto.interlocking_system.InterlockingSystemStateMessage} returns this
 */
proto.interlocking_system.InterlockingSystemStateMessage.prototype.setLinedirectionstatus = function(value) {
  return jspb.Message.setProto3EnumField(this, 3, value);
};





if (jspb.Message.GENERATE_TO_OBJECT) {
/**
 * Creates an object representation of this proto.
 * Field names that are reserved in JavaScript and will be renamed to pb_name.
 * Optional fields that are not set will be set to undefined.
 * To access a reserved field use, foo.pb_<name>, eg, foo.pb_default.
 * For the list of reserved names please see:
 *     net/proto2/compiler/js/internal/generator.cc#kKeyword.
 * @param {boolean=} opt_includeInstance Deprecated. whether to include the
 *     JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @return {!Object}
 */
proto.interlocking_system.SetInterlockingSystemStateResponse.prototype.toObject = function(opt_includeInstance) {
  return proto.interlocking_system.SetInterlockingSystemStateResponse.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.interlocking_system.SetInterlockingSystemStateResponse} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.interlocking_system.SetInterlockingSystemStateResponse.toObject = function(includeInstance, msg) {
  var f, obj = {
    newstate: (f = msg.getNewstate()) && proto.interlocking_system.InterlockingSystemStateMessage.toObject(includeInstance, f),
    success: jspb.Message.getBooleanFieldWithDefault(msg, 2, false)
  };

  if (includeInstance) {
    obj.$jspbMessageInstance = msg;
  }
  return obj;
};
}


/**
 * Deserializes binary data (in protobuf wire format).
 * @param {jspb.ByteSource} bytes The bytes to deserialize.
 * @return {!proto.interlocking_system.SetInterlockingSystemStateResponse}
 */
proto.interlocking_system.SetInterlockingSystemStateResponse.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.interlocking_system.SetInterlockingSystemStateResponse;
  return proto.interlocking_system.SetInterlockingSystemStateResponse.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.interlocking_system.SetInterlockingSystemStateResponse} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.interlocking_system.SetInterlockingSystemStateResponse}
 */
proto.interlocking_system.SetInterlockingSystemStateResponse.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = new proto.interlocking_system.InterlockingSystemStateMessage;
      reader.readMessage(value,proto.interlocking_system.InterlockingSystemStateMessage.deserializeBinaryFromReader);
      msg.setNewstate(value);
      break;
    case 2:
      var value = /** @type {boolean} */ (reader.readBool());
      msg.setSuccess(value);
      break;
    default:
      reader.skipField();
      break;
    }
  }
  return msg;
};


/**
 * Serializes the message to binary data (in protobuf wire format).
 * @return {!Uint8Array}
 */
proto.interlocking_system.SetInterlockingSystemStateResponse.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.interlocking_system.SetInterlockingSystemStateResponse.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.interlocking_system.SetInterlockingSystemStateResponse} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.interlocking_system.SetInterlockingSystemStateResponse.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getNewstate();
  if (f != null) {
    writer.writeMessage(
      1,
      f,
      proto.interlocking_system.InterlockingSystemStateMessage.serializeBinaryToWriter
    );
  }
  f = message.getSuccess();
  if (f) {
    writer.writeBool(
      2,
      f
    );
  }
};


/**
 * optional InterlockingSystemStateMessage newState = 1;
 * @return {?proto.interlocking_system.InterlockingSystemStateMessage}
 */
proto.interlocking_system.SetInterlockingSystemStateResponse.prototype.getNewstate = function() {
  return /** @type{?proto.interlocking_system.InterlockingSystemStateMessage} */ (
    jspb.Message.getWrapperField(this, proto.interlocking_system.InterlockingSystemStateMessage, 1));
};


/**
 * @param {?proto.interlocking_system.InterlockingSystemStateMessage|undefined} value
 * @return {!proto.interlocking_system.SetInterlockingSystemStateResponse} returns this
*/
proto.interlocking_system.SetInterlockingSystemStateResponse.prototype.setNewstate = function(value) {
  return jspb.Message.setWrapperField(this, 1, value);
};


/**
 * Clears the message field making it undefined.
 * @return {!proto.interlocking_system.SetInterlockingSystemStateResponse} returns this
 */
proto.interlocking_system.SetInterlockingSystemStateResponse.prototype.clearNewstate = function() {
  return this.setNewstate(undefined);
};


/**
 * Returns whether this field is set.
 * @return {boolean}
 */
proto.interlocking_system.SetInterlockingSystemStateResponse.prototype.hasNewstate = function() {
  return jspb.Message.getField(this, 1) != null;
};


/**
 * optional bool success = 2;
 * @return {boolean}
 */
proto.interlocking_system.SetInterlockingSystemStateResponse.prototype.getSuccess = function() {
  return /** @type {boolean} */ (jspb.Message.getBooleanFieldWithDefault(this, 2, false));
};


/**
 * @param {boolean} value
 * @return {!proto.interlocking_system.SetInterlockingSystemStateResponse} returns this
 */
proto.interlocking_system.SetInterlockingSystemStateResponse.prototype.setSuccess = function(value) {
  return jspb.Message.setProto3BooleanField(this, 2, value);
};





if (jspb.Message.GENERATE_TO_OBJECT) {
/**
 * Creates an object representation of this proto.
 * Field names that are reserved in JavaScript and will be renamed to pb_name.
 * Optional fields that are not set will be set to undefined.
 * To access a reserved field use, foo.pb_<name>, eg, foo.pb_default.
 * For the list of reserved names please see:
 *     net/proto2/compiler/js/internal/generator.cc#kKeyword.
 * @param {boolean=} opt_includeInstance Deprecated. whether to include the
 *     JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @return {!Object}
 */
proto.interlocking_system.Nothing.prototype.toObject = function(opt_includeInstance) {
  return proto.interlocking_system.Nothing.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.interlocking_system.Nothing} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.interlocking_system.Nothing.toObject = function(includeInstance, msg) {
  var f, obj = {

  };

  if (includeInstance) {
    obj.$jspbMessageInstance = msg;
  }
  return obj;
};
}


/**
 * Deserializes binary data (in protobuf wire format).
 * @param {jspb.ByteSource} bytes The bytes to deserialize.
 * @return {!proto.interlocking_system.Nothing}
 */
proto.interlocking_system.Nothing.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.interlocking_system.Nothing;
  return proto.interlocking_system.Nothing.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.interlocking_system.Nothing} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.interlocking_system.Nothing}
 */
proto.interlocking_system.Nothing.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    default:
      reader.skipField();
      break;
    }
  }
  return msg;
};


/**
 * Serializes the message to binary data (in protobuf wire format).
 * @return {!Uint8Array}
 */
proto.interlocking_system.Nothing.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.interlocking_system.Nothing.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.interlocking_system.Nothing} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.interlocking_system.Nothing.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
};


goog.object.extend(exports, proto.interlocking_system);
