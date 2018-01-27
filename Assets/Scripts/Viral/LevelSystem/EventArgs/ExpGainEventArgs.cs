﻿namespace Viral.LevelSystem.EventArgs
{
    public class ExpGainEventArgs : System.EventArgs
    {
        public int ExpGained { get; private set; }

        public ExpGainEventArgs(int expGained)
        {
            ExpGained = expGained;
        }
    }
}