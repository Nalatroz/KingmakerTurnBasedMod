﻿using static TurnBased.Utility.StatusWrapper;

namespace TurnBased.Utility
{
    public class BugfixOption
    {
        public bool ForTB;
        public bool ForRT;

        public static implicit operator bool(BugfixOption option) 
            => IsEnabled() ? option.ForTB : option.ForRT;

        public BugfixOption() { }

        public BugfixOption(bool forTB, bool forRT)
        {
            ForTB = forTB;
            ForRT = forRT;
        }
    }
}