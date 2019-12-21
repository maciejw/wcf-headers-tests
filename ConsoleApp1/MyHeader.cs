using System.Runtime.Serialization;

namespace ConsoleApp1
{
    [DataContract]
    public class MyHeader
    {
        public static readonly string Name = nameof(MyHeader);
        public static readonly string Ns = "http://tempuri.org";

        [DataMember]
        public string SomeData { get; set; }
    }
}
