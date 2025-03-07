// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Metadata;

namespace System.Text.Json.Serialization.Converters
{
    // Converter for F# lists: https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-list-1.html
    internal sealed class FSharpListConverter<TList, TElement> : IEnumerableDefaultConverter<TList, TElement>
        where TList : IEnumerable<TElement>
    {
        private readonly Func<IEnumerable<TElement>, TList> _listConstructor;

        [RequiresUnreferencedCode(FSharpCoreReflectionProxy.FSharpCoreUnreferencedCodeMessage)]
        [RequiresDynamicCode(FSharpCoreReflectionProxy.FSharpCoreUnreferencedCodeMessage)]
        public FSharpListConverter()
        {
            _listConstructor = FSharpCoreReflectionProxy.Instance.CreateFSharpListConstructor<TList, TElement>();
        }

        protected override void Add(in TElement value, ref ReadStack state)
        {
            ((List<TElement>)state.Current.ReturnValue!).Add(value);
        }

        internal override bool SupportsCreateObjectDelegate => false;
        protected override void CreateCollection(ref Utf8JsonReader reader, ref ReadStack state, JsonSerializerOptions options)
        {
            state.Current.ReturnValue = new List<TElement>();
        }

        protected override void ConvertCollection(ref ReadStack state, JsonSerializerOptions options)
        {
            state.Current.ReturnValue = _listConstructor((List<TElement>)state.Current.ReturnValue!);
        }
    }
}
