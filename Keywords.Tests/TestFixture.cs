using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Dsl;
using Moq;

namespace Keywords.Tests;

public abstract class TestFixture
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    protected T A<T>() => _fixture.Create<T>();
    protected List<T> CreateMany<T>(int count) => _fixture.CreateMany<T>(count).ToList();
    protected ICustomizationComposer<T> Build<T>() => _fixture.Build<T>();
    protected T Freeze<T>() where T : class => _fixture.Freeze<T>();
    protected Mock<T> Mock<T>() where T : class => _fixture.Freeze<Mock<T>>();
}
