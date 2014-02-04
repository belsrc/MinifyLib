// -------------------------------------------------------------------------------
//    Manipulation.cs
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
namespace MinifyLib.Manipulate {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using MinifyLib.Color;

    /// <summary>
    /// Class for manipulating the CSS source file string.
    /// </summary>
    public class Manipulation {

        // Placeholders for values we don't want to run through the minifier.
        private const string PSEUDO_REPLACE = "___PSEUDO_REPLACE___";
        private const string URL_REPLACE = "___URL_REPLACE___";
        private const string DATA_REPLACE = "___DATA_REPLACE___";
        private const string CONTENT_REPLACE = "___CONTENT_REPLACE___";
        private const string IMPORTANT_COM = "___IMPORTANT_COM___";

        // Lists for holding the replaced values.
        private List<string> _urls;
        private List<string> _data;
        private List<string> _contents;
        private List<string> _comments;

        private IColorCompressor _compress;

        /// <summary>
        /// Initializes a new instance of the Manipulation class.
        /// </summary>
        /// <param name="compressor">An instance of IColorCompressor.</param>
        /// <param name="source">The CSS source string.</param>
        public Manipulation( IColorCompressor compressor, string source ) {
            if( compressor == null ) {
                throw new ArgumentNullException( "compressor", "The compressor can not be null." );
            }

            if( source == null ) {
                throw new ArgumentNullException( "source", "The source string can not be null." );
            }

            this._compress = compressor;
            this.BaseString = this.AlteredString = source;
            this._urls = new List<string>();
            this._data = new List<string>();
            this._contents = new List<string>();
            this._comments = new List<string>();
        }

        /// <summary>
        /// Gets the base source string.
        /// </summary>
        public string BaseString { get; private set; }

        /// <summary>
        /// Gets the altered source string.
        /// </summary>
        public string AlteredString { get; private set; }

        /// <summary>
        /// Empties the list containing the placeheld values.
        /// </summary>
        public void EmptyContainers() {
            this._urls.Clear();
            this._data.Clear();
            this._contents.Clear();
        }

        /// <summary>
        /// Cleans up quoted content strings.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <remarks>Remove quoted strings, remaining spaces, put quoted strings back</remarks>
        /// <returns>The cleaned content string.</returns>
        public string CleanContents( string input ) {
            List<string> quoted = new List<string>();
            string tmp = "___quote___";

            input = Regex.Replace(
                input,
                "\".*?\"",
                m => {
                    quoted.Add( m.Value );
                    return tmp;
                } );

            input = Regex.Replace( input, "\\s*", string.Empty );

            for( int i = 0; i < quoted.Count; i++ ) {
                input = this.ReplaceFirst( input, tmp, quoted[i] );
            }

            return input;
        }

        /// <summary>
        /// Cleans the format of data urls.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <remarks>
        /// Get rid of any spaces and quotes in the type declaration
        /// Since theres a possible charset (for fonts) Ill jump to the base64 and leave it at that,
        /// Get rid of any space and quotes at the end
        /// </remarks>
        /// <returns>The cleaned data url string.</returns>
        public string CleanDataUrls( string input ) {
            input = Regex.Replace( input, "^url\\s*\\(\\s*[\\\"']?data\\s*:\\s*(?<Type>.*?)\\s*;\\s*", "url(data:${Type};" );
            input = Regex.Replace( input, "\\s*base64\\s*,\\s*", "base64," );
            input = Regex.Replace( input, "[\\\"']?\\s*\\)$", ")" );

            return input;
        }

        /// <summary>
        /// Cleans the format of urls.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <remarks>
        /// Remove the quotes since they arent needed in urls
        /// Get rid of any space at the beginning or end
        /// </remarks>
        /// <returns>The cleaned url string.</returns>
        public string CleanNormUrls( string input ) {
            input = Regex.Replace( input, "[\"']", string.Empty );
            input = Regex.Replace( input, "^url\\s*\\(\\s*", "url(" );
            input = Regex.Replace( input, "\\s*\\)$", ")" );

            return input;
        }

        /// <summary>
        /// Replaces some values from the CSS source string that shouldn't be ran through the minifier.
        /// </summary>
        /// <returns>The current Manipulation object.</returns>
        /// <remarks>
        /// Sets placeholders for Data URLs, normal URLs, Pseudo classes, Content values, and important Comments.
        /// </remarks>
        public Manipulation SwapForPlaceholders() {
            // data url
            this.AlteredString = Regex.Replace(
                this.AlteredString,
                "(?<=[:;\\s]+)url\\(\\s?[\\'\\\\\"]?data:(.*?)[\\'\\\\\"]?\\s?\\)(?=[;\\}\\s]+)",
                m => {

                    _data.Add( this.CleanDataUrls( m.Value ) );
                    return DATA_REPLACE;
                } );

            // url
            this.AlteredString = Regex.Replace(
                this.AlteredString,
                "(?<=[:;\\s]+)url\\(\\s?[\\'\\\\\"]?(?!\\w+:)(.*?)[\\'\\\\\"]?\\s?\\)(?=[;\\}\\s]+)",
                m => {
                    _urls.Add( this.CleanNormUrls( m.Value ) );
                    return URL_REPLACE;
                } );

            // pseudo-class colon
            this.AlteredString = Regex.Replace(
                this.AlteredString,
                "(^|\\})(([^\\{:])+:)+([^\\{]*\\{)",
                m => {
                    return m.Value.Replace( ":", PSEUDO_REPLACE );
                } );

            // content
            this.AlteredString = Regex.Replace(
                this.AlteredString,
                "(?<=content\\s*:).*?(?=;)",
                m => {
                    _contents.Add( this.CleanContents( m.Value ) );
                    return CONTENT_REPLACE;
                } );

            // important comment /*!  */
            this.AlteredString = Regex.Replace(
                this.AlteredString,
                "/\\*!.+?\\*/",
                m => {
                    _comments.Add( Regex.Replace( m.Value, "\\s+", " " ) );
                    return IMPORTANT_COM;
                },
                RegexOptions.Singleline );

            return this;
        }

        /// <summary>
        /// Normalizes the content of the CSS source string.
        /// </summary>
        /// <returns>The current Manipulation object.</returns>
        /// <remarks>Get rid of comment strings, Get rid of newline characters, Normalize remainig whitespace.</remarks>
        public Manipulation NormalizeSource() {
            this.AlteredString = Regex.Replace(
                this.AlteredString,
                "/\\*.+?\\*/",
                string.Empty,
                RegexOptions.Singleline
            );

            this.AlteredString = Regex.Replace(
                this.AlteredString,
                "([\\r\\n])*",
                string.Empty
            );

            this.AlteredString = Regex.Replace(
                this.AlteredString,
                "\\s+",
                " "
            );

            return this;
        }

        /// <summary>
        /// Removes whitespace around operators.
        /// </summary>
        /// <returns>The current Manipulation object.</returns>
        /// <remarks>Operators: : ; ,  * > + ~ = ^= $= *= |= ~= !</remarks>
        public Manipulation CleanSelectors() {
            this.AlteredString = Regex.Replace(
                this.AlteredString,
                "\\s*(?<Selector>(:|;|,|\\*|>|\\+|=|~|/|(\\^=)|(\\$=)|(\\*=)|(\\|=)|(~=)|!))\\s*",
                "${Selector}"
            );

            return this;
        }

        /// <summary>
        /// Cleans the whitespace around the braces in the source string.
        /// </summary>
        /// <returns>The current Manipulation object.</returns>
        /// <remarks>Braces: {} [] ()</remarks>
        public Manipulation CleanBraces() {
            // Since theres no special cases with them just grab all the spaces around them
            this.AlteredString = Regex.Replace(
                this.AlteredString,
                "\\s*(?<Brace>[\\{\\}])\\s*",
                "${Brace}"
            );

            this.AlteredString = Regex.Replace(
                this.AlteredString,
                "(?<Open>[\\(\\[])\\s+",
                "${Open}"
            );

            this.AlteredString = Regex.Replace(
                this.AlteredString,
                "\\s+(?<Close>[\\)\\]])",
                "${Close}"
            );

            return this;
        }

        /// <summary>
        /// Removes/shortens various unnecessary values.
        /// </summary>
        /// <returns>The current Manipulation object.</returns>
        /// <remarks>
        /// Get rid of last semi-colon before closing brace, 
        /// leading zeros on decimals, 
        /// measurements on zero values, 
        /// shorten zero'd out values (margin: 0 0 0 0), 
        /// set various borders to zero if they were previously 'none', 
        /// shorten MS filter, 
        /// remove unused rule-sets.
        /// </remarks>
        public Manipulation CleanUnnecessary() {
            this.AlteredString = Regex.Replace(
                this.AlteredString,
                ";}\\s*",
                "}"
            );

            this.AlteredString = Regex.Replace(
                this.AlteredString,
                "(?<=[:,\\s\\(])0+(?<Value>\\.\\d+)",
                "${Value}"
            );

            this.AlteredString = Regex.Replace(
                this.AlteredString,
                "(?<=[:,\\s\\(])0+(%|in|[cme]m|ex|p[tcx]|rem)",
                "0"
            );

            this.AlteredString = Regex.Replace(
                this.AlteredString,
                ":(0 0 0 0|0 0)(?=;|\\})",
                ":0"
            );

            this.AlteredString = Regex.Replace(
                this.AlteredString,
                "(?<Borders>(border(-top|-right|-bottom|-left)?|outline|background)):none",
                "${Borders}:0"
            );

            // Removed the box|text shadow replacement since, apparently, its illegal to set those to zero, 
            // which would be nice. Especially since you can do it with borders and a number of other props.
            // Hopefully it will eventually change so I can put it back in and save more space.

            this.AlteredString = Regex.Replace(
                this.AlteredString,
                "progid:DXImageTransform\\.Microsoft\\.Alpha\\(Opacity=",
                "alpha(opacity="
            );

            this.AlteredString = Regex.Replace(
                this.AlteredString,
                "[^\\};\\{\\/]+\\{\\}",
                string.Empty
            );

            return this;
        }

        /// <summary>
        /// Converts RGB and HSL color values to their hex equivalent.
        /// </summary>
        /// <returns>The current Manipulation object.</returns>
        /// <remarks>Replace all rgb|hsl(#,#,#) and rgba|hsla(#,#,#,1) strings with their hex value.</remarks>
        public Manipulation ConvertColors() {
            char[] parens = { '(', ')' };
            string[] nums;

            this.AlteredString = Regex.Replace(
                this.AlteredString,
                "rgba?\\(\\d{1,3},\\d{1,3},\\d{1,3}(,1)?\\)",
                m => {
                    nums = m.Value.Split( parens )[1].Split( ',' );
                    try {
                        byte[] rgb = { 
                            Byte.Parse( nums[ 0 ], CultureInfo.InvariantCulture ),
                            Byte.Parse( nums[ 1 ], CultureInfo.InvariantCulture ),
                            Byte.Parse( nums[ 2 ], CultureInfo.InvariantCulture )
                        };

                        return m.Value.Replace( m.Value, this._compress.CompressRgb( rgb ) );
                    }
                    catch( Exception e ) {
                        throw new MinifyException( "Failed to parse RGBA color codes.", e );
                    }
                } );

            // Have to '{0,1}' the '%' since they may have been removed in a previous step
            this.AlteredString = Regex.Replace(
                this.AlteredString,
                "hsla?\\(\\d{1,3},\\d{1,3}%?,\\d{1,3}%?(,1)?\\)",
                m => {
                    nums = m.Value.Replace( "%", string.Empty ).Split( parens )[1].Split( ',' );
                    float h, s, l;

                    try {
                        h = float.Parse( nums[0], CultureInfo.InvariantCulture );
                        s = float.Parse( nums[1], CultureInfo.InvariantCulture );
                        l = float.Parse( nums[2], CultureInfo.InvariantCulture );

                        return m.Value.Replace( m.Value, this._compress.CompressHsl( h, s, l ) );
                    }
                    catch( Exception ex ) {
                        throw new MinifyException( "Failed to parse HSLA color codes.", ex );
                    }
                } );

            return this;
        }

        /// <summary>
        /// Compresses hex values if possible.
        /// </summary>
        /// <returns>The current Manipulation object.</returns>
        /// <remarks>
        /// Match hex values that are 6 long and compress them down to 3 if possible, 
        /// Grab all the hex strings and replace with the literal color name if is shorter than the hex value.
        /// </remarks>
        public Manipulation CompressHexValues() {
            string tmp = string.Empty;

            this.AlteredString = Regex.Replace(
                this.AlteredString,
                "#[0-9a-fA-F]{6}(?=,|;|\\}\\)\\s)",
                m => {
                    tmp = this._compress.CompressHex( m.Value.Replace( "#", string.Empty ) );
                    return m.Value.Replace( m.Value, tmp );
                } );

            this.AlteredString = Regex.Replace(
                this.AlteredString,
                "#([0-9a-fA-F]{3}){1,2}(?=,|;|\\}\\)\\s)",
                m => {
                    tmp = this._compress.HexadecimalToName( m.Value );
                    return m.Value.Replace( m.Value, tmp );
                } );

            return this;
        }

        /// <summary>
        /// Replaces any color literal names with the shorter hex value.
        /// </summary>
        /// <returns>The current Manipulation object.</returns>
        public Manipulation CompressColorNames() {
            string tmp = string.Empty;
            List<string> names = new List<string> { 
                "aliceblue",       "antiquewhite",     "aqua",                 "aquamarine",
                "black",           "blanchedalmond",   "blue",                 "blueviolet",
                "burlywood",       "cadetblue",        "chartreuse",           "chocolate",
                "cornflowerblue",  "cornsilk",         "crimson",              "cyan",
                "darkblue",        "darkcyan",         "darkgoldenrod",        "darkgray",
                "darkgrey",        "darkgreen",        "darkkhaki",            "darkmagenta",
                "darkolivegreen",  "darkorange",       "darkorchid",           "darkred",
                "darksalmon",      "darkseagreen",     "darkslateblue",        "darkslategray",
                "darkslategrey",   "darkturquoise",    "darkviolet",           "deeppink",
                "deepskyblue",     "dimgray",          "dimgrey",              "dodgerblue",
                "firebrick",       "floralwhite",      "forestgreen",          "fuchsia",
                "gainsboro",       "ghostwhite",       "goldenrod",            "greenyellow",
                "honeydew",        "hotpink",          "indianred",            "lavender",
                "lavenderblush",   "lawngreen",        "lemonchiffon",         "lightblue",
                "lightcoral",      "lightcyan",        "lightgoldenrodyellow", "lightgray",
                "lightgrey",       "lightgreen",       "lightpink",            "lightsalmon",
                "lightseagreen",   "lightskyblue",     "lightslategray",       "lightslategrey",
                "lightsteelblue",  "lightyellow",      "lime",                 "limegreen",
                "magenta",         "mediumaquamarine", "mediumblue",           "mediumorchid",
                "mediumpurple",    "mediumseagreen",   "mediumslateblue",      "mediumspringgreen",
                "mediumturquoise", "mediumvioletred",  "midnightblue",         "mintcream",
                "mistyrose",       "moccasin",         "navajowhite",          "oldlace",
                "olivedrab",       "orangered",        "palegoldenrod",        "palegreen",
                "paleturquoise",   "palevioletred",    "papayawhip",           "peachpuff",
                "powderblue",      "rosybrown",        "royalblue",            "saddlebrown",
                "sandybrown",      "seagreen",         "seashell",             "skyblue",
                "slateblue",       "slategray",        "slategrey",            "springgreen",
                "steelblue",       "thistle",          "turquoise",            "white",
                "whitesmoke",      "yellowgreen"
            };

            foreach( var name in names ) {
                this.AlteredString = Regex.Replace(
                    this.AlteredString,
                    name + "(?=,|;|\\}\\)\\s)",
                    m => {
                        tmp = this._compress.NameToHexadecimal( m.Value );
                        return m.Value.Replace( m.Value, tmp );
                    },
                    RegexOptions.IgnoreCase
                );
            }

            return this;
        }

        /// <summary>
        /// Fixes any damaged HSL values from other compression methods.
        /// </summary>
        /// <returns>The current Manipulation object.</returns>
        /// <remarks>
        /// Since I kind of...butcher hsla colors with 0 values I need to fix them
        /// Find all hsl/hsla colors with no '%' on the S and L values
        /// Split the match on the ','
        /// Since one may be good and the other not, strip '%'
        /// Put it back together with the '%'
        /// </remarks>
        public Manipulation FixIllFormedHsl() {
            this.AlteredString = Regex.Replace(
                this.AlteredString,
                "(?<=hsla?\\(\\d{1,3},)\\d{1,3}%?,\\d{1,3}%?(?=(,(0|1)?(\\.\\d+)?)?\\))",
                m => {
                    string[] tmp = m.Value.Split( ',' );
                    return tmp[0].Replace( "%", string.Empty ) + "%," + tmp[1]
                                 .Replace( "%", string.Empty ) + "%";
                    // Kind of hack-ish I admit
                } );

            return this;
        }

        /// <summary>
        /// Replaces transparent color codes with the literal name.
        /// </summary>
        /// <returns>The current Manipulation object.</returns>
        /// <remarks>
        /// 'transparent' == rgba(0,0,0,0) == hsla(0,0%,0%,0)
        /// Should be fine...if it supports Alpha should support the 'transparent' color literal
        /// ...right?
        /// </remarks>
        public Manipulation ReplaceTransparent() {
            this.AlteredString = this.AlteredString.Replace( "rgba(0,0,0,0)", "transparent" )
                                                  .Replace( "hsla(0,0%,0%,0)", "transparent" );

            return this;
        }

        /// <summary>
        /// Replaces the named literal font weight with the numeric value.
        /// </summary>
        /// <returns>The current Manipulation object.</returns>
        public Manipulation ReplaceFontWeight() {
            this.AlteredString = this.AlteredString.Replace( "font-weight:bold", "font-weight:700" )
                                                  .Replace( "font-weight:normal", "font-weight:400" );

            return this;
        }

        /// <summary>
        /// Returns the values the were taken out with SwapForPlaceholders.
        /// </summary>
        /// <returns>The current Manipulation object.</returns>
        public Manipulation ReplacePlaceholders() {
            // url(data)
            for( int i = 0; i < this._data.Count; i++ ) {
                this.AlteredString = this.ReplaceFirst(
                    this.AlteredString,
                    DATA_REPLACE,
                    this._data[i]
                );
            }

            // url()
            for( int i = 0; i < this._urls.Count; i++ ) {
                this.AlteredString = this.ReplaceFirst(
                    this.AlteredString,
                    URL_REPLACE,
                    this._urls[i]
                );
            }

            // pseudo-class colons
            this.AlteredString = this.AlteredString.Replace( PSEUDO_REPLACE, ":" );

            // content:
            for( int i = 0; i < this._contents.Count; i++ ) {
                this.AlteredString = this.ReplaceFirst(
                    this.AlteredString,
                    CONTENT_REPLACE,
                    this._contents[i]
                );
            }

            // important comments
            for( int i = 0; i < this._comments.Count; i++ ) {
                this.AlteredString = this.ReplaceFirst(
                    this.AlteredString,
                    IMPORTANT_COM,
                    this._comments[i]
                );
            }

            return this;
        }

        // Replace the first occurrence of a specified string  
        // This method performs an ordinal (case-sensitive and culture-insensitive) 
        // search to find oldValue. Original code from DotNetPerls - 
        // www.dotnetperls.com/replace-extension
        private string ReplaceFirst( string source, string oldValue, string newValue ) {
            int index = source.IndexOf( oldValue, StringComparison.Ordinal );

            if( index == -1 ) {
                return source;
            }

            int replacementLength = newValue.Length;
            int patternLength = oldValue.Length;
            int valueLength = source.Length;

            char[] array = new char[valueLength + replacementLength - patternLength];
            source.CopyTo( 0, array, 0, index );
            newValue.CopyTo( 0, array, index, replacementLength );
            source.CopyTo( index + patternLength, array, index + replacementLength, valueLength - ( index + patternLength ) );

            return new string( array );
        }
    }
}
