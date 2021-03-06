﻿// -------------------------------------------------------------------------------
//    Minifier.cs
//    Copyright (c) 2014 Bryan Kizer
//    All rights reserved.
//
//    Redistribution and use in source and binary forms, with or without
//    modification, are permitted provided that the following conditions are
//    met:
//
//    Redistributions of source code must retain the above copyright notice,
//    this list of conditions and the following disclaimer.
//
//    Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
//
//    Neither the name of the Organization nor the names of its contributors
//    may be used to endorse or promote products derived from this software
//    without specific prior written permission.
//
//    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
//    IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
//    TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
//    PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
//    HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
//    SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
//    TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//    PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//    LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//    NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//    SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// -------------------------------------------------------------------------------
namespace MinifyLib {
    using MinifyLib.Color;
    using MinifyLib.Manipulate;

    /// <summary>
    /// A class to handle the minification of CSS files.
    /// </summary>
    /// <remarks>
    /// Some RE's were pulled from Isaac Schlueter's rules list as is, 
    /// others (most) were shortened or modified in some manner and 
    /// some are new (where I thought I could go a little more in depth)
    /// 
    /// Isaac's list can me found on github - 
    /// github.com/isaacs/cssmin/blob/master/rules.txt
    /// </remarks>
    public class Minifier {
        private Manipulation _manip;

        /// <summary>
        /// Initializes a new instance of the Minifier class.
        /// </summary>
        public Minifier() { }

        /// <summary>
        /// Cleans/compresses several aspects of CSS code.
        /// </summary>
        /// <remarks>
        /// This should be everything...
        /// </remarks>
        /// <param name="css">The string value of the file(s).</param>
        /// <returns>A minified version of the supplied CSS string.</returns>
        public string Minify( string css ) {

            ColorCompressor colors = new ColorCompressor( new ColorConverter() );
            this._manip = new Manipulation( colors, css );
            this._manip.SwapForPlaceholders()
                       .NormalizeSource()
                       .CleanSelectors()
                       .CleanBraces()
                       .CleanUnnecessary()
                       .ConvertColors()
                       .CompressHexValues()
                       .CompressColorNames()
                       .FixIllFormedHsl()
                       .ReplaceTransparent()
                       .ReplaceFontWeight()
                       .ReplacePlaceholders();

            // Return the string after trimming any leading or trailing spaces
            return this._manip.AlteredString.Trim();
        }
    }
}
