using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ApiApplication.Test.TestFixture
{
    [CollectionDefinition("DB collection")]
    public class DataBaseFixtureCollection : ICollectionFixture<DataBaseFixture>
    {
        
    }
}
