using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalringoApi.Networking
{
    /// <summary>
    /// Default Carrier for delegates used across the system
    /// </summary>
    public delegate void DataCarrier();
    /// <summary>
    /// Default Carrier for delegates used across the system
    /// </summary>
    /// <typeparam name="T">The type of the data</typeparam>
    /// <param name="data">The data to be passed</param>
    public delegate void DataCarrier<T>(T data);
    /// <summary>
    /// Default Carrier for delegates used across the system
    /// </summary>
    /// <typeparam name="T1">The type of the first data parameter</typeparam>
    /// <typeparam name="T2">The type of the second data parameter</typeparam>
    /// <param name="d1">The first data parameter</param>
    /// <param name="d2">The second data parameter</param>
    public delegate void DataCarrier<T1, T2>(T1 d1, T2 d2);
}
