#TodoAPI

Una API RESTful desarrollada en ASP.NET Core con arquitectura en capas y un modelo genérico para gestionar tareas de distintos tipos de datos.

## ✨ Características principales

- Arquitectura en capas: **Domain**, **Application**, **Infrastructure**, **API**
- Uso de repositorios genéricos con soporte para cualquier tipo de datos en tareas (e.g., `Todo<string>`, `Todo<int>`)
- Manejo de excepciones centralizado
- Sembrado inicial de datos (DataSeeder)
- Swagger UI habilitado
- Entity Framework Core + SQL Server

---

## 🗂️ Estructura del Proyecto

src/
├── API/ # Capa de presentación (controllers, middlewares, etc.)
├── Application/ # Casos de uso, servicios de negocio
├── Domain/ # Entidades y contratos (interfaces)
├── Infrastructure/ # DbContext, Repositorios, Extensiones, Migraciones


---

## 🧱 Tecnologías

- .NET 8
- Entity Framework Core
