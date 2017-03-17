using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using GitHub.Unity;
using TestUtils;

namespace UnitTests
{
    [TestFixture]
    class GitStatusEntryFactoryTests
    {
        protected SubstituteFactory SubstituteFactory { get; private set; }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            SubstituteFactory = new SubstituteFactory();
        }

        [Test]
        public void CreateObjectWhenProjectRootIsChildOfGitRootAndFileInGitRoot()
        {
            var repositoryPath = "/Source".ToNPath();
            var unityProjectPath = repositoryPath.Combine("UnityProject");

            var gitEnvironment = SubstituteFactory.CreateProcessEnvironment(repositoryPath);
            var environment = SubstituteFactory.CreateEnvironment(new CreateEnvironmentOptions {
                RepositoryPath = repositoryPath,
                UnityProjectPath = unityProjectPath
            });

            NPathFileSystemProvider.Current = SubstituteFactory.CreateFileSystem(new CreateFileSystemOptions {
                CurrentDirectory = repositoryPath
            });

            const string inputPath = "Something.sln";
            const GitFileStatus inputStatus = GitFileStatus.Added;

            var expectedFullPath = repositoryPath.Combine(inputPath);
            var expectedProjectPath = expectedFullPath.RelativeTo(unityProjectPath);

            var expected = new GitStatusEntry(inputPath, expectedFullPath, expectedProjectPath, inputStatus);

            var gitStatusEntryFactory = new GitObjectFactory(environment);

            var result = gitStatusEntryFactory.CreateGitStatusEntry(inputPath, inputStatus);

            result.Should().Be(expected);
        }

        [Test]
        public void CreateObjectWhenProjectRootIsChildOfGitRootAndFileInProjectRoot()
        {
            var repositoryPath = "/Source".ToNPath();
            var unityProjectPath = repositoryPath.Combine("UnityProject");

            var gitEnvironment = SubstituteFactory.CreateProcessEnvironment(repositoryPath);
            var environment = SubstituteFactory.CreateEnvironment(new CreateEnvironmentOptions {
                RepositoryPath = repositoryPath,
                UnityProjectPath = unityProjectPath
            });
            NPathFileSystemProvider.Current = SubstituteFactory.CreateFileSystem(new CreateFileSystemOptions {
                CurrentDirectory = repositoryPath
            });

            var inputPath = "UnityProject/Something.sln".ToNPath().ToString();
            const GitFileStatus inputStatus = GitFileStatus.Added;

            var expectedFullPath = repositoryPath.Combine(inputPath);
            const string expectedProjectPath = "Something.sln";

            var expected = new GitStatusEntry(inputPath, expectedFullPath, expectedProjectPath, inputStatus);

            var gitStatusEntryFactory = new GitObjectFactory(environment);

            var result = gitStatusEntryFactory.CreateGitStatusEntry(inputPath, inputStatus);

            result.Should().Be(expected);
        }

        [Test]
        public void CreateObjectWhenProjectRootIsSameAsGitRootAndFileInGitRoot()
        {
            var repositoryPath = "/Source".ToNPath();
            var unityProjectPath = repositoryPath;

            var gitEnvironment = SubstituteFactory.CreateProcessEnvironment(repositoryPath);
            var environment = SubstituteFactory.CreateEnvironment(new CreateEnvironmentOptions {
                RepositoryPath = repositoryPath,
                UnityProjectPath = unityProjectPath
            });
            NPathFileSystemProvider.Current = SubstituteFactory.CreateFileSystem(new CreateFileSystemOptions {
                CurrentDirectory = repositoryPath
            });

            const string inputPath = "Something.sln";
            const GitFileStatus inputStatus = GitFileStatus.Added;

            var expectedFullPath = repositoryPath.Combine(inputPath);
            const string expectedProjectPath = inputPath;

            var expected = new GitStatusEntry(inputPath, expectedFullPath, expectedProjectPath, inputStatus);

            var gitStatusEntryFactory = new GitObjectFactory(environment);

            var result = gitStatusEntryFactory.CreateGitStatusEntry(inputPath, inputStatus);

            result.Should().Be(expected);
        }
    }
}