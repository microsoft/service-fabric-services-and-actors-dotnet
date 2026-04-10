---
description: "Use when writing or reviewing .NET interop code."
applyTo: "**/*.cs"
---

# Interop Guidelines

- Product code must work identically on `net462` (legacy COM interop) and `net8.0+` (ComWrappers/`[GeneratedComInterface]`).
  Prefer a single implementation over `#if`-separated code paths.
- For COM method parameters that contain strings or string arrays, prefer passing a blittable struct via `IntPtr` rather
  than using `[MarshalAs]` attributes. Marshalling attributes behave differently between legacy COM and ComWrappers,
  but a blittable `IntPtr` passes through both runtimes without any marshalling.
- Define native structs with `[StructLayout(LayoutKind.Sequential, Pack = 8)]` using only `IntPtr` and `uint` fields.
  Include an `IntPtr Reserved` field for future extensibility (allowing `_EX1`, `_EX2` extensions).
- Use `GCHandle.Alloc(value, GCHandleType.Pinned)` + `stackalloc` to pin strings and build pointer arrays on the caller
  side. See `Meter.RecordViaNative` and `MeterProvider.CreateNativeMeter` for the canonical pattern.
- Do not use `PinCollection` for new code unless the interop call requires it for other reasons.
