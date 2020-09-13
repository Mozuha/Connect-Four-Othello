/*
 * Sep 3, 2020
 * Mizuki Hashimoto
 * 
 * This is a class of red disc. It has disc color and disc symbol as its member,
 * which are "red" and "o" respectively.
 */

using System;

namespace ConnectFourOthello {
  [Serializable]
  public class RedDisc : Disc {
    public override string Color => "red";  // property returns "red" as a value
    public override char Symbol => 'o';  // property returns "o" as a value
  }
}