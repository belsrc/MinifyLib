// -------------------------------------------------------------------------------
//    ColorConverter.cs
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

    /// <summary>
    /// Class to convert between different color codes.
    /// </summary>
    public class ColorConverter : IColorConverter {

        /// <summary>
        /// Initializes a new instance of the ColorConverter class.
        /// </summary>
        public ColorConverter() { }

        /// <summary>
        /// Converts an RGB color value to Hexadecimal.
        /// </summary>
        /// <param name="rgb">A byte array containing the R, G, B values</param>
        /// <returns>A hexadecimal value representing the supplied RGB values.</returns>
        public string ConvertRgbToHex( byte[] rgb ) {
            return BitConverter.ToString( rgb ).Replace( "-", string.Empty ).ToLowerInvariant();
        }

        /// <summary>
        /// Converts an RGB color value to Hexadecimal.
        /// </summary>
        /// <param name="red">The Red color value.</param>
        /// <param name="green">The Green color value.</param>
        /// <param name="blue">The Blue color value.</param>
        /// <returns>A hexadecimal value representing the supplied RGB values.</returns>
        public string ConvertRgbToHex( byte red, byte green, byte blue ) {
            return this.ConvertRgbToHex( new byte[] { red, green, blue } );
        }

        /// <summary>
        /// Converts an HSL color value to Hexadecimal.
        /// </summary>
        /// <remarks>
        /// <para>Ported from mjijackson.com/2008/02/rgb-to-hsl-and-rgb-to-hsv-color-model-conversion-algorithms-in-javascript.</para>
        /// <para>Conversion formula adapted from en.wikipedia.org/wiki/HSL_color_space.</para>
        /// </remarks>
        /// <param name="hue">Hue value contained in the set [0, 360].</param>
        /// <param name="saturation">Saturation value contained in the set [0, 100].</param>
        /// <param name="lightness">Lightness value contained in the set [0, 100].</param>
        /// <returns>A hexadecimal value representing the supplied HSL values.</returns>
        public string ConvertHslToHex( float hue, float saturation, float lightness ) {
            float r, g, b, q, p;
            hue = hue / 360F;
            saturation = saturation / 100F;
            lightness = lightness / 100F;

            if( saturation == 0 ) {
                r = g = b = lightness;
            }
            else {
                q = lightness < 0.5F ? lightness * ( 1F + saturation ) : lightness + saturation - ( lightness * saturation );
                p = 2F * lightness - q;
                r = this.HueToRgb( p, q, hue + 1F / 3F );
                g = this.HueToRgb( p, q, hue );
                b = this.HueToRgb( p, q, hue - 1F / 3F );
            }

            byte rb = (byte)Math.Round( r * 255 );
            byte gb = (byte)Math.Round( g * 255 );
            byte bb = (byte)Math.Round( b * 255 );

            byte[] rgb = { rb, gb, bb };

            return this.ConvertRgbToHex( rgb );
        }

        // Factored out section of the ConvertHslToHex method.
        private float HueToRgb( float p, float q, float t ) {
            if( t < 0F ) { t += 1F; }
            if( t > 1F ) { t -= 1F; }

            if( t * 6F < 1F ) { return p + ( q - p ) * 6F * t; }
            if( t * 2F < 1F ) { return q; }
            if( t * 3F < 2F ) { return p + ( q - p ) * ( ( 2F / 3F ) - t ) * 6F; }

            return p;
        }
    }
}
