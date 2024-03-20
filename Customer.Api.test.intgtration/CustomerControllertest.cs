using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Customer.Api.test.intgtration
{
    public class CustomerControllertest : IAsyncLifetime  // : IDisposable
    {
        //arrange
        private readonly HttpClient httpclient = new()
        {
            BaseAddress = new Uri("https://localhost:5001/")
        };

        public CustomerControllertest()
        {
        }

        //skip to ignore test
        [Fact(Skip ="we have better ways now")]
        public async Task Get_ReturnNotFound_CustomerNotExist()
        {
            //act
            var response = await httpclient.GetAsync($"customers/{Guid.NewGuid()}");
            //assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        public async Task InitializeAsync()
        {
            await Task.Delay(1000);
        }

        ////clean up by implement IAsyncLifetime
        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        #region dispose
        //clean up by implement Idisposable
        //public void Dispose()
        //{
        //    throw new NotImplementedException();
        //}
        #endregion


        // test 2 
        // pass parameter to function using inline data & theory instead of fact
        [Theory]
        [InlineData("AE46FAEC-9A09-41C6-921D-41AA51CE6B09",Skip = "skipping this")]
        [InlineData("AE46FAEC-9A09-41C6-921D-41AA51CE6gh09")]
        [InlineData("AE46FAEC-9A09-99C6-921D-41AA51CE6B09")]
        [InlineData("AE46FAEC-9A82-41C6-921D-41AA51CE6B09")]
        public async Task Get_ReturnNotFound_CustomerNotExist2(string guidText)
    {
        //act
        var response = await httpclient.GetAsync($"customers/{guidText}");
        //assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

        // test 3 
        // pass parameter to function using MemberData & theory instead of fact
        [Theory]
        [MemberData(nameof(data))]
        public async Task Get_ReturnNotFound_CustomerNotExist3(string guidText)
        {
            //act
            var response = await httpclient.GetAsync($"customers/{guidText}");
            //assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            
        }
        public static IEnumerable<object[]> data { get; } = new[]
        {
           new string[]{ "AE46FAEC-9A09-41C6-921D-41AA51CE6B09" },
           new string[]{ "AE46FAEC-9A09-41C6-921D-41AA51CE6g09" },
           new string[]{ "AE46FAEC-9A09-99C6-921D-41AA51CE6B09" },
           new string[]{ "AE89FAEC-9A82-41C6-921D-41AA51CE6B09" },
           new string[]{ "AE45yAEC-9A82-41C6-921D-41AA51CE6B09" },
           new string[]{ "AE46qqEC-9A82-41C6-921D-41AA51CE6B09" },
           new string[]{ "AE46FAEC-9A82-41d9-921D-41AA51CE6B09" },
        };


        // test 4 
        // pass parameter to function using ClassData & theory instead of fact
        [Theory]
        [ClassData(typeof(ClassData))]
        public async Task Get_ReturnNotFound_CustomerNotExist4(string guidText)
        {
            //act
            var response = await httpclient.GetAsync($"customers/{guidText}");
            //assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        }
        public class ClassData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] {"AE46FAEC-9A09-41C6-921D-41AA51CE6B09" };
                yield return new object[] {"AE46FAEC-9A09-41C6-921D-41AA51CE6g09" };
                yield return new object[] {"AE46FAEC-9A09-99C6-921D-41AA51CE6B09" };
                yield return new object[] { "AE89FAEC-9A82-41C6-921D-41AA51CE6B09" };
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
