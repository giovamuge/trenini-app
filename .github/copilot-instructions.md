# TreniniApp Development Guidelines for GitHub Copilot

## Project Overview
TreniniApp is a .NET MAUI cross-platform mobile application for train schedule management, built following Clean Architecture principles, SOLID design patterns, modern C# best practices, and .NET MAUI AOT (Ahead-of-Time) compilation guidelines for optimal performance and reduced startup times.

## Architecture & Design Patterns

### Clean Architecture Layers
```
├── Pages/           # Presentation Layer - UI Components
├── ViewModels/      # Presentation Layer - Business Logic
├── Views/           # Presentation Layer - Reusable UI Elements  
├── Services/        # Application Layer - Business Services
├── Models/          # Domain Layer - Core Entities
├── Constants/       # Domain Layer - Application Constants
├── Utils/           # Infrastructure Layer - Helper Utilities
└── Platforms/       # Infrastructure Layer - Platform-specific code
```

### MVVM Pattern Implementation
- **Pages**: Always inherit from `BaseContentPage<TViewModel>`
- **ViewModels**: Always inherit from `BaseViewModel` with lifecycle methods
- **Binding**: Use `CommunityToolkit.Maui.Markup` for fluent UI declarations
- **Commands**: Use `[RelayCommand]` for UI actions with optional `CanExecute`

### Dependency Injection
- Register all services in `MauiProgram.cs`
- Use interfaces for all services (`INavigationService`, `IStationService`, `IWebScrapingService`)
- Constructor injection only - avoid service locator pattern
- Resolve dependencies via `DipendencyInjectionUtil.GetService<T>()`

## Code Standards & Best Practices

### Naming Conventions
```csharp
// ✅ PascalCase for public members
public ObservableCollection<Station> FilteredStations { get; } = [];
public async Task LoadStationsAsync()

// ✅ camelCase with underscore for private fields
private readonly IStationService _stationService;
private bool _isLoading = false;

// ✅ Descriptive method names
LoadMoreStationsAsync() // not LoadData()
OnSearchTextChanged()   // not OnTextChanged()
```

### Async Programming Patterns
```csharp
// ✅ Always use async/await for I/O operations
var stations = await _stationService.GetAllStationsAsync();

// ✅ Proper exception handling with try-catch-finally
try 
{
    _isLoading = true;
    // async operation
}
catch (Exception ex)
{
    // user-friendly error handling
    await _dispatcher.DispatchAsync(() => DisplayAlert(...));
}
finally 
{
    _isLoading = false;
}

// ✅ Use Task not async void (except event handlers)
public async Task LoadStationsAsync() // ✅
public async void OnButtonClicked()   // ✅ (event handler only)
```

### Performance Optimization
```csharp
// ✅ Static binding expressions for performance
.Bind(Label.TextProperty, static (Station s) => s.Name)

// ✅ Collection expressions for modern C#
_allStations = [.. stations];

// ✅ Infinite scrolling for large datasets
private int _pageSize = 50;
private int _currentPage = 0;
private bool _allLoaded = false;

// ✅ Prevent concurrent operations
if (_isLoading || _allLoaded) return;
_isLoading = true;

// ✅ AOT-compatible static delegates (avoid runtime code generation)
.Where(static s => s.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))

// ✅ Avoid reflection in AOT builds
// Use explicit type conversions instead of dynamic/reflection
var station = (Station)item; // instead of dynamic casting
```

### UI Development with Markup
```csharp
// ✅ Fluent UI declarations
new SearchBar { Placeholder = "Search station..." }
    .Bind(SearchBar.TextProperty, 
          static (SelectStationViewModel vm) => vm.SearchText,
          mode: BindingMode.TwoWay)
    .Row(0)

// ✅ Grid layout with typed definitions
new Grid
{
    RowDefinitions = Rows.Define((0, GridLength.Auto), (1, GridLength.Star)),
    Children = { /* content */ }
}

// ✅ Platform-specific configurations
page.On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.PageSheet);
```

### Navigation Pattern
```csharp
// ✅ Always use INavigationService for navigation
await _navigationService.PushAsync<SelectStationPage>();
await _navigationService.PopAsync();

// ✅ Modal presentation with proper styling
await _navigationService.PushModalAsync(page);
```

### State Management
```csharp
// ✅ ObservableProperty for reactive UI
[ObservableProperty]
private string? searchText;

[ObservableProperty]
private Station? selectedStation;

// ✅ ObservableCollection for dynamic lists
public ObservableCollection<Station> FilteredStations { get; } = [];

// ✅ Preferences for data persistence
Preferences.Set(StationConstant.SelectedStationKey, selectedValue);
var savedValue = Preferences.Get(StationConstant.SelectedStationKey, defaultValue);
```

### Error Handling Standards
```csharp
// ✅ User-friendly error messages
await _dispatcher.DispatchAsync(() =>
{
    App.Current?.Windows?[0]?.Page?.DisplayAlert(
        "Error", 
        $"Failed to load stations: {ex.Message}", 
        "OK"
    );
});

// ✅ Defensive programming with null checks
if (station == null) return;
private bool CanSelectStation() => SelectedStation != null;

// ✅ Early returns to reduce nesting
if (_isLoading) return;
if (string.IsNullOrWhiteSpace(value)) return;
```

## Anti-Patterns to Avoid

❌ **Don't use `Application.Current` for service resolution**
❌ **Don't create service instances manually**  
❌ **Don't bind directly to async Task methods**
❌ **Don't block UI thread with `.Result` or `.Wait()`**
❌ **Don't duplicate business logic between ViewModels**
❌ **Don't use magic strings - always use Constants**
❌ **Don't break MVVM by putting business logic in code-behind**
❌ **Don't use reflection or dynamic code generation (AOT incompatible)**
❌ **Don't use Activator.CreateInstance() or Assembly.GetTypes()**

## SOLID Principles in Practice

### Single Responsibility Principle (SRP)
- Each ViewModel handles one specific feature (e.g., `SelectStationViewModel` only for station selection)
- Services have focused responsibilities (`IStationService` only for station data)

### Open/Closed Principle (OCP)
- Extend functionality through interfaces and inheritance
- Use `BaseViewModel` and `BaseContentPage<T>` for common behavior

### Liskov Substitution Principle (LSP)
- All service implementations must be substitutable with their interfaces
- ViewModels can be used polymorphically through `BaseViewModel`

### Interface Segregation Principle (ISP)
- Create specific, focused interfaces (`INavigationService`, `IStationService`)
- Don't force classes to depend on methods they don't use

### Dependency Inversion Principle (DIP)
- Depend on abstractions (interfaces) not concrete implementations
- Use constructor injection for all dependencies

## Testing Guidelines

### Unit Testing Standards
- Test all ViewModels and Services
- Use xUnit, NSubstitute for mocking, and FluentAssertions
- Follow AAA pattern (Arrange, Act, Assert)
- Test both happy path and error scenarios
- Mock all external dependencies (services, navigation, dispatcher)

### Test Structure
```csharp
// ✅ Test class naming convention
public class SelectStationViewModelTests
{
    private readonly SelectStationViewModel _viewModel;
    private readonly IStationService _stationService = Substitute.For<IStationService>();
    private readonly INavigationService _navigationService = Substitute.For<INavigationService>();
    private readonly IDispatcher _dispatcher = Substitute.For<IDispatcher>();

    // ✅ Test method naming: MethodName_Scenario_ExpectedResult
    [Fact]
    public async Task LoadStationsAsync_WhenCalled_ShouldLoadStationsAndSetSelectedStation()
    {
        // Arrange
        var stations = new List<Station> { new("Test Station", 1) };
        _stationService.GetAllStationsAsync().Returns(stations);

        // Act
        await _viewModel.LoadStationsAsync();

        // Assert
        _viewModel.FilteredStations.Should().HaveCount(1);
        _viewModel.FilteredStations.First().Name.Should().Be("Test Station");
    }
}
```

### Testing Best Practices
- Test ViewModels in isolation using mocked dependencies
- Verify ObservableProperty changes and command execution
- Test async methods with proper async/await patterns
- Use TestDispatcher for UI thread operations in tests
- Test command CanExecute logic separately
- Avoid reflection in tests for AOT compatibility
- Use explicit assertions instead of dynamic property access

### Test Project Structure
```
TreniniApp/
├── TreniniApp.Tests/         # Unit test project
│   ├── ViewModels/           # ViewModel unit tests
│   ├── Services/             # Service unit tests
│   └── TreniniApp.Tests.csproj
├── Pages/                    # Main project files
├── ViewModels/
├── Services/
└── TreniniApp.csproj
```

**Example test setup:**
```csharp
public class SelectStationViewModelTests
{
    private readonly SelectStationViewModel _viewModel;
    private readonly IStationService _stationService = Substitute.For<IStationService>();
    private readonly INavigationService _navigationService = Substitute.For<INavigationService>();
    private readonly IDispatcher _dispatcher = Substitute.For<IDispatcher>();

    public SelectStationViewModelTests()
    {
        _viewModel = new SelectStationViewModel(_dispatcher, _stationService, _navigationService);
    }
}
```

## Template for New Features

When implementing new features, follow this structure:

1. **Domain Model** (`Models/NewEntity.cs`)
2. **Service Interface** (`Services/INewService.cs`)
3. **Service Implementation** (`Services/NewService.cs`)
4. **Unit Tests** (`Tests/Services/NewServiceTests.cs`, `Tests/ViewModels/NewViewModelTests.cs`)
5. **Register in DI** (`MauiProgram.cs`)
6. **ViewModel** (`ViewModels/NewViewModel.cs` inheriting `BaseViewModel`)
7. **Page** (`Pages/NewPage.cs` inheriting `BaseContentPage<NewViewModel>`)
8. **Navigation** (integrate with `INavigationService`)

## Code Quality Checklist

Before submitting code, ensure:
- [ ] All async methods end with `Async` suffix
- [ ] Proper exception handling with user-friendly messages
- [ ] No magic strings - use Constants
- [ ] Static binding expressions where possible
- [ ] Proper disposal of resources
- [ ] Constructor injection for all dependencies
- [ ] ObservableProperty for reactive properties
- [ ] RelayCommand for UI actions
- [ ] Platform-specific configurations using `.On<Platform>()`
- [ ] AOT-compatible code (avoid reflection, use static delegates)
- [ ] Explicit type conversions instead of dynamic casting
- [ ] Unit tests for ViewModels and Services
- [ ] Mocked dependencies in tests

## Performance Guidelines

- Use infinite scrolling for lists > 100 items
- Implement debouncing for search functionality
- Use `ConfigureAwait(false)` in service layer methods
- Avoid complex calculations in binding expressions
- Use static binding for better performance
- Enable collection synchronization for thread-safe ObservableCollections

## .NET MAUI AOT Compliance Guidelines

### AOT-Safe Patterns
```csharp
// ✅ Use static delegates instead of lambda expressions where possible
.Where(static item => item.IsActive)

// ✅ Avoid reflection - use explicit type conversions
var station = (Station)bindingContext; // not Convert.ChangeType()

// ✅ Use source generators (CommunityToolkit.Mvvm)
[ObservableProperty] // generates code at compile time
private string? searchText;

// ✅ Explicit generic type parameters
services.GetRequiredService<IStationService>(); // not GetService(typeof(IStationService))
```

### AOT Restrictions to Avoid
```csharp
// ❌ Avoid dynamic code generation
// ❌ Don't use Expression.Compile()
// ❌ Avoid Activator.CreateInstance() for generic types
// ❌ Don't use Assembly.GetTypes() or Type.GetTypes()
// ❌ Avoid late-bound method calls via reflection
// ❌ Don't use JsonSerializer without source generators
```

### AOT-Compatible Patterns
- Use compile-time code generation (source generators)
- Prefer static methods and delegates
- Use explicit type registrations in DI container
- Implement interfaces explicitly rather than using dynamic dispatch
- Use DataContract serialization or System.Text.Json source generators

Remember: Clean, maintainable, and testable code is always preferred over clever or complex solutions.

### Commenting Standards
- Use XML documentation comments for public members
- Comment complex logic or decisions in the code
- Keep comments up-to-date with code changes
- Avoid redundant comments that state the obvious
- Use english for all comments and documentation
