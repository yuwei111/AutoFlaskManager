using System.Collections.Generic;

namespace FlaskManager
{
    class FlaskInformation
    {
        public Dictionary<string, FlaskAction> UniqueFlaskNames { get; set; }
        public Dictionary<string, FlaskAction> FlaskTypes { get; set; }
        public Dictionary<string, FlaskAction> FlaskMods { get; set; }
        public Dictionary<string, FlaskAction> Ignore_Pattern { get; set; }
        public Dictionary<string, FlaskAction> Offensive_Pattern { get; set; }
        public Dictionary<string, FlaskAction> Defensive_Pattern { get; set; }
    }
}
