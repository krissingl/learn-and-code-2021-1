﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace learn_and_code
{
    public class Card
    {
        private static readonly UInt32 FacetBase = 0b__0001_0000;

        [Flags]
        public enum Facet : UInt32
        {
            // quantities
            DifferentQuantities = 0b__0001_0000__0000_0000__0000_0000__0000_0000,
            One                 = (DifferentQuantities << 1),
            Two                 = (DifferentQuantities << 2),
            Three               = (DifferentQuantities << 3),
            // colors
            DifferentColors     = 0b__0000_0000__0001_0000__0000_0000__0000_0000,
            Red                 = (DifferentColors << 1),
            Purple              = (DifferentColors << 2),
            Green               = (DifferentColors << 3),
            // shading
            DifferentShadings   = 0b__0000_0000__0000_0000__0001_0000__0000_0000,
            Solid               = (DifferentShadings << 1),
            Striped             = (DifferentShadings << 2),
            Outlined            = (DifferentShadings << 3),
            // shapes
            DifferentShapes     = 0b__0000_0000__0000_0000__0000_0000__0001_0000,
            Oval                = (DifferentShapes << 1),
            Squiggle            = (DifferentShapes << 2),
            Diamond             = (DifferentShapes << 3),
        }

        public enum WhichFacet : UInt32
        {
            Quantity = 0b_1110_0000_0000_0000_0000_0000_0000,
            Color    = 0b_0000_0000_1110_0000_0000_0000_0000,
            Shading  = 0b_0000_0000_0000_0000_1110_0000_0000,
            Shape    = 0b_0000_0000_0000_0000_0000_0000_1110,
        }

        public static Boolean IsValidFacet(Facet facet, WhichFacet whichFacet)
        {
            return ((1 == BitOperations.PopCount((UInt32)facet)) &&
                    ((UInt32)facet == ((UInt32)facet & (UInt32)whichFacet)));
        }

        // special
        // XXX: BUG - s/public/private/g
        // XXX: TO DO - refactor in terms of one another
        public static readonly UInt32 MagicOrMask     = 0b__0001_0000__0001_0000__0001_0000__0001_0000;
        public static readonly UInt32 MagicDelta      = 0b__0000_0010__0000_0010__0000_0010__0000_0010;
        public static readonly UInt32 NonInvertedMask = 0b__1111_0000__1111_0000__1111_0000__1111_0000;
        public static readonly UInt32 InvertedMask    = 0b__0000_1111__0000_1111__0000_1111__0000_1111;
        public static readonly UInt32 MagicXorMask    = (MagicOrMask | InvertedMask);

        private Facet _facets; // MUST have four bits set (one for each component facet)

        public Card (Facet facets)
        {
            this._facets = facets;
        }

        public Card (Facet quantity, Facet color, Facet shading, Facet shape)
        {
            Trace.Assert(IsValidFacet(quantity, WhichFacet.Quantity) &&
                         IsValidFacet(color, WhichFacet.Color) &&
                         IsValidFacet(shading, WhichFacet.Shading) &&
                         IsValidFacet(shape, WhichFacet.Shape));
            this._facets = quantity | color | shading | shape;
        }

        public static Facet StringToFacet(string input)
        {
            UInt32 accumulator = 0;
            foreach (char c in input)
            {
                accumulator <<= 8;
                accumulator |= FacetBase << (c - '0');
            }
            return (Facet)(accumulator ^ (accumulator >> 4) ^ MagicXorMask);
        }

        public Card (string input)
        {
            this._facets = StringToFacet(input);
        }

        ////////////////////////////////////////////////////////////////////////

        public static Facet FindMatch(Facet[] facets)
        {
            // Trace.Assert(2 == facets.Length);
            UInt32
                union = (UInt32)(facets[0] | facets[1]) & ~MagicOrMask,
                xorMask = ((union + MagicDelta) & MagicOrMask) * 0b1110;
            return (Facet)((union ^ xorMask) & NonInvertedMask);
        }

        public static Boolean IsMatch(Card[] cards)
        {
            Trace.Assert(3 == cards.Length);
            UInt32
                intersection = (UInt32)(cards[0]._facets & cards[1]._facets & cards[2]._facets),
                allDifferentCheck = intersection - MagicDelta,
                matches = (allDifferentCheck & NonInvertedMask) ^ MagicOrMask;
            return (4 == BitOperations.PopCount(matches));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Card[] cards = {
                new Card("1212"),
                new Card("2311"),
                new Card("3113")
            };

            // Console.WriteLine("{0:G} / {1:G} / {2:G}", facets[0], facets[1], Card.FindMatch(facets));
            Console.WriteLine("Match status: {0}", Card.IsMatch(cards));
        }
    }
}

// XXX: TO DO -
//
// - Apply MagicOrMask JIT (not stored in this._facets)
