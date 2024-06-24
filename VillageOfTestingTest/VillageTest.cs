using Moq;
using VillageOfTesting;
using VillageOfTesting.CompleteActions;
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
            var workers = new List<Worker>
            {
                new Worker("Alice", "farmer", mockOccupationAction.Object),
                new Worker("Mange", "farmer", mockOccupationAction.Object),
                new Worker("Carl", "farmer", mockOccupationAction.Object),
                new Worker("David", "farmer", mockOccupationAction.Object),
                new Worker("Mona", "farmer", mockOccupationAction.Object),
                new Worker("Lisa", "farmer", mockOccupationAction.Object)
            };

            var village = new Village(false, 0, 0, 0, workers, new List<Building>(), new List<Project>(), occupationDictionary, new Dictionary<string, PossibleProject>(), 1, 1, 1, 6, 0);

            village.AddWorker("Tomas", "farmer");

            Assert.Equal(village.MaxWorkers, village.Workers.Count);
            Assert.DoesNotContain(village.Workers, worker => worker.Name == "Tomas");
        }

        [Fact]
        public void Day_NoWorkers_AdvancesDay()
        {
            var village = new Village(false, 10, 10, 10, new List<Worker>(), new List<Building>(), new List<Project>(), new Dictionary<string, IOccupationAction>(), new Dictionary<string, PossibleProject>(), 1, 1, 1, 6, 0);

            village.Day();

            Assert.Equal(1, village.DaysGone);
            Assert.False(village.GameOver);
        }

        [Fact]
        public void Day_WorkersDoTheirJob()
        {
            var mockOccupationAction = new Mock<IOccupationAction>();
            var mockWorker1 = new Mock<Worker>("Mark", "farmer", mockOccupationAction.Object) { CallBase = true };
            var mockWorker2 = new Mock<Worker>("Lisa", "lumberjack", mockOccupationAction.Object) { CallBase = true };
            mockWorker1.Setup(w => w.DoWork());
            mockWorker2.Setup(w => w.DoWork());

            var workers = new List<Worker> { mockWorker1.Object, mockWorker2.Object };
            var village = new Village(false, 10, 10, 10, workers, new List<Building>(), new List<Project>(), new Dictionary<string, IOccupationAction>(), new Dictionary<string, PossibleProject>(), 1, 1, 1, 6, 0);

            village.Day();

            mockWorker1.Verify(w => w.DoWork(), Times.Once);
            mockWorker2.Verify(w => w.DoWork(), Times.Once);
        }

        [Fact]
        public void Day_WorkersWithoutFood_StarveAndDie()
        {
            var worker = new Worker("Sven", "miner", Mock.Of<IOccupationAction>()) { Alive = true };
            var workers = new List<Worker> { worker };
            var village = new Village(false, 0, 10, 10, workers, new List<Building>(), new List<Project>(), new Dictionary<string, IOccupationAction>(), new Dictionary<string, PossibleProject>(), 1, 1, 1, 6, 0);

            for (int i = 0; i < 6; i++)
            {
                village.Day();
            }

            Assert.False(worker.Alive);
            Assert.Equal(5, worker.DaysHungry);
            Assert.True(village.GameOver);
        }

        [Fact]
        public void AddProject_ValidProject_AddsProject()
        {
            var mockCompleteAction = new Mock<ICompleteAction>();
            var possibleProject = new PossibleProject("House", 10, 0, 3, mockCompleteAction.Object);
            var possibleProjects = new Dictionary<string, PossibleProject>
            {
                { "House", possibleProject }
            };
            var projects = new List<Project>();
            var village = new Village(false, 0, 20, 10, new List<Worker>(), new List<Building>(), projects, new Dictionary<string, IOccupationAction>(), possibleProjects, 1, 1, 1, 6, 0);

            village.AddProject("House");

            Assert.Single(projects);
            Assert.Equal("House", projects[0].Name);
        }

        [Fact]
        public void AddProject_InsufficientResources_DoesNotAddProject()
        {
            var mockCompleteAction = new Mock<ICompleteAction>();
            var possibleProject = new PossibleProject("House", 10, 0, 3, mockCompleteAction.Object);
            var possibleProjects = new Dictionary<string, PossibleProject>
            {
                { "House", possibleProject }
            };
            var projects = new List<Project>();
            var village = new Village(false, 0, 2, 0, new List<Worker>(), new List<Building>(), projects, new Dictionary<string, IOccupationAction>(), possibleProjects, 1, 1, 1, 6, 0);

            village.AddProject("House");

            Assert.Empty(projects);
        }

        [Fact]
        public void CompleteProject_BuildingHasEffect()
        {
            var mockCompleteAction = new Mock<ICompleteAction>();
            var possibleProject = new PossibleProject("Woodmill", 5, 1, 1, mockCompleteAction.Object);
            var possibleProjects = new Dictionary<string, PossibleProject>
            {
                { "Woodmill", possibleProject }
            };
            var projects = new List<Project>();

            var workers = new List<Worker>
            {
                new Worker("Finn", "builder", Mock.Of<IOccupationAction>())
            };

            var village = new Village(false, 0, 20, 10, workers, new List<Building>(), projects, new Dictionary<string, IOccupationAction>(), possibleProjects, 1, 1, 1, 6, 0);

            village.AddProject("Woodmill");
            village.Day();

            Assert.Single(village.Projects);
            Assert.Equal("Woodmill", village.Projects[0].Name);
        }

        [Fact]
        public void FullGame_Simulation_WinGame()
        {
            // Arrange
            var mockOccupationAction = new Mock<IOccupationAction>();
            var mockCompleteAction = new Mock<ICompleteAction>();
            var occupationDictionary = new Dictionary<string, IOccupationAction>
            {
                { "builder", mockOccupationAction.Object }
            };
            var possibleProject = new PossibleProject("Castle", 10, 5, 3, new CastleComplete());
            var possibleProjects = new Dictionary<string, PossibleProject>
            {
                { "Castle", possibleProject }
            };
            var workers = new List<Worker>
            {
                new Worker("Alice", "builder", mockOccupationAction.Object)
            };
            var projects = new List<Project>();
            var village = new Village(false, 20, 20, 20, workers, new List<Building>(), projects, occupationDictionary, possibleProjects, 1, 1, 1, 6, 0);

            // Act
            village.AddProject("Castle");
            for (int i = 0; i < 3; i++)
            {
                village.Day(); // Simulate the days required to complete the project
            }

            // Assert
            Assert.Single(village.Projects);
            Assert.Equal("Castle", village.Projects[0].Name);
            Assert.True(village.GameOver); // Game should be over
        }
    }
}