using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using System;
using System.Threading.Tasks;

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

public interface IMYService: IService
{
    Task<string> GetWord();
    
}
