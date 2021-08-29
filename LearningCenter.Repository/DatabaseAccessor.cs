using System;
using System.Collections.Generic;
using System.Text;
using LearningCenter.Database.Context;

namespace LearningCenter.Repository
{
    class DatabaseAccessor
    {
        public static MiniCStructorContext Instance { get; private set; }

        static DatabaseAccessor()
        {
            Instance = new MiniCStructorContext();
        }
    }
}
