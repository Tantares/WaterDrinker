using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaterDrinker
{
    public static class WDStatics
    {
        public const string WD_GROUP_CODE = "WaterDrinker";

        // Propeller animation.

        public const float ENGINE_SHUTDOWN_COOLDOWN_MULT = 0.1F;

        // LOCS

        public const string LOC_WD_GROUP_NAME = "#LOC_wd_group_name";                           // WaterDrinker

        public const string LOC_WD_MIRROR_FLAG = "#LOC_wd_mirror_flag";                         // Mirrored
        public const string LOC_WD_MIRROR_FLAG_ENABLED = "#LOC_wd_mirror_flag_enabled";         // Clockwise
        public const string LOC_WD_MIRROR_FLAG_DISABLED = "#LOC_wd_mirror_flag_disabled";       // Anticlockwise

    }
}
