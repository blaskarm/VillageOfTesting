using Moq;
using VillageOfTesting;
using VillageOfTesting.Interfaces;
using VillageOfTesting.Objects;

namespace VillageOfTestingTest
{
    public class VillageTest
    {
        [Theory]
        [InlineData("Bob", "farmer")]
        [InlineData("Larsa", "lumberjack")]
        public void AddWorker_ValidOccupation_AddsWorkers(string name, string occupation)
        {
            var mockOccupationAction = new Mock<IOccupationAction>();
            var occupationDictionary = new Dictionary<string, IOccupationAction>
            {
                { occupation, mockOccupationAction.Object }
            };
            var workers = new List<Worker>();
            var village = new Village(false, 0, 0, 0, workers, new List<Building>(), new List<Project>(), occupationDictionary, new Dictionary<string, PossibleProject>(), 1, 1, 1, 6, 0);

            village.AddWorker(name, occupation);

            Assert.Single(workers);
            Assert.Equal(name, workers[0].Name);
            Assert.Equal(occupation, workers[0].Occupation);
        }

        [Theory]
        [InlineData("Bob", "jogger")]
        [InlineData("Karen", "nagger")]
        public void AddWorker_InvalidOccupation_DoesNotAddWorker(string name, string occupation)
        {
            var occupationDictionary = new Dictionary<string, IOccupationAction>();
            var workers = new List<Worker>();
            var village = new Village();

            village.AddWorker(name, occupation);

            Assert.Empty(workers);
        }

        [Fact]
        public void AddWorker_WorkerListFull_DoesNotAddWorker()
        {
            var mockOccupationAction = new Mock<IOccupationAction>();
            var occupationDictionary = new Dictionary<string, IOccupationAction>
            {
                { "farmer", mockOccupationAction.Object }
            };
            /*
            var workers = new List<Worker>
            {
                new Worker("Alice", "farmer", mockOccupationAction.Object),
                new Worker("Mange", "farmer", mockOccupationAction.Object),
                new Worker("Carl", "farmer", mockOccupationAction.Object),
                new Worker("David", "farmer", mockOccupationAction.Object),
                new Worker("Mona", "farmer", mockOccupationAction.Object),
                new Worker("Lisa", "farmer", mockOccupationAction.Object)
            };
            */
            var village = new Village();
            village.Workers.Add(new Worker("Alice", "farmer", mockOccupationAction.Object));
            village.Workers.Add(new Worker("Mange", "farmer", mockOccupationAction.Object));
            village.Workers.Add(new Worker("Carl", "farmer", mockOccupationAction.Object));
            village.Workers.Add(new Worker("David", "farmer", mockOccupationAction.Object));
            village.Workers.Add(new Worker("Mona", "farmer", mockOccupationAction.Object));
            village.Workers.Add(new Worker("Lisa", "farmer", mockOccupationAction.Object));

            village.AddWorker("Tomas", "farmer");

            Assert.Equal(village.MaxWorkers, village.Workers.Count);
            Assert.DoesNotContain(village.Workers, worker => worker.Name == "Tomas");
        }
    }
}