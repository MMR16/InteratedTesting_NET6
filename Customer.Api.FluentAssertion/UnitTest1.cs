using FluentAssertions;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using TestInternal;

namespace Customer.Api.FluentAssertion
{
    public class UnitTest1
    {
        public class CustomerControllertest
        {
            //arrange
            private readonly HttpClient httpclient = new()
            {
                BaseAddress = new Uri("https://localhost:5001/")
            };

            [Fact]
            public async Task Get_ReturnNotFound_CustomerNotExist()
            {
                //act
                var response = await httpclient.GetAsync($"customers/{Guid.NewGuid}");
                //assert
                // Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }


            [Fact]
            public async Task CheckIfNameIsCorrectFromTest()
            {
                //act
                var obj = new Test();
                var nikName = obj.NikName;
                var fullName = obj.FullName;
                //assert
                nikName.Should().Be("MMR");
                fullName.Should().StartWith("Mostafa");
                //fullName.Should().HaveLength(40);
            }

            [Fact]
            public async Task CheckIfAgeIsCorrectFromTest()
            {
                //act
                var obj = new Test();
                var age = obj.Age;
                //assert
                age.Should().Be(30);
                age.Should().BeGreaterThan(20);
                age.Should().BeLessThan(40);
                age.Should().BeLessThanOrEqualTo(30);
            }


            [Fact]
            public async Task CheckIfDateOfBirthIsCorrectFromTest()
            {
                //act
                var obj = new Test();
                var dateOfbirth = obj.DateOfBirth;
                //assert
                dateOfbirth.Should().Be(new DateOnly(1994, 3, 16));
                dateOfbirth.Should().HaveYear(1994);
                dateOfbirth.Should().BeOnOrAfter(new DateOnly(1990, 7, 6));
            }


            [Fact]
            public async Task CheckIfAgainstObject()
            {
                //arrange
                var user = new { FullName = "Mostafa Mahmoud", Age = 30, DateOfBirth = new DateOnly(1994, 3, 16), NikName = "MMR" };

                //act
                var actualUser = new Test();
                //assert
                //Assert.Equal(user, actualUser);  // faild due to object is reference type
                //user.Should().Be(actualUser);   // faild due to object is reference type
                // user.Age.Should().Be(actualUser.Age); // pass because of proprty
                user.Should().BeEquivalentTo(actualUser); // pass because of BeEquivalentTo compare every property
            }


            [Fact]
            public async Task CheckIEnumerableOfObjects()
            {
                //arrange 
                var users = User.Users;
                //act
                var user = new Test { FullName = "Mostafa Mahmoud", Age = 30, DateOfBirth = new DateOnly(1994, 3, 16), NikName = "MMR" };
                //assert
                users.Should().ContainEquivalentOf(user);
                users.Should().HaveCount(5);
                users.Should().Contain(e=>e.FullName.StartsWith("Mostafa") && e.DateOfBirth.Month == 3);
            }


            [Fact]
            public async Task ExceptionThrow()
            {
                // arrange
                var test=new Test();
                // act
                Action result =()=> test.Divide(10,0);
                // assert
                 result.Should().Throw<DivideByZeroException>().WithMessage("attempted to divide by zero.");
            }



            [Fact]
            public async Task InternalAccesss()
            {
                // arrange
                var test = new TestInternals();
                // act
                var number = test.number;


                // assert
                number.Should().Be(99);
            }


        }
    }








    public class Test
    {
        public string NikName = "MMR";
        public string FullName = "Mostafa Mahmoud";
        public int Age { get; set; } = 30;
        public DateOnly DateOfBirth { get; set; } = new DateOnly(1994, 3, 16);
        public  float Divide(int x, int y)
        {
            if (y == 0)
            {
                throw new DivideByZeroException();
            }
            return x / y;
        }
    }

    public static class User
    {
        public static IEnumerable<Test> Users = new[]{
            new Test() {Age=5,FullName="fsdff",DateOfBirth=new DateOnly(1700,5,7),NikName="ddssddd"},
            new Test() {Age=5,FullName="fdsdff",DateOfBirth=new DateOnly(4444,5,7),NikName="ddfdsddd"},
            new Test() {Age=5,FullName="dd",DateOfBirth=new DateOnly(9992,5,7),NikName="dddffdd"},
            new Test() {Age=5,FullName="ffggf",DateOfBirth=new DateOnly(5146,5,7),NikName="ddffggddd"},
            new Test()};
    }

    public class Test2
    {
        internal event EventHandler ExampleEvent;
        internal int InternalSecretNumber = 42;
        public virtual void RaiseExampleEvent()
        {
            ExampleEvent(this,EventArgs.Empty);
        }
    }
}

