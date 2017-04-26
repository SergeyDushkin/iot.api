namespace iot.api.Domain
{
    public class IRDeviceSetting
    {
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string Template { get; set; }
        public int Command_length { get; set; }
        public OneStruct One { get; set; }
        public OneStruct Zero { get; set; }
        public int Post_data_bits { get; set; }
        public int Repeat_gap { get; set; }
        public int Min_repeat { get; set; }
        public PropertyStruct Properties { get; set; }

        public class HeaderStruct
        {
            public int Pulse { get; set; }
            public int Space { get; set; }
        }

        public class OneStruct
        {
            public int Pulse { get; set; }
            public int Space { get; set; }
        }

        public class ZeroStruct
        {
            public int Pulse { get; set; }
            public int Space { get; set; }
        }

        public class TemplateStruct
        {
        }

        public class PropertyStruct
        {
            public HeaderStruct Header { get; set; }
            public string Command { get; set; }
            public int Post_data { get; set; }
            public int Ptrail { get; set; }
            public int Gap { get; set; }
        }
    }
}
