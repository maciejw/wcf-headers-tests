using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        Task<string> DoWorkAsync();
    }
}
