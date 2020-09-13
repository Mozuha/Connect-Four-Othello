/*
 * Sep 3, 2020
 * Mizuki Hashimoto
 * 
 * This is an abstract class of a disc. It has disc color and disc symbol as its member.
 */

using System;

namespace ConnectFourOthello {
  [Serializable]
  public abstract class Disc {
    public abstract string Color { get; }
    public abstract char Symbol { get; }
  }
}