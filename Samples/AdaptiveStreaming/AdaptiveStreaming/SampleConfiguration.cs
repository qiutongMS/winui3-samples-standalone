//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml.Controls;

namespace SDKTemplate
{
    public class BitrateHelper
    {
        private Dictionary<uint, Symbol> symbolAssignment = new Dictionary<uint, Symbol>();

        public BitrateHelper(IEnumerable<uint> availableBitRates)
        {
            uint[] sortedBitRates = availableBitRates.ToArray();
            Array.Sort(sortedBitRates);
            int i = 0;
            while (i < sortedBitRates.Length * 1 / 4)
            {
                symbolAssignment.Add(sortedBitRates[i++], Symbol.OneBar);
            }
            while (i < sortedBitRates.Length * 2 / 4)
            {
                symbolAssignment.Add(sortedBitRates[i++], Symbol.TwoBars);
            }
            while (i < sortedBitRates.Length * 3 / 4)
            {
                symbolAssignment.Add(sortedBitRates[i++], Symbol.ThreeBars);
            }
            while (i < sortedBitRates.Length * 4 / 4)
            {
                symbolAssignment.Add(sortedBitRates[i++], Symbol.FourBars);
            }
        }

        public Symbol GetBitrateSymbol(uint currentBitrate)
        {
            Symbol symbol = Symbol.ZeroBars;
            if (symbolAssignment.TryGetValue(currentBitrate, out symbol))
            {
                return symbol;
            }
            return Symbol.ZeroBars;
        }
    }
}
