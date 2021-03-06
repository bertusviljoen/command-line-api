﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Binding;
using System.Linq;

namespace System.CommandLine.Parsing
{
    public class CommandResult : SymbolResult
    {
        private ArgumentConversionResultSet? _results;

        internal CommandResult(
            ICommand command,
            Token token,
            CommandResult? parent = null) :
            base(command ?? throw new ArgumentNullException(nameof(command)),
                 parent)
        {
            Command = command;

            Token = token ?? throw new ArgumentNullException(nameof(token));
        }

        public ICommand Command { get; }

        public Token Token { get; }

        internal ArgumentConversionResultSet ArgumentConversionResults
        {
            get
            {
                if (_results is null)
                {
                    var results = Children
                                  .OfType<ArgumentResult>()
                                  .Select(r => r.GetArgumentConversionResult());

                    _results = new ArgumentConversionResultSet();

                    foreach (var result in results)
                    {
                        _results.Add(result);
                    }
                }

                return _results;
            }
        }

        internal override bool UseDefaultValueFor(IArgument argument) =>
            Children.ResultFor(argument) switch
            {
                ArgumentResult arg => arg.Argument.HasDefaultValue && 
                                      arg.Tokens.Count == 0,
                _ => false
            };
    }
}
