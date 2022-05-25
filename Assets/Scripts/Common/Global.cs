using System.Collections.Generic;
using Controller;

namespace Common
{
    public enum Player
    {
        P1,
        P2
    }
    public static class Global
    {
        public static PlayerController P1;
        public static PlayerController P2;

        public static List<PortalController> PortalControllers;
    }
}