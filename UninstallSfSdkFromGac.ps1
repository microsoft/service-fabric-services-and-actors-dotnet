$assemblies = "Microsoft.ServiceFabric.Data.Interfaces",
              "Microsoft.ServiceFabric.Data.Interfaces.V2"

foreach ($assembly in $assemblies)
{
    gacutil.exe /u $assembly
}