# 📦 OpenTelemetry.Simplify — Библиотека для упрощения и стандартизации работы с OpenTelemetry в .NET

> ⚠️ **Внимание:** Этот пакет **не является официальным** компонентом проекта OpenTelemetry. Это сторонняя обёртка, созданная для упрощения интеграции OpenTelemetry в .NET-приложениях.

`OpenTelemetry.Simplify` — это NuGet-библиотека, предназначенная для стандартизации и упрощения подключения OpenTelemetry к .NET приложениям. Она предоставляет полную интеграцию метрик и трассировок с Prometheus, OTLP, Jaeger, поддерживает регистрацию кастомных метрик, автоматическое подключение `Meter`, а также готовые Grafana дешборды.

---

## 🔧 Возможности библиотеки

- 📊 Интеграция системных метрик: ASP.NET Core, Runtime, Uptime, Oracle (опционально)
- ⚙️ Поддержка кастомных метрик через интерфейс `ICustomMetric`
- 🔍 Поддержка трассировки: входящие и исходящие HTTP-запросы
- 🌐 Поддержка экспортёров: Prometheus, OTLP, Jaeger, Console
- 🧩 Автоматическая регистрация `Meter` через `IMeterAccessor`
- 🚀 Extension-методы для удобной регистрации метрик и трейсов

---

## 📦 Установка

```bash
dotnet add package OpenTelemetry.Simplify
```

```xml
<!-- Или вручную в csproj -->
<PackageReference Include="OpenTelemetry.Simplify" Version="1.0.0" />
```

---

## 🚀 Быстрый старт

```csharp
builder.Services.AddOpenTelemetrySimplify(options =>
{
    options.ServiceName = "MyService";
    options.ServiceVersion = "1.0.0";

    options.AddMetrics(metrics =>
    {
        metrics.UseAspNetCoreInstrumentation();
        metrics.UseRuntimeInstrumentation();
        metrics.UseUptime();
        metrics.UseExporter(MetricExporterType.Prometheus);
    });

    options.AddTrace(tracing =>
    {
        tracing.UseAspNetCoreInstrumentation();
        tracing.UseHttpClientInstrumentation();
        tracing.UseExporter(TraceExporterType.Jaeger);
    });
});

var app = builder.Build();
app.UseOpenTelemetryPrometheusScrapingEndpoint();
```

---

## 📊 Встроенные метрики

### ASP.NET Core
- Количество запросов
- Время выполнения (latency)
- Активные запросы
- Коды ответов

### Runtime
- Память
- GC
- Исключения
- Потоки и аллокации

### Uptime
- Время непрерывной работы приложения (в секундах)

### Oracle *(опционально)*
- Метрики пула соединений и времени выполнения команд (требует Oracle.ManagedDataAccess.Core 23.3.2+)

---

## 🧠 Кастомные метрики через `ICustomMetric`

### Пример без зависимостей

```csharp
public class SampleCustomMetric : ICustomMetric
{
    private readonly DateTimeOffset _startTime = DateTimeOffset.UtcNow;

    public void Register(Meter meter)
    {
        meter.CreateObservableGauge<double>(
            name: "test_uptime_seconds_test",
            observeValues: () =>
            {
                double uptime = (DateTimeOffset.UtcNow - _startTime).TotalSeconds;
                return new[] { new Measurement<double>(uptime) };
            },
            unit: "seconds",
            description: "Description");
    }
}
```

### Пример с зависимостью

```csharp
public class RandomCustomMetric : ICustomMetric
{
    private readonly IRandomService _service;
    public RandomCustomMetric(IRandomService service) => _service = service;

    public void Register(Meter meter)
    {
        meter.CreateObservableGauge("random_number",
            () => new Measurement<int>(_service.GetRandomNumber()));
    }
}
```

Регистрация:

```csharp
builder.Services.AddOpenTelemetrySimplify(options =>
{
    options.AddMetrics(metrics =>
    {
        metrics.AddCustomMetric<SampleCustomMetric>();
    });
});

app.RegisterCustomMetrics(
    new RandomCustomMetric(app.Services.GetRequiredService<IRandomService>())
);
```

---

## 🔧 IMeterAccessor — правильный способ получить Meter

```csharp
public interface IMeterAccessor
{
    Meter Meter { get; }
}
```

```csharp
var meter = app.Services.GetRequiredService<IMeterAccessor>().Meter;
```

---

## ⚙️ Использование Meter напрямую в middleware или HostedService

```csharp
public class MetricsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Counter<long> _counter;

    public MetricsMiddleware(RequestDelegate next, IMeterAccessor meterAccessor)
    {
        _next = next;
        _counter = meterAccessor.Meter.CreateCounter<long>("custom_middleware_requests_total", "req", "Запросы через middleware");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _counter.Add(1);
        await _next(context);
    }
}
```

```csharp
app.UseMiddleware<MetricsMiddleware>();
```

---

## 📊 Grafana Dashboards

> (в процессе подготовки)

---

## 📁 Структура библиотеки

| Путь                                | Назначение                                |
|-------------------------------------|--------------------------------------------|
| `TelemetryConfigurator.cs`          | Конфигурация метрик и трейсов              |
| `TelemetryOptions.cs`               | Корневая конфигурация                      |
| `MetricsOptions.cs`                 | Опции метрик и экспортёров                 |
| `TracingOptions.cs`                 | Опции трассировки                          |
| `ICustomMetric.cs`                  | Интерфейс для кастомной метрики            |
| `IMeterAccessor.cs`                 | Интерфейс доступа к Meter                  |
| `MeterAccessor.cs`                  | Реализация `IMeterAccessor`                |
| `TelemetryAppExtensions.cs`         | Extension-методы для регистрации после Build|
| `UptimeMetric.cs`                   | Метрика аптайма                            |

---

## ✅ Рекомендации по использованию

> ℹ️ **Важно:** если вы используете метрики, зависящие от сервисов через Dependency Injection (DI), обязательно регистрируйте их через `app.RegisterCustomMetrics(...)` после `app.Build()`.

- Не используй `new Meter(...)` — только `IMeterAccessor`
- Всегда указывай `ServiceName` и `ServiceVersion`
- Метрики с зависимостями регистрируй после `app.Build()`
- Даём meaningful-названия метрикам (например: `my_service.http_duration_seconds`)

---

## 🗂 Структура проекта

```
OpenTelemetry.Simplify/
├── src/
│   └── OpenTelemetry.Simplify/
├── examples/
│   └── OpenTelemetry.Simplify.Example/ (Web API)
└── OpenTelemetry.Simplify.sln
```

---

## 📝 Лицензия

MIT © 2025 — OpenTelemetry.Simplify Contributors

