using System;

namespace Common.Trading
{
    [Flags]
    public enum Side
    {
        Buy = 1,
        Sell,
        Unknown
    }

    public static class SideUtils
    {
        //public static Side FromXenaString(string side) => side switch
        //{
        //    XenaExchange.Client.Messages.Constants.Side.Buy => Side.Buy,
        //    XenaExchange.Client.Messages.Constants.Side.Sell => Side.Sell
        //};

        //public static Side InvertSide(string side) => InvertSide(FromXenaString(side));        
        public static Side InvertSide(Side side) => side switch
        {
            Side.Buy => Side.Sell,
            Side.Sell => Side.Buy,
            _ => Side.Unknown
        };

        public static Side Invert(this Side side) => InvertSide(side);

        //public static string ToXenaString(this Side side) => ToString(side);
        //public static string ToString(Side side) => side switch
        //{
        //    Side.Buy => XenaExchange.Client.Messages.Constants.Side.Buy,
        //    Side.Sell => XenaExchange.Client.Messages.Constants.Side.Sell
        //};

        public static Side FromSign(this decimal volume) => volume >= 0 ? Side.Buy : Side.Sell;
        public static Side FromSign(this double volume) => volume >= 0 ? Side.Buy : Side.Sell;
        public static int GetSign(this Side side) => side == Side.Buy ? 1 : -1;

    }
}
