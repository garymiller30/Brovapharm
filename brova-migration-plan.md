# Міграція ProvapharmNext на .NET 8

## Статус: План міграції

Цей документ описує процес міграції проєкту ProvapharmNext з .NET Framework 4.8 на сучасний .NET 8.

---

## 📋 Зміст

1. [Аналіз поточного стану](#аналіз-поточного-стану)
2. [Стратегія міграції](#стратегія-міграції)
3. [Новий архітектурний патерн](#новий-архітектурний-патерн)
4. [Кроки впровадження](#кроки-впровадження)
5. [Вимоги до системи](#вимоги-до-системи)

---

## Аналіз поточного стану

### Поточна архітектура
- **Платформа**: WPF (.NET Framework 4.8)
- **MVVM Pattern**: Частково реалізований
- **Структура**:
  - `ViewModels/` - ViewModels
  - `Controls/` - Користувацькі елементи (PdfViewer)
  - `UserControls/` - UI компоненти
  - `Commands/` - Command pattern implementation

### Виявлені проблеми

| Категорія | Проблема | Вплив |
|-----------|---------|--------|
| Архітектура | Статичний стан (`Preparats.PreparatList`) | Складно тестувати, memory leaks |
| MVVM | Відсутній ViewModelBase | Повторюваний код INotifyPropertyChanged |
| Command pattern | ICommand без CanExecuteChanged підписки | UI не оновлюється |
| Обробка помилок | `MessageBox.Show()` замість узгодженого Notify | Непослідовний UX |
| Безпека | Hardcoded шляхи до репозиторію | Проблеми з деплоємом |
| Тестування | Відсутні unit tests | Ризик регресій |
| Сумісність | Windows.Data.Pdf у .NET Framework | Не працює без UWP залежностей |

---

## Стратегія міграції

### Етап 1: Підготовка (1-2 тижні)
```
1. Створити нову гілку git
2. Створити структуру проекту для .NET 8
3. Перенести models без змін логіки
4. Налаштувати CI/CD для перевірки компіляції
```

### Етап 2: Архітектурне рефакторинг (2-3 тижні)
```
1. Реалізувати ViewModelBase з INotifyPropertyChanged
2. Створити команди (RelayCommand, AsyncRelayCommand)
3. Винести сервіси в інтерфейси (IPasteService, ISearchService)
4. Замінити static singleton на dependency injection
```

### Етап 3: Core functionality migration (2-3 тижні)
```
1. Перенести MainViewModel з логікою
2. Реалізувати PasteCommand
3. Реалізувати ExportCommand
4. Створити PdfViewer для .NET 8 (SkiaSharp або PdfSharp)
```

### Етап 4: Testing & Polish (1-2 тижні)
```
1. Додати unit tests (60% coverage)
2. Реалізувати логування
3. Додати error handling
4. Оптимізувати UI/UX
```

---

## Новий архітектурний патерн

### Структура проекту

```
Brovapharm/
├── Brovapharm.sln
├── Brovapharm.Models/           # .NET 8 class library
│   ├── Preparat.cs
│   ├── PreparatFile.cs
│   ├── GlobalSettings.cs
│   └── Json/FileItem.cs
└── Brovapharm.Desktop/          # .NET 8 WPF application
    ├── App.xaml
    ├── MainWindow.xaml
    ├── ViewModels/
    │   ├── MainViewModel.cs
    │   ├── PasteViewModel.cs
    │   └── ExportViewModel.cs
    ├── Commands/
    │   ├── RelayCommand.cs
    │   └── AsyncRelayCommand.cs
    ├── Services/
    │   ├── IPasteService.cs
    │   ├── ISearchService.cs
    │   └── LoggingService.cs
    ├── Controls/
    │   └── PdfViewer.xaml
    └── Themes/
        └── LightTheme.xaml
```

### Важливі зміни

#### 1. ViewModelBase (new)
```csharp
public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

#### 2. RelayCommand ( CommunityToolkit.Mvvm)
```csharp
public partial class MainViewModel : ObservableObject
{
    [RelayCommand]
    private void OnExport()
    {
        // Implementation
    }
}
```

#### 3. Dependency Injection (new)
```csharp
// In App.xaml.cs
protected override void OnStartup(StartupEventArgs e)
{
    var container = new ContainerBuilder();
    
    container.RegisterType<IPasteService, PasteService>();
    container.RegisterType<ISearchService, SearchService>();
    
    base.OnStartup(e);
}
```

#### 4. Async/Await (new pattern)
```csharp
[RelayCommand]
private async Task LoadFilesAsync()
{
    try
    {
        await Task.Run(() => SearchFiles());
    }
    catch (Exception ex)
    {
        _logger.Error(ex, "Error loading files");
    }
}
```

---

## Кроки впровадження

### 1. Створити нову гілку

```bash
git checkout -b bionic/migrate-to-dotnet8
```

### 2. Створити структуру проекту

```bash
# Create solution
dotnet new sln -n Brovapharm

# Create class library for models
dotnet new classlib -n Brovapharm.Models -f net8.0

# Create WPF application
dotnet new wpf -n Brovapharm.Desktop -f net8.0-windows10.0.19041.0

# Add projects to solution
dotnet sln Brovapharm.sln add Brovapharm.Models/Brovapharm.Models.csproj
dotnet sln Brovapharm.sln add Brovapharm.Desktop/Brovapharm.Desktop.csproj

# Add project reference
dotnet add Brovapharm.Desktop reference Brovapharm.Models
```

### 3. Встановити пакети NuGet

```bash
# In Brovapharm.Desktop
dotnet add package CommunityToolkit.Mvvm
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Serilog.Sinks.Console
dotnet add package iText7
dotnet add package SkiaSharp
```

### 4. Мігрувати код по модулям

1. **Models** - перенести без змін (тільки namespace)
2. **Services** - винести інтерфейси, реалізувати DI
3. **ViewModels** - рефакторинг з ViewModelBase
4. **Commands** - заміна на CommunityToolkit.Mvvm
5. **UI** - оновлення XAML для нових патернів

---

## Вимоги до системи

### Для розробки (.NET 8)
```
- Windows 10 version 1809 або новіше
- Visual Studio 2022 v17.8+ або VS Code + C# Dev Kit
- .NET 8 SDK
- Git 2.34+
```

### Для запуску
```
- Windows 10/11 (x64)
- .NET 8 Desktop Runtime
- PDF Reader (для відкриття файлів)
```

---

## Мажорні вдосконалення

| Функція | Стан в 2.x | Покращення в 3.0 |
|---------|-----------|-------------------|
| Тестування | Відсутнє | xUnit + Moq, 60%+ coverage |
| Логування | Console.WriteLine | Serilog з файлом |
| DI Container | Відсутній | Microsoft.Extensions.DependencyInjection |
| Async pattern | async void (problematic) | async Task + error handling |
| UI Theme | Light only | Light/Dark + system theme detection |
| Error Handling | MessageBox.Show() | Toast notifications + log |

---

## Тестова стратегія

```csharp
// Example unit test
public class PasteServiceTests
{
    [Fact]
    public void GetPreparatsFromClipboard_ShouldParseValidData()
    {
        // Arrange
        var service = new PasteService();
        
        // Mock clipboard data
        Clipboard.SetText("1\tMedicine A\tAB-001\n2\tMedicine B\tAB-002");
        
        // Act
        var result = service.GetPreparatsFromClipboard();
        
        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Medicine A", result[0].Name);
    }
}
```

---

## Терміни виконання

| Етап | Тривалість | Примітки |
|------|-----------|----------|
| Підготовка | 2 тижні | Структура, CI/CD |
| Core models | 1 тиждень | без змін |
| Services | 2 тижні | DI + interfaces |
| ViewModels | 2 тижні | ViewModelBase |
| UI migration | 2 тижні | XAML updates |
| Testing | 1 тиждень | unit + integration |
| **Разом** | **10 тижнів** | ~2.5 місяці |

---

## Висновки

Міграція на .NET 8 забезпечить:
- ✅ Сучасну архітектуру (MVVM + DI)
- ✅ Краще тестування
- ✅ Async/await pattern
- ✅ Сумісність з Windows 10/11
- ✅ Легший деплой (Self-contained)

**Наступний крок**: Запустити команди створення проекту та почати міграцію моделей.
