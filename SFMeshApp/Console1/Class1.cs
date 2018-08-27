
using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting.Base;

public class Class1 : IMYService
{
    public Class1()
    {

    }

    public Task<string> GetWord()
    {
        return Task.FromResult<string>("Hello");
    }
}

public interface IMYService : IService
{
    Task<string> GetWord();

}
