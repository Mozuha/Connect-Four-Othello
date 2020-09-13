/*
 * Sep 3, 2020
 * Mizuki Hashimoto
 * 
 * This is a class of empty disc, to represent a square in grid is empty. 
 * It has disc color and disc symbol as its member, which are "empty" and "-" respectively.
 */

using System;

namespace ConnectFourOthello {
  [Serializable]
  public class EmptyDisc : Disc {
    public override string Color => "empty";  // property returns "empty" as a value
    public override char Symbol => '-';  // property returns "-" as a value
  }
}