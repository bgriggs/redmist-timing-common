
# RedMist TimingCommon

[![Build](https://github.com/bgriggs/redmist-timing-common/actions/workflows/build.yml/badge.svg)](https://github.com/bgriggs/redmist-timing-common/actions/workflows/build.yml)
[![.NET](https://img.shields.io/badge/.NET-10.0-purple)](https://dotnet.microsoft.com/download)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

A .NET library providing shared model classes the Red Mist Timing project. This can be used as a reference for building integrations with the Red Mist API.

## Overview

RedMist.TimingCommon is a shared library that defines the core data models, serialization attributes, and code generation utilities used across Red Mist Timing applications. It provides a strongly-typed API for representing racing events, sessions, car positions, timing data, and configuration metadata. Most objects support JSON and MessagePack serialization. This package is also available on nuget at https://www.nuget.org/packages/RedMist.TimingCommon/

## Features

- **Racing Models**: Complete data models for events, sessions, car positions, timing data, driver information, and more
- **Multiple Timing System Support**: Integration support for industry-standard timing systems including:
  - Orbits
  - X2 Racing
- **High-Performance Serialization**: Built-in support for MessagePack binary serialization for efficient data transfer
- **Automatic Patch Generation**: Source generator for creating nullable patch variants of models to support partial updates
- **In-Car Systems**: Models for in-car video systems and driver mode displays
- **Flag Management**: Track flag states and durations
- **Configuration Management**: Organization, event, and scheduling configuration models

## Target Frameworks

- .NET 10.0
- .NET Standard 2.0 (for source generators)

## Projects

### RedMist.TimingCommon
The main library containing all shared models, attributes, and extensions.

### RedMist.PatchGenerator
A Roslyn source generator that automatically creates patch classes for models decorated with `[GeneratePatch]`. Patch classes have all properties converted to nullable types, enabling efficient partial updates and delta synchronization.

**Features:**
- Automatic generation of nullable patch variants
- Configurable MessagePack, JSON, and validation attribute inclusion
- Automatic Mapperly mapper generation for applying patches
- Customizable class and namespace naming

## Installation

```bash
dotnet add package RedMist.TimingCommon
```

## Core Model Categories

### Session & Event Models
- `SessionState` - Complete race session state including timing, positions, and flags
- `Event` - Racing event details with sessions and scheduling
- `EventEntry` - Individual competitor entries
- `Session` - Session/run information

### Position & Timing Models
- `CarPosition` - Real-time car position and timing data
- `CompletedSection` - Section completion times
- `DriverInfo` - Driver and team information
- `CompetitorMetadata` - Extended competitor information

### Configuration Models
- `Organization` - Series/league configuration and branding
- `OrbitsConfiguration` - Orbits timing system settings
- `X2Configuration` - X2 Racing system settings
- `ClassMetadata` - Racing class definitions
- `EventSchedule` - Event scheduling and calendar

### In-Car Systems
- `VideoMetadata` - In-car video system configuration
- `VideoDestination` - Video streaming/recording destinations
- `CarStatus` - Real-time car status for driver displays
- `InCarPayload` - Complete in-car system data payload

### Flag & Announcement Models
- `Flags` - Track flag states
- `FlagDuration` - Duration of flag periods
- `Announcement` - Race announcements and messages

## Usage Examples

### Basic Model Usage with MessagePack

```csharp
using RedMist.TimingCommon.Models;
using MessagePack;

// Serialize session state
var sessionState = new SessionState
{
    EventId = 123,
    EventName = "Championship Race",
    SessionId = 1,
    SessionName = "Feature Race",
    LapsToGo = 10
};

byte[] data = MessagePackSerializer.Serialize(sessionState);

// Deserialize
var restored = MessagePackSerializer.Deserialize<SessionState>(data);
```

### Using the Patch Generator

```csharp
using RedMist.TimingCommon.Attributes;

[GeneratePatch]
public class MyRaceData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int LapCount { get; set; }
}

// Source generator automatically creates:
// - MyRaceDataPatch (with nullable properties)
// - MyRaceDataMapper (for applying patches)

// Apply partial update
var patch = new MyRaceDataPatch { LapCount = 15 };
var mapper = new MyRaceDataMapper();
mapper.ApplyPatch(patch, myRaceData);
```

## Extensions

The library includes helpful extension methods:
- `CarPositionExtensions` - Position and gap calculations
- `SessionExtensions` - Session state helpers
- `PayloadExtensions` - Data payload utilities