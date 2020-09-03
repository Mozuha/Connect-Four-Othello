/*
 * Sep 3, 2020
 * Mizuki Hashimoto
 * 
 * This is a class of yellow disc. It has disc color and disc symbol as its member,
 * which are "yellow" and "x" respectively.
 */

using System;

namespace ConnectFourOthello {
    public class YellowDisc : Disc {
        public override string Color => "yellow";  // property returns "yellow" as a value
        public override char Symbol => 'x';  // property returns "x" as a value
    }
}
