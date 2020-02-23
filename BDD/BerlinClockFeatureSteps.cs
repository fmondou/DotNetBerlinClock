using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Infra.Helpers;
using Infra.Helpers.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TechTalk.SpecFlow;

namespace BerlinClock
{
    [Binding]
    public class TheBerlinClockSteps
    {
        private IWindsorContainer _Container;
        private ITimeConverter _BerlinClock;
        private String _TimeToConvert;

        /// <summary>Initializes a new instance of the <see cref="TheBerlinClockSteps"/> class.</summary>
        public TheBerlinClockSteps()
        {
            // Initialize container to enable dependency injection.
            _Container = new WindsorContainer();
            _Container.Register(
                Component.For<ITimeConverter>().ImplementedBy<TimeConverter>(),
                Component.For<ILog>().ImplementedBy<StubILog>()
                );

            _BerlinClock = _Container.Resolve<ITimeConverter>();
        }

        [When(@"the time is ""(.*)""")]
        public void WhenTheTimeIs(string time)
        {
            _TimeToConvert = time;
        }

        [Then(@"the clock should look like")]
        public void ThenTheClockShouldLookLike(string theExpectedBerlinClockOutput)
        {
            Assert.AreEqual(_BerlinClock.ConvertTime(_TimeToConvert), theExpectedBerlinClockOutput);
        }

    }
}
