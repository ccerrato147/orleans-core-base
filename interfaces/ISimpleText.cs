using System;
using System.Threading.Tasks;

namespace interfaces
{
    public interface ISimpleText: Orleans.IGrainWithIntegerKey
    {
        Task<string> GetString(string param);
    }
}
