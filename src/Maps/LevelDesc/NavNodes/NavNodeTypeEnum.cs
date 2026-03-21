using System;

namespace WallyMapSpinzor2;

public enum NavNodeTypeEnum
{
    _,
    W,
    A,
    L,
    G,
    T,
    S
}

[Flags]
public enum NavNodePathTypeFlags
{
    D,
    A,
    L,
    G,
    T,
    S,
}