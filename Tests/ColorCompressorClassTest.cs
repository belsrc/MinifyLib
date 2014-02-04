
namespace MinifyLibTests {
    using System;
    using System.Collections.Generic;
    using MinifyLib.Color;
    using NUnit.Framework;

    [TestFixture]
    public class ColorCompressorClassTest {
        
        private ColorCompressor _compressor;

        public ColorCompressorClassTest() {
            this._compressor = new ColorCompressor( new ColorConverter() );
        }

        [Test]
        public void CompressHexTest() {
            List<string> hexes = new List<string>() {
                "ffffff",
                "27ae60",
                "22bb22"
            };

            List<string> expecteds = new List<string>() {
                "#fff",
                "#27ae60",
                "#2b2"
            };

            for( int i = 0; i < hexes.Count; i++ ) {
                string actual = this._compressor.CompressHex( hexes[i] );
                Assert.AreEqual( expecteds[i], actual, "Expected " + expecteds[i] + " but was " + actual );
            }
        }

        [Test]
        [ExpectedException( typeof( ArgumentNullException ) )]
        public void ConstructorException() {
            var tmp = new ColorCompressor( null ); 
        }

        [Test]
        [ExpectedException( typeof( ArgumentNullException ) )]
        public void CompressHexExceptionTest() {
            this._compressor.CompressHex( null );
        }

        [Test]
        public void CompressRgbTest() {
            List<byte[]> rgbs = new List<byte[]>() {
                new byte[] { 187, 238, 187 },
                new byte[] { 39, 174, 96 },
                new byte[] { 34, 187, 34 },
            };
            
            List<string> expecteds = new List<string>() {
                "#beb",
                "#27ae60",
                "#2b2"
            };

            for( int i = 0; i < rgbs.Count; i++ ) {
                string actual = this._compressor.CompressRgb( rgbs[i][0], rgbs[i][1], rgbs[i][2] );
                Assert.AreEqual( expecteds[i], actual, "Expected " + expecteds[i] + " but was " + actual );
            }
        }

        [Test]
        public void CompressHslTest() {
            List<float[]> hsls = new List<float[]>() {
                new float[] { 206f, 79f, 61f },
                new float[] { 300f, 100f, 28f },
                new float[] { 15f, 100f, 46f }
            };

            List<string> expecteds = new List<string>() {
                "#4da6ea",
                "#8f008f",
                "#eb3b00"
            };

            for( int i = 0; i < hsls.Count; i++ ) {
                string actual = this._compressor.CompressHsl( hsls[i][0], hsls[i][1], hsls[i][2] );
                Assert.AreEqual( expecteds[i], actual, "Expected " + expecteds[i] + " but was " + actual );
            }
        }

        [Test]
        [ExpectedException( typeof( ArgumentOutOfRangeException ) )]
        public void CompressHslHueExceptionTest() {
            this._compressor.CompressHsl( 365f, 50f, 70f );
        }

        [Test]
        [ExpectedException( typeof( ArgumentOutOfRangeException ) )]
        public void CompressHslSaturationExceptionTest() {
            this._compressor.CompressHsl( 120f, -20f, 70f );
        }

        [Test]
        [ExpectedException( typeof( ArgumentOutOfRangeException ) )]
        public void CompressHslLightExceptionTest() {
            this._compressor.CompressHsl( 120f, 70f, 120f );
        }

        [Test]
        public void HexadecimalToNameTest() {
            List<string> hexes = new List<string>() {
                "#f00",
                "#000080",
                "#fff"
            };

            List<string> expecteds = new List<string>() {
                "red",
                "navy",
                "#fff"
            };

            for( int i = 0; i < hexes.Count; i++ ) {
                string actual = this._compressor.HexadecimalToName( hexes[i] );
                Assert.AreEqual( expecteds[i], actual, "Expected " + expecteds[i] + " but was " + actual );
            }
        }

        [Test]
        public void NameToHexadecimalTest() {
            List<string> names = new List<string>() {
                "mediumvioletred",
                "yellowgreen",
                "red"
            };

            List<string> expecteds = new List<string>() {
                "#c71585",
                "#9acd32",
                "red"
            };

            for( int i = 0; i < names.Count; i++ ) {
                string actual = this._compressor.NameToHexadecimal( names[i] );
                Assert.AreEqual( expecteds[i], actual, "Expected " + expecteds[i] + " but was " + actual );
            }
        }
    }
}
