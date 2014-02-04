
namespace MinifyLibTests {
    using NUnit.Framework;
    using MinifyLib.Manipulate;
    using MinifyLib.Color;
    using System.Collections.Generic;

    [TestFixture]
    public class ManipulationClassTest {

        private Manipulation _manip;

        public void Init( string source ) {
            var comp = new ColorCompressor( new ColorConverter() );
            this._manip = new Manipulation( comp, source );
        }

        [TearDown]
        public void Cleanup() {
            this._manip = null;
        }

        // Call Init(string) for each test so I can specify string.
    }
}
