namespace ZundaChan.Core
{
    public class TalkTask
    {
        public TalkTask(string text)
        {
            Text = text;
        }
        public string Text { get; init; }
        public int Speed { get; init; } = -1;
        public int Tone { get; init; } = -1;
        public int Volume { get; init; } = -1;
        public int Type { get; init; } = -1;
    }
}
#if !NET5_0_OR_GREATER
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Runtime.CompilerServices
{
    internal sealed class IsExternalInit
    { }
}
#endif