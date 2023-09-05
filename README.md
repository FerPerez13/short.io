# 🩳 Short.io | 🇪🇸

## Introducción

Short.io es un proyecto de API REST básico con el que podemos acortar URLs tal y como lo hacen aplicaciones como Cuttly, Bittly, ...

Para ello se ha desarrollado una API en NET 7. Como base de datos se ha utilizado para las pruebas una SqLite que esta dentro del repositorio, aunque lo ideal es que tengamos una BBDD en un servicio como Microsoft Azure, Google Cloud o Amazon Web Services.&#x20;

La API no tiene ninguna inyección de dependencias ya que no es necesario por que en el propio `program.cs` tenemos las llamadas `POST` y `GET` que se necesitan utilizando:

```csharp
app.MapPost("/create", async (UrlRedirect urlRedirect) => { }
app.MapGet("/{shortUrl}", async (string shortUrl) => { }
```
