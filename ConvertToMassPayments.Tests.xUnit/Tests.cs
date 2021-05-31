using System;

using ConvertToMassPayments.ViewModels;

using Xunit;

namespace ConvertToMassPayments.Tests.XUnit
{
    // TODO WTS: Add appropriate tests
    public class Tests
    {
        [Fact]
        public void TestMethod1()
        {
        }

        // TODO WTS: Add tests for functionality you add to ConvertViewModel.
        [Fact]
        public void TestConvertViewModelCreation()
        {
            // This test is trivial. Add your own tests for the logic you add to the ViewModel.
            var vm = new ConvertViewModel();
            Assert.NotNull(vm);
        }
    }
}
