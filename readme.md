# EpicChain Toolkit Persistence Library

[![Build Status](https://github.com/epicchain/epicchain-toolkit-library/actions/workflows/push.yml/badge.svg?branch=master)](https://github.com/epicchain/epicchain-toolkit-library/actions/)

This repository contains code shared between managed projects in the EpicChain Toolkit. In particular, these libraries are used in [Epic-Express](https://github.com/epicchain/epic-express) and the [Epic Smart Contract Debugger for VS Code](https://github.com/epicchain/epic-debugger).

Continuous Integration build packages are available via [Azure Artifacts](https://dev.azure.com/epicchain/Build/_packaging?_a=feed&feed=public).

## Models

This library contains classes for reading and writing `.epic-express` files and [EIP-19 compatible debug information](https://github.com/epicchain/proposals/blob/master/eip-19.mediawiki).

## Contract Parameter Parsing

This library contains code to parse Epic Express contract invoke files as specified in [EDX-DN12](https://github.com/epicchain/design-notes/blob/master/EDX-DN12%20-%20Epic%20Express%20Invoke%20Files.md). This includes custom handling of JSON native types (boolean, integer, null, array) as well as custom handling of JSON strings for encoding addresses with `@` prefix, hashes with `#` prefix, and hex strings with `0x` prefix.

## Persistence

This library contains two `EpicChain.Persistence.IStore` implementations:

- **RocksDbStore**: This implementation stores blockchain information in a [RocksDb](https://rocksdb.org/). It is similar to the RocksDbStore implementation in [epic-modules](https://github.com/epicchain/epic-modules), but is optimized for developer scenarios, including live checkpoint support.

- **MemoryTrackingStore**: This implementation sits on top of any `EpicChain.Persistence.IReadOnlyStore` implementation and stores all changes in memory. This enables test/debug runs to use live data without persisting further changes.

- **PersistentTrackingStore**: This implementation sits on top of any `EpicChain.Persistence.IReadOnlyStore` implementation and stores all changes on disk.

- **CheckpointStore**: This implementation of `EpicChain.Persistence.IReadOnlyStore` pulls data from an Epic Express checkpoint. Combined with a tracking store, this enables test/debug runs to use live data without persisting further changes.

- **StateServiceStore**: This implementation of `EpicChain.Persistence.IReadOnlyStore` sits on top of a [StateService node](https://github.com/epicchain/epic-modules/tree/master/src/StateService) running with `FullState: true`. Combined with a tracking store, this enables code to use live data from a public EpicChain network such as MainNet or TestNet.

## Trace Models

This library contains the model classes that read/write Time Travel Debugging (TTD) traces. TTD traces are encoded using [MessagePack](https://msgpack.org/). These model classes use the [MessagePack](https://github.com/neuecc/MessagePack-CSharp) managed library. In addition to the trace model classes, this library includes message pack formatters for Epic types that are serialized in TTD traces as well as a MessagePack resolver.

## Application Engines

This library contains two `EpicChain.SmartContract.ApplicationEngine` subclasses:

- **TestApplicationEngine**: This implementation is used across test scenarios. It supports overriding the CheckWitness service and collecting code coverage information.
- **TraceApplicationEngine**: This implementation writes trace information to a provided ITraceDebugSink. The Trace Model classes (described above) include an implementation of ITraceDebugSink that writes trace messages to a file in MessagePack format. 

## Appreciations

We really appreciate all the partners contributing code to this project, especially [vikkko](https://github.com/vikkkko) and [joeqian](https://github.com/joeqian10). Also, [WSbaikaishui](https://github.com/WSbaikaishui), [zifanwangsteven](https://github.com/zifanwangsteven), and [RookieCoderrr](https://github.com/RookieCoderrr) for their valuable advice. [Celia18305](https://github.com/Celia18305) is a perfect document worker who helps to organize all the documents.

Don't forget to give us a STAR if you like it!