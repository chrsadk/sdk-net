using NUnit.Framework;
using Penneo;

namespace PenneoTests
{
   
    [SetUpFixture]
    public class Init
    {
      
        [OneTimeSetUp]
        public void AssemblyInit()
        {
            PenneoConnector.Initialize(null, null);
        }
    }
}