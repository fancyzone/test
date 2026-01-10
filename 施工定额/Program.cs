namespace 施工定额
{
    internal static class Program
    {
        public static class ValueStorage
        {
            public static string SharedValue = "";
            public static string SharedValue2 = "";//定额编码
            public static string SharedValue3 = "";//ID号
        }
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}