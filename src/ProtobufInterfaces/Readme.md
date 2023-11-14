### Generating ts/js grpc files:
Requires the packages installed:
```console
npm i protoc-gen-grpc-web protoc-gen-js
```
Generating:
```console
protoc -I=./proto <your_proto_filename>.proto \
  --js_out=import_style=commonjs:. \
  --grpc-web_out=import_style=typescript,mode=grpcwebtext:.

```
