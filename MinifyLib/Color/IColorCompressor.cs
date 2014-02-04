// -------------------------------------------------------------------------------
//    IColorCompressor.cs
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
namespace MinifyLib.Color {
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Exposes the methods needed to compress color codes.
    /// </summary>
    public interface IColorCompressor {

        /// <summary>
        /// Compresses a six character hexadecimal value to its three character representation if possible.
        /// </summary>
        /// <param name="hexadecimal">A hexadecimal value.</param>
        /// <returns>
        /// If possible, returns the compressed hex value. Otherwise, returns the original hex.
        /// </returns>
        string CompressHex( string hexadecimal );

        /// <summary>
        /// Compresses RGB values.
        /// </summary>
        /// <param name="rgb">A byte array containing the Red, Green, Bblue values</param>
        /// <returns>A hexadecimal value representing the supplied RGB values.</returns>
        string CompressRgb( byte[] rgb );

        /// <summary>
        /// Compresses HSL values.
        /// </summary>
        /// <param name="hue">Hue value contained in the set [0, 1].</param>
        /// <param name="saturation">Saturation value contained in the set [0, 1].</param>
        /// <param name="lightness">Lightness value contained in the set [0, 1].</param>
        /// <returns>A hexadecimal value representing the supplied HSL values.</returns>
        string CompressHsl( float hue, float saturation, float lightness );

        /// <summary>
        /// Swaps out a hexadecimal color value for its smaller literal color name.
        /// </summary>
        /// <param name="hexadecimal">A compressed hexadecimal value.</param>
        /// <returns>
        /// Literal color name, it a smaller one exists. Otherwise, returns the hex value.
        /// </returns>
        string HexadecimalToName( string hexadecimal );

        /// <summary>
        /// Swaps out a literal color name for its smaller hexadecimal color value.
        /// </summary>
        /// <param name="name">The color name.</param>
        /// <returns>
        /// The supplied string with the literal names converted to hex if possible.
        /// </returns>
        string NameToHexadecimal( string name );
    }
}
